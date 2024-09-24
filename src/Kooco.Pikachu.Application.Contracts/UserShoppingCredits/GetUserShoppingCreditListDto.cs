using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.UserShoppingCredits;

public class GetUserShoppingCreditListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? UserId { get; set; }
    public int? MinAmount { get; set; }
    public int? MaxAmount { get; set; }
    public int? MinCurrentRemainingCredits { get; set; }
    public int? MaxCurrentRemainingCredits { get; set; }
    public string? TransactionDescription { get; set; }
    public DateTime? MinExpirationDate { get; set; }
    public DateTime? MaxExpirationDate { get; set; }
    public bool? IsActive { get; set; }
}