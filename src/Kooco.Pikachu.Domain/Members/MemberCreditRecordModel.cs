using System;

namespace Kooco.Pikachu.Members;

public class MemberCreditRecordModel
{
    public Guid Id { get; set; }
    public DateTime UsageTime { get; set; }
    public string? TransactionDescription { get; set; }
    public int Amount { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal RemainingCredits { get; set; }
}