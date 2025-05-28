using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.LogisticStatusRecords
{
    /// <summary>
    /// 物流狀態記錄應用服務接口
    /// </summary>
    [RemoteService(Name = "LogisticStatusRecords")]
    public interface ILogisticStatusRecordAppService : IApplicationService
    {
        /// <summary>
        /// 創建單一物流狀態記錄
        /// </summary>
        Task<LogisticStatusRecordDto> CreateAsync(CreateLogisticStatusRecordDto input);
        
        /// <summary>
        /// 批量創建物流狀態記錄
        /// </summary>
        Task<List<LogisticStatusRecordDto>> CreateManyAsync(CreateLogisticStatusRecordListDto input);
    }
}
