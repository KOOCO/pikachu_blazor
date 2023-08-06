using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Kooco.Pikachu.Items
{
    /// <summary>
    /// 商品群組,同一個群組會包含不同的商品屬性,例如:商品分類,商品顏色,商品尺寸等等
    /// </summary>
    public  class GroupItem : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {

        public Guid? TenantId { get; set; }

        public string GroupName { get; set; }
        public string GroupDesciption { get; set; }
        public string GroupMainImageURL { get; set; }
        public string Property1Name { get; set; }
        public Array Property1Value { get; set; }

        public string Property2Name { get; set; }

        public Array Property2Value { get; set; }
        public string Property3Name { get; set; }
        public Array Property3Value { get; set; }
        public string Unit { get; set; }
        public string Supplier { get; set; }
        public string Brand { get; set; }
    }
}
