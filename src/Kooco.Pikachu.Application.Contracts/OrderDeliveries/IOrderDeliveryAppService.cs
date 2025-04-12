using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.OrderDeliveries;

/// <summary>
/// 訂單配送應用服務介面，提供訂單配送相關操作。
/// </summary>
public interface IOrderDeliveryAppService : IApplicationService
{
    /// <summary>
    /// 根據訂單識別碼獲取配送清單。
    /// </summary>
    /// <param name="Id">訂單識別碼</param>
    /// <returns>訂單配送資料清單</returns>
    Task<List<OrderDeliveryDto>> GetListByOrderAsync(Guid id);

    /// <summary>
    /// 根據配送識別碼獲取配送訂單資料。
    /// </summary>
    /// <param name="Id">配送識別碼</param>
    /// <returns>訂單配送資料</returns>
    Task<OrderDeliveryDto> GetDeliveryOrderAsync(Guid id);

    /// <summary>
    /// 更新配送詳細資訊。
    /// </summary>
    /// <param name="id">配送識別碼</param>
    /// <param name="input">訂單建立資料</param>
    /// <returns>更新後的訂單配送資料</returns>
    Task<OrderDeliveryDto> UpdateShippingDetails(Guid id, CreateOrderDto input);

    /// <summary>
    /// 更新訂單配送狀態為已發貨。
    /// </summary>
    /// <param name="Id">配送識別碼</param>
    Task UpdateOrderDeliveryStatus(Guid id);

    /// <summary>
    /// 變更訂單的配送狀態。
    /// </summary>
    /// <param name="orderId">訂單識別碼</param>
    Task ChangeShippingStatus(Guid orderId);

    /// <summary>
    /// 更新訂單狀態為已送達。
    /// </summary>
    /// <param name="orderId">訂單識別碼</param>
    Task UpdateDeliveredStatus(Guid orderId);

    /// <summary>
    /// 更新訂單狀態為已取貨完成。
    /// </summary>
    /// <param name="orderId">訂單識別碼</param>
    Task UpdatePickedUpStatus(Guid orderId);
}