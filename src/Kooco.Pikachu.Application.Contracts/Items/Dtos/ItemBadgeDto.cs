using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Items.Dtos;

public class ItemBadgeDto
{
    [MaxLength(4)]
    public string ItemBadge { get; set; } = "";

    [MaxLength(16)]
    public string? ItemBadgeColor { get; set; } = ItemConsts.DefaultBadgeColor;

    public override bool Equals(object? obj)
    {
        if (obj is not ItemBadgeDto other) return false;
        return ItemBadge == other.ItemBadge && ItemBadgeColor == other.ItemBadgeColor;
    }

    public override int GetHashCode() => HashCode.Combine(ItemBadge, ItemBadgeColor);
}
