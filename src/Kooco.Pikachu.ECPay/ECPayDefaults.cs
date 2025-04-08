using System.Globalization;
using System.Net;
using System.Text;

namespace Kooco.Pikachu;
public static class ECPayDefaults
{
    static readonly Encoding StringEncoding = Encoding.UTF8;
    static readonly char[] HexCharsLower = "0123456789abcdef".ToCharArray();

    public static string GetMerchantTradeDate(this DateTime time)
    {
        return time.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
    public static async Task<string> EncryptForECPayAsync(this string text, string key, string iv)
    {
        ArgumentNullException.ThrowIfNull(text);
       // var result = text.EncodeUrlForECPay();
        return await text.AesEncryptAsync(key, iv).ConfigureAwait(false);
    }
    public static async Task<string> DecryptForECPayAsync(this string text, string key, string iv)
    {
        ArgumentNullException.ThrowIfNull(text);
        var result = await text.AesDecryptAsync(key, iv).ConfigureAwait(false);
        return WebUtility.UrlDecode(result);
    }
    public static string EncodeUrlForECPay(this string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var estimatedCapacity = value.Length + value.Length / 2;
        StringBuilder result = new(estimatedCapacity);
        var utf8ByteBuffer = new byte[4];
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (!ShouldEncode(c)) { result.Append(c); }
            else
            {
                int bytesWritten;
                if (char.IsHighSurrogate(c) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
                {
                    bytesWritten = StringEncoding.GetBytes(value, i, 2, utf8ByteBuffer, 0);
                    i++;
                }
                else
                {
                    bytesWritten = StringEncoding.GetBytes(value, i, 1, utf8ByteBuffer, 0);
                }

                for (var j = 0; j < bytesWritten; j++)
                {
                    var b = utf8ByteBuffer[j];
                    result.Append('%');
                    result.Append(HexCharsLower[b >> 4]);
                    result.Append(HexCharsLower[b & 0x0F]);
                }
            }
        }
        return result.ToString();
        static bool ShouldEncode(char c) => !(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9');
    }
}