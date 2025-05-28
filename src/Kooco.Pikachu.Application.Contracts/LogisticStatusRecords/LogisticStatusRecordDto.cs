using System;

namespace Kooco.Pikachu.LogisticStatusRecords
{
    /// <summary>
    /// 物流狀態記錄DTO
    /// </summary>
    public class LogisticStatusRecordDto
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string? Reference { get; set; }
        public string? Location { get; set; }
        public string? Datetime { get; set; }
        public DateTime? DatetimeParsed { get; set; }
        public string? Code { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? RawLine { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
