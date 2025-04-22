using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
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
    private readonly IGroupBuyRepository _groupBuyRepository;
    private readonly IItemDetailsRepository _itemDetailsRepository;

    public PikachuTestDataSeedContributor(IItemRepository itemRepository, ISetItemRepository setItemRepository, 
        IGroupBuyRepository groupBuyRepository, IItemDetailsRepository itemDetailsRepository)
    {
        _itemRepository = itemRepository;
        _setItemRepository = setItemRepository;
        _groupBuyRepository = groupBuyRepository;
        _itemDetailsRepository = itemDetailsRepository;
    }
    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        await _itemRepository.InsertAsync(Item);
        await _itemDetailsRepository.InsertAsync(ItemDetail);
        await _setItemRepository.InsertAsync(SetItem);
        await _groupBuyRepository.InsertAsync(GroupBuy);
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
                "#1F1F1F",
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

    #region ITEM DETAIL
    private static ItemDetails ItemDetail
    {
        get
        {
            var itemDetail = new ItemDetails(
                TestData.ItemDetail1Id,
                "ITEM",
                "ITEM",
                TestData.Item1Id,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                
                "TEST",
                "ITEM",
                null,
                null,
                "https://www.example.com",
                null,
                1
            );
            return itemDetail;
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
                "Set Item Description",
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

    #region GROUP BUY
    private static GroupBuy GroupBuy
    {
        get
        {
            var input = new GroupBuy(TestData.GroupBuy1Id)
            {
                GroupBuyNo = 12345,
                Status = "New",
                GroupBuyName = "Sample Group Buy",
                ShortCode = "2025022502",
                EntryURL = "https://dev2.goodpoint.tw/groupBuy/859b6c74-4b69-9474-0dec-3a184ee3f8aa",
                EntryURL2 = null,
                SubjectLine = null,
                ShortName = null,
                NotificationBar = "Limited Time Offer Before Holiday",
                LogoURL = "https://pikachublobs.blob.core.windows.net/images/sample.jpeg",
                BannerURL = null,
                StartTime = DateTime.Parse("2025-02-25T12:00:00"),
                EndTime = DateTime.Parse("2025-02-28T12:00:00"),
                FreeShipping = false,
                AllowShipToOuterTaiwan = false,
                AllowShipOversea = false,
                ExpectShippingDateFrom = null,
                ExpectShippingDateTo = null,
                MoneyTransferValidDayBy = 0,
                MoneyTransferValidDays = null,
                IssueInvoice = true,
                AutoIssueTriplicateInvoice = false,
                InvoiceNote = null,
                ProtectPrivacyData = false,
                InviteCode = null,
                ProfitShare = 10,
                MetaPixelNo = null,
                FBID = null,
                IGID = null,
                LineID = null,
                GAID = null,
                GTM = null,
                WarningMessage = null,
                OrderContactInfo = null,
                ExchangePolicy = null,
                NotifyMessage = "<p><br></p>",
                IsDefaultPaymentGateWay = true,
                ExcludeShippingMethod = "[\"FamilyMartC2C\",\"TCatDeliveryFreeze\",\"SevenToElevenC2C\",\"BlackCat1\",\"TCatDeliveryFrozen\",\"HomeDelivery\"]",
                GroupBuyConditionDescription = "Sample Condition Description",
                CustomerInformationDescription = null,
                ExchangePolicyDescription = "<p>Sample Exchange Policy</p>",
                GroupBuyCondition = null,
                CustomerInformation = null,
                PaymentMethod = "Credit Card , Bank Transfer , Cash On Delivery , LinePay",
                IsEnterprise = false,
                FreeShippingThreshold = null,
                SelfPickupDeliveryTime = null,
                BlackCatDeliveryTime = "[\"Inapplicable\",\"Before13PM\",\"Between14To18PM\"]",
                HomeDeliveryDeliveryTime = "[\"Weekday9To13\",\"Weekday14To18\",\"Weekend9To13\",\"Weekend14To18\"]",
                DeliveredByStoreDeliveryTime = "[]",
                TaxType = TaxType.NonTaxable,
                ProductType = ProductType.GeneralFood,
                FacebookLink = null,
                InstagramLink = null,
                LINELink = null,
                TemplateType = GroupBuyTemplateType.PikachuTwo,
                ColorSchemeType = ColorScheme.ForestDawn,
                PrimaryColor = "#133854",
                SecondaryColor = "#CAE28D",
                BackgroundColor = "#FFFFFF",
                SecondaryBackgroundColor = "#DCD6D0",
                AlertColor = "#A1E82D",
                BlockColor = "#EFF4EB",
                AddOnProduct = false,
                ProductDetailsDisplayMethod = ProductDetailsDisplayMethod.PureImage_LeftRightSlide,
                ItemGroups = [
                new(){
                    SortOrder = 1,
                    GroupBuyModuleType = GroupBuyModuleType.IndexAnchor,
                    ModuleNumber = 1
                }]
            };

            return input;
        }
    }
    #endregion
}
