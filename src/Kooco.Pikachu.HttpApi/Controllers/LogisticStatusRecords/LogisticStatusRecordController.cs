using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Kooco.Pikachu.LogisticStatusRecords;

namespace Kooco.Pikachu.Controllers.LogisticStatusRecords
{
    [RemoteService(IsEnabled = true)]
    [Area("app")]
    [Route("api/app/logistic-status-records")]
    public class LogisticStatusRecordController : PikachuController
    {
        private readonly ILogisticStatusRecordAppService _logisticStatusRecordAppService;

        public LogisticStatusRecordController(ILogisticStatusRecordAppService logisticStatusRecordAppService)
        {
            _logisticStatusRecordAppService = logisticStatusRecordAppService;
        }

        /// <summary>
        /// 創建單一物流狀態記錄
        /// </summary>
        [HttpPost("create")]
        public Task<LogisticStatusRecordDto> CreateAsync([FromBody] CreateLogisticStatusRecordDto input)
        {
            return _logisticStatusRecordAppService.CreateAsync(input);
        }

        /// <summary>
        /// 批量創建物流狀態記錄
        /// </summary>
        [HttpPost("create-batch")]
        public Task<List<LogisticStatusRecordDto>> CreateManyAsync([FromBody] CreateLogisticStatusRecordListDto input)
        {
            return _logisticStatusRecordAppService.CreateManyAsync(input);
        }
    }
}
