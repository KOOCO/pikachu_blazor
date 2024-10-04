using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSpecificProductDto : Entity<Guid>
    {
        public Guid ShoppingCreditsUsageSettingId { get; set; }
        public Guid ProductId { get; set; }
    }
}
