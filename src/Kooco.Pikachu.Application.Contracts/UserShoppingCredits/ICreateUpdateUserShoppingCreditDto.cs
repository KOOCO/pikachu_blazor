using System.ComponentModel.DataAnnotations;
using System;

namespace Kooco.Pikachu.UserShoppingCredits;

public interface ICreateUpdateUserShoppingCreditDto
{
    [DataType(DataType.Date)]
    [MustBeGreaterThanToday]
    [ExpirationDateRequiredIfLimited]
    public DateTime? ExpirationDate { get; set; }

    [Required]
    public bool? CanExpire { get; set; }
}
