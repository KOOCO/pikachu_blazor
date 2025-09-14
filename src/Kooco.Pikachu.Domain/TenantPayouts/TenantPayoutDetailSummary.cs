namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutDetailSummary
{
    public decimal NetAmount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalAmount { get; set; }
    public int Transactions { get; set; }
}
