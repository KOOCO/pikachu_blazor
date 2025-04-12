using Kooco.Pikachu.Tenants.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Tenants.Repositories;

/// <summary>
/// 租戶錢包儲存庫介面
/// </summary>
public interface ITenantWalletRepository
{
    /// <summary>
    /// 插入新的錢包交易記錄
    /// </summary>
    /// <param name="transaction">錢包交易記錄</param>
    /// <param name="ct">取消操作的標記</param>
    Task InsertAsync(TenantWalletTransaction transaction, CancellationToken ct = default);

    /// <summary>
    /// 更新錢包資訊
    /// </summary>
    /// <param name="wallet">錢包實體</param>
    /// <param name="ct">取消操作的標記</param>
    Task UpdateAsync(TenantWallet wallet, CancellationToken ct = default);

    /// <summary>
    /// 根據錢包 ID 獲取錢包資訊
    /// </summary>
    /// <param name="walletId">錢包 ID</param>
    Task<TenantWallet> GetAsync(Guid walletId);

    /// <summary>
    /// 獲取錢包餘額
    /// </summary>
    /// <param name="walletId">錢包 ID</param>
    Task<decimal> GetBalanceAsync(Guid walletId);

    /// <summary>
    /// 分頁獲取所有租戶及其錢包資訊
    /// </summary>
    /// <param name="skipCount">跳過的記錄數</param>
    /// <param name="maxResultCount">最大返回記錄數</param>
    /// <param name="searchTerm">搜尋條件</param>
    /// <param name="ct">取消操作的標記</param>
    Task<(int totalCount, List<(Tenant tenant, TenantWallet wallet)> values)> GetPagedAllAsync(
        int skipCount = default,
        int maxResultCount = default,
        string? searchTerm = default,
        CancellationToken ct = default);

    /// <summary>
    /// 根據錢包 ID 查找對應的租戶和錢包
    /// </summary>
    /// <param name="walletId">錢包 ID</param>
    /// <param name="ct">取消操作的標記</param>
    Task<(Tenant? tenant, TenantWallet? wallet)> FindTenantAndWalletAsync(Guid walletId, CancellationToken ct = default);

    /// <summary>
    /// 計算指定錢包的存款交易總金額
    /// </summary>
    /// <param name="walletId">錢包 ID</param>
    /// <param name="ct">取消操作的標記</param>
    Task<decimal> SumDepositTransactionAmountAsync(Guid walletId, CancellationToken ct = default);

    /// <summary>
    /// 計算指定錢包的扣款交易總金額
    /// </summary>
    /// <param name="walletId">錢包 ID</param>
    /// <param name="ct">取消操作的標記</param>
    Task<decimal> SumDeductionTransactionAmountAsync(Guid walletId, CancellationToken ct = default);
}
