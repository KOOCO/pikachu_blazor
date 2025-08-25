using Kooco.Pikachu.LogisticsFeeManagements.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Repositories;
using Kooco.Pikachu.Tenants.Entities;
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.Tenants.Requests;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.OrderTransactions;

using TenantWallet = Kooco.Pikachu.Tenants.Entities.TenantWallet;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Kooco.Pikachu.Emails;
using Hangfire.States;
using Volo.Abp.TenantManagement;
using Kooco.Pikachu.TenantDeliveryFees;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    [BackgroundJobName("logistics_fee_processing")]
    public class LogisticsFeeProcessingJob : AsyncBackgroundJob<LogisticsFeeProcessingJobArgs>, ITransientDependency
    {
        private readonly ILogisticsFeeFileImportRepository _fileImportRepository;
        private readonly ITenantLogisticsFeeRecordRepository _recordRepository;
        private readonly ITenantLogisticsFeeFileProcessingSummaryRepository _summaryRepository;
        private readonly IFileParsingService _fileParsingService;
        private readonly IOrderMatchingService _orderMatchingService;
        private readonly ITenantWalletAppService _walletDeductionService;
        private readonly ILogisticsFeeNotificationService _notificationService;
        private readonly ILogger<LogisticsFeeProcessingJob> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<TenantWallet, Guid> _tenantWalletRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IEmailAppService _emailAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantDeliveryFeeRepository _tenantDeliveryFeeRepository;
        public LogisticsFeeProcessingJob(
            ILogisticsFeeFileImportRepository fileImportRepository,
            ITenantLogisticsFeeRecordRepository recordRepository,
            ITenantLogisticsFeeFileProcessingSummaryRepository summaryRepository,
            IFileParsingService fileParsingService,
            IOrderMatchingService orderMatchingService,
            ITenantWalletAppService walletDeductionService,
            ILogisticsFeeNotificationService notificationService,
            ILogger<LogisticsFeeProcessingJob> logger,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TenantWallet, Guid> tenantWalletRepository,
            IDataFilter dataFilter,
            IEmailAppService emailAppService,
            ITenantRepository tenantRepository,
            ITenantDeliveryFeeRepository tenantDeliveryFeeRepository)
        {
            _fileImportRepository = fileImportRepository;
            _recordRepository = recordRepository;
            _summaryRepository = summaryRepository;
            _fileParsingService = fileParsingService;
            _orderMatchingService = orderMatchingService;
            _walletDeductionService = walletDeductionService;
            _notificationService = notificationService;
            _logger = logger;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantWalletRepository = tenantWalletRepository;
            _dataFilter = dataFilter;
            _emailAppService = emailAppService;
            _tenantRepository = tenantRepository;
            _tenantDeliveryFeeRepository = tenantDeliveryFeeRepository;
        }

        public override async Task ExecuteAsync(LogisticsFeeProcessingJobArgs arg)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            using (_dataFilter.Disable<IMultiTenant>())
            {
                try
                {
                    _logger.LogInformation("Starting logistics fee processing for batch: {BatchId}", arg.BatchId);

                    var fileImport = await _fileImportRepository.GetAsync(arg.BatchId);
                    if (fileImport == null)
                    {
                        _logger.LogWarning("File import not found: {BatchId}", arg.BatchId);
                        return;
                    }

                    // Mark as processing
                    fileImport.StartProcessing();
                    await _fileImportRepository.UpdateAsync(fileImport);
                    await uow.SaveChangesAsync();

                    // Parse the file
                    var parsingResult = await _fileParsingService.ParseFileAsync(fileImport.FilePath, fileImport.FileType);
                    if (!parsingResult.IsSuccess)
                    {
                        fileImport.FailProcessing($"File parsing failed: {parsingResult.ErrorMessage}");
                        await _fileImportRepository.UpdateAsync(fileImport);
                        await uow.CompleteAsync();
                        return;
                    }

                    await _fileImportRepository.UpdateAsync(fileImport);

                    // Process each record
                    var tenantSummaries = new Dictionary<Guid, TenantProcessingSummary>();
                    var recordsToInsert = new List<TenantLogisticsFeeRecord>();

                    foreach (var parsedRecord in parsingResult.Records)
                    {
                        var processedRecord = await ProcessSingleRecordAsync(fileImport, parsedRecord, tenantSummaries);
                        if (processedRecord != null)
                        {
                            recordsToInsert.Add(processedRecord);
                        }
                    }

                    // Bulk insert all records
                    await _recordRepository.InsertManyAsync(recordsToInsert);

                    // Create tenant summaries
                    await CreateTenantSummariesAsync(fileImport.Id, tenantSummaries);

                    fileImport.TotalRecords = recordsToInsert.Count(x => x.TenantId != null);
                    fileImport.TotalAmount = recordsToInsert.Where(x => x.TenantId != null).Sum(x => x.LogisticFee);
                    // Update file import with final statistics
                    UpdateFileImportStatistics(fileImport, tenantSummaries);
                    if (!recordsToInsert.Any(x => x.DeductionStatus == WalletDeductionStatus.Failed))
                    {
                        fileImport.SuccessProcessing("Processing completed successfully");
                    }
                    else if (!recordsToInsert.Any(x => x.DeductionStatus == WalletDeductionStatus.Completed))
                    {
                        fileImport.FailProcessing("Processing failed");


                    }
                    else
                    {
                        fileImport.PartialSuccessProcessing("Processing partial Completed");

                    }
                    await _fileImportRepository.UpdateAsync(fileImport);

                    await uow.CompleteAsync();

                    if (arg.IsMailSend)
                    {
                        foreach (var record in recordsToInsert.DistinctBy(x => x.TenantId))
                        {
                            if (record.TenantId != null && record.TenantId != Guid.Empty)
                            {
                                var tenant = await _tenantRepository.GetAsync(record.TenantId.Value);
                                await _emailAppService.SendLogisticsFeeProcessingEmailAsync(new LogisticsFeeEmailModel
                                {

                                    TenantName = tenant.Name,
                                    Email = tenant.GetProperty<string>("TenantContactEmail"),
                                    FileName = fileImport.OriginalFileName,
                                    TotalRecords = fileImport.TotalRecords,
                                    SuccessfulDeductions = fileImport.SuccessfulRecords,
                                    FailedDeductions = fileImport.FailedRecords,
                                    TotalAmount = recordsToInsert.Where(x => x.TenantId != null && x.DeductionStatus == WalletDeductionStatus.Completed).Sum(x => x.LogisticFee),
                                    ProcessingDate = fileImport.ProcessingCompletedAt ?? DateTime.Now,
                                    FileType = fileImport.FileType.ToString()
                                });
                            }
                        }


                    }

                    _logger.LogInformation("Completed logistics fee processing for batch: {BatchId}", arg.BatchId);
                }
                catch (Exception ex)
                {
                    await uow.RollbackAsync();

                    try
                    {
                        var fileImport = await _fileImportRepository.GetAsync(arg.BatchId);
                        fileImport.FailProcessing($"Processing failed: {ex.Message}");
                        await _fileImportRepository.UpdateAsync(fileImport);
                        await uow.SaveChangesAsync();
                        await uow.CompleteAsync();
                    }
                    catch (Exception updateEx)
                    {
                        _logger.LogError(updateEx, "Failed to update file import status after processing error");
                    }

                    _logger.LogError(ex, "Error processing logistics fee batch: {BatchId}", arg.BatchId);
                    throw;
                }
            }
        }

        private async Task<TenantLogisticsFeeRecord> ProcessSingleRecordAsync(
            LogisticsFeeFileImport fileImport,
            LogisticsFeeRecord parsedRecord,
            Dictionary<Guid, TenantProcessingSummary> tenantSummaries)
        {
            // Find matching order
            var orderMatch = await _orderMatchingService.FindOrderAsync(parsedRecord.MerchantTradeNo);

            if (!orderMatch.IsFound || !orderMatch.TenantId.HasValue)
            {
                var record1 = new TenantLogisticsFeeRecord(
                    Guid.NewGuid(),
                    fileImport.Id,
                    Guid.Empty, // No tenant found
                    parsedRecord.MerchantTradeNo,
                    parsedRecord.FeeAmount,
                    parsedRecord.ShippingFee,
                    parsedRecord.ExtraShippingFee,
                    parsedRecord.AdditionalLogisticFee,
                    fileImport.FileType
                );
                record1.MarkAsFailed("Order not found");

                return record1;
            }

            var tenantId = orderMatch.TenantId.Value;
            var record = new TenantLogisticsFeeRecord(
      Guid.NewGuid(),
      fileImport.Id,
      tenantId,
      orderMatch.OrderNumber ?? parsedRecord.MerchantTradeNo,
      parsedRecord.FeeAmount,
      parsedRecord.ShippingFee,
      parsedRecord.ExtraShippingFee,
      parsedRecord.AdditionalLogisticFee,
      fileImport.FileType
  );
            DeliveryProvider deliveryProvider = fileImport.FileType == EnumValues.LogisticsFileType.ECPay ? DeliveryProvider.ECPay : DeliveryProvider.TCat;

            var tenantDeliveryFee = await _tenantDeliveryFeeRepository.GetByTenantIdAndDeliveryTypeAsync(tenantId, deliveryProvider);
            if (tenantDeliveryFee?.IsEnabled == true)
            {
                decimal additionalFee = 0;

                switch (tenantDeliveryFee.FeeKind)
                {
                    case FeeKind.FixedAmount:
                        additionalFee = tenantDeliveryFee.FixedAmount ?? 0;
                        break;

                    case FeeKind.Percentage:
                        if (tenantDeliveryFee.PercentValue is > 0)
                        {
                            additionalFee = parsedRecord.FeeAmount * tenantDeliveryFee.PercentValue.Value / 100;
                        }
                        break;
                }

                parsedRecord.AdditionalLogisticFee = additionalFee;
                parsedRecord.FeeAmount += additionalFee;

            }

            if (fileImport.FileType == EnumValues.LogisticsFileType.ECPay)
            {
                record.DeductionDate = parsedRecord.DeductionDate.IsNullOrEmpty() ? null : DateTime.Parse(parsedRecord.DeductionDate);
                if (record.DeductionDate is null && fileImport.FileType == EnumValues.LogisticsFileType.ECPay)
                {


                    record = null;
                    return record;
                }

            }

            // Initialize tenant summary if not exists
            if (!tenantSummaries.ContainsKey(tenantId))
            {
                tenantSummaries[tenantId] = new TenantProcessingSummary
                {
                    TenantId = tenantId,
                    TotalRecords = 0,
                    SuccessfulDeductions = 0,
                    FailedDeductions = 0,
                    TotalAmount = 0
                };
            }

            var summary = tenantSummaries[tenantId];
            summary.TotalRecords++;
            summary.TotalAmount += parsedRecord.FeeAmount;
            var tenantWallet = (await _tenantWalletRepository.GetQueryableAsync()).Where(x => x.TenantId == summary.TenantId).FirstOrDefault();

            if (parsedRecord.FeeAmount > tenantWallet.WalletBalance || tenantWallet.WalletBalance == 0)
            {
                record.MarkAsFailed("Insufficient Balance");
                summary.FailedDeductions++;
            }
            else
            {
                // Attempt wallet deduction
                var transaction = new CreateWalletTransactionDto
                {
                    TenantWalletId = tenantWallet.Id,
                    TransactionAmount = parsedRecord.FeeAmount,
                    TransactionType = WalletTransactionType.LogisticsFeeDeduction,
                    TransactionNotes = $"Logistics fee deduction for order {orderMatch.OrderNumber ?? parsedRecord.MerchantTradeNo}",
                    DeductionStatus = WalletDeductionStatus.Completed,
                    TradingMethods = WalletTradingMethods.LogisticsFee


                };
                var deductionResult = await _walletDeductionService.AddDeductionTransactionAsync(
                    tenantWallet.Id,
                    parsedRecord.FeeAmount,
                transaction
                );

                if (deductionResult.TransactionStatus == WalletDeductionStatus.Completed && deductionResult.Id != Guid.Empty)
                {
                    record.MarkAsDeducted(deductionResult.Id);
                    summary.SuccessfulDeductions++;
                }
                else
                {
                    record.MarkAsFailed("Unknown error");
                    summary.FailedDeductions++;
                }
            }
            return record;
        }

        private async Task CreateTenantSummariesAsync(
            Guid fileImportId,
            Dictionary<Guid, TenantProcessingSummary> tenantSummaries)
        {
            var summariesToInsert = new List<TenantLogisticsFeeFileProcessingSummary>();

            foreach (var kvp in tenantSummaries)
            {
                var summary = new TenantLogisticsFeeFileProcessingSummary(
                   Guid.NewGuid(),
                    fileImportId,
                    kvp.Key
                );

                summary.TenantTotalRecords = kvp.Value.TotalRecords;
                summary.TenantSuccessfulRecords = kvp.Value.SuccessfulDeductions;
                summary.TenantFailedRecords = kvp.Value.FailedDeductions;
                summary.TenantTotalAmount = kvp.Value.TotalAmount;

                summariesToInsert.Add(summary);
            }

            if (summariesToInsert.Any())
            {
                await _summaryRepository.InsertManyAsync(summariesToInsert);
            }
        }

        private void UpdateFileImportStatistics(
            LogisticsFeeFileImport fileImport,
            Dictionary<Guid, TenantProcessingSummary> tenantSummaries)
        {
            fileImport.TotalRecords = tenantSummaries.Values.Sum(s => s.TotalRecords);
            fileImport.SuccessfulRecords = tenantSummaries.Values.Sum(s => s.SuccessfulDeductions);
            fileImport.FailedRecords = tenantSummaries.Values.Sum(s => s.FailedDeductions);
        }

        private class TenantProcessingSummary
        {
            public Guid TenantId { get; set; }
            public int TotalRecords { get; set; }
            public int SuccessfulDeductions { get; set; }
            public int FailedDeductions { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
