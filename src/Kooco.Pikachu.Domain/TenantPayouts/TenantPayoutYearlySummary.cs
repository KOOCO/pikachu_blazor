namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutYearlySummary
{
    public int Year { get; set; }
    public decimal TotalFees { get; set; }
    public int Transactions { get; set; }
    public decimal AvgFeeRate { get; set; }
}
