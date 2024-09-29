using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.AddOnProducts
{
    public class AddOnProductSpecificGroupbuyDto:EntityDto<Guid>
    {
        public Guid AddOnProductId { get; set; }
        public Guid GroupbuyId { get; set; }
    }
}
