using Kooco.Pikachu.TenantPaymentFees;
using Kooco.Pikachu.TenantPayouts;
using System;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public class TenantPayoutContext
{
    public ITenantPayoutAppService Service { get; init; } = default!;
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public PaymentFeeType? FeeType { get; set; }
    public int? Year { get; set; }

    // For showing loading on buttons
    public bool Exporting { get; set; }
    public bool Filtering { get; set; }
    public bool Resetting { get; set; }
}
