using AntDesign.JsInterop;
using Blazored.TextEditor;
using Blazorise;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using JsonSerializer = System.Text.Json.JsonSerializer;
namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class EditGroupBuy
{
    #region Inject
    [Parameter]
    public string id { get; set; }
    public Guid Id { get; set; }

    private const int MaxtextCount = 60;

    private Modal AddLinkModal { get; set; }
    private CreateImageDto SelectedImageDto = new();
    private GroupBuyDto GroupBuy { get; set; }
    private GroupBuyUpdateDto EditGroupBuyDto { get; set; }
    private List<CreateImageDto> CarouselImages { get; set; }
    public List<StyleForCarouselImages?> StyleForCarouselImages { get; set; } = new List<StyleForCarouselImages?>();
    private List<CreateImageDto> BannerImages { get; set; }
    private const int MaxAllowedFilesPerUpload = 5;
    private const int TotalMaxAllowedFiles = 5;
    private const int MaxAllowedFileSize = 1024 * 1024 * 10;
    private string? TagInputValue { get; set; }
    private IReadOnlyList<string> ItemTags { get; set; }  //used for store item tags 
    private List<string> ShippingMethods { get; set; } = Enum.GetNames(typeof(DeliveryMethod)).ToList();
    private List<string> PaymentMethodTags { get; set; } = new List<string>(); //used for store item tags 
    private string PaymentTagInputValue { get; set; }
    private List<CollapseItem> CollapseItem = new();
    bool CreditCard { get; set; }
    bool BankTransfer { get; set; }
    bool IsCashOnDelivery { get; set; }
    bool IsLinePay { get; set; }
    public List<string> SelfPickupTimeList = new List<string>();
    public List<string> BlackCateDeliveryTimeList = new List<string>();
    public List<string> HomeDeliveryTimeList = new List<string>();
    public List<string> DeliveredByStoreTimeList = new List<string>();
    public string _ProductPicture = "Product Picture";
    private readonly IGroupBuyAppService _groupBuyAppService;
    private readonly IImageAppService _imageAppService;
    private readonly IObjectMapper _objectMapper;
    private readonly IUiMessageService _uiMessageService;
    private readonly IItemAppService _itemAppService;
    private BlazoredTextEditor GroupBuyHtml { get; set; }
    private readonly ISetItemAppService _setItemAppService;
    private BlazoredTextEditor CustomerInformationHtml { get; set; }
    private BlazoredTextEditor ExchangePolicyHtml { get; set; }
    private BlazoredTextEditor NotifyEmailHtml { get; set; }
    protected Validations EditValidationsRef;
    private FilePicker LogoPickerCustom { get; set; }
    private FilePicker BannerPickerCustom { get; set; }
    private FilePicker CarouselPickerCustom { get; set; }
    private List<FilePicker> CarouselFilePickers = [];
    private List<FilePicker> BannerFilePickers = [];
    private List<FilePicker> GroupPurchaseOverviewFilePickers = [];
    private List<FilePicker> GroupBuyOrderInstructionPickers = [];
    private List<FilePicker> ProductRankingCarouselPickers = [];
    private List<ItemWithItemTypeDto> ItemsList { get; set; } = [];
    private readonly ImageContainerManager _imageContainerManager;
    private readonly List<string> ValidFileExtensions = [".jpg", ".png", ".svg", ".jpeg", ".webp"];
    public readonly List<string> ValidPaymentMethods = ["ALL", "Credit", "WebATM", "ATM", "CVS", "BARCODE", "Alipay", "Tenpay", "TopUpUsed", "GooglePay"];
    private string? PaymentMethodError { get; set; } = null;
    private List<ImageDto> ExistingImages { get; set; } = [];
    private List<ImageDto> ExistingBannerImages { get; set; } = [];
    private LoadingIndicator Loading { get; set; } = new();
    private bool LoadingItems { get; set; } = true;
    private int CurrentIndex { get; set; }

    public bool IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = false;

    public bool IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;

    public bool IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = false;

    public GroupBuyTemplateType? SelectedTemplate;

    public bool IsSelectedModule = true;

    public List<List<CreateImageDto>> CarouselModules = [];
    public List<List<CreateImageDto>> BannerModules = [];
    public List<GroupPurchaseOverviewDto> GroupPurchaseOverviewModules = [];
    public List<GroupBuyOrderInstructionDto> GroupBuyOrderInstructionModules = [];
    public List<ProductRankingCarouselModule> ProductRankingCarouselModules = [];

    private readonly IGroupPurchaseOverviewAppService _GroupPurchaseOverviewAppService;

    private readonly IGroupBuyOrderInstructionAppService _GroupBuyOrderInstructionAppService;

    private readonly IGroupBuyProductRankingAppService _GroupBuyProductRankingAppService;

    private List<GroupBuyTemplateType> TemplateTypes = [GroupBuyTemplateType.PikachuOne, GroupBuyTemplateType.PikachuTwo];
    private List<string> ImageSizes = ["SmallImage", "LargeImage"];

    public List<LogisticsProviderSettingsDto> LogisticsProviders = [];

    private readonly ILogisticsProvidersAppService _LogisticsProvidersAppService;

    private bool IsColorPickerOpen = false;
    #endregion

    private List<string> OrderedDeliveryMethods =
    [
        DeliveryMethod.SevenToEleven1.ToString(),
        DeliveryMethod.SevenToElevenC2C.ToString(),
        DeliveryMethod.FamilyMart1.ToString(),
        DeliveryMethod.FamilyMartC2C.ToString(),
        DeliveryMethod.PostOffice.ToString(),
        DeliveryMethod.BlackCat1.ToString(),
        DeliveryMethod.TCatDeliveryNormal.ToString(),
        DeliveryMethod.TCatDeliverySevenElevenNormal.ToString(),
        DeliveryMethod.BlackCatFreeze.ToString(),
        DeliveryMethod.TCatDeliveryFreeze.ToString(),
        DeliveryMethod.TCatDeliverySevenElevenFreeze.ToString(),
        DeliveryMethod.BlackCatFrozen.ToString(),
        DeliveryMethod.SevenToElevenFrozen.ToString(),
        DeliveryMethod.TCatDeliveryFrozen.ToString(),
        DeliveryMethod.TCatDeliverySevenElevenFrozen.ToString(),
        DeliveryMethod.SelfPickup.ToString(),
        DeliveryMethod.HomeDelivery.ToString(),
        DeliveryMethod.DeliveredByStore.ToString()
    ];

    #region Constructor
    public EditGroupBuy(
        IGroupBuyAppService groupBuyAppService,
        IImageAppService imageAppService,
        IObjectMapper objectMapper,
        IUiMessageService uiMessageService,
        ImageContainerManager imageContainerManager,
        IItemAppService itemAppService,
        ISetItemAppService setItemAppService,
        IGroupPurchaseOverviewAppService GroupPurchaseOverviewAppService,
        IGroupBuyOrderInstructionAppService GroupBuyOrderInstructionAppService,
        ILogisticsProvidersAppService LogisticsProvidersAppService,
        IGroupBuyProductRankingAppService GroupBuyProductRankingAppService
    )
    {
        _groupBuyAppService = groupBuyAppService;
        _imageAppService = imageAppService;
        _objectMapper = objectMapper;
        _uiMessageService = uiMessageService;
        _imageContainerManager = imageContainerManager;
        _itemAppService = itemAppService;
        _setItemAppService = setItemAppService;
        _GroupPurchaseOverviewAppService = GroupPurchaseOverviewAppService;
        _GroupBuyOrderInstructionAppService = GroupBuyOrderInstructionAppService;
        _LogisticsProvidersAppService = LogisticsProvidersAppService;
        _GroupBuyProductRankingAppService = GroupBuyProductRankingAppService;
        EditGroupBuyDto = new GroupBuyUpdateDto();
        CarouselImages = [];
        GroupBuy = new();
        Loading = new LoadingIndicator();
        id ??= "";
    }
    #endregion

    #region Methods
    protected override async Task OnInitializedAsync()
    {
        try
        {
            //await Loading.Show();
            LogisticsProviders = await _LogisticsProvidersAppService.GetAllAsync();

            Id = Guid.Parse(id);
            GroupBuy = await _groupBuyAppService.GetWithItemGroupsAsync(Id);

            GroupBuy.ItemGroups = await _groupBuyAppService.GetGroupBuyItemGroupsAsync(Id);

            EditGroupBuyDto = _objectMapper.Map<GroupBuyDto, GroupBuyUpdateDto>(GroupBuy);

            //await LoadHtmlContent();

            EditGroupBuyDto.ShortCode = EditGroupBuyDto?.ShortCode == "" ? null : EditGroupBuyDto?.ShortCode;

            IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = !EditGroupBuyDto.SelfPickupDeliveryTime.IsNullOrEmpty() &&
                                                                EditGroupBuyDto.SelfPickupDeliveryTime.Contains("UnableToSpecifyDuringPeakPeriods") ? true : false;

            IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = !EditGroupBuyDto.HomeDeliveryDeliveryTime.IsNullOrEmpty() &&
                                                                 EditGroupBuyDto.HomeDeliveryDeliveryTime.Contains("UnableToSpecifyDuringPeakPeriods") ? true : false;

            if (!string.IsNullOrEmpty(GroupBuy.ExcludeShippingMethod))
                EditGroupBuyDto.ShippingMethodList = JsonSerializer.Deserialize<List<string>>(GroupBuy.ExcludeShippingMethod);

            if (!string.IsNullOrEmpty(GroupBuy.SelfPickupDeliveryTime))
                SelfPickupTimeList = JsonSerializer.Deserialize<List<string>>(GroupBuy.SelfPickupDeliveryTime);

            if (!string.IsNullOrEmpty(GroupBuy.BlackCatDeliveryTime))
                BlackCateDeliveryTimeList = JsonSerializer.Deserialize<List<string>>(GroupBuy.BlackCatDeliveryTime);

            if (!string.IsNullOrEmpty(GroupBuy.HomeDeliveryDeliveryTime))
                HomeDeliveryTimeList = JsonSerializer.Deserialize<List<string>>(GroupBuy.HomeDeliveryDeliveryTime);

            if (EditGroupBuyDto.FreeShipping && EditGroupBuyDto.FreeShippingThreshold == null)
                EditGroupBuyDto.FreeShippingThreshold = 0;

            if (!GroupBuy.PaymentMethod.IsNullOrEmpty())
            {
                string[] payments = GroupBuy.PaymentMethod.Split(" , ");

                CreditCard = payments.Contains("Credit Card");
                BankTransfer = payments.Contains("Bank Transfer");
                IsCashOnDelivery = payments.Contains("Cash On Delivery");
                IsLinePay = payments.Contains("LinePay");
            }

            EditGroupBuyDto.EntryURL = $"{(await MyTenantAppService.FindTenantUrlAsync(CurrentTenant.Id))?.TrimEnd('/')}/groupBuy/{Id}";

            SelectedTemplate = GroupBuy.TemplateType;

            IsColorPickerOpen = EditGroupBuyDto.ColorSchemeType is not null;

            await LoadItemGroups();

            if (EditValidationsRef is not null) await EditValidationsRef.ClearAll();
            //
            StateHasChanged();
            //await Loading.Hide();
        }
        catch (Exception ex)
        {
            //await Loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            //await Loading.Hide();
        }
    }

    public void OnEnterPriseChange(bool e)
    {
        EditGroupBuyDto.IsEnterprise = e;

        if (EditGroupBuyDto.IsEnterprise)
        {
            IsCashOnDelivery = true; CreditCard = false; BankTransfer = false;IsLinePay = false;
            OnShippingMethodCheckedChange("SelfPickup", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("SelfPickup", new ChangeEventArgs { Value = true });
            OnShippingMethodCheckedChange("DeliveredByStore", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("HomeDelivery", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("BlackCat1", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("BlackCatFreeze", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("BlackCatFrozen", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("TCatDeliveryNormal", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("TCatDeliveryFreeze", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("TCatDeliveryFrozen", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("TCatDeliverySevenElevenNormal", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("TCatDeliverySevenElevenFreeze", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("TCatDeliverySevenElevenFrozen", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("SevenToEleven1", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("SevenToElevenFrozen", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("SevenToElevenC2C", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("FamilyMart1", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("FamilyMartC2C", new ChangeEventArgs { Value = false });
            OnShippingMethodCheckedChange("PostOffice", new ChangeEventArgs { Value = false });
        }
    }

    private void OnColorSchemeChange(ChangeEventArgs e)
    {
        string? selectedTheme = e.Value.ToString()!= "Choose Color Theme"? e.Value.ToString() :null;
       
        EditGroupBuyDto.ColorSchemeType = !selectedTheme.IsNullOrEmpty() ? Enum.Parse<ColorScheme>(selectedTheme) : null;

        IsColorPickerOpen = true;
        if (GroupBuyOrderInstructionModules.Count > 0)
        {
            string[] fileNames = { "Forest Dawn.png", "Tropical Sunset.png", "Deep Sea Night.png", "Sweet Apricot Cream.png", "Desert Dawn.png" };
            foreach (var item in GroupBuyOrderInstructionModules)
            {
                if (fileNames.Any(fileName => item.Image.Contains(fileName)))
                {
                    item.Image = "https://pikachublobs.blob.core.windows.net/images/" + L["Enum:ColorSchemeFile." + (int)EditGroupBuyDto.ColorSchemeType.Value] + ".png";
                }

            }

        }

        switch (EditGroupBuyDto.ColorSchemeType)
        {
            case ColorScheme.ForestDawn:
                EditGroupBuyDto.PrimaryColor = "#23856D";
                EditGroupBuyDto.SecondaryColor = "#FFD057";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#C9D6BD";
                EditGroupBuyDto.AlertColor = "#FF902A";
                EditGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.TropicalSunset:
                EditGroupBuyDto.PrimaryColor = "#FF902A";
                EditGroupBuyDto.SecondaryColor = "#BDDA8D";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#E5D19A";
                EditGroupBuyDto.AlertColor = "#FF902A";
                EditGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.DeepSeaNight:
                EditGroupBuyDto.PrimaryColor = "#133854";
                EditGroupBuyDto.SecondaryColor = "#CAE28D";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#DCD6D0";
                EditGroupBuyDto.AlertColor = "#A1E82D";
                EditGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.SweetApricotCream:
                EditGroupBuyDto.PrimaryColor = "#FFA085";
                EditGroupBuyDto.SecondaryColor = "#BDDA8D";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#DCBFC3";
                EditGroupBuyDto.AlertColor = "#FFC123";
                EditGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.DesertDawn:
                EditGroupBuyDto.PrimaryColor = "#C08C5D";
                EditGroupBuyDto.SecondaryColor = "#E7AD99";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#EBC7AD";
                EditGroupBuyDto.AlertColor = "#FF902A";
                EditGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            default:
                EditGroupBuyDto.PrimaryColor = string.Empty;
                EditGroupBuyDto.SecondaryColor = string.Empty;
                EditGroupBuyDto.BackgroundColor = string.Empty;
                EditGroupBuyDto.SecondaryBackgroundColor = string.Empty;
                EditGroupBuyDto.AlertColor = string.Empty;
                EditGroupBuyDto.BlockColor = string.Empty;
                IsColorPickerOpen = false;
                break;
        }
    }

    private void OnProductDetailsDisplayMethodChange(ChangeEventArgs e)
    {
        string? selectedMethod = e.Value.ToString();

        EditGroupBuyDto.ProductDetailsDisplayMethod = !selectedMethod.IsNullOrEmpty() ?
                                                      Enum.Parse<ProductDetailsDisplayMethod>(selectedMethod) :
                                                      null;
    }

    public bool IsShippingMethodEnabled(string method)
    {
        if (LogisticsProviders is { Count: 0 }) return false;

        if (!Enum.TryParse(method, out DeliveryMethod deliveryMethod))
            return false;

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

        return providerMapping.TryGetValue(deliveryMethod, out var provider) &&
               LogisticsProviders.FirstOrDefault(w => w.LogisticProvider == provider)?.IsEnabled == true;
    }

    private void SelectTemplate(ChangeEventArgs e)
    {
        SelectedTemplate = Enum.Parse<GroupBuyTemplateType>(e.Value.ToString());

        EditGroupBuyDto.TemplateType = SelectedTemplate;

        IsSelectedModule = false;

        List<GroupBuyModuleType> templateModules = new();

        if (SelectedTemplate is GroupBuyTemplateType.PikachuOne) templateModules = [.. GetPikachuOneList()];

        else if (SelectedTemplate is GroupBuyTemplateType.PikachuTwo) templateModules = [.. GetPikachuTwoList()];

        if (templateModules is { Count: > 0 })
        {
            foreach (CollapseItem module in CollapseItem)
            {
                if (templateModules.Contains(module.GroupBuyModuleType)) module.IsWarnedForInCompatible = false;

                else module.IsWarnedForInCompatible = true;
            }
        }
    }

    private void OpenAddLinkModal(CreateImageDto createImageDto)
    {
        SelectedImageDto = createImageDto;

        AddLinkModal.Show();
    }
    private void CloseAddLinkModal()
    {
        AddLinkModal.Hide();
    }
    private async Task ApplyAddLinkAsync()
    {
        await AddLinkModal.Hide();

        await InvokeAsync(StateHasChanged);
    }
    private Task OnModalClosing(ModalClosingEventArgs e)
    {
        return Task.CompletedTask;
    }

    public IEnumerable<GroupBuyModuleType> GetPikachuOneList()
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

    public IEnumerable<GroupBuyModuleType> GetPikachuTwoList()
    {
        return [
            GroupBuyModuleType.ProductGroupModule,
            GroupBuyModuleType.CarouselImages,
            GroupBuyModuleType.IndexAnchor,
            GroupBuyModuleType.BannerImages,
            GroupBuyModuleType.GroupPurchaseOverview,
            GroupBuyModuleType.CountdownTimer,
            GroupBuyModuleType.OrderInstruction,
            GroupBuyModuleType.ProductRankingCarouselModule
        ];
    }

    public async Task GetProductRankingCarouselsAsync()
    {
        try
        {
            //await Task.Delay(500);

            List<ImageDto> productRankingImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyProductRankingCarousel);

            List<CreateImageDto> createImageDtos = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(productRankingImages);

            List<List<CreateImageDto>> createImagesGroupedByModules = [.. createImageDtos.GroupBy(g => g.ModuleNumber).Select(s => s.ToList())];

            List<GroupBuyProductRankingDto> productRankingDto = await _GroupBuyProductRankingAppService.GetListByGroupBuyIdAsync(Id);

            foreach (GroupBuyProductRankingDto ranking in productRankingDto)
            {
                ProductRankingCarouselModules.Add(new()
                {
                    Id = ranking.Id,
                    Title = ranking.Title,
                    Content = ranking.Content,
                    SubTitle = ranking.SubTitle,
                    ModuleNumber = ranking.ModuleNumber
                });
            }

            foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
            {
                module.Images = [.. createImageDtos.Where(w => w.ModuleNumber == module.ModuleNumber)];

                List<GroupBuyItemGroupDetailsDto> itemDetails = GroupBuy.ItemGroups
                                                                        .SelectMany(w => w.ItemGroupDetails.Where(d => d.ModuleNumber == module.ModuleNumber))
                                                                        .ToList();

                foreach (GroupBuyItemGroupDetailsDto groupBuyItemGroup in itemDetails)
                {
                    module.Selected.Add(new()
                    {
                        Id = groupBuyItemGroup.Id,
                        Item = groupBuyItemGroup?.Item,
                        ItemType = groupBuyItemGroup.ItemType,
                        Name = groupBuyItemGroup.Item != null ? groupBuyItemGroup.Item.ItemName : "",
                        SetItem = groupBuyItemGroup.SetItem
                    });
                }

                while (module.Selected.Count < 3)
                {
                    module.Selected.Add(new());
                }
            }
        }
        catch (Exception ex)
        {
            await GetProductRankingCarouselsAsync();
        }
    }

    protected override async Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (isFirstRender)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("updateDropText");
                await Loading.Show();
                await OnInitializedAsync();
                //await LoadHtmlContent();
                //ExistingImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyCarouselImage);

                //ExistingBannerImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyBannerImage);

                //CarouselImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingImages);

                //CarouselModules = CarouselImages.GroupBy(g => g.ModuleNumber).Select(s => s.ToList()).ToList();

                //BannerImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingBannerImages);

                // BannerModules = [.. BannerImages.GroupBy(g => g.ModuleNumber).Select(s => s.ToList())];

                GroupPurchaseOverviewModules = await _GroupPurchaseOverviewAppService.GetListByGroupBuyIdAsync(Id);

                GroupBuyOrderInstructionModules = await _GroupBuyOrderInstructionAppService.GetListByGroupBuyIdAsync(Id);

                await GetProductRankingCarouselsAsync();

                //foreach (List<CreateImageDto> carouselImages in CarouselModules)
                //{
                //    int? moduleNumber = carouselImages.GroupBy(g => g.ModuleNumber).FirstOrDefault().Key;

                //    CollapseItem collapseItem = new()
                //    {
                //        Id = GroupBuy.ItemGroups.FirstOrDefault(w => w.ModuleNumber == moduleNumber && w.GroupBuyModuleType == GroupBuyModuleType.CarouselImages).Id,
                //        Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                //        SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                //        GroupBuyModuleType = GroupBuyModuleType.CarouselImages,
                //        ModuleNumber = moduleNumber
                //    };

                //    //if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.CarouselImages))
                //    //    CollapseItem.Add(new()
                //    //    {
                //    //        Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                //    //        SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                //    //        GroupBuyModuleType = GroupBuyModuleType.CarouselImages
                //    //    });

                //    CollapseItem.Add(collapseItem);

                //    CarouselFilePickers.Add(new());
                //}

                //if (CarouselModules is { Count: 0 })
                //{
                //    CarouselFilePickers.Add(new());

                //    CarouselModules.Add([]);
                //}

                //foreach (List<CreateImageDto> carouselImages in BannerModules)
                //{
                //    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.BannerImages))
                //        CollapseItem.Add(new()
                //        {
                //            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                //            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                //            GroupBuyModuleType = GroupBuyModuleType.BannerImages
                //        });

                //    BannerFilePickers.Add(new());
                //}

                //if (BannerModules is { Count: 0 })
                //{
                //    BannerFilePickers.Add(new());

                //    BannerModules.Add([]);
                //}

                foreach (GroupPurchaseOverviewDto module in GroupPurchaseOverviewModules)
                {
                    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview))
                        CollapseItem.Add(new()
                        {
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                            GroupBuyModuleType = GroupBuyModuleType.GroupPurchaseOverview
                        });

                    GroupPurchaseOverviewFilePickers.Add(new());
                }

                if (GroupPurchaseOverviewModules is { Count: 0 } && CollapseItem.Any(x => x.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview))
                {
                    GroupPurchaseOverviewFilePickers.Add(new());

                    GroupPurchaseOverviewModules.Add(new());
                }

                foreach (GroupBuyOrderInstructionDto module in GroupBuyOrderInstructionModules)
                {
                    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction))
                        CollapseItem.Add(new()
                        {
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                            GroupBuyModuleType = GroupBuyModuleType.OrderInstruction
                        });

                    GroupBuyOrderInstructionPickers.Add(new());
                }

                if (GroupBuyOrderInstructionModules is { Count: 0 } && CollapseItem.Any(x => x.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction))
                {
                    GroupBuyOrderInstructionPickers.Add(new());

                    GroupBuyOrderInstructionModules.Add(new());
                }

                foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                {
                    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule))
                        CollapseItem.Add(new()
                        {
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                            GroupBuyModuleType = GroupBuyModuleType.ProductRankingCarouselModule
                        });

                    ProductRankingCarouselPickers.Add(new());
                }

                if (ProductRankingCarouselModules is { Count: 0 } && CollapseItem.Any(x => x.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule))
                {
                    ProductRankingCarouselPickers.Add(new());

                    ProductRankingCarouselModules.Add(new()
                    {
                        Selected = [
                            new ItemWithItemTypeDto(),
                            new ItemWithItemTypeDto(),
                            new ItemWithItemTypeDto()
                        ]
                    });
                }

                ItemsList = await _itemAppService.GetItemsLookupAsync();
                var setItemsList = await _setItemAppService.GetItemsLookupAsync();
                ItemsList.AddRange(setItemsList);
                LoadingItems = false;
                await LoadHtmlContent();

                StateHasChanged();
                await Loading.Hide();
            }
            catch (BusinessException ex)
            {
                await Loading.Hide();
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
                await _uiMessageService.Error(L[ex.Code]);

            }
            catch (Exception ex)
            {
                await Loading.Hide();
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
                await _uiMessageService.Error(ex.GetType().ToString());
            }
            finally
            {
                await Loading.Hide();
                LoadingItems = false;
            }
        }
    }

    private async Task LoadHtmlContent()
    {
        await Task.Delay(2);
        if (EditGroupBuyDto.GroupBuyName == null)
        {
            await LoadHtmlContent();
        }
        //await GroupBuyHtml.LoadHTMLContent(EditGroupBuyDto.GroupBuyConditionDescription);
        //await CustomerInformationHtml.LoadHTMLContent(EditGroupBuyDto.CustomerInformationDescription);
        await ExchangePolicyHtml.LoadHTMLContent(EditGroupBuyDto.ExchangePolicyDescription);
        await NotifyEmailHtml.LoadHTMLContent(EditGroupBuyDto.NotifyMessage);
    }

    private async Task LoadItemGroups(bool isRefreshItemGroup = false)
    {
        if (CollapseItem.Count == 0)
        {
            ICollection<GroupBuyItemGroupDto> itemGroups = [];

            if (isRefreshItemGroup)
                GroupBuy.ItemGroups = await _groupBuyAppService.GetGroupBuyItemGroupsAsync(Id);

            itemGroups = GroupBuy.ItemGroups;

            if (itemGroups.Any())
            {
                if (itemGroups.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.CarouselImages))
                {
                    ExistingImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyCarouselImage);

                    CarouselImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingImages);
                }

                else if (itemGroups.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.BannerImages))
                {
                    ExistingBannerImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyBannerImage);

                    BannerImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingBannerImages);
                }

                foreach (var itemGroup in itemGroups)
                {
                    CollapseItem collapseItem = new();

                    if (itemGroup.GroupBuyModuleType is GroupBuyModuleType.CarouselImages ||
                        itemGroup.GroupBuyModuleType is GroupBuyModuleType.BannerImages ||
                        itemGroup.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview ||
                        itemGroup.GroupBuyModuleType is GroupBuyModuleType.CountdownTimer ||
                        itemGroup.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction ||
                        itemGroup.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
                    {
                        collapseItem = new()
                        {
                            Id = itemGroup.Id,
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = itemGroup.SortOrder,
                            GroupBuyModuleType = itemGroup.GroupBuyModuleType,
                            AdditionalInfo = itemGroup.AdditionalInfo,
                            ModuleNumber = itemGroup.ModuleNumber
                        };

                        if (itemGroup.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
                        {
                            List<CreateImageDto> imageList = await _imageAppService.GetImageListByModuleNumberAsync(Id, ImageType.GroupBuyCarouselImage, collapseItem.ModuleNumber!.Value);

                            CarouselFilePickers.Add(new());

                            CarouselModules.Add(imageList is { Count: > 0 } ? imageList : [new() { ModuleNumber = collapseItem.ModuleNumber }]);
                        }

                        else if (itemGroup.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
                        {
                            List<CreateImageDto> imageList = await _imageAppService.GetImageListByModuleNumberAsync(Id, ImageType.GroupBuyBannerImage, collapseItem.ModuleNumber!.Value);

                            BannerFilePickers.Add(new());

                            BannerModules.Add(imageList is { Count: > 0 } ? imageList : [new() { ModuleNumber = collapseItem.ModuleNumber }]);
                        }
                    }

                    else
                    {
                        collapseItem = new CollapseItem
                        {
                            Id = itemGroup.Id,
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = itemGroup.SortOrder,
                            GroupBuyModuleType = itemGroup.GroupBuyModuleType,
                            ProductGroupModuleTitle = itemGroup.ProductGroupModuleTitle,
                            ProductGroupModuleImageSize = itemGroup.ProductGroupModuleImageSize,
                            Selected = []
                        };
                    }

                    CollapseItem.Add(collapseItem);
                }
            }
        }
    }

    async Task OnCollapseVisibleChanged(CollapseItem collapseItem, bool isVisible)
    {
        if (collapseItem.Selected.Count > 0)
        {
            return;
        }

        try
        {
            if (isVisible && collapseItem.Id.HasValue)
            {
                await Loading.Show();

                var itemGroup = await _groupBuyAppService.GetGroupBuyItemGroupAsync(collapseItem.Id.Value);

                int index = CollapseItem.IndexOf(collapseItem);
                CollapseItem[index].IsModified = true;
                if (itemGroup != null)
                {
                    while (LoadingItems)
                    {
                        await Task.Delay(100);
                    }
                    foreach (var item in itemGroup.ItemGroupDetails.OrderBy(x => x.SortOrder))
                    {
                        if (itemGroup.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor)
                        {
                            var itemWithItemType = new ItemWithItemTypeDto
                            {
                                Id = item.ItemId ?? Guid.Empty,
                                Name = item.DisplayText,
                                ItemType = item.ItemType,
                                Item = item.Item,
                                SetItem = item.SetItem,
                                IsFirstLoad = item.ItemId == null && item.SetItemId == null
                            };
                            CollapseItem[index].Selected.Add(itemWithItemType);
                        }
                        else
                        {
                            var itemWithItemType = new ItemWithItemTypeDto
                            {
                                Id = item.ItemType == ItemType.Item ? item.ItemId.Value : item.SetItemId.Value,
                                Name = item.ItemType == ItemType.Item ? item.Item.ItemName : item.SetItem.SetItemName,
                                ItemType = item.ItemType,
                                Item = item.Item,
                                SetItem = item.SetItem
                            };
                            CollapseItem[index].Selected.Add(itemWithItemType);
                        }

                        StateHasChanged();
                    }

                    if (itemGroup.GroupBuyModuleType is not GroupBuyModuleType.ProductDescriptionModule
                        && itemGroup.GroupBuyModuleType is not GroupBuyModuleType.IndexAnchor
                        && itemGroup.GroupBuyModuleType is not GroupBuyModuleType.CarouselImages
                        && itemGroup.GroupBuyModuleType is not GroupBuyModuleType.BannerImages
                        && itemGroup.GroupBuyModuleType is not GroupBuyModuleType.GroupPurchaseOverview
                        && itemGroup.GroupBuyModuleType is not GroupBuyModuleType.CountdownTimer
                        && itemGroup.GroupBuyModuleType is not GroupBuyModuleType.OrderInstruction
                        && itemGroup.ItemGroupDetails.Count < 3)
                    {
                        for (int x = itemGroup.ItemGroupDetails.Count; x < 3; x++)
                        {
                            CollapseItem[index].Selected.Add(new ItemWithItemTypeDto());
                        }
                    }
                }
                else
                {
                    CollapseItem[index].Selected = [];
                }
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            StateHasChanged();
            await Loading.Hide();
        }
    }

    void AddProductItem(GroupBuyModuleType groupBuyModuleType)
    {
        if (CollapseItem.Count >= 20)
        {
            _uiMessageService.Error(L[PikachuDomainErrorCodes.CanNotAddMoreThan20Modules]);
            return;
        }

        if (groupBuyModuleType is GroupBuyModuleType.ProductDescriptionModule
            || groupBuyModuleType is GroupBuyModuleType.IndexAnchor)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected =
                [
                    new ItemWithItemTypeDto()
                ]
            };

            CollapseItem.Add(collapseItem);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.CarouselImages)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                ModuleNumber = CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.CarouselImages) > 0 ?
                               CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.CarouselImages) + 1 : 1
            };

            CollapseItem.Add(collapseItem);

            CarouselFilePickers.Add(new());

            CarouselModules.Add([new() { ModuleNumber = collapseItem.ModuleNumber }]);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.BannerImages)
        {

            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                ModuleNumber = CollapseItem.Any(c => c.GroupBuyModuleType is GroupBuyModuleType.BannerImages) ?
                               CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.BannerImages) + 1 : 1
            };

            CollapseItem.Add(collapseItem);

            BannerFilePickers.Add(new());

            BannerModules.Add([new() { ModuleNumber = collapseItem.ModuleNumber }]);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview)
        {
            if (GroupPurchaseOverviewModules is { Count: 0 })
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            GroupPurchaseOverviewFilePickers.Add(new());

            GroupPurchaseOverviewModules.Add(new());
        }

        else if (groupBuyModuleType is GroupBuyModuleType.OrderInstruction)
        {
            if (GroupBuyOrderInstructionModules is { Count: 0 })
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            GroupBuyOrderInstructionPickers.Add(new());

            string? img = null;
            if (EditGroupBuyDto.ColorSchemeType.HasValue)
            {
                img = "https://pikachublobs.blob.core.windows.net/images/" + L["Enum:ColorSchemeFile." + (int)EditGroupBuyDto.ColorSchemeType.Value] + ".png";
            }
            GroupBuyOrderInstructionModules.Add(new GroupBuyOrderInstructionDto
            {
                Title = L["OrderInstruction"],
                Image = img
            });
        }

        else if (groupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
        {

            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                ModuleNumber = CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule) > 0 ?
                           CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule) + 1 : 1
            };

            CollapseItem.Add(collapseItem);


            ProductRankingCarouselPickers.Add(new());

            ProductRankingCarouselModules.Add(new()
            {
                ModuleNumber = collapseItem.ModuleNumber,
                Selected = [
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto()
                ]
            });
        }

        else if (groupBuyModuleType is GroupBuyModuleType.CountdownTimer)
        {
            if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.CountdownTimer))
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }
        }

        else
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected =
                [
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto()
                ]
            };

            CollapseItem.Add(collapseItem);
        }

        List<GroupBuyModuleType> templateModules = new();

        if (SelectedTemplate is GroupBuyTemplateType.PikachuOne) templateModules = [.. GetPikachuOneList()];

        else if (SelectedTemplate is GroupBuyTemplateType.PikachuTwo) templateModules = [.. GetPikachuTwoList()];

        if (templateModules is { Count: > 0 })
        {
            foreach (CollapseItem module in CollapseItem)
            {
                if (templateModules.Contains(module.GroupBuyModuleType)) module.IsWarnedForInCompatible = false;

                else module.IsWarnedForInCompatible = true;
            }
        }
    }

    #region GroupPurchaseOverview Module Section
    private void ToggleButtonVisibility(bool e, GroupPurchaseOverviewDto module)
    {
        if (!module.IsButtonEnable)
        {
            module.ButtonText = null;

            module.ButtonLink = null;
        }
    }

    public async Task OnImageUploadAsync(
        FileChangedEventArgs e,
        GroupPurchaseOverviewDto module,
        FilePicker filePicker
    )
    {
        try
        {
            if (e.Files.Length is 0) return;

            if (e.Files.Length > 1)
            {
                await _uiMessageService.Error("Cannot add more 1 image.");

                await filePicker.Clear();

                return;
            }

            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);

                await filePicker.Clear();

                return;
            }

            string newFileName = Path.ChangeExtension(
                Guid.NewGuid().ToString().Replace("-", ""),
                Path.GetExtension(e.Files[0].Name)
            );

            Stream stream = e.Files[0].OpenReadStream(long.MaxValue);

            try
            {
                MemoryStream memoryStream = new();

                await stream.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                string url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                module.Image = url;

                await filePicker.Clear();
            }
            finally
            {
                stream.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    public async Task DeleteImageAsync(MouseEventArgs e, GroupPurchaseOverviewDto module)
    {
        bool confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);

        if (confirmed)
        {
            await _imageContainerManager.DeleteAsync(module.Image);

            module.Image = string.Empty;

            StateHasChanged();
        }
    }
    #endregion

    #region GroupBuyOrderInstruction Module Section
    public async Task OnImageUploadAsync(
        FileChangedEventArgs e,
        GroupBuyOrderInstructionDto module,
        FilePicker filePicker
    )
    {
        try
        {
            if (e.Files.Length is 0) return;

            if (e.Files.Length > 1)
            {
                await _uiMessageService.Error("Cannot add more 1 image.");

                await filePicker.Clear();

                return;
            }

            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);

                await filePicker.Clear();

                return;
            }

            string newFileName = Path.ChangeExtension(
                Guid.NewGuid().ToString().Replace("-", ""),
                Path.GetExtension(e.Files[0].Name)
            );

            Stream stream = e.Files[0].OpenReadStream(long.MaxValue);

            try
            {
                MemoryStream memoryStream = new();

                await stream.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                string url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                module.Image = url;

                await filePicker.Clear();
            }
            finally
            {
                stream.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    public async Task DeleteImageAsync(MouseEventArgs e, GroupBuyOrderInstructionDto module)
    {
        bool confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);

        if (confirmed)
        {
            await _imageContainerManager.DeleteAsync(module.Image);

            module.Image = string.Empty;

            StateHasChanged();
        }
    }
    #endregion
    private string LocalizeFilePicker(string key, object[] args)
    {
        return L[key];
    }

    async Task OnLogoUploadAsync(FileChangedEventArgs e)
    {
        if (e.Files.Length > 1)
        {
            await _uiMessageService.Error("Select Only 1 Logo Upload");
            await LogoPickerCustom.Clear();
            return;
        }
        if (e.Files.Length == 0)
        {
            return;
        }
        try
        {
            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);
                await LogoPickerCustom.Clear();
                return;
            }
            if (e.Files[0].Size > MaxAllowedFileSize)
            {
                await LogoPickerCustom.RemoveFile(e.Files[0]);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                return;
            }
            string newFileName = Path.ChangeExtension(
                  Guid.NewGuid().ToString().Replace("-", ""),
                  Path.GetExtension(e.Files[0].Name));
            var stream = e.Files[0].OpenReadStream(long.MaxValue);
            try
            {
                await Loading.Show();
                var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);
                EditGroupBuyDto.LogoURL = url;
                await LogoPickerCustom.Clear();
            }
            finally
            {
                await Loading.Hide();
                stream.Close();
            }
        }
        catch (Exception exc)
        {
            await Loading.Hide();
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    async Task OnImageModuleUploadAsync(
        FileChangedEventArgs e,
        List<CreateImageDto> carouselImages,
        int carouselModuleNumber,
        FilePicker carouselPicker,
        ImageType imageType
    )
    {
        if (carouselImages.Count >= 5)
        {
            await carouselPicker.Clear();
            return;
        }

        int count = 0;
        try
        {
            foreach (var file in e.Files.Take(5))
            {
                if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name)))
                {
                    await carouselPicker.RemoveFile(file);
                    await _uiMessageService.Error(L["InvalidFileType"]);
                    continue;
                }
                if (file.Size > MaxAllowedFileSize)
                {
                    count++;
                    await carouselPicker.RemoveFile(file);
                    return;
                }
                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(file.Name));
                var stream = file.OpenReadStream(long.MaxValue);
                try
                {
                    await Loading.Show();
                    var memoryStream = new MemoryStream();

                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                    int sortNo = carouselImages.LastOrDefault()?.SortNo ?? 0;

                    if (sortNo is 0 && carouselImages.Any(a => a.Id != Guid.Empty && a.CarouselStyle != null && a.BlobImageName == string.Empty))
                    {
                        carouselImages[0].Name = file.Name;
                        carouselImages[0].BlobImageName = newFileName;
                        carouselImages[0].ImageUrl = url;
                        carouselImages[0].ImageType = imageType;
                        carouselImages[0].SortNo = sortNo + 1;
                        carouselImages[0].ModuleNumber = carouselModuleNumber;
                    }

                    else
                    {
                        //int indexInCarouselModules = CarouselModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));
                        int indexInCarouselModules = 0;

                        if (imageType is ImageType.GroupBuyCarouselImage)
                            indexInCarouselModules = CarouselModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));

                        else if (imageType is ImageType.GroupBuyBannerImage)
                            indexInCarouselModules = BannerModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));
                        else if (imageType is ImageType.GroupBuyProductRankingCarousel)
                            indexInCarouselModules = ProductRankingCarouselModules.FindIndex(img => img.ModuleNumber == carouselModuleNumber);

                        if (indexInCarouselModules >= 0)
                        {
                            //List<CreateImageDto> originalCarouselImages = CarouselModules[indexInCarouselModules];

                            List<CreateImageDto> originalCarouselImages = new();

                            if (imageType is ImageType.GroupBuyCarouselImage)
                                originalCarouselImages = CarouselModules[indexInCarouselModules];

                            else if (imageType is ImageType.GroupBuyBannerImage)
                                originalCarouselImages = BannerModules[indexInCarouselModules];
                            else if (imageType is ImageType.GroupBuyProductRankingCarousel)
                                originalCarouselImages = ProductRankingCarouselModules[indexInCarouselModules].Images;
                            if (originalCarouselImages.Any(a => a.SortNo is 0))
                            {
                                int index = originalCarouselImages.IndexOf(originalCarouselImages.First(f => f.SortNo == 0));

                                originalCarouselImages[index].Name = file.Name;
                                originalCarouselImages[index].BlobImageName = newFileName;
                                originalCarouselImages[index].ImageUrl = url;
                                originalCarouselImages[index].ImageType = imageType;
                                originalCarouselImages[index].SortNo = sortNo + 1;
                                originalCarouselImages[index].ModuleNumber = carouselModuleNumber;
                            }

                            else
                            {
                                originalCarouselImages.Add(new CreateImageDto
                                {
                                    Name = file.Name,
                                    BlobImageName = newFileName,
                                    ImageUrl = url,
                                    ImageType = imageType,
                                    SortNo = sortNo + 1,
                                    ModuleNumber = carouselModuleNumber,
                                    CarouselStyle = originalCarouselImages.FirstOrDefault(f => f.CarouselStyle != null)?.CarouselStyle
                                });
                            }
                        }
                    }

                    await carouselPicker.Clear();
                }
                finally
                {
                    await Loading.Hide();
                    stream.Close();
                }
            }
            if (count > 0)
            {
                await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception exc)
        {
            await Loading.Hide();
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    async Task OnBannerImageModuleUploadAsync(
        FileChangedEventArgs e,
        List<CreateImageDto> bannerImages,
        int carouselModuleNumber,
        FilePicker bannerPicker,
        ImageType imageType
    )
    {

        if (e.Files.Length > 1)
        {
            await _uiMessageService.Error("Select Only 1 Banner to Upload");
            await LogoPickerCustom.Clear();
            return;
        }
        if (e.Files.Length == 0)
        {
            return;
        }

        int count = 0;
        try
        {
            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);
                await LogoPickerCustom.Clear();
                return;
            }
            if (e.Files[0].Size > MaxAllowedFileSize)
            {
                await LogoPickerCustom.RemoveFile(e.Files[0]);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                return;
            }
            string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(e.Files[0].Name));
            var stream = e.Files[0].OpenReadStream(long.MaxValue);
            try
            {
                var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                int sortNo = bannerImages[0].SortNo is 0 ? bannerImages[0].SortNo + 1 : 1;

                bannerImages[0].Name = e.Files[0].Name;
                bannerImages[0].BlobImageName = newFileName;
                bannerImages[0].ImageUrl = url;
                bannerImages[0].ImageType = imageType;
                bannerImages[0].SortNo = sortNo;
                bannerImages[0].ModuleNumber = carouselModuleNumber;

                SelectedImageDto.Link = string.Empty;

                await bannerPicker.Clear();
            }
            finally
            {
                stream.Close();
            }

            if (count > 0)
            {
                await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    void OnStyleCarouselChange(ChangeEventArgs e, List<CreateImageDto> carouselImages, int carouselModuleNumber)
    {
        if (carouselImages is { Count: 0 })
        {
            carouselImages.Add(new CreateImageDto()
            {
                Name = string.Empty,
                ImageUrl = string.Empty,
                ImageType = ImageType.GroupBuyCarouselImage,
                BlobImageName = string.Empty,
                CarouselStyle = e.Value is not null ? Enum.Parse<StyleForCarouselImages>(e.Value.ToString()) : null,
                ModuleNumber = carouselModuleNumber,
                SortNo = 0
            });
        }

        else
        {
            foreach (CreateImageDto image in carouselImages)
            {
                image.CarouselStyle = e.Value is not null ? Enum.Parse<StyleForCarouselImages>(e.Value.ToString()) : null;
            }
        }

        StateHasChanged();
    }

    async Task OnBannerUploadAsync(FileChangedEventArgs e)
    {
        if (e.Files.Length > 1)
        {
            await _uiMessageService.Error("Select Only 1 Logo Upload");
            await BannerPickerCustom.Clear();
            return;
        }
        if (e.Files.Length == 0)
        {

            return;
        }
        try
        {
            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);
                await BannerPickerCustom.Clear();
                return;
            }
            if (e.Files[0].Size > MaxAllowedFileSize)
            {
                await BannerPickerCustom.RemoveFile(e.Files[0]);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                return;
            }
            string newFileName = Path.ChangeExtension(
                  Guid.NewGuid().ToString().Replace("-", ""),
                  Path.GetExtension(e.Files[0].Name));
            var stream = e.Files[0].OpenReadStream(long.MaxValue);
            try
            {
                await Loading.Show();
                var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                EditGroupBuyDto.BannerURL = url;

                await BannerPickerCustom.Clear();
            }
            finally
            {
                await Loading.Hide();
                stream.Close();
            }
        }
        catch (Exception exc)
        {
            await Loading.Hide();
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    void SelfPickupDeliveryTimeCheckedChange(string method, ChangeEventArgs e)
    {
        if (method is PikachuResource.UnableToSpecifyDuringPeakPeriods)
        {
            if (IsUnableToSpecifyDuringPeakPeriodsForSelfPickups)
            {
                SelfPickupTimeList.Clear();

                IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = false;
            }
            else
            {
                IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = true;

                SelfPickupTimeList.Clear();

                SelfPickupTimeList.Add(PikachuResource.UnableToSpecifyDuringPeakPeriods);
            }
        }

        bool value = (bool)(e?.Value ?? false);

        if (!IsUnableToSpecifyDuringPeakPeriodsForSelfPickups)
        {
            if (value) SelfPickupTimeList.Add(method);

            else SelfPickupTimeList.Remove(method);
        }

        EditGroupBuyDto.SelfPickupDeliveryTime = JsonConvert.SerializeObject(SelfPickupTimeList);
    }
    void BlackCatDeliveryTimeCheckedChange(string method, ChangeEventArgs e)
    {
        var value = (bool)(e?.Value ?? false);
        if (value)
        {
            if(!BlackCateDeliveryTimeList.Contains(method))
            BlackCateDeliveryTimeList.Add(method);
        }
        else
        {
            BlackCateDeliveryTimeList.Remove(method);
        }

        EditGroupBuyDto.BlackCatDeliveryTime = JsonConvert.SerializeObject(BlackCateDeliveryTimeList.Distinct());
    }
    void HomeDeliveryTimeCheckedChange(string method, ChangeEventArgs e, bool clearAll = false)
    {
        if (method is PikachuResource.UnableToSpecifyDuringPeakPeriods && !clearAll)
        {
            if (IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery)
            {
                HomeDeliveryTimeList.Clear();

                IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;
            }
            else
            {
                IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = true;

                HomeDeliveryTimeList.Clear();

                HomeDeliveryTimeList.Add(PikachuResource.UnableToSpecifyDuringPeakPeriods);
            }
        }

        if (clearAll)
        {
            IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;
        }

        bool value = (bool)(e?.Value ?? false);

        if (!IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery || clearAll)
        {
            if (value) HomeDeliveryTimeList.Add(method);

            else HomeDeliveryTimeList.Remove(method);
        }

        EditGroupBuyDto.HomeDeliveryDeliveryTime = JsonConvert.SerializeObject(HomeDeliveryTimeList.Distinct());
    }
    void DeliveredByStoreTimeCheckedChange(string method, ChangeEventArgs e, bool clearAll = false)
    {
        if (method is PikachuResource.UnableToSpecifyDuringPeakPeriods && !clearAll)
        {
            if (IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore) IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = false;

            else IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = true;
        }

        if (clearAll)
        {
            IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = false;
        }

        bool value = (bool)(e?.Value ?? false);

        if (value) DeliveredByStoreTimeList.Add(method);

        else DeliveredByStoreTimeList.Remove(method);

        EditGroupBuyDto.DeliveredByStoreDeliveryTime = JsonConvert.SerializeObject(DeliveredByStoreTimeList.Distinct());
    }
    void OnShippingMethodCheckedChange(string method, ChangeEventArgs e)
    {
        bool value = (bool)(e?.Value ?? false);

        if (value)
        {
            // If the selected method is SevenToEleven or FamilyMart, uncheck the corresponding C2C method
            if (method == "SevenToEleven1" && EditGroupBuyDto.ShippingMethodList.Contains("SevenToElevenC2C"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("SevenToElevenC2C");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenC2C");
            }
            else if (method == "FamilyMart1" && EditGroupBuyDto.ShippingMethodList.Contains("FamilyMartC2C"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("FamilyMartC2C");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "FamilyMartC2C");
            }
            else if (method is "SevenToElevenFrozen" && EditGroupBuyDto.ShippingMethodList.Contains("SevenToElevenC2C"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("SevenToElevenC2C");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenC2C");
            }
            else if (method is "SevenToElevenC2C" && EditGroupBuyDto.ShippingMethodList.Contains("SevenToElevenFrozen"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("SevenToElevenFrozen");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenFrozen");
            }
            else if (method == "SevenToElevenC2C" && EditGroupBuyDto.ShippingMethodList.Contains("SevenToEleven1"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("SevenToEleven1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToEleven1");
            }
            else if (method == "FamilyMartC2C" && EditGroupBuyDto.ShippingMethodList.Contains("FamilyMart1"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("FamilyMart1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "FamilyMart1");
            }
            else if (method == "BlackCat1" && EditGroupBuyDto.ShippingMethodList.Contains("BlackCatFreeze"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("BlackCatFreeze");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFreeze");
            }
            else if (method == "BlackCat1" && EditGroupBuyDto.ShippingMethodList.Contains("BlackCatFrozen"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("BlackCatFrozen");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFrozen");
            }
            else if (method == "BlackCatFreeze" && EditGroupBuyDto.ShippingMethodList.Contains("BlackCat1"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("BlackCat1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCat1");
            }
            else if (method == "BlackCatFreeze" && EditGroupBuyDto.ShippingMethodList.Contains("BlackCatFrozen"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("BlackCatFrozen");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFrozen");
            }
            else if (method == "BlackCatFrozen" && EditGroupBuyDto.ShippingMethodList.Contains("BlackCat1"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("BlackCat1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCat1");
            }
            else if (method == "BlackCatFrozen" && EditGroupBuyDto.ShippingMethodList.Contains("BlackCatFreeze"))
            {
                EditGroupBuyDto.ShippingMethodList.Remove("BlackCatFreeze");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFreeze");
            }
        }

        // Update the selected method in the CreateGroupBuyDto.ShippingMethodList
        if (value)
        {
            if (method == "DeliveredByStore")
            {
                foreach (var item in EditGroupBuyDto.ShippingMethodList)
                {
                    JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);

                }
                EditGroupBuyDto.ShippingMethodList = new List<string>();
            }
            else
            {
                EditGroupBuyDto.ShippingMethodList.Remove("DeliveredByStore");

            }

            EditGroupBuyDto.ShippingMethodList.Add(method);
        }
        else
        {
            EditGroupBuyDto.ShippingMethodList.Remove(method);
        }

        var blackCat = new[]
        {
            DeliveryMethod.TCatDeliveryNormal.ToString(),
            DeliveryMethod.TCatDeliverySevenElevenNormal.ToString(),
            DeliveryMethod.TCatDeliveryFreeze.ToString(),
            DeliveryMethod.TCatDeliverySevenElevenFreeze.ToString(),
            DeliveryMethod.TCatDeliveryFrozen.ToString(),
            DeliveryMethod.TCatDeliverySevenElevenFrozen.ToString(),
            DeliveryMethod.BlackCat1.ToString(),
            DeliveryMethod.BlackCatFreeze.ToString(),
            DeliveryMethod.BlackCatFrozen.ToString(),
        };

        if (!EditGroupBuyDto.ShippingMethodList.Any(x => blackCat.Contains(x)))
        {
            DeliveryTimeConts.BlackCat.ForEach(item =>
            {
                BlackCatDeliveryTimeCheckedChange(item, new ChangeEventArgs { Value = false });
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);
            });
        }

        if (!EditGroupBuyDto.ShippingMethodList.Contains(DeliveryMethod.HomeDelivery.ToString()))
        {
            DeliveryTimeConts.HomeDelivery.ForEach(item =>
            {
                HomeDeliveryTimeCheckedChange(item, new ChangeEventArgs { Value = false }, true);
                if (!(EditGroupBuyDto.ShippingMethodList.Any(x => blackCat.Contains(x)) && item == "Inapplicable"))
                    JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);
            });
        }

        if (!EditGroupBuyDto.ShippingMethodList.Contains(DeliveryMethod.DeliveredByStore.ToString()))
        {
            DeliveryTimeConts.DeliveredByStore.ForEach(item =>
            {
                DeliveredByStoreTimeCheckedChange(item, new ChangeEventArgs { Value = false }, true);
                if (!(EditGroupBuyDto.ShippingMethodList.Any(x => blackCat.Contains(x)) && item == "Inapplicable"))
                    JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);
            });
        }

        // Serialize the updated list and assign it to ExcludeShippingMethod
        EditGroupBuyDto.ExcludeShippingMethod = JsonConvert.SerializeObject(EditGroupBuyDto.ShippingMethodList);
    }
    async Task DeleteLogoAsync(string blobImageName)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                string filename = System.IO.Path.GetFileName(blobImageName);
                confirmed = await _imageContainerManager.DeleteAsync(filename);
                if (confirmed)
                {
                    EditGroupBuyDto.LogoURL = null;
                    StateHasChanged();
                }
                else
                {
                    throw new BusinessException(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
        }
    }
    async Task DeleteBannerAsync(string blobImageName)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                string filename = System.IO.Path.GetFileName(blobImageName);
                confirmed = await _imageContainerManager.DeleteAsync(filename);
                if (confirmed)
                {
                    EditGroupBuyDto.BannerURL = null;
                    StateHasChanged();
                }
                else
                {
                    throw new BusinessException(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
        }
    }
    async Task DeleteImageAsync(string blobImageName, List<CreateImageDto> carouselImages, ImageType imageType)
    {
        try
        {
            bool confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                await Loading.Show();
                await _imageContainerManager.DeleteAsync(blobImageName);

                if (imageType is ImageType.GroupBuyCarouselImage)
                {
                    if (ExistingImages.Any(x => x.BlobImageName == blobImageName))
                    {
                        var image = ExistingImages.Where(x => x.BlobImageName == blobImageName).FirstOrDefault();
                        await _imageAppService.DeleteAsync(image.Id);
                    }

                    foreach (List<CreateImageDto> carouselModule in CarouselModules)
                    {
                        int? moduleNumber = carouselModule
                                                .Where(r => r.BlobImageName == blobImageName)
                                                .Select(s => s.ModuleNumber)
                                                .FirstOrDefault();

                        carouselModule.RemoveAll(r => r.BlobImageName == blobImageName);

                        if (!carouselModule.Any() && moduleNumber.HasValue)
                            carouselModule.Add(new CreateImageDto
                            {
                                Name = string.Empty,
                                ImageUrl = string.Empty,
                                ImageType = ImageType.GroupBuyCarouselImage,
                                BlobImageName = string.Empty,
                                CarouselStyle = null,
                                ModuleNumber = moduleNumber.Value,
                                SortNo = 0
                            });
                    }

                    carouselImages = carouselImages.Where(x => x.BlobImageName != blobImageName).ToList();

                    ExistingImages = ExistingImages.Where(x => x.BlobImageName != blobImageName).ToList();
                }

                else if (imageType is ImageType.GroupBuyBannerImage)
                {
                    if (ExistingBannerImages.Any(a => a.BlobImageName == blobImageName))
                    {
                        ImageDto? image = ExistingBannerImages.Where(w => w.BlobImageName == blobImageName).FirstOrDefault();

                        await _imageAppService.DeleteAsync(image.Id);
                    }

                    foreach (List<CreateImageDto> bannerModule in BannerModules)
                    {
                        int? moduleNumber = bannerModule
                                               .Where(r => r.BlobImageName == blobImageName)
                                               .Select(s => s.ModuleNumber)
                                               .FirstOrDefault();

                        bannerModule.RemoveAll(r => r.BlobImageName == blobImageName);

                        if (!bannerModule.Any() && moduleNumber.HasValue)
                            bannerModule.Add(new CreateImageDto
                            {
                                Name = string.Empty,
                                ImageUrl = string.Empty,
                                ImageType = ImageType.GroupBuyBannerImage,
                                BlobImageName = string.Empty,
                                CarouselStyle = null,
                                ModuleNumber = moduleNumber.Value,
                                SortNo = 0
                            });
                    }

                    carouselImages = [.. carouselImages.Where(w => w.BlobImageName != blobImageName)];

                    ExistingBannerImages = [.. ExistingBannerImages.Where(w => w.BlobImageName != blobImageName)];
                }


                else if (imageType is ImageType.GroupBuyProductRankingCarousel)
                {
                    foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                    {
                        var image = module.Images.Where(x => x.BlobImageName == blobImageName).FirstOrDefault();
                        await _imageAppService.DeleteAsync(image.Id);


                        module.Images.RemoveAll(r => r.BlobImageName == blobImageName);


                    }
                }


                StateHasChanged();
                await Loading.Hide();
            }
        }
        catch (Exception ex)
        {
            await Loading.Hide();
            Console.WriteLine(ex.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
        }
    }

    private static void OnProductGroupValueChange(ChangeEventArgs e, CollapseItem collapseItem)
    {
        int takeCount = int.Parse(e?.Value.ToString());
        if (collapseItem.Selected.Count > takeCount)
        {
            collapseItem.Selected = collapseItem.Selected.Take(takeCount).ToList();
        }
        else
        {
            collapseItem.Selected.Add(new ItemWithItemTypeDto());
        }
    }

    //private void HandleItemTagInputKeyUp(List<string> e)
    //{

    //    foreach (var item in e)
    //    {
    //        ItemTags.Add(TagInputValue);
    //    }


    //}

    //private void HandleItemTagDelete(string item)
    //{
    //    ItemTags.Remove(item);
    //}

    private void HandlePaymentTagInputKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            var matchedPaymentMethod = ValidPaymentMethods.FirstOrDefault(pm => string.Equals(pm, PaymentTagInputValue, StringComparison.OrdinalIgnoreCase));
            if (matchedPaymentMethod != null)
            {
                PaymentMethodTags.Add(matchedPaymentMethod);
            }
            else
            {
                PaymentMethodError = $"{PaymentTagInputValue} is not a valid Payment Method";
            }
            PaymentTagInputValue = string.Empty;
        }
    }

    private void HandlePaymentTagDelete(string item)
    {
        PaymentMethodTags.Remove(item);
    }

    protected virtual async Task UpdateEntityAsync()
    {
        try
        {
            await Loading.Show();
            if (EditGroupBuyDto.GroupBuyName.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyNameCannotBeNull]);
                await Loading.Hide();
                return;
            }
            if (CollapseItem.Any(a => a.IsWarnedForInCompatible))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.InCompatibleModule]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.ShortCode.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyShortCodeCannotBeNull]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.LogoURL.IsNullOrEmpty())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.LogoIsRequired]);
                await Loading.Hide();
                return;
            }
            var check = false;
            string shortCode = EditGroupBuyDto.ShortCode;
            if (!string.IsNullOrEmpty(shortCode))
            {
                string pattern = @"^[A-Za-z0-9]{4,12}$";
                bool isPatternValid = Regex.IsMatch(shortCode, pattern);

                if (!isPatternValid)
                {
                    await _uiMessageService.Warn(L["ShortCodePatternDoesnotMatch"]);
                    await Loading.Hide();
                    return;
                }
                check = await _groupBuyAppService.CheckShortCodeForEdit(EditGroupBuyDto.ShortCode, Id);

                if (check)
                {
                    await _uiMessageService.Warn(L["Short Code Alredy Exist"]);
                    await Loading.Hide();
                    return;
                }
            }
            else EditGroupBuyDto.ShortCode = "";

            //if (ItemTags.Any())
            //{
            //    EditGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
            //}
            //if (PaymentMethodTags.Any())
            //{
            //    EditGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
            //}

            List<string> paymentMethods = new List<string>();

            if (CreditCard) paymentMethods.Add("Credit Card");
            if (BankTransfer) paymentMethods.Add("Bank Transfer");
            if (IsCashOnDelivery) paymentMethods.Add("Cash On Delivery");
            if (IsLinePay) paymentMethods.Add("LinePay");
            if (paymentMethods.Count > 0) EditGroupBuyDto.PaymentMethod = string.Join(" , ", paymentMethods);
            else EditGroupBuyDto.PaymentMethod = string.Empty;

            if (EditGroupBuyDto.ProductType is null)
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ProductTypeIsRequired]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.PaymentMethod.IsNullOrEmpty())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOnePaymentMethodIsRequired]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.ExcludeShippingMethod.IsNullOrEmpty() || EditGroupBuyDto.ExcludeShippingMethod == "[]")
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneShippingMethodIsRequired]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.IsEnterprise && (EditGroupBuyDto.ExcludeShippingMethod is not "[\"SelfPickup\"]"))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.EnterprisePurchaseCanOnlyUseSelfPickup]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.FreeShipping && EditGroupBuyDto.FreeShippingThreshold == null)
            {
                await _uiMessageService.Warn(L["PleaseEnterThresholdAmount"]);
                await Loading.Hide();
                return;
            }
            if ((!EditGroupBuyDto.ExcludeShippingMethod.IsNullOrEmpty()) && (EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCat1")
                || EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFreeze") || EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFrozen")
                || EditGroupBuyDto.ExcludeShippingMethod.Contains("SelfPickup") || EditGroupBuyDto.ExcludeShippingMethod.Contains("HomeDelivery")
                || EditGroupBuyDto.ExcludeShippingMethod.Contains("DeliveredByStore")))
            {
                if (EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCat1") && (EditGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty() || EditGroupBuyDto.BlackCatDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForBlackCat]);
                    await Loading.Hide();
                    return;
                }
                if (EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFreeze") && (EditGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty() || EditGroupBuyDto.BlackCatDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForBlackCatFreeze]);
                    await Loading.Hide();
                    return;
                }
                if (EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFrozen") && (EditGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty() || EditGroupBuyDto.BlackCatDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForBlackCatFrozen]);
                    await Loading.Hide();
                    return;
                }
                else if (EditGroupBuyDto.ExcludeShippingMethod.Contains("SelfPickup") && (EditGroupBuyDto.SelfPickupDeliveryTime.IsNullOrEmpty() || EditGroupBuyDto.SelfPickupDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForSelfPickup]);
                    await Loading.Hide();
                    return;
                }
                else if (EditGroupBuyDto.ExcludeShippingMethod.Contains("HomeDelivery") && (EditGroupBuyDto.HomeDeliveryDeliveryTime.IsNullOrEmpty() || EditGroupBuyDto.HomeDeliveryDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForHomeDelivery]);
                    await Loading.Hide();
                    return;
                }
                else if (EditGroupBuyDto.ExcludeShippingMethod.Contains("DeliveredByStore") && (EditGroupBuyDto.DeliveredByStoreDeliveryTime.IsNullOrEmpty() || EditGroupBuyDto.DeliveredByStoreDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForDeliverdByStore]);
                    await Loading.Hide();
                    return;
                }
            }
            if (EditGroupBuyDto.ColorSchemeType == null || EditGroupBuyDto.ColorSchemeType == 0)
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ColorSchemeRequired]);
                await Loading.Hide();
                return;

            }
            if (EditGroupBuyDto.ProductDetailsDisplayMethod == null || EditGroupBuyDto.ProductDetailsDisplayMethod==0)
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ProductDetailsDisplayMethodRequired]);
                await Loading.Hide();
                return;

            }
            if (EditGroupBuyDto.GroupBuyConditionDescription.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyConditionRequired]);
                await Loading.Hide();
                return;

            }
            EditGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
            if (EditGroupBuyDto.ExchangePolicyDescription.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ExchangePolicyRequired]);
                await Loading.Hide();
                return;

            }
            if (GroupPurchaseOverviewModules is { Count: > 0 })
            {
                foreach (GroupPurchaseOverviewDto groupPurchaseOverview in GroupPurchaseOverviewModules)
                {
                    if (groupPurchaseOverview.Title.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("Title Cannot be empty in Group Purchase Overview Module");

                        await Loading.Hide();

                        return;
                    }

                    if (groupPurchaseOverview.Image.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("Please Add Image in Group Purchase Overview Module");

                        await Loading.Hide();

                        return;
                    }

                    if (groupPurchaseOverview.IsButtonEnable)
                    {
                        if (groupPurchaseOverview.ButtonText.IsNullOrEmpty())
                        {
                            await _uiMessageService.Error("If you have enabled Button, then Button Text is required.");

                            await Loading.Hide();

                            return;
                        }

                        if (groupPurchaseOverview.ButtonLink.IsNullOrEmpty())
                        {
                            await _uiMessageService.Error("If you have enabled Button, then Button Link is required.");

                            await Loading.Hide();

                            return;
                        }
                    }
                }
            }

            if (GroupBuyOrderInstructionModules is { Count: > 0 })
            {
                foreach (GroupBuyOrderInstructionDto groupBuyOrderInstruction in GroupBuyOrderInstructionModules)
                {
                    if (groupBuyOrderInstruction.Title.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("Title Cannot be empty in Group Purchase Overview Module");

                        await Loading.Hide();

                        return;
                    }

                    if (groupBuyOrderInstruction.Image.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("Please Add Image in Group Purchase Overview Module");

                        await Loading.Hide();

                        return;
                    }
                }
            }

            if (ProductRankingCarouselModules is { Count: > 0 })
            {
                foreach (ProductRankingCarouselModule productRankingCarouselModule in ProductRankingCarouselModules)
                {
                    if (productRankingCarouselModule.Title.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("Title Cannot be empty in Group Purchase Overview Module");

                        await Loading.Hide();

                        return;
                    }

                    if (productRankingCarouselModule.SubTitle.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("SubTitle Cannot be empty in Group Purchase Overview Module");

                        await Loading.Hide();

                        return;
                    }
                }
            }

            EditGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
            //EditGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();

            //EditGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

            EditGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

            foreach (CollapseItem item in CollapseItem)
            {
                int j = 1;
                check = (item.IsModified && item.Id.HasValue)
                    || item.Selected.Any(x => x.Id != Guid.Empty || (item.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor && !x.Name.IsNullOrEmpty()));
                if (check)
                {
                    GroupBuyItemGroupCreateUpdateDto itemGroup = new()
                    {
                        Id = item.Id,
                        SortOrder = item.SortOrder,
                        GroupBuyModuleType = item.GroupBuyModuleType,
                        GroupBuyId = GroupBuy.Id,
                        AdditionalInfo = item.AdditionalInfo,
                        ProductGroupModuleTitle = item.ProductGroupModuleTitle,
                        ProductGroupModuleImageSize = item.ProductGroupModuleImageSize,
                        ModuleNumber = item.ModuleNumber
                    };

                    if (item.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule &&
                        ProductRankingCarouselModules is { Count: > 0 })
                    {
                        foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                        {
                            foreach (ItemWithItemTypeDto itemDetail in module.Selected)
                            {
                                if (itemDetail.Id == Guid.Empty) continue;

                                itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                                {
                                    SortOrder = j++,
                                    ItemId = itemDetail.ItemType is ItemType.Item && itemDetail.Id != Guid.Empty ? itemDetail.Id : null,
                                    SetItemId = itemDetail.ItemType == ItemType.SetItem && itemDetail.Id != Guid.Empty ? itemDetail.Id : null,
                                    ItemType = itemDetail.ItemType,
                                    DisplayText = itemGroup.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor ? itemDetail.Name : null,
                                    ModuleNumber = ProductRankingCarouselModules.IndexOf(module) + 1
                                });
                            }
                        }
                    }

                    else
                    {
                        foreach (var itemDetail in item.Selected)
                        {
                            if (itemDetail.Id != Guid.Empty || (item.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor && !itemDetail.Name.IsNullOrEmpty()))
                            {
                                itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                                {
                                    SortOrder = j++,
                                    ItemId = itemDetail.ItemType == ItemType.Item && itemDetail.Id != Guid.Empty ? itemDetail.Id : null,
                                    SetItemId = itemDetail.ItemType == ItemType.SetItem && itemDetail.Id != Guid.Empty ? itemDetail.Id : null,
                                    ItemType = itemDetail.ItemType,
                                    DisplayText = itemGroup.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor ? itemDetail.Name : null
                                });
                            }
                        }
                    }

                    EditGroupBuyDto.ItemGroups.Add(itemGroup);
                }

                else if (item.Id is null || item.Id == Guid.Empty)
                {
                    GroupBuyItemGroupCreateUpdateDto itemGroup = new()
                    {
                        SortOrder = item.SortOrder,
                        GroupBuyModuleType = item.GroupBuyModuleType,
                        AdditionalInfo = item.AdditionalInfo,
                        ModuleNumber = item.ModuleNumber
                    };

                    if (item.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule &&
                        ProductRankingCarouselModules is { Count: > 0 })
                    {
                        foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                        {
                            foreach (ItemWithItemTypeDto itemDetail in module.Selected)
                            {
                                if (itemDetail.Id == Guid.Empty) continue;

                                itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                                {
                                    SortOrder = j++,
                                    ItemId = itemDetail.ItemType is ItemType.Item && itemDetail.Id != Guid.Empty ? itemDetail.Id : null,
                                    SetItemId = itemDetail.ItemType == ItemType.SetItem && itemDetail.Id != Guid.Empty ? itemDetail.Id : null,
                                    ItemType = itemDetail.ItemType,
                                    DisplayText = itemGroup.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor ? itemDetail.Name : null,
                                    ModuleNumber = ProductRankingCarouselModules.IndexOf(module) + 1
                                });
                            }
                        }
                    }

                    EditGroupBuyDto.ItemGroups.Add(itemGroup);
                }
            }
            await Loading.Show();

            try
            {
                GroupBuyDto result = await _groupBuyAppService.UpdateAsync(Id, EditGroupBuyDto);
           

            if (EditGroupBuyDto.IsEnterprise) await _OrderAppService.UpdateOrdersIfIsEnterpricePurchaseAsync(Id);

            foreach (List<CreateImageDto> carouselImages in CarouselModules)
            {
                foreach (CreateImageDto carouselImage in carouselImages)
                {
                    if ((!ExistingImages.Any(a => a.BlobImageName == carouselImage.BlobImageName)) ||
                        (ExistingImages.Any(a => a.CarouselStyle != carouselImage.CarouselStyle)))
                    {
                        if (carouselImage.Id != Guid.Empty)
                        {
                            await _imageAppService.UpdateImageAsync(carouselImage);
                        }

                        else
                        {
                            carouselImage.TargetId = Id;

                            await _imageAppService.CreateAsync(carouselImage);
                        }
                    }
                }
            }

            foreach (List<CreateImageDto> bannerImages in BannerModules)
            {
                foreach (CreateImageDto bannerImage in bannerImages)
                {
                    if (!ExistingBannerImages.Any(a => a.BlobImageName == bannerImage.BlobImageName))
                    {
                        if (bannerImage.Id != Guid.Empty)
                        {
                            await _imageAppService.UpdateImageAsync(bannerImage);
                        }

                        else
                        {
                            bannerImage.TargetId = Id;

                            await _imageAppService.CreateAsync(bannerImage);
                        }
                    }
                }
            }

            if (GroupPurchaseOverviewModules is { Count: > 0 })
            {
                foreach (GroupPurchaseOverviewDto groupPurchaseOverview in GroupPurchaseOverviewModules)
                {
                    if (groupPurchaseOverview.Id == Guid.Empty)
                    {
                        groupPurchaseOverview.GroupBuyId = result.Id;

                        await _GroupPurchaseOverviewAppService.CreateGroupPurchaseOverviewAsync(groupPurchaseOverview);
                    }

                    else if (groupPurchaseOverview.Id != Guid.Empty)
                        await _GroupPurchaseOverviewAppService.UpdateGroupPurchaseOverviewAsync(groupPurchaseOverview);
                }
            }

            if (GroupBuyOrderInstructionModules is { Count: > 0 })
            {
                foreach (GroupBuyOrderInstructionDto groupBuyOrderInstruction in GroupBuyOrderInstructionModules)
                {
                    if (groupBuyOrderInstruction.Id == Guid.Empty)
                    {
                        groupBuyOrderInstruction.GroupBuyId = result.Id;

                        await _GroupBuyOrderInstructionAppService.CreateGroupBuyOrderInstructionAsync(groupBuyOrderInstruction);
                    }

                    else if (groupBuyOrderInstruction.Id != Guid.Empty)
                        await _GroupBuyOrderInstructionAppService.UpdateGroupPurchaseOverviewAsync(groupBuyOrderInstruction);
                }
            }

            if (ProductRankingCarouselModules is { Count: > 0 })
            {
                foreach (ProductRankingCarouselModule productRankingCarouselModule in ProductRankingCarouselModules)
                {
                    foreach (CreateImageDto image in productRankingCarouselModule.Images)
                    {
                        if (image.TargetId == Guid.Empty)
                        {
                            image.TargetId = result.Id;

                            await _imageAppService.CreateAsync(image);
                        }

                    }

                    if (productRankingCarouselModule.Id == Guid.Empty)
                    {
                        await _GroupBuyProductRankingAppService.CreateGroupBuyProductRankingAsync(new()
                        {
                            GroupBuyId = result.Id,
                            Title = productRankingCarouselModule.Title,
                            SubTitle = productRankingCarouselModule.SubTitle,
                            Content = productRankingCarouselModule.Content,
                            ModuleNumber = ProductRankingCarouselModules.IndexOf(productRankingCarouselModule) + 1
                        });
                    }

                    else
                    {
                        await _GroupBuyProductRankingAppService.UpdateGroupBuyProductRankingAsync(new()
                        {
                            Id = productRankingCarouselModule.Id,
                            GroupBuyId = result.Id,
                            Title = productRankingCarouselModule.Title,
                            SubTitle = productRankingCarouselModule.SubTitle,
                            Content = productRankingCarouselModule.Content,
                            ModuleNumber = ProductRankingCarouselModules.IndexOf(productRankingCarouselModule) + 1
                        });
                    }
                }
            }
            }
            catch (AbpDbConcurrencyException e)
            {
                await Loading.Hide();
                NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/Edit/" + id);
            }

            await Loading.Hide();
            NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
        }
        catch (Exception ex)
        {
            await Loading.Hide();
            await HandleErrorAsync(ex);
        }
    }

    public void OnProductTypeChange(ChangeEventArgs e)
    {
        EditGroupBuyDto.ProductType = Enum.TryParse<ProductType>(Convert.ToString(e.Value),
                                                                 out ProductType selectedValue) ? selectedValue : null;
    }

    private async Task OnSelectedValueChanged(Guid? id, CollapseItem collapseItem, ItemWithItemTypeDto? selectedItem = null)
    {
        try
        {
            var item = ItemsList.FirstOrDefault(x => x.Id == id);
            var index = collapseItem.Selected.IndexOf(selectedItem);
            if (item != null)
            {
                if (item.ItemType == ItemType.Item)
                {
                    selectedItem.Item = await _itemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.Item.Id;
                    selectedItem.Name = selectedItem.Item.ItemName;
                    selectedItem.ItemType = ItemType.Item;
                }
                if (item.ItemType == ItemType.SetItem)
                {
                    selectedItem.SetItem = await _setItemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.SetItem.Id;
                    selectedItem.Name = selectedItem.SetItem.SetItemName;
                    selectedItem.ItemType = ItemType.SetItem;
                }
                collapseItem.Selected[index] = selectedItem;
            }
            else
            {
                if (collapseItem.GroupBuyModuleType != GroupBuyModuleType.IndexAnchor || collapseItem.Selected[index].Id != Guid.Empty)
                {
                    collapseItem.Selected[index] = new();
                }
                //else if (!collapseItem.Selected[index].IsFirstLoad)
                //{
                //    collapseItem.Selected[index] = new();
                //}
                //else
                //{
                //    collapseItem.Selected[index].IsFirstLoad = false;
                //}
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    private async Task OnSelectedValueChanged(Guid? id, ProductRankingCarouselModule module, ItemWithItemTypeDto? selectedItem = null)
    {
        try
        {
            var item = ItemsList.FirstOrDefault(x => x.Id == id);
            var index = module.Selected.IndexOf(selectedItem);
            if (item != null)
            {
                if (item.ItemType == ItemType.Item)
                {
                    selectedItem.Item = await _itemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.Item.Id;
                    selectedItem.Name = selectedItem.Item.ItemName;
                    selectedItem.ItemType = ItemType.Item;
                }
                if (item.ItemType == ItemType.SetItem)
                {
                    selectedItem.SetItem = await _setItemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.SetItem.Id;
                    selectedItem.Name = selectedItem.SetItem.SetItemName;
                    selectedItem.ItemType = ItemType.SetItem;
                }
                module.Selected[index] = selectedItem;
            }
            else
            {
                if (module.Selected[index].Id != Guid.Empty)
                {
                    module.Selected[index] = new();
                }
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    private void CloseGroupBuyEdit()
    {
        NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
    }

    private async void RemoveCollapseItem(int index)
    {
        try
        {
            var item = CollapseItem.FirstOrDefault(f => f.Index == index);

            if (item?.Id is not null)
            {
                bool confirm = await _uiMessageService.Confirm(L["ThisDeleteActionCannotBeReverted"]);

                if (!confirm) return;

                await Loading.Show();

                Guid GroupBuyId = Guid.Parse(id);

                await _groupBuyAppService.DeleteGroupBuyItemAsync(item.Id.Value, GroupBuyId);

                if (item.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview)
                    await _GroupPurchaseOverviewAppService.DeleteByGroupIdAsync(GroupBuyId);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction)
                    await _GroupBuyOrderInstructionAppService.DeleteByGroupBuyIdAsync(GroupBuyId);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
                    await _imageAppService.DeleteByGroupBuyIdAndImageTypeAsync(GroupBuyId, ImageType.GroupBuyCarouselImage, item.ModuleNumber!.Value);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
                    await _imageAppService.DeleteByGroupBuyIdAndImageTypeAsync(GroupBuyId, ImageType.GroupBuyBannerImage, item.ModuleNumber!.Value);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
                    await _GroupBuyProductRankingAppService.DeleteByGroupBuyIdAsync(GroupBuyId);

                StateHasChanged();
            }

            int moduleNumber = item.ModuleNumber.HasValue ? (int)item.ModuleNumber! : 0;

            if (item.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
            {
                await _groupBuyAppService.GroupBuyItemModuleNoReindexingAsync(Id, GroupBuyModuleType.CarouselImages);

                CarouselFilePickers = [];

                CarouselModules = [];

                //CarouselFilePickers.RemoveAt(moduleNumber - 1);

                //CarouselModules.RemoveAll(r => r.Any(w => w.ModuleNumber == moduleNumber));
            }

            else if (item.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
            {
                await _groupBuyAppService.GroupBuyItemModuleNoReindexingAsync(Id, GroupBuyModuleType.BannerImages);

                BannerFilePickers = [];

                BannerModules = [];

                //BannerFilePickers.RemoveAt(moduleNumber - 1);

                //BannerModules.RemoveAll(r => r.Any(w => w.ModuleNumber == moduleNumber));
            }
            
            else if (item.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction)
            {
                await _groupBuyAppService.GroupBuyItemModuleNoReindexingAsync(Id, GroupBuyModuleType.OrderInstruction);
                GroupBuyOrderInstructionModules = [];
            }

            else if (item.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview)
            {
                GroupPurchaseOverviewModules = [];
                GroupPurchaseOverviewFilePickers = [];
            }

            CollapseItem = [];

            await LoadItemGroups(true);

            //CollapseItem.Remove(item);
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
            StateHasChanged();
        }
    }

    void StartDrag(CollapseItem item)
    {
        CurrentIndex = CollapseItem.IndexOf(item);
    }
    private void AddCarouselStyle()
    {
        StyleForCarouselImages.Add(new StyleForCarouselImages());
    }
    private async Task UpdateCarouselStyle(CreateImageDto image)
    {
        await _imageAppService.UpdateCarouselStyleAsync(image);
    }

    async void Drop(CollapseItem item)
    {
        if (item != null)
        {
            var index = CollapseItem.IndexOf(item);

            var current = CollapseItem[CurrentIndex];

            CollapseItem.RemoveAt(CurrentIndex);
            CollapseItem.Insert(index, current);

            CurrentIndex = index;

            for (int i = 0; i < CollapseItem.Count; i++)
            {
                CollapseItem[i].Index = i + 1;
                CollapseItem[i].SortOrder = i + 1;
            }
            await UpdateSortOrderAsync();
            StateHasChanged();
        }
    }

    async Task UpdateSortOrderAsync()
    {
        await Loading.Show();
        try
        {
            var itemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();
            CollapseItem.ForEach(item =>
            {
                itemGroups.Add(new GroupBuyItemGroupCreateUpdateDto
                {
                    Id = item.Id,
                    SortOrder = item.SortOrder
                });
            });
            await _groupBuyAppService.UpdateSortOrderAsync(Id, itemGroups);
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }

    async Task CollapseToggled()
    {
        await JSRuntime.InvokeVoidAsync("updateDropText");
    }
    #endregion
}
