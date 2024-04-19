using System.ComponentModel;

namespace Kooco.Pikachu.EnumValues;

public enum DeliveryMethod
{
    [Description("郵局")]
    PostOffice=1,

    [Description("自取")]
    SelfPickup,

    [Description("黑貓")]
    BlackCat,

    [Description("親送")]
    HomeDelivery,
    SevenToEleven1,
    SevenToElevenC2C,
    FamilyMart1,
    FamilyMartC2C,
    [Description("廠商統一出貨")]
    DeliveredByStore

}