using System.Globalization;
using System.Text.Json;

namespace Kooco;
public static class ECPayDefaults
{
    public static string GetMerchantTradeDate(this DateTime time)
    {
        return time.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}