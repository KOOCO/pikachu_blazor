using System.ComponentModel;

namespace Kooco.Pikachu.EnumValues;

public enum DeliveryMethod
{
    [Description("郵局")]
    PostOffice=1,

    [Description("自取")]
    SelfPickup,

    [Description("黑貓")]
    BlackCat1,
    BlackCatFreeze = 10,
    BlackCatFrozen = 11,
    [Description("親送")]
    HomeDelivery=4,
    SevenToEleven1=5,
    SevenToElevenC2C=6,
    FamilyMart1=7,
    FamilyMartC2C=8,
    [Description("廠商統一出貨")]
    DeliveredByStore=9,
    TCatDelivery=12,
    TCatSevenToEleven=13
}