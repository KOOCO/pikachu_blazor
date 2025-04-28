using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyItemGroupDetails : Entity<Guid>
    {
        public Guid GroupBuyItemGroupId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? SetItemId { get; set; }
        public Guid? ItemDetailId { get; set; }

        public string? DisplayText { get; set; }
        public int SortOrder { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(SetItemId))]
        public SetItem? SetItem { get; set; }
        public ItemType ItemType { get; set; }
        public int? ModuleNumber { get; set; }
        public GroupBuyItemGroupDetails()
        {
            
        }

        public GroupBuyItemGroupDetails(
            Guid id,
            Guid groupBuyItemGroupId,
            int sortOrder,
            Guid? itemId,
            Guid? setItemId,
            ItemType itemType,
            string? displayText,
            int? moduleNumber,
            Guid? itemDetailId
            ) : base(id)
        {
            GroupBuyItemGroupId = groupBuyItemGroupId;
            SortOrder = sortOrder;
            ItemId = itemId;
            SetItemId = setItemId;
            ItemType = itemType;
            DisplayText = displayText;
            ModuleNumber = moduleNumber;
            ItemDetailId = itemDetailId;
        }
    }
}
