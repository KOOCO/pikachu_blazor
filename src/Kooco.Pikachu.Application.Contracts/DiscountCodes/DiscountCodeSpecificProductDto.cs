using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCodeSpecificProductDto:EntityDto<Guid>
    {

        public Guid DiscountCodeId { get; set; }
        public Guid ProductId { get; set; }
    }
}
