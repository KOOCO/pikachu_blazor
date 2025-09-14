using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantPaymentFees;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantPayouts;

public class GetTenantPayoutRecordListDto : PagedAndSortedResultRequestDto
{
    public Guid TenantId { get; set; }
    public PaymentFeeType FeeType { get; set; }
    public int Year { get; set; }
    public string? Filter { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public bool? IsPaid { get; set; }
}
