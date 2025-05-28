using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.LogisticStatusRecords
{
    /// <summary>
    /// 用於創建單一物流狀態記錄的DTO
    /// </summary>
    public class CreateLogisticStatusRecordDto
    {
        [Required]
        [StringLength(32)]
        public string OrderId { get; set; }

        [StringLength(64)]
        public string? Reference { get; set; }

        [StringLength(128)]
        public string? Location { get; set; }

        [StringLength(32)]
        public string? Datetime { get; set; }

        [StringLength(32)]
        public string? Code { get; set; }

        [StringLength(16)]
        public string? StatusCode { get; set; }

        [StringLength(128)]
        public string? StatusMessage { get; set; }

        [StringLength(512)]
        public string? RawLine { get; set; }
    }
}
