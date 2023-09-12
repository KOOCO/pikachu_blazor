using Kooco.Pikachu.Images;
using System;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemGroupDetailCreateUpdateDto
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
        public string? ItemDescription { get; set; }

        public Guid? ItemId { get; set; }

        public Guid? ImageId { get; set; }

        public CreateImageDto Image { get; set; }
    }
}
