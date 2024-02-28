using System.ComponentModel;

namespace Kooco.Pikachu.EnumValues;

public enum DeliveryMethod
{
    [Description("郵局")]
    PostOffice=0,

    [Description("自取")]
    SelfPickup,

    [Description("黑貓")]
    BlackCat,

    [Description("親送")]
    HomeDelivery,
    SevenToEleven,
    SevenToElevenC2C,
    FamilyMart,
    FamilyMartC2C

}