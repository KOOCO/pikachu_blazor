using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Domain.LogisticStatusRecords
{
    public class LogisticStatusRecord : Entity<int>
    {
        public string OrderId { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Location { get; set; }
        public string? Datetime { get; set; } // 原始字串型別的日期時間
        public DateTime? DatetimeParsed { get; set; } // 轉換後的 DateTime 型別
        public string? Code { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? RawLine { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}
