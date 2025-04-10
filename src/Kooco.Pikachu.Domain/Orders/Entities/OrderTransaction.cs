using Kooco.Pikachu.OrderTransactions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單交易
/// </summary>
public class OrderTransaction : FullAuditedEntity<Guid>, IMultiTenant
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 交易類型
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// 支付渠道
    /// </summary>
    public PaymentChannel? PaymentChannel { get; set; }

    /// <summary>
    /// 交易狀態
    /// </summary>
    public TransactionStatus TransactionStatus { get; set; }

    /// <summary>
    /// 失敗原因
    /// </summary>
    public string? FailedReason { get; set; }

    /// <summary>
    /// 交易金額
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// 租戶識別碼
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 訂單導覽屬性
    /// </summary>
    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }

    /// <summary>
    /// 預設建構函式
    /// </summary>
    public OrderTransaction() { }

    /// <summary>
    /// 建立訂單交易的建構函式
    /// </summary>
    /// <param name="id">交易識別碼</param>
    /// <param name="orderId">訂單識別碼</param>
    /// <param name="orderNo">訂單編號</param>
    /// <param name="amount">交易金額</param>
    /// <param name="transactionType">交易類型</param>
    /// <param name="transactionStatus">交易狀態</param>
    /// <param name="paymentChannel">支付渠道</param>
    /// <param name="failedReason">失敗原因</param>
    public OrderTransaction(
        Guid id,
        Guid orderId,
        string orderNo,
        decimal amount,
        TransactionType transactionType,
        TransactionStatus transactionStatus,
        PaymentChannel? paymentChannel = null,
        string? failedReason = null
        ) : base(id)
    {
        OrderId = orderId;
        OrderNo = orderNo;
        TransactionType = transactionType;
        TransactionStatus = transactionStatus;
        PaymentChannel = paymentChannel;
        FailedReason = failedReason;
        SetAmount(amount);
    }

    /// <summary>
    /// 設定交易金額
    /// 如果是付款類型，則金額為正數；其他類型（如退款）則為負數
    /// </summary>
    /// <param name="amount">金額數值</param>
    public void SetAmount(decimal amount)
    {
        if (TransactionType == TransactionType.Payment)
        {
            Amount = amount;
        }
        else
        {
            Amount = amount * -1;
        }
    }
}