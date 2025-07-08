using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders;

public class ManualBankTransferRecordDto : EntityDto<Guid>
{
    public Guid OrderId { get; set; }
    public string BankAccountNumber { get; set; }
    public int PaymentAmount { get; set; }
    public DateTime ReceivedTime { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime? ConfirmationTime { get; set; }
    public Guid? ConfirmById { get; set; }
    public string? ConfirmByName { get; set; }
    public Guid? TenantId { get; set; }
}
