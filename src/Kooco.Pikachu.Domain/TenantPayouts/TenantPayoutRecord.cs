using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantPaymentFees;
using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutRecord : Entity<Guid>, IHasCreationTime, ISoftDelete
{
    public Guid? TenantId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public FeeKind? FeeKind { get; set; }
    public decimal GrossOrderAmount { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal FeeRate { get; set; }
    public decimal ProcessingFee { get; set; }
    public decimal NetAmount { get; set; }
    public PaymentFeeType FeeType { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidTime { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime OrderCreationTime { get; set; }

    private TenantPayoutRecord() { }

    public TenantPayoutRecord(
        Guid id,
        Guid orderId,
        string orderNo,
        PaymentMethods paymentMethod,
        FeeKind? feeKind,
        decimal grossOrderAmount,
        decimal feeRate,
        decimal handlingFee,
        decimal processingFee,
        PaymentFeeType feeType,
        DateTime orderCreationTime,
        Guid? tenantId
        ) : base(id)
    {
        Id = id;
        OrderId = orderId;
        OrderNo = orderNo;
        PaymentMethod = paymentMethod;
        FeeKind = feeKind;
        FeeRate = feeRate;
        SetNetAmount(grossOrderAmount, handlingFee, processingFee);
        FeeType = feeType;
        OrderCreationTime = orderCreationTime;
        TenantId = tenantId;
    }

    public void SetNetAmount(
        decimal grossOrderAmount,
        decimal handlingFee,
        decimal processingFee
        )
    {
        GrossOrderAmount = grossOrderAmount;
        HandlingFee = handlingFee;
        ProcessingFee = processingFee;
        NetAmount = GrossOrderAmount - HandlingFee - ProcessingFee;
    }

    public void SetPaid(bool isPaid, DateTime? paidTime = null)
    {
        IsPaid = isPaid;
        PaidTime = isPaid ? paidTime ?? DateTime.Now : null;
    }
}