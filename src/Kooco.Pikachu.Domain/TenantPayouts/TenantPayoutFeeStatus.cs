using Kooco.Pikachu.TenantPaymentFees;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutFeeStatus
{
    public PaymentFeeType FeeType { get; set; }
    public bool IsActive { get; set; }
}
