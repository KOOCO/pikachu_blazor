using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders.Repositories;

/// <summary>
/// 訂單配送
/// </summary>
public interface IOrderDeliveryRepository : IRepository<OrderDelivery, Guid>
{
    /// <summary>
    /// 根據配送識別碼取得包含詳細資訊的訂單配送列表
    /// </summary>
    /// <param name="id">配送識別碼</param>
    /// <returns>包含詳細資訊的訂單配送列表</returns>
    Task<List<OrderDelivery>> GetWithDetailsAsync(Guid id);

    /// <summary>
    /// 根據歐付寶物流識別碼取得訂單識別碼
    /// </summary>
    /// <param name="allPayLogisticsId">歐付寶物流識別碼</param>
    /// <returns>訂單識別碼</returns>
    Task<Guid> GetOrderIdByAllPayLogisticsId(string allPayLogisticsId);

    /// <summary>
    /// 按出貨狀態取得所有出貨訂單
    /// </summary>
    Task<List<OrderDelivery>> GetByStatusAsync(Guid tenantId, DeliveryStatus status, CancellationToken ct);
}