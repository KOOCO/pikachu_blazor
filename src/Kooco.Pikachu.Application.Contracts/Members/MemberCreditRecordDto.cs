using Kooco.Pikachu.UserShoppingCredits;
using System;

namespace Kooco.Pikachu.Members;

public class MemberCreditRecordDto
{
    public Guid Id { get; set; }
    public DateTime? UsageTime { get; set; }
    public string? TransactionDescription { get; set; }
    public string? TransactionDescriptionZh { get; set; }
    public int Amount { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal RemainingCredits { get; set; }
    public string? OrderNo { get; set; }
    public Guid? OrderId { get; set; }
    public UserShoppingCreditType ShoppingCreditType { get; set; }
}