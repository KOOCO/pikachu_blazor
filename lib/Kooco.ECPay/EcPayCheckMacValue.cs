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
}
