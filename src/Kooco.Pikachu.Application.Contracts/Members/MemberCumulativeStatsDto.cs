namespace Kooco.Pikachu.Members;

public class MemberCumulativeStatsDto
{
    public int PaidCount { get; set; }
    public decimal PaidAmount { get; set; }

    public int UnpaidCount { get; set; }
    public decimal UnpaidAmount { get; set; }

    public int RefundCount { get; set; }
    public decimal RefundAmount { get; set; }

    public int OpenCount { get; set; }
    public decimal OpenAmount { get; set; }

    public int ExchangeCount { get; set; }
    public decimal ExchangeAmount { get; set; }

    public int ReturnCount { get; set; }
    public decimal ReturnAmount { get; set; }
}