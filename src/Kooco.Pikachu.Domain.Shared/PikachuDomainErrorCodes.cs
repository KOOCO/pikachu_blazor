namespace Kooco.Pikachu;

public static class PikachuDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    public const string ItemNameCannotBeNull = "Pikachu:ItemNameCannotBeNull";
    public const string GroupBuyNameCannotBeNull = "Pikachu:GroupBuyNameCannotBeNull";
    public const string ItemWithSameNameAlreadyExists = "Pikachu:ItemWithSameNameAlreadyExists";
    public const string ItemDetailsCannotBeEmpty = "Pikachu:ItemDetailsCannotBeEmpty";
    public const string ItemWithSKUAlreadyExists = "Pikachu:ItemWithSKUAlreadyExists";
    public const string SKUForItemDetailsCannotBeNull = "Pikachu:SKUForItemDetailsCannotBeNull";
    public const string SellingPriceForItemShouldBeGreaterThanZero = "Pikachu:SellingPriceForItemShouldBeGreaterThanZero";
    public const string SystemIsUnableToCopyAtTheMoment = "Pikachu:SystemIsUnableToCopyAtTheMoment";
    public const string SomethingWentWrongWhileDeletingImage = "Pikachu:SomethingWentWrongWhileDeletingImage";
    public const string AreYouSureToDeleteImage = "Pikachu:AreYouSureToDeleteImage";
    public const string SomethingWrongWhileFileUpload = "Pikachu:SomethingWrongWhileFileUpload";
    public const string FilesAreGreaterThanMaxAllowedFileSize = "Pikachu:FilesAreGreaterThanMaxAllowedFileSize";
    public const string AlreadyUploadMaxAllowedFiles = "Pikachu:AlreadyUploadMaxAllowedFiles";
    public const string FilesExceedMaxAllowedPerUpload = "Pikachu:FilesExceedMaxAllowedPerUpload";
}
