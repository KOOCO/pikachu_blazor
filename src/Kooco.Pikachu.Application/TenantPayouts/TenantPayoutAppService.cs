using CsvHelper;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.TenantPaymentFees;
using Kooco.Pikachu.Tenants.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.TenantPayouts;

[Authorize(PikachuPermissions.TenantPayouts.Default)]
public class TenantPayoutAppService : PikachuAppService, ITenantPayoutAppService
{
    private readonly ITenantPayoutRepository _tenantPayoutRepository;
    private readonly IRepository<TenantWallet, Guid> _tenantWalletRepository;

    public TenantPayoutAppService(
        ITenantPayoutRepository tenantPayoutRepository,
        IRepository<TenantWallet, Guid> tenantWalletRepository
        )
    {
        _tenantPayoutRepository = tenantPayoutRepository;
        _tenantWalletRepository = tenantWalletRepository;
    }

    public async Task<PagedResultDto<TenantPayoutSummaryDto>> GetTenantSummariesAsync(GetTenantSummariesDto input, CancellationToken cancellationToken = default)
    {
        var summaries = await _tenantPayoutRepository
            .GetTenantSummariesAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                cancellationToken
                );

        return new PagedResultDto<TenantPayoutSummaryDto>
        {
            TotalCount = summaries.TotalCount,
            Items = ObjectMapper.Map<List<TenantPayoutSummary>, List<TenantPayoutSummaryDto>>(summaries.Items)
        };
    }

    public Task<List<PaymentFeeType>> GetActivePaymentProvidersAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return _tenantPayoutRepository.GetActivePaymentProvidersAsync(tenantId, cancellationToken);
    }

    public async Task<List<TenantPayoutYearlySummaryDto>> GetTenantPayoutYearlySummariesAsync(Guid tenantId, PaymentFeeType feeType, CancellationToken cancellationToken = default)
    {
        var summaries = await _tenantPayoutRepository.GetTenantPayoutYearlySummariesAsync(tenantId, feeType, cancellationToken);
        return ObjectMapper.Map<List<TenantPayoutYearlySummary>, List<TenantPayoutYearlySummaryDto>>(summaries);
    }

    public async Task<TenantPayoutDetailSummaryDto> GetTenantPayoutDetailSummaryAsync(GetTenantPayoutRecordListDto input, CancellationToken cancellationToken = default)
    {
        var summary = await _tenantPayoutRepository.GetTenantPayoutDetailSummaryAsync(
            input.TenantId,
            input.FeeType,
            input.StartDate,
            input.EndDate,
            input.PaymentMethod,
            input.Filter,
            input.IsPaid,
            cancellationToken
            );
        return ObjectMapper.Map<TenantPayoutDetailSummary, TenantPayoutDetailSummaryDto>(summary);
    }

    public async Task<PagedResultDto<TenantPayoutRecordDto>> GetListAsync(GetTenantPayoutRecordListDto input, CancellationToken cancellationToken = default)
    {
        var result = await _tenantPayoutRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.TenantId,
            input.FeeType,
            input.StartDate,
            input.EndDate,
            input.PaymentMethod,
            input.Filter,
            input.IsPaid,
            cancellationToken
            );

        return new PagedResultDto<TenantPayoutRecordDto>
        {
            TotalCount = result.TotalCount,
            Items = ObjectMapper.Map<List<TenantPayoutRecord>, List<TenantPayoutRecordDto>>(result.Items)
        };
    }

    public Task MarkAsPaidAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return _tenantPayoutRepository.MarkAsPaidAsync(ids, cancellationToken);
    }

    public async Task<int> TransferToWalletAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            int result = 0;

            var payoutRecords = await _tenantPayoutRepository.GetListAsync(r => !r.IsPaid && ids.Contains(r.Id), cancellationToken: cancellationToken);

            var tenantIds = payoutRecords.Select(r => r.TenantId).Distinct().ToList();

            var tenantWallets = await _tenantWalletRepository.GetListAsync(w => w.TenantId.HasValue && tenantIds.Contains(w.TenantId.Value), cancellationToken: cancellationToken);
            var walletMap = tenantWallets.ToDictionary(w => w.TenantId.Value);

            foreach (var record in payoutRecords)
            {
                walletMap.TryGetValue(record.TenantId.Value, out var tenantWallet);
                if (tenantWallet != null)
                {
                    var floorAmount = Math.Floor(record.NetAmount);

                    tenantWallet.WalletBalance += floorAmount;
                    tenantWallet.TenantWalletTransactions ??= [];
                    
                    tenantWallet.TenantWalletTransactions.Add(new TenantWalletTransaction
                    {
                        DeductionStatus = WalletDeductionStatus.Completed,
                        TradingMethods = WalletTradingMethods.SystemInput,
                        TransactionType = WalletTransactionType.Payout,
                        TransactionAmount = floorAmount,
                        TransactionNotes = $"Logistics fee deduction for order {record.OrderNo}",
                        TenantWalletId = tenantWallet.Id
                    });

                    record.SetPaid(true);
                }
                else
                {
                    result = -1;
                }
            }

            return result; 
        }
    }

    public async Task<byte[]> ExportAsync(GetTenantPayoutRecordListDto input, string exportType, List<TenantPayoutRecordDto> selected = null!)
    {
        var records = selected.OrEmptyListIfNull();
        if (records.Count == 0)
        {
            if (input.ExportCurrent)
            {
                var query = await _tenantPayoutRepository
                    .GetFilteredQueryableAsync(
                        input.TenantId,
                        input.FeeType,
                        input.StartDate,
                        input.EndDate,
                        input.PaymentMethod,
                        input.Filter,
                        input.IsPaid
                        );

                records = ObjectMapper.Map<List<TenantPayoutRecord>, List<TenantPayoutRecordDto>>(await query.ToListAsync());
            }
            else
            {
                var startDate = new DateTime(input.Year, 1, 1);
                var endDate = new DateTime(input.Year, 12, 31);

                var query = await _tenantPayoutRepository
                    .GetFilteredQueryableAsync(
                        input.TenantId,
                        input.FeeType,
                        startDate,
                        endDate
                        );

                records = ObjectMapper.Map<List<TenantPayoutRecord>, List<TenantPayoutRecordDto>>(await query.ToListAsync());
            }
        }
        var headers = new Dictionary<string, string>
        {
            { "OrderCreationTime", L["OrderCreationTime"] },
            { "OrderNumber", L["OrderNumber"] },
            { "PaymentType", L["PaymentType"] },
            { "OrderAmount", L["OrderAmount"] },
            { "FeeRate", L["FeeRate"] },
            { "HandlingFee", L["HandlingFee"] },
            { "ProcessingFee", L["ProcessingFee"] },
            { "NetAmount", L["NetAmount"] },
            { "Status", L["Status"] },
            { "PaidOn", L["PaidOn"] }
        };
        var rows = records.Select(x => new Dictionary<string, object>
        {
            { headers["OrderCreationTime"], x.OrderCreationTime },
            { headers["OrderNumber"], x.OrderNo },
            { headers["PaymentType"], L[x.PaymentMethod.ToString()] },
            { headers["OrderAmount"], x.GrossOrderAmount.ToMoneyString() },
            { headers["FeeRate"], x.FeeRate.ToPercentageString() },
            { headers["HandlingFee"], x.HandlingFee.ToMoneyString() },
            { headers["ProcessingFee"], x.ProcessingFee.ToMoneyString() },
            { headers["NetAmount"], x.NetAmount.ToMoneyString() },
            { headers["Status"], x.IsPaid ? L["Paid"] : L["Unpaid"] },
            { headers["PaidOn"], x.PaidTime?.ToShortDateString() }
        });

        if (!rows.Any())
        {
            rows =
            [
                headers.ToDictionary(k => k.Value, k => (object)string.Empty)
            ];
        }

        if (exportType == "Excel")
        {
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(rows);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }
        if (exportType == "Csv")
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var headersList = headers.Values.ToList();
            foreach (var header in headersList)
            {
                csv.WriteField(header);
            }
            csv.NextRecord();

            foreach (var row in rows)
            {
                foreach (var header in headersList)
                {
                    csv.WriteField(row[header]);
                }
                csv.NextRecord();
            }

            writer.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }
        return null!;
    }
}
