using System.ComponentModel;

namespace Kooco.Pikachu.EnumValues;

public enum DeliveryMethod
{
    [Description("ECPay|PostOffice")]
    PostOffice=1,

  
    SelfPickup,

    [Description("ECPay|BlackCat1")]
    BlackCat1,
    [Description("ECPay|BlackCatFreeze")]
    BlackCatFreeze = 10,
    [Description("ECPay|BlackCatFrozen")]
    BlackCatFrozen = 11,
  
    HomeDelivery=4,
    [Description("ECPay|SevenToEleven1")]
    SevenToEleven1 =5,
    [Description("ECPay|SevenToElevenFrozen")]
    SevenToElevenFrozen =18,
    [Description("ECPay|SevenToElevenC2C")]
    SevenToElevenC2C =6,
    [Description("ECPay|FamilyMart1")]
    FamilyMart1 =7,
    [Description("ECPay|FamilyMartC2C")]
    FamilyMartC2C =8,
   
    DeliveredByStore=9,
    TCatDeliveryNormal=12,
    TCatDeliveryFreeze=13,
    TCatDeliveryFrozen=14,
    TCatDeliverySevenElevenNormal=15,
    TCatDeliverySevenElevenFreeze=16,
    TCatDeliverySevenElevenFrozen=17,
}