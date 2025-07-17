using AngleSharp.Common;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyAppServiceTests : PikachuApplicationTestBase
{
    private readonly IGroupBuyAppService _groupBuyAppService;
    private readonly IGroupPurchaseOverviewAppService _groupPurchaseOverviewAppService;
    private readonly IGroupBuyOrderInstructionAppService _groupBuyOrderInstructionAppService;
    private readonly IGroupBuyProductRankingAppService _groupBuyProductRankingAppService;

    public GroupBuyAppServiceTests()
    {
        _groupBuyAppService = GetRequiredService<IGroupBuyAppService>();
        _groupPurchaseOverviewAppService = GetRequiredService<IGroupPurchaseOverviewAppService>();
        _groupBuyOrderInstructionAppService = GetRequiredService<IGroupBuyOrderInstructionAppService>();
        _groupBuyProductRankingAppService = GetRequiredService<IGroupBuyProductRankingAppService>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create()
    {
        var input = GetInput();
        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        groupBuy.Id.ShouldNotBe(Guid.Empty);
        groupBuy.GroupBuyNo.ShouldBe(input.GroupBuyNo);
        groupBuy.GroupBuyName.ShouldBe(input.GroupBuyName);

        groupBuy.ItemGroups.Count.ShouldBe(1);
    }

    [Fact]
    public async Task CreateAsync_Should_Set_Social_Links_When_Provided()
    {
        var input = GetInput();
        input.FacebookLink = "https://www.facebook.com";
        input.InstagramLink = "https://www.instagram.com";
        input.LINELink = "https://www.line.com";

        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        
        groupBuy.Id.ShouldNotBe(Guid.Empty);

        groupBuy.FacebookLink.ShouldNotBeNull().ShouldBe(input.FacebookLink);
        groupBuy.InstagramLink.ShouldNotBeNull().ShouldBe(input.InstagramLink);
        groupBuy.LINELink.ShouldNotBeNull().ShouldBe(input.LINELink);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_On_Same_Name()
    {
        var input = GetInput();
        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        groupBuy.Id.ShouldNotBe(Guid.Empty);
        groupBuy.GroupBuyNo.ShouldBe(input.GroupBuyNo);
        groupBuy.GroupBuyName.ShouldBe(input.GroupBuyName);

        // Use same input to test duplicate name detection
        var exception = await Assert.ThrowsAsync<BusinessException>(
            async () => await _groupBuyAppService.CreateAsync(input)
            );
        exception.Code.ShouldNotBeNull();
        exception.Code.ShouldContain(PikachuDomainErrorCodes.GroupBuyWithSameNameAlreadyExists);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Product_Group_Module()
    {
        var input = GetInput();
        input.ItemGroups = GetProductGroupModules();
        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        groupBuy.Id.ShouldNotBe(Guid.Empty);
        groupBuy.GroupBuyNo.ShouldBe(input.GroupBuyNo);
        groupBuy.GroupBuyName.ShouldBe(input.GroupBuyName);

        groupBuy.ItemGroups.Count.ShouldBe(1);
        groupBuy.ItemGroups.First().ItemGroupDetails.Count.ShouldBe(2);

        for (int i = 0; i < input.ItemGroups.Count; i++)
        {
            var itemGroupInput = input.ItemGroups.GetItemByIndex(i);
            var itemGroup = groupBuy.ItemGroups.GetItemByIndex(i);

            itemGroup.Id.ShouldNotBe(Guid.Empty);
            itemGroup.ItemGroupDetails.Count.ShouldBe(itemGroupInput.ItemDetails.Count);
            itemGroup.GroupBuyModuleType.ShouldBe(itemGroupInput.GroupBuyModuleType);
            itemGroup.GroupBuyModuleType.ShouldBe(itemGroupInput.GroupBuyModuleType);
        }
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Purchase_Overview_Module()
    {
        var input = GetInput();
        input.ItemGroups = [];
        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        groupBuy.Id.ShouldNotBe(Guid.Empty);
        groupBuy.GroupBuyNo.ShouldBe(input.GroupBuyNo);
        groupBuy.GroupBuyName.ShouldBe(input.GroupBuyName);

        groupBuy.ItemGroups.Count.ShouldBe(0);

        var moduleInput = GetGroupPurchaseOverview(groupBuy.Id);
        var purchaseOverviewModule = await _groupPurchaseOverviewAppService.CreateGroupPurchaseOverviewAsync(moduleInput);

        purchaseOverviewModule.ShouldNotBeNull();
        purchaseOverviewModule.Id.ShouldNotBe(Guid.Empty);
        purchaseOverviewModule.GroupBuyId.ShouldBe(groupBuy.Id);
        purchaseOverviewModule.Title.ShouldBe(moduleInput.Title);
        purchaseOverviewModule.SubTitle.ShouldBe(moduleInput.SubTitle);
        purchaseOverviewModule.Image.ShouldBe(moduleInput.Image);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Order_Instruction_Module()
    {
        var input = GetInput();
        input.ItemGroups = [];
        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        groupBuy.Id.ShouldNotBe(Guid.Empty);
        groupBuy.GroupBuyNo.ShouldBe(input.GroupBuyNo);
        groupBuy.GroupBuyName.ShouldBe(input.GroupBuyName);

        groupBuy.ItemGroups.Count.ShouldBe(0);

        var moduleInput = GetGroupBuyOrderInstruction(groupBuy.Id);
        var orderInstructionModule = await _groupBuyOrderInstructionAppService.CreateGroupBuyOrderInstructionAsync(moduleInput);

        orderInstructionModule.ShouldNotBeNull();
        orderInstructionModule.Id.ShouldNotBe(Guid.Empty);
        orderInstructionModule.GroupBuyId.ShouldBe(groupBuy.Id);
        orderInstructionModule.Title.ShouldBe(moduleInput.Title);
        orderInstructionModule.Image.ShouldBe(moduleInput.Image);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Product_Ranking_Module()
    {
        var input = GetInput();
        input.ItemGroups = [];
        var groupBuy = await _groupBuyAppService.CreateAsync(input);
        groupBuy.Id.ShouldNotBe(Guid.Empty);
        groupBuy.GroupBuyNo.ShouldBe(input.GroupBuyNo);
        groupBuy.GroupBuyName.ShouldBe(input.GroupBuyName);

        groupBuy.ItemGroups.Count.ShouldBe(0);

        var moduleInput = GetGroupBuyProductRanking(groupBuy.Id);
        var productRanking = await _groupBuyProductRankingAppService.CreateGroupBuyProductRankingAsync(moduleInput);

        productRanking.ShouldNotBeNull();
        productRanking.Id.ShouldNotBe(Guid.Empty);
        productRanking.GroupBuyId.ShouldBe(groupBuy.Id);
        productRanking.Title.ShouldBe(moduleInput.Title);
    }

    private static GroupBuyCreateDto GetInput()
    {
        // Generate unique data to avoid conflicts
        var uniqueGroupBuyNo = TestDataGenerator.GenerateUniqueGroupBuyNo();
        var uniqueGroupBuyName = TestDataGenerator.GenerateUniqueItemName("Sample Group Buy");
        var uniqueShortCode = TestDataGenerator.GenerateUniqueShortCode();
        
        var input = new GroupBuyCreateDto
        {
            GroupBuyNo = uniqueGroupBuyNo,
            Status = "New",
            GroupBuyName = uniqueGroupBuyName,
            ShortCode = uniqueShortCode,
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

    private static List<GroupBuyItemGroupCreateUpdateDto> GetProductGroupModules()
    {
        return [
            new GroupBuyItemGroupCreateUpdateDto
            {
                SortOrder = 2,
                GroupBuyModuleType = GroupBuyModuleType.ProductGroupModule,
                ProductGroupModuleTitle = "Product Group Module",
                ProductGroupModuleImageSize = "Small",
                ItemDetails = [
                    new(){
                        ItemId = TestData.Item1Id,
                        ItemType = ItemType.Item,
                        SortOrder = 1,
                        DisplayText = "Item",
                        ModuleNumber = 1
                    },
                    new(){
                        SetItemId = TestData.SetItem1Id,
                        ItemType = ItemType.SetItem,
                        SortOrder = 2,  
                        DisplayText = "Set Item",
                        ModuleNumber = 1
                    }]
            }];
    }

    private static GroupPurchaseOverviewDto GetGroupPurchaseOverview(Guid groupBuyId)
    {
        return new GroupPurchaseOverviewDto
        {
            GroupBuyId = groupBuyId,
            Title = "Group Purchase Overview Module",
            SubTitle = "Group Purchase Overview Subtitle",
            BodyText = "This is some body text",
            Image = "https://www.example.com"
        };
    }

    private static GroupBuyOrderInstructionDto GetGroupBuyOrderInstruction(Guid groupBuyId)
    {
        return new GroupBuyOrderInstructionDto
        {
            GroupBuyId = groupBuyId,
            Title = "Group Purchase Overview Module",
            BodyText = "This is some body text",
            Image = "https://www.example.com"
        };
    }

    private static GroupBuyProductRankingDto GetGroupBuyProductRanking(Guid groupBuyId)
    {
        return new GroupBuyProductRankingDto
        {
            GroupBuyId = groupBuyId,
            Title = "Group Purchase Overview Module",
            SubTitle = "This is some body text",
            Content = "https://www.example.com",
            ModuleNumber = 1,
            CarouselImages = ["https://www.example1.com", "https://www.example2.com"]
        };
    }
}