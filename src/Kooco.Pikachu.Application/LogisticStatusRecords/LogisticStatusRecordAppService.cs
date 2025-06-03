using Kooco.Pikachu.Domain.LogisticStatusRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticStatusRecords
{
    public class LogisticStatusRecordAppService : PikachuAppService, ILogisticStatusRecordAppService
    {
        private readonly IRepository<LogisticStatusRecord, int> _logisticStatusRecordRepository;
        private readonly TCatSFTPService _tcatSftpService;

        public LogisticStatusRecordAppService(IRepository<LogisticStatusRecord, int> logisticStatusRecordRepository, TCatSFTPService tcatSftpService)
        {
            _tcatSftpService = tcatSftpService;
            _logisticStatusRecordRepository = logisticStatusRecordRepository;
        }

        /// <summary>
        /// 嘗試解析自定義格式的日期時間字串
        /// </summary>
        private DateTime? TryParseCustomDateTime(string datetimeStr)
        {
            // 嘗試標準解析
            if (DateTime.TryParse(datetimeStr, out var standardDt))
            {
                return standardDt;
            }

            // 嘗試解析格式為 "yyyyMMddHHmmss" 的字串
            if (datetimeStr.Length == 14 && DateTime.TryParseExact(
                datetimeStr,
                "yyyyMMddHHmmss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var customDt))
            {
                return customDt;
            }

            return null;
        }

        /// <summary>
        /// 創建單一物流狀態記錄
        /// </summary>
        public async Task<LogisticStatusRecordDto> CreateAsync(CreateLogisticStatusRecordDto input)
        {
            var record = new LogisticStatusRecord
            {
                OrderId = input.OrderId,
                Reference = input.Reference,
                Location = input.Location,
                Datetime = input.Datetime,
                DatetimeParsed = !string.IsNullOrEmpty(input.Datetime) ? TryParseCustomDateTime(input.Datetime) : null,
                Code = input.Code,
                StatusCode = input.StatusCode,
                StatusMessage = input.StatusMessage,
                RawLine = input.RawLine,
                CreateTime = DateTime.UtcNow
            };

            await _logisticStatusRecordRepository.InsertAsync(record);

            return ObjectMapper.Map<LogisticStatusRecord, LogisticStatusRecordDto>(record);
        }

        /// <summary>
        /// 批量創建物流狀態記錄
        /// </summary>
        public async Task<List<LogisticStatusRecordDto>> CreateManyAsync(CreateLogisticStatusRecordListDto input)
        {
            var records = input.Records.Select(r => new LogisticStatusRecord
            {
                OrderId = r.OrderId,
                Reference = r.Reference,
                Location = r.Location,
                Datetime = r.Datetime,
                DatetimeParsed = !string.IsNullOrEmpty(r.Datetime) ? TryParseCustomDateTime(r.Datetime) : null,
                Code = r.Code,
                StatusCode = r.StatusCode,
                StatusMessage = r.StatusMessage,
                RawLine = r.RawLine,
                CreateTime = DateTime.UtcNow
            }).ToList();

            await _logisticStatusRecordRepository.InsertManyAsync(records);

            return records.Select(r => ObjectMapper.Map<LogisticStatusRecord, LogisticStatusRecordDto>(r)).ToList();
        }

        public async Task Read()
        {
            await _tcatSftpService.ExecuteAsync();
        }
    }
}
