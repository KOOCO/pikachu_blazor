using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.UserShoppingCredits
{
    public class RecordUserShoppingCreditDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Range(0, int.MaxValue)]
        public int Amount { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentRemainingCredits { get { return Amount; } }

        [Required]
        [MaxLength(UserShoppingCreditConsts.MaxTransactionDescriptionLength)]
        public string? TransactionDescription { get; set; }

        //[DataType(DataType.Date)]
        [MustBeGreaterThanToday]
        //[ExpirationDateRequiredIfLimited]
        public DateTime? ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        public UserShoppingCreditType? ShoppingCreditType { get; set; }
    }
}
