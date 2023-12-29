using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.Items
{
    public class ItemWithItemType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
    }
}
