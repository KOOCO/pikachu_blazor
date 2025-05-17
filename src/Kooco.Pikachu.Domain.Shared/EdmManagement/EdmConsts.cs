namespace Kooco.Pikachu.EdmManagement;

public class EdmConsts
{
    public const string DefaultSorting = "CreationTime DESC";
    public const int MaxSubjectLength = 255;

    public static string GetSortingOrDefault(string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting)) return DefaultSorting;
        return sorting!;
    }
}
