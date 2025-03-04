using Kooco.Pikachu.Orders;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.OrderTransactions;

public class OrderTransactionDto : FullAuditedEntityDto<Guid>
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
    public virtual OrderDto? Order { get; set; }
    public bool IsSelected { get; set; }
}