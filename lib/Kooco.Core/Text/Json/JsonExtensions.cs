using System.Text.Encodings.Web;
using System.Text.Json;

namespace Text.Json;
public static class JsonExtensions
{
    public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        options ??= new()
        {
            MaxDepth = 100,
            WriteIndented = true,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        return JsonSerializer.Serialize(obj, options);
    }
    public static T? ToObject<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}