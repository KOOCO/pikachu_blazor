using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.InventoryManagement;

public static class InventoryLogConsts
{
    public const string DefaultSorting = "CreationTime DESC";

    public const int MaxSkuLength = 256;
    public const int MaxAttributesLength = 256;
    public const int MaxDescriptionLength = 512;

    public static string GetAttributes(object obj)
    {
        var values = new[]
        {
            obj?.GetType().GetProperty("Attribute1Value")?.GetValue(obj) as string,
            obj?.GetType().GetProperty("Attribute2Value")?.GetValue(obj) as string,
            obj?.GetType().GetProperty("Attribute3Value")?.GetValue(obj) as string
        };

        return string.Join(" / ", values.Where(v => !string.IsNullOrEmpty(v)));
    }

    public static string GetAttributes(List<string> attributes)
    {
        return
            string.Join(" / ", (attributes ?? []).Where(attr => !string.IsNullOrEmpty(attr)))
            ?? "";
    }

    public static string GetAttributes(string? attribute1, string? attribute2, string? attribute3)
    {
        return
            string.Join(" / ", new[]
            {
                attribute1,
                attribute2,
                attribute3
            }.Where(attr => !string.IsNullOrEmpty(attr)))
            ?? "";
    }

    public static string GetAmountString(int amount)
    {
        return (amount >= 0 ? "+" : "") + amount;
    }

    public static string GetActionTypeDescription(this InventoryActionType actionType)
    {
        InventoryActionTypeKeyMap.TryGetValue(actionType, out var key);
        if (key == null) return string.Empty;

        return key;
    }

    public static string GetLocalizedDisplayName(this InventoryActionType actionType, IStringLocalizer l, string? orderNumber)
    {
        var key = GetActionTypeDescription(actionType);
        return l[key, orderNumber ?? ""];
    }

    public static readonly Dictionary<InventoryActionType, string> InventoryActionTypeKeyMap =
    new()
    {
        { InventoryActionType.AddStock, "StockAdded" },
        { InventoryActionType.DeductStock, "StockDeducted" },
        { InventoryActionType.ItemSold, "ItemSold" },
        { InventoryActionType.AddOnProductSold, "AddOnProductSold" },
        { InventoryActionType.ItemUnsold, "ItemUnsold" },
        { InventoryActionType.AddOnProductUnsold, "AddOnProductUnsold" }
    };
}
