using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Kooco.Reconciliations;

public sealed class EcPayReconciliationMap : ClassMap<EcPayReconciliationResponse>
{
    public EcPayReconciliationMap()
    {
        Map(m => m.OrderDate).Name("訂單日期").TypeConverter<DateTimeConverter>();
        Map(m => m.MerchantTradeNo).Name("廠商訂單編號");
        Map(m => m.EcPayTradeNo).Name("綠界訂單編號");
        Map(m => m.StoreCode).Name("店鋪代號").Optional();
        Map(m => m.MID).Name("MID").Optional();
        Map(m => m.PlatformName).Name("平台名稱").Optional();
        Map(m => m.PaymentType).Name("付款方式");
        Map(m => m.FeeRate).Name("費率(每筆)").Optional();
        Map(m => m.CreditCardAuthCode).Name("信用卡授權單號").Optional();
        Map(m => m.CreditCardLast4).Name("信用卡卡號末4碼").Optional();
        Map(m => m.ConvenienceStoreInfo).Name("超商資訊/ATM虛擬帳號").Optional();
        Map(m => m.PaymentStatus).Name("付款狀態");
        Map(m => m.TransactionAmount).Name("交易金額").TypeConverter<DecimalConverter>();
        Map(m => m.RefundDate).Name("退款日期").Optional();
        Map(m => m.RefundAmount).Name("退款金額").TypeConverter<DecimalConverter>().Optional();
        Map(m => m.HandlingFee).Name("交易手續費").TypeConverter<DecimalConverter>();
        Map(m => m.PlatformFee).Name("平台手續費").TypeConverter<DecimalConverter>();
        Map(m => m.NetAmount).Name("應收款項(淨額)").TypeConverter<DecimalConverter>();
        Map(m => m.PayoutStatus).Name("撥款狀態");
        Map(m => m.Remarks).Name("備註").Optional();
        Map(m => m.MerchantRemarks).Name("廠商備註").Optional();
        Map(m => m.ItemName).Name("商品名稱");
        Map(m => m.Description).Name("交易描述");
        Map(m => m.PayerName).Name("付款人姓名").Optional();
        Map(m => m.PayerPhone).Name("付款人手機").Optional();
        Map(m => m.PayerEmail).Name("付款人Email").Optional();
        Map(m => m.ReceiverName).Name("收件人姓名").Optional();
        Map(m => m.ReceiverPhone).Name("收件人手機").Optional();
        Map(m => m.ReceiverAddress).Name("收件人地址").Optional();
        Map(m => m.ReceiverEmail).Name("收件人Email").Optional();
        Map(m => m.UnifiedBusinessNumber).Name("統一編號").Optional();
        Map(m => m.ProcessingFee).Name("交易處理費").TypeConverter<DecimalConverter>();
    }
}

public class DecimalConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
            return 0m;

        if (decimal.TryParse(text, out decimal result))
            return result;

        return 0m;
    }
}

public class DateTimeConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
            return DateTime.MinValue;

        if (DateTime.TryParse(text, out DateTime result))
            return result;

        return DateTime.MinValue;
    }
}
