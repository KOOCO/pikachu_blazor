using System.ComponentModel;

namespace Kooco.Pikachu.EnumValues;

public enum DeliveryMethod
{
    [Description("郵局")]
    PostOffice,

    [Description("自取")]
    SelfPickup,

    [Description("黑貓")]
    BlackCat,

    [Description("親送")]
    HomeDelivery
}