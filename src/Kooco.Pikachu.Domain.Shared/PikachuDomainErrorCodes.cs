namespace Kooco.Pikachu;

public static class PikachuDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    public const string ItemNameCannotBeNull = "Pikachu:ItemNameCannotBeNull";
    public const string GroupBuyNameCannotBeNull = "Pikachu:GroupBuyNameCannotBeNull";
    public const string GroupBuyShortCodeCannotBeNull = "Pikachu:GroupBuyShortCodeCannotBeNull";
    public const string ItemWithSameNameAlreadyExists = "Pikachu:ItemWithSameNameAlreadyExists";
    public const string FreebieWithSameNameAlreadyExists = "Pikachu:FreebieWithSameNameAlreadyExists";
    public const string ItemDetailsCannotBeEmpty = "Pikachu:ItemDetailsCannotBeEmpty";
    public const string MinimumAmountReachCannotBeEmpty = "Pikachu:MinimumAmountReachCannotBeEmpty";
    public const string MinimumPieceCannotBeEmpty = "Pikachu:MinimumPieceCannotBeEmpty";
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
    public const string EntityWithGivenIdDoesnotExist = "Pikachu:EntityWithGivenIdDoesnotExist";
    public const string FreebieAmountCannotBeZero = "Pikachu:FreebieAmountCannotBeZero";
    public const string SelectAtLeastOneGroupBuy = "Pikachu:SelectAtLeastOneGroupBuy";
    public const string SelectAtLeastOneImage = "Pikachu:SelectAtLeastOneImage";
    public const string InvalidItemName = "Pikachu:InvalidItemName";
    public const string GroupBuyModuleCannotBeEmpty = "Pikachu:GroupBuyModuleCannotBeEmpty";
    public const string CanNotAddMoreThan20Modules = "Pikachu:CanNotAddMoreThan20Modules";
    
    public const string RefundForSameOrderAlreadyExists = "Pikachu:RefundForSameOrderAlreadyExists";
    public const string RefundIsOnlyAvailableForCreditCardPayments = "Pikachu:RefundIsOnlyAvailableForCreditCardPayments";
    public const string AtLeastOneDeliveryTimeIsRequiredForBlackCat = "Pikachu:AtLeastOneDeliveryTimeIsRequiredForBlackCat";
    public const string AtLeastOneDeliveryTimeIsRequiredForBlackCatFreeze = "Pikachu:AtLeastOneDeliveryTimeIsRequiredForBlackCatFreeze";
    public const string AtLeastOneDeliveryTimeIsRequiredForBlackCatFrozen = "Pikachu:AtLeastOneDeliveryTimeIsRequiredForBlackCatFrozen";
    public const string AtLeastOneDeliveryTimeIsRequiredForSelfPickup = "Pikachu:AtLeastOneDeliveryTimeIsRequiredForSelfPickup";
    public const string AtLeastOneDeliveryTimeIsRequiredForHomeDelivery = "Pikachu:AtLeastOneDeliveryTimeIsRequiredForHomeDelivery";
    public const string AtLeastOneDeliveryTimeIsRequiredForDeliverdByStore = "Pikachu:AtLeastOneDeliveryTimeIsRequiredForDeliverdByStore";
    public const string AtLeastOneShippingMethodIsRequired = "Pikachu:AtLeastOneShippingMethodIsRequired";
    public const string AtLeastOnePaymentMethodIsRequired = "Pikachu:AtLeastOnePaymentMethodIsRequired";
    public const string DeliverdByStoreMethodIsRequired = "Pikachu:DeliverdByStoreMethodIsRequired";
}
