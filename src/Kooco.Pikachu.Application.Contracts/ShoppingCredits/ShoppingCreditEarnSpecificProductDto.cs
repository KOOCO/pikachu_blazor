using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditEarnSpecificProductDto : Entity<Guid>
    {
        public Guid ShoppingCreditEranSettingId { get; set; }
        public Guid ProductId { get; set; }
    }
}