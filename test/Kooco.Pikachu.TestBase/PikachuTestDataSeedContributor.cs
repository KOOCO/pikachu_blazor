using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Kooco.Pikachu;

public class PikachuTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IItemRepository _itemRepository;
    public PikachuTestDataSeedContributor(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }
    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        var item = new Item(
            Guid.Parse("170a29a7-95cf-4897-b60c-30e1f654ab0a"),
            "Sample Item Name",
            "123",  // itemBadge
            "Sample Description Title",
            "This is a detailed item description.",
            "Tag1, Tag2, Tag3",  // itemTags
            DateTime.UtcNow,  // limitAvailableTimeStart
            DateTime.UtcNow.AddDays(7),  // limitAvailableTimeEnd
            5.5f,  // shareProfit
            true,  // isFreeShipping
            false, // isReturnable
            1,  // shippingMethodId
            2,  // taxTypeId

            "Value1", "Custom Field 1",
            "Value2", "Custom Field 2",
            "Value3", "Custom Field 3",
            "Value4", "Custom Field 4",
            "Value5", "Custom Field 5",
            "Value6", "Custom Field 6",
            "Value7", "Custom Field 7",
            "Value8", "Custom Field 8",
            "Value9", "Custom Field 9",
            "Value10", "Custom Field 10",

            "Attribute 1",
            "Attribute 2",
            "Attribute 3",

            ItemStorageTemperature.Normal // Example enum value
        );
        item.ItemNo = 12345;
        await _itemRepository.InsertAsync(item);
    }
}
