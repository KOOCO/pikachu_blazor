using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditEarnSpecificGroupbuyDto : EntityDto<Guid>
    {
        public Guid ShoppingCreditEarnSettingId { get; set; }
        public Guid GroupbuyId { get; set; }
    }
}
