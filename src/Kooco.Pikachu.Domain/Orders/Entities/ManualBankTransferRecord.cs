using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Orders.Entities;

public class ManualBankTransferRecord : Entity<Guid>, IMultiTenant
{
    public Guid OrderId { get; set; }
    public string BankAccountNumber { get; set; }
    public int PaymentAmount { get; set; }
    public DateTime ReceivedTime { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime? ConfirmationTime { get; set; }
    public Guid? ConfirmById { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }

    [ForeignKey(nameof(ConfirmById))]
    public virtual IdentityUser? ConfirmedBy { get; set; }

    private ManualBankTransferRecord() { }

    public ManualBankTransferRecord(
        Guid id,
        Guid orderId,
        string bankAccountNumber,
        int paymentAmount,
        DateTime receivedTime,
        bool isConfirmed = false,
        DateTime? confirmationTime = null,
        Guid? confirmById = null
        ) : base(id)
    {
        OrderId = orderId;
        BankAccountNumber = Check.NotNullOrWhiteSpace(bankAccountNumber, nameof(BankAccountNumber), maxLength: 32);
        PaymentAmount = Check.Range(paymentAmount, nameof(PaymentAmount), 0);
        ReceivedTime = Check.NotNull(receivedTime, nameof(ReceivedTime));
        IsConfirmed = isConfirmed;
        ConfirmationTime = confirmationTime;
        ConfirmById = confirmById;
    }

    public void Confirm(Guid confirmById)
    {
        Check.NotDefaultOrNull<Guid>(confirmById, nameof(confirmById));
        IsConfirmed = true;
        ConfirmationTime = DateTime.Now;
        ConfirmById = confirmById;
    }
}
