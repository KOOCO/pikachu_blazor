using Kooco.Pikachu.Items;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Images
{
    public class Image : FullAuditedAggregateRoot<Guid>
    {
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid? ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}
