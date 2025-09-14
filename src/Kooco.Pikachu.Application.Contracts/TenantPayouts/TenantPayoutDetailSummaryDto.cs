namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutDetailSummaryDto
{
    public decimal NetAmount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalAmount { get; set; }
    public int Transactions { get; set; }
}
