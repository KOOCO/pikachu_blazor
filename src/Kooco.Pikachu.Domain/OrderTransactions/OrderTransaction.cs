using Kooco.Pikachu.Orders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.OrderTransactions;

public class OrderTransaction : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public PaymentChannel? PaymentChannel { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public string? FailedReason { get; set; }
    public decimal Amount { get; private set; }

    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }

    public OrderTransaction()
    {
        
    }

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