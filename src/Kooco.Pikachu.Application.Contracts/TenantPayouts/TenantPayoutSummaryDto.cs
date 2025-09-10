using System;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutSummaryDto
{
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public DateTime CreationTime { get; set; }
    public decimal TotalFees { get; set; }
    public int TotalTransactions { get; set; }
}