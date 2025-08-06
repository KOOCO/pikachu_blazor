using Kooco.Pikachu.EnumValues;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public interface ILogisticsFeeAppService : IApplicationService
    {
        // File Import Management
        Task<FileUploadResult> UploadFileAsync(IFormFile file, LogisticsFileType fileType);
        Task<PagedResultDto<LogisticsFeeFileImportDto>> GetFileImportsAsync(GetLogisticsFeeFileImportsInput input);
        Task<LogisticsFeeFileImportDto> GetFileImportAsync(Guid id);
        Task DeleteFileImportAsync(Guid id);

        // Records Management
        Task<PagedResultDto<TenantLogisticsFeeRecordDto>> GetRecordsAsync(GetTenantLogisticsFeeRecordsInput input);
        Task<TenantLogisticsFeeRecordDto> GetRecordAsync(Guid id);
        Task<RetryRecordResult> RetryRecordAsync(Guid recordId);
        Task<RetryBatchResult> RetryBatchAsync(RetryBatchInput input);

        // Tenant Summary
        Task<PagedResultDto<TenantLogisticsFeeFileProcessingSummaryDto>> GetTenantSummariesAsync(
            Guid? tenantId = null,
            int skipCount = 0,
            int maxResultCount = 10
        );

        // Dashboard/Statistics
        Task<List<TenantLogisticsFeeRecordDto>> GetFailedRecordsAsync(Guid? fileImportId = null);
        Task<Dictionary<string, object>> GetDashboardStatsAsync();
    }

}
