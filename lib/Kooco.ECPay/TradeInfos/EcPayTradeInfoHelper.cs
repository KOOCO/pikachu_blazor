namespace Kooco.TradeInfos;

public static class EcPayTradeInfoHelper
{
    public static Dictionary<string, string> ParseResponse(string content)
    {
        return content.Split('&')
            .Select(part => part.Split('='))
            .Where(pair => pair.Length == 2)
            .ToDictionary(pair => pair[0], pair => Uri.UnescapeDataString(pair[1]));
    }

    public static EcPayTradeInfoResponse MapToTradeInfo(Dictionary<string, string> dict)
    {
        return new EcPayTradeInfoResponse
        {
            ActualWeight = TryParseDecimal(dict, "ActualWeight"),
            AllPayLogisticsID = GetValue(dict, "AllPayLogisticsID"),
            BookingNote = GetValue(dict, "BookingNote"),
            CollectionAllocateAmount = TryParseDecimal(dict, "CollectionAllocateAmount") ?? 0,
            CollectionAllocateDate = TryParseDate(dict, "CollectionAllocateDate"),
            CollectionAmount = TryParseDecimal(dict, "CollectionAmount") ?? 0,
            CollectionChargeFee = TryParseDecimal(dict, "CollectionChargeFee") ?? 0,
            CVSPaymentNo = GetValueOrNull(dict, "CVSPaymentNo"),
            CVSValidationNo = GetValueOrNull(dict, "CVSValidationNo"),
            GoodsAmount = TryParseDecimal(dict, "GoodsAmount") ?? 0,
            GoodsName = GetValueOrNull(dict, "GoodsName"),
            GoodsWeight = TryParseDecimal(dict, "GoodsWeight"),
            HandlingCharge = TryParseDecimal(dict, "HandlingCharge") ?? 0,
            LogisticsStatus = GetValue(dict, "LogisticsStatus"),
            LogisticsType = GetValue(dict, "LogisticsType"),
            MerchantID = GetValue(dict, "MerchantID"),
            MerchantTradeNo = GetValue(dict, "MerchantTradeNo"),
            SenderCellPhone = GetValueOrNull(dict, "SenderCellPhone"),
            SenderName = GetValue(dict, "SenderName"),
            SenderPhone = GetValue(dict, "SenderPhone"),
            ShipChargeDate = TryParseDate(dict, "ShipChargeDate"),
            ShipmentNo = GetValueOrNull(dict, "ShipmentNo"),
            TradeDate = TryParseDate(dict, "TradeDate"),
            CheckMacValue = GetValue(dict, "CheckMacValue")
        };
    }

    public static string GetValue(Dictionary<string, string> dict, string key)
    {
        return dict.TryGetValue(key, out string? value) ? value : string.Empty;
    }

    public static string? GetValueOrNull(Dictionary<string, string> dict, string key)
    {
        return dict.TryGetValue(key, out string? value) && !string.IsNullOrEmpty(value) ? value : null;
    }

    public static decimal? TryParseDecimal(Dictionary<string, string> dict, string key)
    {
        return dict.TryGetValue(key, out string? value) && decimal.TryParse(value, out var decimalValue) ? decimalValue : null;
    }

    public static DateTime? TryParseDate(Dictionary<string, string> dict, string key)
    {
        return dict.TryGetValue(key, out string? value) && DateTime.TryParse(value, out var date) ? date : null;
    }
}
