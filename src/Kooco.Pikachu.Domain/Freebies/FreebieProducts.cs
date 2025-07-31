using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Freebies
{
    public class FreebieProducts : Entity
    {
        public Guid FreebieId { get; set; }
        public Guid ProductId { get; set; }
        public FreebieProducts()
        {

        }
        public FreebieProducts(Guid freeBieId, Guid productId)
        {
            FreebieId = freeBieId;
            ProductId = productId;
        }
        public override object[] GetKeys()
        {
            return new object[] { FreebieId, ProductId };
        }
    }
}
