using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditEarnSpecificProduct : Entity<Guid>
    {
        public Guid ShoppingCreditEranSettingId { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ShoppingCreditEranSettingId))]
        public ShoppingCreditEarnSetting? ShoppingCreditEarnSetting { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Item? Product { get; set; }
        public ShoppingCreditEarnSpecificProduct(Guid id, Guid shoppingCreditEranSettingId, Guid productId)
            : base(id)
        {
            ShoppingCreditEranSettingId = shoppingCreditEranSettingId;
            ProductId = productId;
        }
    }
    }
