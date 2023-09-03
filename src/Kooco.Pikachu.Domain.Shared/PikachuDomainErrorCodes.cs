namespace Kooco.Pikachu;

public static class PikachuDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    public const string ItemNameCannotBeNull = "Pikachu:ItemNameCannotBeNull";
    public const string ItemWithSameNameAlreadyExists = "Pikachu:ItemWithSameNameAlreadyExists";
    public const string ItemDetailsCannotBeEmpty = "Pikachu:ItemDetailsCannotBeEmpty";
    public const string ItemWithSKUAlreadyExists = "Pikachu:ItemWithSKUAlreadyExists";
    public const string SKUForItemDetailsCannotBeNull = "Pikachu:SKUForItemDetailsCannotBeNull";
    public const string SellingPriceForItemShouldBeGreaterThanZero = "Pikachu:SellingPriceForItemShouldBeGreaterThanZero";
}
