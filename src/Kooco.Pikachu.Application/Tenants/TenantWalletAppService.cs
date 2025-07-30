using AutoMapper;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using Kooco.Pikachu.Tenants.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants;

[Authorize]
public class TenantWalletAppService : PikachuAppService, ITenantWalletAppService
{
    private readonly IRepository<TenantWalletTransaction, Guid> _transactionRepository;
    private readonly IEmailAppService _emailAppService;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRepository<TenantWallet, Guid> _tenantWalletRepository;
    public TenantWalletAppService(IRepository<TenantWalletTransaction, Guid> transactionRepository, IEmailAppService emailAppService, ITenantRepository tenantRepository, IRepository<TenantWallet, Guid> tenantWalletRepository)
    {
        _transactionRepository = transactionRepository;
        _emailAppService = emailAppService;
        _tenantRepository = tenantRepository;
        _tenantWalletRepository = tenantWalletRepository;


    }
    [RemoteService(false)]
    public async Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import)
    {
        var transaction = ObjectMapper.Map<CreateWalletTransactionDto, TenantWalletTransaction>(import);
        await TenantWalletRepository.InsertAsync(transaction);

        var wallet = await TenantWalletRepository.GetAsync(walletId);
        wallet.WalletBalance += amount;

        await TenantWalletRepository.UpdateAsync(wallet);

        if (import.IsEmailNotificationEnabled)
        {
            var tenant = await _tenantRepository.GetAsync(wallet.TenantId.Value);
            await _emailAppService.SendWalletRechargeEmailAsync(
                tenant.GetProperty<string>("TenantContactEmail"),
                tenant.Name,
                transaction.TransactionAmount,
                 transaction.TransactionType.ToString(),
                wallet.WalletBalance);
        }
    }

    [RemoteService(false)]
    public async Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import)
    {
        var transaction = ObjectMapper.Map<CreateWalletTransactionDto, TenantWalletTransaction>(import);
        await TenantWalletRepository.InsertAsync(transaction);

        var wallet = await TenantWalletRepository.GetAsync(walletId);
        wallet.WalletBalance -= amount;

        await TenantWalletRepository.UpdateAsync(wallet);
        if (import.IsEmailNotificationEnabled)
        {
            var tenant = await _tenantRepository.GetAsync(wallet.TenantId.Value);
            await _emailAppService.SendWalletRechargeEmailAsync(
                tenant.GetProperty<string>("TenantContactEmail"),
                tenant.Name,
                transaction.TransactionAmount,
                 transaction.TransactionType.ToString(),
                wallet.WalletBalance);
        }
    }
    [RemoteService(false)]
    public async Task<List<TenantWalletTransactionDto>> GetWalletTransactionsAsync(Guid walletId)
    {
        var sixMonthsAgo = Clock.Now.AddMonths(-6);

        var transactions = await (await _transactionRepository.GetQueryableAsync())
            .Where(x => x.TenantWalletId == walletId && x.CreationTime >= sixMonthsAgo)
            .OrderBy(x => x.CreationTime) // Ascending for correct balance
            .ToListAsync();

        decimal runningBalance = 0;
        var list = new List<TenantWalletTransactionDto>();

        foreach (var tx in transactions)
        {
            var isDeposit = tx.TransactionType.ToString().ToLowerInvariant() == "deposit";
            var amount = tx.TransactionAmount;

            // Add or subtract based on TransactionType
            if (isDeposit)
                runningBalance += amount;
            else
                runningBalance -= amount;

            list.Add(new TenantWalletTransactionDto
            {
                Id = tx.Id,
                Timestamp = tx.CreationTime,
                TransactionNo = tx.Id.ToString(),
                TransactionType = tx.TransactionType,
                TransactionStatus = tx.DeductionStatus,
                Amount = isDeposit ? amount : -amount, // optional for display (e.g., +500 / -500)
                Balance = runningBalance,
                Note = tx.TransactionNotes
            });
        }

        // Latest first for display
        return list.OrderByDescending(x => x.Timestamp).ToList();
    }
    public async Task<IRemoteStreamContent> ExportWalletTransactionsAsync(Guid walletId, List<Guid>? selectedIds = null)
    {
        var sixMonthsAgo = Clock.Now.AddMonths(-6);

        var query = await _transactionRepository.GetQueryableAsync();
        var transactions = await query
            .Where(x => x.TenantWalletId == walletId && x.CreationTime >= sixMonthsAgo)
            .OrderBy(x => x.CreationTime)
            .ToListAsync();

        if (selectedIds != null && selectedIds.Any())
            transactions = transactions.Where(x => selectedIds.Contains(x.Id)).ToList();

        decimal runningBalance = 0;
        var records = new List<TenantWalletTransactionDto>();

        foreach (var tx in transactions)
        {
            var isDeposit = tx.TransactionType.ToString().ToLowerInvariant() == "deposit";
            var amount = tx.TransactionAmount;

            runningBalance += isDeposit ? amount : -amount;

            records.Add(new TenantWalletTransactionDto
            {
                Id = tx.Id,
                Timestamp = tx.CreationTime,
                TransactionNo = tx.Id.ToString(),
                TransactionType = tx.TransactionType,
                TransactionStatus = tx.DeductionStatus,
                Amount = isDeposit ? amount : -amount,
                Balance = runningBalance,
                Note = tx.TransactionNotes
            });
        }

        var csvBuilder = new StringBuilder();

        // Localized headers
        csvBuilder.AppendLine($"{L["Timestamp"]},{L["TransactionNo"]},{L["TransactionType"]},{L["TransactionStatus"]},{L["Amount"]},{L["Balance"]},{L["Note"]}");

        foreach (var item in records)
        {
            // Escape any commas or newlines inside notes
            var note = item.Note?.Replace("\"", "\"\""); // CSV escaping
            csvBuilder.AppendLine($"\"{item.Timestamp:u}\",\"{item.TransactionNo}\",\"{L[item.TransactionType.ToString()]}\",\"{L[item.TransactionStatus.ToString()]}\",\"{item.Amount}\",\"{item.Balance}\",\"{note}\"");
        }

        var fileName = $"WalletTransactions_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        var fileStream = new MemoryStream(fileBytes);

        return new RemoteStreamContent(fileStream, fileName, "text/csv");
    }
    [RemoteService(false)]
    public async Task<List<TenantWalletTransactionDto>> GetWalletTransactionsByTenantIdAsync(Guid tenantId)
    {
        var sixMonthsAgo = Clock.Now.AddMonths(-6);
        var walletId = await (await _tenantWalletRepository.GetQueryableAsync()).Where(x => x.TenantId == tenantId).Select(x => x.Id).FirstOrDefaultAsync();
        var transactions = await (await _transactionRepository.GetQueryableAsync())
            .Where(x => x.TenantWalletId == walletId && x.CreationTime >= sixMonthsAgo)
            .OrderBy(x => x.CreationTime) // Ascending for correct balance
            .ToListAsync();

        decimal runningBalance = 0;
        var list = new List<TenantWalletTransactionDto>();

        foreach (var tx in transactions)
        {
            var isDeposit = tx.TransactionType.ToString().ToLowerInvariant() == "deposit";
            var amount = tx.TransactionAmount;

            // Add or subtract based on TransactionType
            if (isDeposit)
                runningBalance += amount;
            else
                runningBalance -= amount;

            list.Add(new TenantWalletTransactionDto
            {
                Id = tx.Id,
                Timestamp = tx.CreationTime,
                TransactionNo = tx.Id.ToString(),
                TransactionType = tx.TransactionType,
                TransactionStatus = tx.DeductionStatus,
                Amount = isDeposit ? amount : -amount, // optional for display (e.g., +500 / -500)
                Balance = runningBalance,
                Note = tx.TransactionNotes
            });
        }

        // Latest first for display
        return list.OrderByDescending(x => x.Timestamp).ToList();
    }
    public async Task<IRemoteStreamContent> ExportWalletTransactionsByTenantIdAsync(Guid tenantId, List<Guid>? selectedIds = null)
    {
        var sixMonthsAgo = Clock.Now.AddMonths(-6);
        var walletId = await (await _tenantWalletRepository.GetQueryableAsync()).Where(x => x.TenantId == tenantId).Select(x => x.Id).FirstOrDefaultAsync();
        var query = await _transactionRepository.GetQueryableAsync();
        var transactions = await query
            .Where(x => x.TenantWalletId == walletId && x.CreationTime >= sixMonthsAgo)
            .OrderBy(x => x.CreationTime)
            .ToListAsync();

        if (selectedIds != null && selectedIds.Any())
            transactions = transactions.Where(x => selectedIds.Contains(x.Id)).ToList();

        decimal runningBalance = 0;
        var records = new List<TenantWalletTransactionDto>();

        foreach (var tx in transactions)
        {
            var isDeposit = tx.TransactionType.ToString().ToLowerInvariant() == "deposit";
            var amount = tx.TransactionAmount;

            runningBalance += isDeposit ? amount : -amount;

            records.Add(new TenantWalletTransactionDto
            {
                Id = tx.Id,
                Timestamp = tx.CreationTime,
                TransactionNo = tx.Id.ToString(),
                TransactionType = tx.TransactionType,
                TransactionStatus = tx.DeductionStatus,
                Amount = isDeposit ? amount : -amount,
                Balance = runningBalance,
                Note = tx.TransactionNotes
            });
        }

        var csvBuilder = new StringBuilder();

        // Localized headers
        csvBuilder.AppendLine($"{L["Timestamp"]},{L["TransactionNo"]},{L["TransactionType"]},{L["TransactionStatus"]},{L["Amount"]},{L["Balance"]},{L["Note"]}");

        foreach (var item in records)
        {
            // Escape any commas or newlines inside notes
            var note = item.Note?.Replace("\"", "\"\""); // CSV escaping
            csvBuilder.AppendLine($"\"{item.Timestamp:u}\",\"{item.TransactionNo}\",\"{L[item.TransactionType.ToString()]}\",\"{L[item.TransactionStatus.ToString()]}\",\"{item.Amount}\",\"{item.Balance}\",\"{note}\"");
        }

        var fileName = $"WalletTransactions_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        var fileStream = new MemoryStream(fileBytes);

        return new RemoteStreamContent(fileStream, fileName, "text/csv");
    }
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<CreateWalletTransactionDto, TenantWalletTransaction>();
        }
    }

    public required ITenantWalletRepository TenantWalletRepository { get; init; }
}