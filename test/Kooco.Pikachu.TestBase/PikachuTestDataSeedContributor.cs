using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu;

public class PikachuTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IItemRepository _itemRepository;
    private readonly ISetItemRepository _setItemRepository;
    public PikachuTestDataSeedContributor(IItemRepository itemRepository, ISetItemRepository setItemRepository)
    {
        _itemRepository = itemRepository;
        _setItemRepository = setItemRepository;
    }
    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        await _itemRepository.InsertAsync(Item);
        await _setItemRepository.InsertAsync(SetItem);
    }

    #region ITEM
    private static Item Item
    {
        get
        {
            var item = new Item(
                TestData.Item1Id,
                "Sample Item Name",
                "123",
                "Sample Description Title",
                "This is a detailed item description.",
                "Tag1, Tag2, Tag3",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                5.5f,
                true,
                false,
                1,
                2,
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
                ItemStorageTemperature.Normal
            )
            {
                ItemNo = 12345
            };

            return item;
        }
    }
    #endregion

    #region SET ITEM
    private static SetItem SetItem
    {
        get
        {
            var setItem = new SetItem(
                TestData.SetItem1Id,
                null,
                "Seed Set Item",
                "Set Item",
                "Set Item",
                "<h1>Set Item</h1>",
                "https://www.example.com",
                999,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                0,
                false,
                ItemStorageTemperature.Normal,
                10
                );
            return setItem;
        }
    }
    #endregion
}
