using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements.Services
{
    public interface IFileParsingService
    {
        Task<FileParsingResult> ParseFileAsync(string filePath, LogisticsFileType fileType);
    }
    public class FileParsingResult
    {
        public bool IsSuccess { get; set; }
        public List<LogisticsFeeRecord> Records { get; set; } = new();
        public string ErrorMessage { get; set; }
        public int TotalRecords { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class LogisticsFeeRecord
    {
        public string MerchantTradeNo { get; set; }
        public decimal FeeAmount { get; set; }
        public int RowNumber { get; set; }

        // Breakdown of fees
        public decimal ShippingFee { get; set; }
        public decimal ExtraShippingFee { get; set; }

        // Common fields
        public string ShippingNo { get; set; }

        // T-Cat specific fields
        public string CustomerCode { get; set; }
        public string CollectionDate { get; set; }
        public string CollectionOffice { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryOffice { get; set; }
        public decimal CodAmount { get; set; }

        // ECPay specific fields
        public string OrderTime { get; set; }
        public string LogisticsProvider { get; set; }
        public string LogisticsStatus { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string DeductionDate { get; set; }
    }
}
