using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public static class DeliveryTimeConts
{
    public static List<string> SelfPickUp = new() { "Weekday9To13", "Weekday14To18", "Weekend9To13", "Weekend14To18", "UnableToSpecifyDuringPeakPeriods", "PleaseContactUs" };
    public static List<string> BlackCat = new() { "Before13PM", "Between14To18PM", "Inapplicable" };
    public static List<string> BlackCatFreeze = new() { "Before13PM", "Between14To18PM", "Inapplicable" };
    public static List<string> BlackCatFrozen = new() { "Before13PM", "Between14To18PM", "Inapplicable" };
    public static List<string> HomeDelivery = new() { "Weekday9To13", "Weekday14To18", "Weekend9To13", "Weekend14To18", "UnableToSpecifyDuringPeakPeriods", "Inapplicable" };
    public static List<string> DeliveredByStore = new() { "Before13PM", "Between14To18PM", "Inapplicable" };

}
