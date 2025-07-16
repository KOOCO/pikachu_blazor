using Kooco.Pikachu.Members;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserShoppingCredits;

public class UserShoppingCreditDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public int Amount { get; set; }
    public int CurrentRemainingCredits { get; set; }
    public string? TransactionDescription { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public string? OrderNo { get; set; }
    public UserShoppingCreditType ShoppingCreditType { get; set; }
    public Guid? TenantId { get; set; }
    public MemberDto? User { get; set; }
}