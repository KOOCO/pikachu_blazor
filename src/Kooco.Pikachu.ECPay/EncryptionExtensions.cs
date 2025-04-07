using System.Security.Cryptography;
using System.Text;

namespace Kooco.Pikachu;
public static class EncryptionExtensions
{
    /// <summary>
    /// 使用 SHA256 演算法計算文字的雜湊值
    /// </summary>
    public static string HmacSHA256(this string message, string key)
    {
        // 使用 UTF-8 編碼將字串轉換為位元組數組
        var keyByte = Encoding.UTF8.GetBytes(key);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        // 使用 HMACSHA256 演算法計算雜湊值
        using HMACSHA256 hmacsha256 = new(keyByte);
        var hashmessage = hmacsha256.ComputeHash(messageBytes);

        // 將雜湊值轉換為 Base64 編碼的字串
        return Convert.ToBase64String(hashmessage);
    }

    /// <summary>
    /// 使用 AES 演算法加密文字
    /// </summary>
    public static async Task<string> AesEncryptAsync(
        this string text,
        string key,
        string iv,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7,
        int keySize = 128)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(iv);

        using Aes aes = Aes.Create();
        aes.KeySize = keySize;
        aes.IV = Encoding.UTF8.GetBytes(iv);
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.Mode = mode;
        aes.Padding = padding;

        MemoryStream ms = new();
        await using (ms.ConfigureAwait(false))
        {
            CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);
            await using (cs.ConfigureAwait(false))
            {
                var data = Encoding.UTF8.GetBytes(text);
                await cs.WriteAsync(data.AsMemory(0, data.Length)).ConfigureAwait(false);
                await cs.FlushFinalBlockAsync().ConfigureAwait(false);
            }

            return Convert.ToBase64String(ms.ToArray());
        }
    }

    /// <summary>
    /// 使用 AES 演算法解密文字
    /// </summary>
    public static async Task<string> AesDecryptAsync(
        this string text,
        string key,
        string iv,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7,
        int keySize = 128)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(iv);

        using var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.IV = Encoding.UTF8.GetBytes(iv);
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.Mode = mode;
        aes.Padding = padding;

        MemoryStream ms = new(Convert.FromBase64String(text));
        await using (ms.ConfigureAwait(false))
        {
            CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            await using (cs.ConfigureAwait(false))
            {
                using StreamReader reader = new(cs);
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}