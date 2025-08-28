using AngleSharp.Text;
using AutoMapper.Internal.Mappers;
using Kooco.Pikachu.LogisticsFeeManagements.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Volo.Abp;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Http;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Requests;
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.AzureStorage.LogisticsFiles;
using Hangfire;
using Kooco.Pikachu.TierManagement;
using Volo.Abp.Data;
using Volo.Abp.Content;
using Kooco.Pikachu.Permissions;
using static System.Net.WebRequestMethods;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    [Authorize(PikachuPermissions.LogisticsFeeManagement.Default)]
    public class LogisticsFeeAppService : ApplicationService, ILogisticsFeeAppService
    {
        private readonly ILogisticsFeeFileImportRepository _fileImportRepository;
        private readonly ITenantLogisticsFeeRecordRepository _recordRepository;
        private readonly ITenantLogisticsFeeFileProcessingSummaryRepository _summaryRepository;
        private readonly ITenantWalletAppService _walletDeductionService;
        private readonly ILogisticsFeeNotificationService _notificationService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ILogger<LogisticsFeeAppService> _logger;
        private readonly IRepository<TenantWallet, Guid> _tenantWalletRepository;
        private readonly LogisticsFileContainerManager _logisticsFileContainerManager;
        private readonly LogisticsFeeProcessingJob _logisticsFeeProcessingJob;
        private readonly IDataFilter _dataFilter;
        public LogisticsFeeAppService(
            ILogisticsFeeFileImportRepository fileImportRepository,
            ITenantLogisticsFeeRecordRepository recordRepository,
            ITenantLogisticsFeeFileProcessingSummaryRepository summaryRepository,
            ITenantWalletAppService walletDeductionService,
            ILogisticsFeeNotificationService notificationService,
            IBackgroundJobManager backgroundJobManager,
            ILogger<LogisticsFeeAppService> logger,
            IRepository<TenantWallet, Guid> tenantWalletRepository,
             LogisticsFileContainerManager logisticsFileContainerManager,
             LogisticsFeeProcessingJob logisticsFeeProcessingJob,
             IDataFilter dataFilter)
        {
            _fileImportRepository = fileImportRepository;
            _recordRepository = recordRepository;
            _summaryRepository = summaryRepository;
            _walletDeductionService = walletDeductionService;
            _notificationService = notificationService;
            _backgroundJobManager = backgroundJobManager;
            _logger = logger;
            _tenantWalletRepository = tenantWalletRepository;
            _logisticsFileContainerManager = logisticsFileContainerManager;
            _logisticsFeeProcessingJob = logisticsFeeProcessingJob;
            _dataFilter = dataFilter;

        }


        public async Task<FileUploadResult> UploadFileAsync(IRemoteStreamContent file, LogisticsFileType fileType, bool isMailSend)
        {
            if (file == null || file.ContentLength == 0)
            {
                throw new UserFriendlyException("File is required");
            }

            var allowedExtensions = new[] { ".csv", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new UserFriendlyException("Only CSV and XLSX files are supported");
            }

            await using var input = file.GetStream();   // forward-only stream
            using var ms = new MemoryStream();
            await input.CopyToAsync(ms);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = await _logisticsFileContainerManager.SaveAsync(fileName, ms.ToArray());


            // Create file import record
            var fileImport = new LogisticsFeeFileImport(
                GuidGenerator.Create(),
                fileName,
                file.FileName,
                filePath,
                fileType,
                CurrentUser.GetId()
            );

            await _fileImportRepository.InsertAsync(fileImport, autoSave: true);

            //var arg = new LogisticsFeeProcessingJobArgs
            //{
            //    BatchId = fileImport.Id,
            //    IsMailSend = isMailSend
            //};
            //// Queue background job for processing
            ////await _backgroundJobManager.EnqueueAsync<Guid>(fileImport.Id);
            ////var JobId = BackgroundJob.Schedule<LogisticsFeeProcessingJob>(
            ////           job => job.ExecuteAsync(arg),
            ////            DateTimeOffset.Now
            ////           );

            //await _logisticsFeeProcessingJob.ExecuteAsync(arg);
            _logger.LogInformation("File uploaded and queued for processing: {FileId}", fileImport.Id);

            return new FileUploadResult
            {
                BatchId = fileImport.Id,
                Message = "File uploaded successfully and queued for processing"
            };
        }

        public async Task<PagedResultDto<LogisticsFeeFileImportDto>> GetFileImportsAsync(GetLogisticsFeeFileImportsInput input)
        {
            var totalCount = await _fileImportRepository.GetCountAsync(
                input.Filter,
                input.FileType,
                input.Status,
                input.StartDate,
                input.EndDate
            );

            var items = await _fileImportRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                input.FileType,
                input.Status,
                input.StartDate,
                input.EndDate
            );

            var dtos = ObjectMapper.Map<List<LogisticsFeeFileImport>, List<LogisticsFeeFileImportDto>>(items);

            return new PagedResultDto<LogisticsFeeFileImportDto>(totalCount, dtos);
        }

        public async Task<LogisticsFeeFileImportDto> GetFileImportAsync(Guid id)
        {
            var fileImport = await _fileImportRepository.GetWithDetailsAsync(id);
            var dto = ObjectMapper.Map<LogisticsFeeFileImport, LogisticsFeeFileImportDto>(fileImport);

            // Load tenant summaries
            var summaries = fileImport.LogisticsFeeTenantSummaries;
            dto.TenantSummaries = new List<TenantLogisticsFeeFileProcessingSummaryDto>();
            dto.TenantSummaries = ObjectMapper.Map<List<TenantLogisticsFeeFileProcessingSummary>, List<TenantLogisticsFeeFileProcessingSummaryDto>>(summaries.ToList());

            return dto;
        }


        public async Task DeleteFileImportAsync(Guid id)
        {
            var fileImport = await _fileImportRepository.GetAsync(id);

            // Delete physical file
            if (System.IO.File.Exists(fileImport.FilePath))
            {
                System.IO.File.Delete(fileImport.FilePath);
            }

            await _fileImportRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<TenantLogisticsFeeRecordDto>> GetRecordsAsync(GetTenantLogisticsFeeRecordsInput input)
        {
            var totalCount = await _recordRepository.GetCountAsync(
                input.Filter,
                input.TenantId,
                input.FileImportId,
                input.FileType,
                input.Status
            );

            var items = await _recordRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                input.TenantId,
                input.FileImportId,
                input.FileType,
                input.Status
            );

            var dtos = ObjectMapper.Map<List<TenantLogisticsFeeRecord>, List<TenantLogisticsFeeRecordDto>>(items);

            return new PagedResultDto<TenantLogisticsFeeRecordDto>(totalCount, dtos);
        }
        public async Task<(int, int)> GetStatusRecordCount(GetTenantLogisticsFeeRecordsInput input)
        {
            var items = await _recordRepository.GetListAsync(
                 input.SkipCount,
                 input.MaxResultCount,
                 input.Sorting,
                 input.Filter,
                 input.TenantId,
                 input.FileImportId,
                 input.FileType,
                 input.Status
             );
            return (items.Count(x => x.DeductionStatus == WalletDeductionStatus.Completed), items.Count(x => x.DeductionStatus == WalletDeductionStatus.Failed));
        }
        public async Task<TenantLogisticsFeeRecordDto> GetRecordAsync(Guid id)
        {
            var record = await _recordRepository.GetAsync(id);
            return ObjectMapper.Map<TenantLogisticsFeeRecord, TenantLogisticsFeeRecordDto>(record);
        }


        public async Task<RetryRecordResult> RetryRecordAsync(Guid recordId)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var record = await _recordRepository.GetAsync(recordId);
                await _recordRepository.EnsurePropertyLoadedAsync(record, x => x.LogisticsFeeFileImport);
                if (record.DeductionStatus == WalletDeductionStatus.Completed)
                {
                    return new RetryRecordResult
                    {
                        RecordId = recordId,
                        Success = false,
                        Reason = "Record already successfully processed"
                    };
                }
                if (record.DeductionDate is null && record.LogisticsFeeFileImport?.FileType == LogisticsFileType.ECPay)
                {
                    record.MarkAsFailed("Missing Deduction Date in Logistic File");
                    await _recordRepository.UpdateAsync(record);

                    return new RetryRecordResult
                    {
                        RecordId = recordId,
                        Success = false,
                        Reason = "Deduction Date Null"
                    };

                }
                var tenantWallet = (await _tenantWalletRepository.GetQueryableAsync()).Where(x => x.TenantId == record.TenantId).FirstOrDefault();
                if (tenantWallet.WalletBalance < record.LogisticFee || tenantWallet.WalletBalance == 0)
                {

                    record.MarkAsFailed("Insufficient Balance");
                    await _recordRepository.UpdateAsync(record);

                    return new RetryRecordResult
                    {
                        RecordId = recordId,
                        Success = false,
                        Reason = ""
                    };

                }
                var transaction = new CreateWalletTransactionDto
                {
                    TenantWalletId = tenantWallet.Id,
                    TransactionAmount = Math.Ceiling(record.LogisticFee),
                    TransactionType = WalletTransactionType.LogisticsFeeDeduction,
                    TransactionNotes = $"Logistics fee deduction for order {record.OrderNumber}",
                    DeductionStatus = WalletDeductionStatus.Completed,
                    TradingMethods = WalletTradingMethods.LogisticsFee


                };
                var deductionResult = await _walletDeductionService.AddDeductionTransactionAsync(
                    tenantWallet.Id,
                    Math.Ceiling(record.LogisticFee),
                  transaction
                );

                if (deductionResult.TransactionStatus == WalletDeductionStatus.Completed && deductionResult.Id != Guid.Empty)
                {
                    record.LogisticsFeeFileImport.SuccessfulRecords += 1;
                    record.LogisticsFeeFileImport.FailedRecords -= 1;
                    await _fileImportRepository.UpdateAsync(record.LogisticsFeeFileImport);
                    record.MarkAsDeducted(deductionResult.Id);
                    await _recordRepository.UpdateAsync(record);

                    _logger.LogInformation("Successfully retried record: {RecordId}", recordId);
                    var file = await _fileImportRepository.GetAsync(record.FileImportId);
                    if (file.SuccessfulRecords == file.TotalRecords)
                    {
                        file.SuccessProcessing("");
                    }
                    else if (file.SuccessfulRecords > 0)
                    {
                        file.PartialSuccessProcessing("");
                    }

                    await _notificationService.SendRetryNotificationAsync(record.TenantId.Value, new BatchRetryResult
                    {
                        SuccessCount = 1,
                        SuccessfulAmount = record.LogisticFee,
                        FileName = file.OriginalFileName,
                        FileType = file.FileType.ToString(),
                        FailureCount = 0

                    });
                    var tenantSummery = (await _summaryRepository.GetQueryableAsync()).FirstOrDefault(x => x.TenantId == record.TenantId && x.FileImportId == record.FileImportId);
                    tenantSummery.TenantSuccessfulRecords += 1;
                    tenantSummery.TenantFailedRecords -= 1;
                    await _summaryRepository.UpdateAsync(tenantSummery);
                    return new RetryRecordResult
                    {
                        RecordId = recordId,
                        Success = true
                    };
                }
                else
                {
                    record.MarkAsFailed("Retry failed");
                    await _recordRepository.UpdateAsync(record);
                    var file = await _fileImportRepository.GetAsync(record.FileImportId);
                    if (file.SuccessfulRecords == file.TotalRecords)
                    {
                        file.SuccessProcessing("");
                    }
                    else if (file.SuccessfulRecords > 0)
                    {
                        file.PartialSuccessProcessing("");
                    }

                    await _notificationService.SendRetryNotificationAsync(record.TenantId.Value, new BatchRetryResult
                    {
                        SuccessCount = 0,
                        SuccessfulAmount = 0,
                        FileName = file.OriginalFileName,
                        FileType = file.FileType.ToString(),
                        FailureCount = 1

                    });
                    return new RetryRecordResult
                    {
                        RecordId = recordId,
                        Success = false,
                        Reason = ""
                    };
                }


            }
        }


        public async Task<RetryBatchResult> RetryBatchAsync(RetryBatchInput input)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                if (input.RecordIds == null || !input.RecordIds.Any())
                {
                    throw new UserFriendlyException("No records selected for retry");
                }

                var records = await _recordRepository.GetByIdsAsync(input.RecordIds);

                var result = new RetryBatchResult();
                int s = 0;
                int f = 0;
                var tenantNotifications = new Dictionary<Guid, BatchRetryResult>();
                Guid fileId = records.Select(x => x.FileImportId).FirstOrDefault();
                foreach (var record in records)
                {
                    if (record.DeductionStatus == WalletDeductionStatus.Pending)
                    {
                        result.Results.Add(new RetryRecordResult
                        {
                            RecordId = record.Id,
                            Success = false,
                            Reason = "Already processed"
                        });
                        continue;
                    }
                    if (record.DeductionDate is null && record.LogisticsFeeFileImport?.FileType == LogisticsFileType.ECPay)
                    {
                        // Initialize tenant notification tracking
                        if (!tenantNotifications.ContainsKey(record.TenantId.Value))
                        {
                            tenantNotifications[record.TenantId.Value] = new BatchRetryResult();
                        }

                        var tenantResult = tenantNotifications[record.TenantId.Value];

                        record.MarkAsFailed("Missing Deduction Date in Logistic File");
                        result.FailureCount++;
                        tenantResult.FailureCount++;
                        tenantResult.FileName = record.LogisticsFeeFileImport.OriginalFileName;
                        tenantResult.FileType = record.LogisticsFeeFileImport.FileType.ToString();



                        result.Results.Add(new RetryRecordResult
                        {
                            RecordId = record.Id,
                            Success = false,
                            Reason = "Missing Deduction Date in Logistic File"
                        });
                    }
                    else
                    {
                        var tenantWallet = (await _tenantWalletRepository.GetQueryableAsync()).Where(x => x.TenantId == record.TenantId).FirstOrDefault();
                        if (tenantWallet.WalletBalance < record.LogisticFee || tenantWallet.WalletBalance == 0)
                        {
                            // Initialize tenant notification tracking
                            if (!tenantNotifications.ContainsKey(record.TenantId.Value))
                            {
                                tenantNotifications[record.TenantId.Value] = new BatchRetryResult();
                            }

                            var tenantResult = tenantNotifications[record.TenantId.Value];
                            tenantResult.FileName = record.LogisticsFeeFileImport.OriginalFileName;
                            tenantResult.FileType = record.LogisticsFeeFileImport.FileType.ToString();
                            record.LogisticsFeeFileImport.FailedRecords = f + 1;
                            await _fileImportRepository.UpdateAsync(record.LogisticsFeeFileImport);
                            record.MarkAsFailed("Insufficient Balance");
                            result.FailureCount++;
                            tenantResult.FailureCount++;
                            tenantResult.FileName = record.LogisticsFeeFileImport.OriginalFileName;
                            tenantResult.FileType = record.LogisticsFeeFileImport.FileType.ToString();



                            result.Results.Add(new RetryRecordResult
                            {
                                RecordId = record.Id,
                                Success = false,
                                Reason = ""
                            });

                        }
                        else
                        {
                            var transaction = new CreateWalletTransactionDto
                            {
                                TenantWalletId = tenantWallet.Id,
                                TransactionAmount = Math.Ceiling(record.LogisticFee),
                                TransactionType = WalletTransactionType.LogisticsFeeDeduction,
                                TransactionNotes = $"Logistics fee deduction for order {record.OrderNumber}",
                                DeductionStatus = WalletDeductionStatus.Completed,
                                TradingMethods = WalletTradingMethods.LogisticsFee


                            };

                            var deductionResult = await _walletDeductionService.AddDeductionTransactionAsync(
                                tenantWallet.Id,
                                Math.Ceiling(record.LogisticFee),
                                transaction
                            );

                            // Initialize tenant notification tracking
                            if (!tenantNotifications.ContainsKey(record.TenantId.Value))
                            {
                                tenantNotifications[record.TenantId.Value] = new BatchRetryResult();
                            }

                            var tenantResult = tenantNotifications[record.TenantId.Value];
                            tenantResult.FileName = record.LogisticsFeeFileImport.OriginalFileName;
                            tenantResult.FileType = record.LogisticsFeeFileImport.FileType.ToString();

                            if (deductionResult.TransactionStatus == WalletDeductionStatus.Completed && deductionResult.Id != Guid.Empty)
                            {
                                record.MarkAsDeducted(deductionResult.Id);
                                result.SuccessCount++;
                                tenantResult.SuccessCount++;
                                tenantResult.SuccessfulAmount += record.LogisticFee;
                                record.LogisticsFeeFileImport.SuccessfulRecords += 1;
                                record.LogisticsFeeFileImport.FailedRecords -= 1;


                                result.Results.Add(new RetryRecordResult
                                {
                                    RecordId = record.Id,
                                    Success = true
                                });
                            }
                            else
                            {
                                record.MarkAsFailed("Retry failed");
                                result.FailureCount++;
                                tenantResult.FailureCount++;
                                tenantResult.FileName = record.LogisticsFeeFileImport.OriginalFileName;
                                tenantResult.FileType = record.LogisticsFeeFileImport.FileType.ToString();



                                result.Results.Add(new RetryRecordResult
                                {
                                    RecordId = record.Id,
                                    Success = false,
                                    Reason = ""
                                });
                            }
                        }
                    }

                    await _recordRepository.UpdateAsync(record);
                }

                // Send notifications to each affected tenant
                foreach (var kvp in tenantNotifications)
                {
                    try
                    {
                        var tenantSummery = (await _summaryRepository.GetQueryableAsync()).FirstOrDefault(x => x.TenantId == kvp.Key && x.FileImportId == fileId);
                        tenantSummery.TenantSuccessfulRecords += kvp.Value.SuccessCount;
                        tenantSummery.TenantFailedRecords -= kvp.Value.SuccessCount;
                        await _summaryRepository.UpdateAsync(tenantSummery);
                        await _notificationService.SendRetryNotificationAsync(kvp.Key, kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send retry notification to tenant: {TenantId}", kvp.Key);
                    }
                }

                _logger.LogInformation("Batch retry completed. Success: {SuccessCount}, Failed: {FailureCount}",
                    result.SuccessCount, result.FailureCount);
                var file = await _fileImportRepository.GetAsync(fileId);

                if (file.SuccessfulRecords == (file.SuccessfulRecords + file.FailedRecords))
                {
                    file.SuccessProcessing("");
                }
                else if (file.SuccessfulRecords > 0)
                {
                    file.PartialSuccessProcessing("");
                }
                return result;
            }
        }

        public async Task<PagedResultDto<TenantLogisticsFeeFileProcessingSummaryDto>> GetTenantSummariesAsync(
            Guid? tenantId = null,
            int skipCount = 0,
            int maxResultCount = 1000)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var tenantSummaries = await _summaryRepository.GetByTenantIdAsync(tenantId, skipCount, maxResultCount);
                var tenantDtos = ObjectMapper.Map<List<TenantLogisticsFeeFileProcessingSummary>, List<TenantLogisticsFeeFileProcessingSummaryDto>>(tenantSummaries);
                return new PagedResultDto<TenantLogisticsFeeFileProcessingSummaryDto>(tenantSummaries.Count, tenantDtos);
            }


        }

        public async Task<List<TenantLogisticsFeeRecordDto>> GetFailedRecordsAsync(Guid? fileImportId = null)
        {
            var failedRecords = await _recordRepository.GetFailedRecordsAsync(fileImportId, CurrentTenant?.Id);
            return ObjectMapper.Map<List<TenantLogisticsFeeRecord>, List<TenantLogisticsFeeRecordDto>>(failedRecords);
        }

        public async Task<Dictionary<string, object>> GetDashboardStatsAsync()
        {
            var stats = new Dictionary<string, object>();

            try
            {
                // Total files processed today
                var today = DateTime.Today;
                var todayFilesCount = await _fileImportRepository.GetCountAsync(
                    startDate: today,
                    endDate: today.AddDays(1)
                );

                // Pending files
                var pendingFilesCount = await _fileImportRepository.GetCountAsync(
                    status: EnumValues.FileProcessingStatus.Processing
                );

                // Failed records count
                var failedRecordsCount = await _recordRepository.GetCountAsync(
                    status: TenantManagement.WalletDeductionStatus.Failed
                );

                // Recent activity
                var recentFiles = await _fileImportRepository.GetListAsync(0, 5, "UploadDate desc");

                stats.Add("todayFilesProcessed", todayFilesCount);
                stats.Add("pendingFiles", pendingFilesCount);
                stats.Add("failedRecords", failedRecordsCount);
                stats.Add("recentActivity", ObjectMapper.Map<List<LogisticsFeeFileImport>, List<LogisticsFeeFileImportDto>>(recentFiles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                stats.Add("error", "Unable to retrieve statistics");
            }

            return stats;
        }
    }
}
