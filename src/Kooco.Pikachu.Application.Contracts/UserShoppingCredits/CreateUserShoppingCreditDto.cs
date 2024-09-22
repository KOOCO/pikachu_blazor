using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.UserShoppingCredits;

public class CreateUserShoppingCreditDto : ICreateUpdateUserShoppingCreditDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [DisplayName("Amount")]
    public string AmountString { get; set; }

    [Range(0, int.MaxValue)]
    public int Amount { get { int.TryParse(AmountString, out int amount); return amount; } }

    [Range(0, int.MaxValue)]
    public int CurrentRemainingCredits { get { return Amount; } }

    [Required]
    [MaxLength(UserShoppingCreditConsts.MaxTransactionDescriptionLength)]
    public string? TransactionDescription { get; set; }

    [DataType(DataType.Date)]
    [MustBeGreaterThanToday]
    [ExpirationDateRequiredIfLimited]
    public DateTime? ExpirationDate { get; set; }

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "SelectionIsRequired")]
    public bool? CanExpire { get; set; }

    public bool SendEmail { get; set; }
    public bool SendSms { get; set; }
}

