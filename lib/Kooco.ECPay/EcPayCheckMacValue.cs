using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Kooco;

public static class EcPayCheckMacValue
{
    public static string Generate(Dictionary<string, string> parameters, string hashKey, string hashIV)
    {
        var sorted = parameters
            .Where(kvp => kvp.Key != "CheckMacValue" && !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ToList();

        var toBeHashed = new StringBuilder();
        toBeHashed
            .Append($"HashKey={hashKey}&")
            .Append(string.Join("&", sorted.Select(kvp => $"{kvp.Key}={kvp.Value}")))
            .Append($"&HashIV={hashIV}");

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

    public static string ForTradeInfo(Dictionary<string, string> parameters, string hashKey, string hashIV)
    {
        // Remove CheckMacValue and empty values, sort keys in ASCII order (case-insensitive)
        var sorted = parameters
            .Where(kvp => kvp.Key != "CheckMacValue" && !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Build the raw string
        var raw = $"HashKey={hashKey}&{string.Join("&", sorted.Select(kvp => $"{kvp.Key}={kvp.Value}"))}&HashIV={hashIV}";

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
        // Hash with SHA256 and return uppercase hex
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(encoded));
        return Convert.ToHexString(hashBytes).ToUpperInvariant();
    }
}
