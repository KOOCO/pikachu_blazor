using System.Linq;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryLogConsts
{
    public const string DefaultSorting = "CreationTime DESC";

    public const int MaxSkuLength = 256;
    public const int MaxAttributesLength = 256;
    public const int MaxDescriptionLength = 512;

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
}
