using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ChiDaram.Common.Properties;

namespace ChiDaram.Common.Helper
{
    public static class StringHelper
    {
        public static string RemoveArabicChars(this string inputString)
        {
            return inputString.Replace("ي", "ی").Replace("ك", "ک");
        }
        public static string RemoveHtmlTags(this string inputString)
        {
            return Regex.Replace(inputString, @"<[^>]*>", "");
        }
        public static string RemoveNoneAlphabeticChars(this string inputString)
        {
            return Regex.Replace(inputString, @"\W", "");
        }
        public static string GetUrlReady(this string inputString)
        {
            return Regex.Replace(inputString.Trim(), @"\W+", "-");
        }
        public static string ToEnglishNumber(this string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return "";
            //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
            return inputString
                .Replace("٠", "0").Replace("۰", "0")
                .Replace("١", "1").Replace("۱", "1")
                .Replace("٢", "2").Replace("۲", "2")
                .Replace("٣", "3").Replace("۳", "3")
                .Replace("٤", "4").Replace("۴", "4")
                .Replace("٥", "5").Replace("۵", "5")
                .Replace("٦", "6").Replace("۶", "6")
                .Replace("٧", "7").Replace("۷", "7")
                .Replace("٨", "8").Replace("۸", "8")
                .Replace("٩", "9").Replace("۹", "9");
        }
        public static string ToCurrentCultureNumber(this string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return "";
            //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
            return inputString
                .Replace("0", Resources.Number0)
                .Replace("1", Resources.Number1)
                .Replace("2", Resources.Number2)
                .Replace("3", Resources.Number3)
                .Replace("4", Resources.Number4)
                .Replace("5", Resources.Number5)
                .Replace("6", Resources.Number6)
                .Replace("7", Resources.Number7)
                .Replace("8", Resources.Number8)
                .Replace("9", Resources.Number9);
        }
        public static string ToMd5Hash(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public const string MyKey = "iO2PCJZcEmgXzjR1ilcXhvBgJvCFFaTf"; // 32 Char
        private const string MyIv = "OwvWZ29YH9N22nng"; // 16 Char

        public static string EncryptString(this string plainText)
        {
            if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException(nameof(plainText));
            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(MyKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(MyIv);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }
            return Convert.ToBase64String(encrypted);
        }
        public static string DecryptString(this string encryptedString)
        {
            if (string.IsNullOrWhiteSpace(encryptedString)) throw new ArgumentNullException(nameof(encryptedString));
            string plaintext;
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(MyKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(MyIv);
                var cipherText = Convert.FromBase64String(encryptedString);
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                plaintext = srDecrypt.ReadToEnd();
            }
            return plaintext;
        }

        public static List<string> SplitOnChars(this string inputString, params char[] chars)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return new List<string>();
            return inputString.Trim().Split(chars, StringSplitOptions.RemoveEmptyEntries)
                    .Where(q => !string.IsNullOrWhiteSpace(q))
                    .Select(q => q.Trim())
                    .ToList();
        }

        public static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    var num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return res.ToString();
        }
    }
}
