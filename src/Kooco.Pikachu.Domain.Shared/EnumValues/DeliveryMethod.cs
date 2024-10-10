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
    SevenToElevenFrozen=18,
    SevenToElevenC2C=6,
    FamilyMart1=7,
    FamilyMartC2C=8,
    [Description("廠商統一出貨")]
    DeliveredByStore=9,
    TCatDeliveryNormal=12,
    TCatDeliveryFreeze=13,
    TCatDeliveryFrozen=14,
    TCatDeliverySevenElevenNormal=15,
    TCatDeliverySevenElevenFreeze=16,
    TCatDeliverySevenElevenFrozen=17,
}