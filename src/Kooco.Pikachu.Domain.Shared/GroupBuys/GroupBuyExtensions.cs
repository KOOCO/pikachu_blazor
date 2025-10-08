using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuys;

public static class GroupBuyExtensions
{
    public static IEnumerable<GroupBuyModuleType> GetPikachuOneList()
    {
        return [
            GroupBuyModuleType.ProductDescriptionModule,
            GroupBuyModuleType.ProductImageModule,
            GroupBuyModuleType.ProductGroupModule,
            GroupBuyModuleType.CarouselImages,
            GroupBuyModuleType.IndexAnchor,
            GroupBuyModuleType.BannerImages,
            GroupBuyModuleType.CountdownTimer
        ];
    }

    public static IEnumerable<GroupBuyModuleType> GetPikachuTwoList()
    {
        return [
            GroupBuyModuleType.ProductGroupModule,
            GroupBuyModuleType.CarouselImages,
            GroupBuyModuleType.IndexAnchor,
            GroupBuyModuleType.BannerImages,
            GroupBuyModuleType.GroupPurchaseOverview,
            GroupBuyModuleType.CountdownTimer,
            GroupBuyModuleType.OrderInstruction,
            GroupBuyModuleType.ProductRankingCarouselModule,
            GroupBuyModuleType.CustomTextModule
        ];
    }

    public static LogisticProviders? GetLogisticsProvider(this DeliveryMethod? deliveryMethod)
    {
        var providerMapping = new Dictionary<DeliveryMethod, LogisticProviders>
        {
            { DeliveryMethod.HomeDelivery, LogisticProviders.HomeDelivery },
            { DeliveryMethod.PostOffice, LogisticProviders.PostOffice },
            { DeliveryMethod.FamilyMart1, LogisticProviders.FamilyMart },
            { DeliveryMethod.SevenToEleven1, LogisticProviders.SevenToEleven },
            { DeliveryMethod.SevenToElevenFrozen, LogisticProviders.SevenToElevenFrozen },
            { DeliveryMethod.BlackCat1, LogisticProviders.BNormal },
            { DeliveryMethod.BlackCatFreeze, LogisticProviders.BFreeze },
            { DeliveryMethod.BlackCatFrozen, LogisticProviders.BFrozen },
            { DeliveryMethod.FamilyMartC2C, LogisticProviders.FamilyMartC2C },
            { DeliveryMethod.SevenToElevenC2C, LogisticProviders.SevenToElevenC2C },
            { DeliveryMethod.TCatDeliveryNormal, LogisticProviders.TCatNormal },
            { DeliveryMethod.TCatDeliveryFreeze, LogisticProviders.TCat711Freeze },
            { DeliveryMethod.TCatDeliveryFrozen, LogisticProviders.TCatFrozen },
            { DeliveryMethod.TCatDeliverySevenElevenNormal, LogisticProviders.TCat711Normal },
            { DeliveryMethod.TCatDeliverySevenElevenFreeze, LogisticProviders.TCat711Freeze },
            { DeliveryMethod.TCatDeliverySevenElevenFrozen, LogisticProviders.TCat711Frozen }
        };

        if (!deliveryMethod.HasValue || !providerMapping.TryGetValue(deliveryMethod.Value, out var logisticProvider))
        {
            return null;
        }

        return logisticProvider;
    }
}
