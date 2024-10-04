using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSpecificGroupbuyDto:EntityDto<Guid>
    {
        public Guid ShoppingCreditsUsageSettingId { get; set; }
        public Guid GroupbuyId { get; set; }
    }
}
