namespace Kooco.Pikachu;

public static class PikachuDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    public const string ItemNameCannotBeNull = "Pikachu:00001";
    public const string ItemWithSameNameAlreadyExists = "Pikachu:00002";
    public const string ItemDetailsCannotBeEmpty = "Pikachu:00003";
    public const string ItemWithSKUAlreadyExists = "Pikachu:00004";
}
