using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.LogisticStatusRecords
{
    /// <summary>
    /// 用於批量創建物流狀態記錄的DTO
    /// </summary>
    public class CreateLogisticStatusRecordListDto
    {
        [Required]
        [MinLength(1)]
        public List<CreateLogisticStatusRecordDto> Records { get; set; } = new();
    }
}
