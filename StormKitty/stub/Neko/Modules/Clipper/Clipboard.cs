/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Threading;

namespace Clipper
{
    internal sealed class Clipboard
    {

        // Get text from clipboard
        public static string GetText()
        {
            string ReturnValue = string.Empty;
            try
            {
                Thread STAThread = new Thread(
                delegate ()
                {
                    ReturnValue = System.Windows.Forms.Clipboard.GetText();
                });
                STAThread.SetApartmentState(ApartmentState.STA);
                STAThread.Start();
                STAThread.Join();

            }
            catch { }
            return ReturnValue;

        }

        // Set text to clipboard
        public static void SetText(string text)
        {
            Thread STAThread = new Thread(
            delegate ()
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(text);
                }
                catch { };
            });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }

    }
}
