using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單配送
/// </summary>
public class OrderDelivery : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 配送方式
    /// </summary>
    public DeliveryMethod DeliveryMethod { get; set; }

    /// <summary>
    /// 配送狀態
    /// </summary>
    public DeliveryStatus DeliveryStatus { get; set; }

    /// <summary>
    /// 實際配送方式
    /// </summary>
    public DeliveryMethod? ActualDeliveryMethod { get; set; }

    /// <summary>
    /// 配送編號
    /// </summary>
    public string? DeliveryNo { get; set; }

    /// <summary>
    /// 狀態名稱
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// 狀態識別碼
    /// </summary>
    public string? StatusId { get; set; }

    /// <summary>
    /// 服務交易識別碼
    /// </summary>
    public string? SrvTranId { get; set; }

    /// <summary>
    /// 檔案編號
    /// </summary>
    public string? FileNo { get; set; }

    /// <summary>
    /// 歐付寶物流識別碼
    /// </summary>
    public string AllPayLogisticsID { get; set; }

    /// <summary>
    /// 編輯者
    /// </summary>
    public string Editor { get; set; }

    public Guid? TenantId { get; set; }

    public Guid OrderId { get; set; }
    public ICollection<OrderItem> Items { get; set; }

    public OrderDelivery() { }
    public OrderDelivery(
        Guid id,
        DeliveryMethod deliveryMethod,
        DeliveryStatus deliveryStatus,
        string? deliveryNo,
        string allPayLogisticsID,
        Guid orderId)
    {
        Id = id;
        DeliveryMethod = deliveryMethod;
        DeliveryStatus = deliveryStatus;
        AllPayLogisticsID = allPayLogisticsID;
        DeliveryNo = deliveryNo;
        OrderId = orderId;
        Editor = "";
    }
}