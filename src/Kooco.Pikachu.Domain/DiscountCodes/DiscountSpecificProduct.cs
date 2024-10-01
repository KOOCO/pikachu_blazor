using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountSpecificProduct:Entity<Guid>
    {
        public Guid DiscountCodeId { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey(nameof(DiscountCodeId))]
        public DiscountCode? DiscountCode { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Item? Product { get; set; }
        public DiscountSpecificProduct(Guid id, Guid discountCodeId, Guid productId) : base(id)
        {
            DiscountCodeId = discountCodeId;
            ProductId = productId;
        }
    }
}
