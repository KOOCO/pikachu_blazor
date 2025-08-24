using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Kooco;

public static class EcPayCheckMacValue
{
    //TODO: Remove hard-coded value after testing and add value to appsettings
    public const string MerchantId = "3087335";
    public const string HashKey = "jxrMfff0dQml5Zhn";
    public const string HashIV = "jggHhdCBky7tPFk6";
    public static string Generate(Dictionary<string, string> parameters, EcPayHttpOptions options)
    {
        var sorted = parameters
            .Where(kvp => kvp.Key != "CheckMacValue" && !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ToList();

        var toBeHashed = new StringBuilder();
        toBeHashed
            .Append($"HashKey={/*options.*/HashKey}&")
            .Append(string.Join("&", sorted.Select(kvp => $"{kvp.Key}={kvp.Value}")))
            .Append($"&HashIV={/*options.*/HashIV}");

        var urlEncoded = HttpUtility.UrlEncode(toBeHashed.ToString())
            .ToLower()
            .Replace("%2d", "-")
            .Replace("%5f", "_")
            .Replace("%2e", ".")
            .Replace("%21", "!")
            .Replace("%2a", "*")
            .Replace("%28", "(")
            .Replace("%29", ")");

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(urlEncoded));
        return Convert.ToHexString(hashBytes).ToUpperInvariant();
    }

    public static string ForTradeInfo(Dictionary<string, string> parameters, EcPayHttpOptions options)
    {
        // Remove CheckMacValue and empty values, sort keys in ASCII order (case-insensitive)
        var sorted = parameters
            .Where(kvp => kvp.Key != "CheckMacValue" && !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Build the raw string
        var raw = $"HashKey={/*options.*/HashKey}&{string.Join("&", sorted.Select(kvp => $"{kvp.Key}={kvp.Value}"))}&HashIV={/*options.*/HashIV}";

        // URL encode and apply EcPay special rules
        var encoded = HttpUtility.UrlEncode(raw).ToLower()
            .Replace("%2d", "-")
            .Replace("%5f", "_")
            .Replace("%2e", ".")
            .Replace("%21", "!")
            .Replace("%2a", "*")
            .Replace("%28", "(")
            .Replace("%29", ")");

        return encoded.ToMd5();
    }
}
