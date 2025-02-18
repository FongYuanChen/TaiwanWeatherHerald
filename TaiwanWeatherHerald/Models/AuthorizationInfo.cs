using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TaiwanWeatherHerald.Models
{
    /// <summary>
    /// 授權相關訊息
    /// </summary>
    public static class AuthorizationInfo
    {
        private static readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenWeatherDataAuthorization.dat");
        private static readonly byte[] _key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes key
        private static readonly byte[] _iv = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes IV

        /// <summary>
        /// OpenWeather API 授權金鑰，用於存取天氣數據
        /// </summary>
        public static string OpenWeatherData
        {
            get => File.Exists(_filePath) ? Decrypt(File.ReadAllBytes(_filePath)) : null;
            set => File.WriteAllBytes(_filePath, Encrypt(value));
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static byte[] Encrypt(string text)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(text);
            swEncrypt.Flush();
            csEncrypt.FlushFinalBlock(); // 確保所有資料寫入加密流
            return msEncrypt.ToArray();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        private static string Decrypt(byte[] cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
