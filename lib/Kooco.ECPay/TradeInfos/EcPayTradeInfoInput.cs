namespace Kooco.TradeInfos;

public class EcPayTradeInfoInput
{
    public string MerchantID { get; set; }
    public string HashKey { get; set; }
    public string HashIV { get; set; }
    public List<string> MerchantTradeNos { get; set; } = new List<string>();
}
