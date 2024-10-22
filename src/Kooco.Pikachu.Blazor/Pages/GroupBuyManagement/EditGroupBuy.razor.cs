using Blazored.TextEditor;
using Blazorise;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
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
using Volo.Abp.ObjectMapping;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.Tenants;
using Volo.Abp.MultiTenancy;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
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

                if (payments.Length > 1)
                {
                    CreditCard = payments.Contains("Credit Card");
                    BankTransfer = payments.Contains("Bank Transfer");
                    IsCashOnDelivery = payments.Contains("Cash On Delivery");
                }

                else if (GroupBuy.PaymentMethod is "Credit Card") CreditCard = true;

                else if (GroupBuy.PaymentMethod is "Bank Transfer") BankTransfer = true;

                else if (GroupBuy.PaymentMethod is "Cash On Delivery") IsCashOnDelivery = true;
            }

            EditGroupBuyDto.EntryURL = $"{(await MyTenantAppService.FindTenantDomainAsync(CurrentTenant.Id))?.TrimEnd('/')}/{Id}";

            SelectedTemplate = GroupBuy.TemplateType;

            IsColorPickerOpen = EditGroupBuyDto.ColorSchemeType is not null;

            LoadItemGroups();

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

    private void OnColorSchemeChange(ChangeEventArgs e)
    {
        string? selectedTheme = e.Value.ToString();

        EditGroupBuyDto.ColorSchemeType = !selectedTheme.IsNullOrEmpty() ? Enum.Parse<ColorScheme>(selectedTheme) : null;

        IsColorPickerOpen = true;

        switch (EditGroupBuyDto.ColorSchemeType)
        {
            case ColorScheme.ForestDawn:
                EditGroupBuyDto.PrimaryColor = "#23856D";
                EditGroupBuyDto.SecondaryColor = "#FFD057";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#C9D6BD";
                EditGroupBuyDto.AlertColor = "#FF902A";
                break;

            case ColorScheme.TropicalSunset:
                EditGroupBuyDto.PrimaryColor = "#FF902A";
                EditGroupBuyDto.SecondaryColor = "#BDDA8D";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#E5D19A";
                EditGroupBuyDto.AlertColor = "#FF902A";
                break;

            case ColorScheme.DeepSeaNight:
                EditGroupBuyDto.PrimaryColor = "#133854";
                EditGroupBuyDto.SecondaryColor = "#CAE28D";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#DCD6D0";
                EditGroupBuyDto.AlertColor = "#A1E82D";
                break;

            case ColorScheme.SweetApricotCream:
                EditGroupBuyDto.PrimaryColor = "#FFA085";
                EditGroupBuyDto.SecondaryColor = "#BDDA8D";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#DCBFC3";
                EditGroupBuyDto.AlertColor = "#FFC123";
                break;

            case ColorScheme.DesertDawn:
                EditGroupBuyDto.PrimaryColor = "#C08C5D";
                EditGroupBuyDto.SecondaryColor = "#E7AD99";
                EditGroupBuyDto.BackgroundColor = "#FFFFFF";
                EditGroupBuyDto.SecondaryBackgroundColor = "#EBC7AD";
                EditGroupBuyDto.AlertColor = "#FF902A";
                break;

            default:
                EditGroupBuyDto.PrimaryColor = string.Empty;
                EditGroupBuyDto.SecondaryColor = string.Empty;
                EditGroupBuyDto.BackgroundColor = string.Empty;
                EditGroupBuyDto.SecondaryBackgroundColor = string.Empty;
                EditGroupBuyDto.AlertColor = string.Empty;
                IsColorPickerOpen = false;
                break;
        }
    }

    public bool IsShippingMethodEnabled(string method)
    {
        if (LogisticsProviders is { Count: 0 }) return false;

        DeliveryMethod deliveryMethod = Enum.Parse<DeliveryMethod>(method);

        if (deliveryMethod is DeliveryMethod.HomeDelivery)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.HomeDelivery).FirstOrDefault()?.IsEnabled ?? false;

        else if (deliveryMethod is DeliveryMethod.PostOffice ||
                 deliveryMethod is DeliveryMethod.FamilyMart1 ||
                 deliveryMethod is DeliveryMethod.SevenToEleven1 ||
                 deliveryMethod is DeliveryMethod.SevenToElevenFrozen ||
                 deliveryMethod is DeliveryMethod.BlackCat1 ||
                 deliveryMethod is DeliveryMethod.BlackCatFreeze ||
                 deliveryMethod is DeliveryMethod.BlackCatFrozen)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.GreenWorldLogistics).FirstOrDefault()?.IsEnabled ?? false;

        else if (deliveryMethod is DeliveryMethod.FamilyMartC2C ||
                 deliveryMethod is DeliveryMethod.SevenToElevenC2C)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.GreenWorldLogisticsC2C).FirstOrDefault()?.IsEnabled ?? false;

        else if (deliveryMethod is DeliveryMethod.TCatDeliveryNormal ||
                 deliveryMethod is DeliveryMethod.TCatDeliveryFreeze ||
                 deliveryMethod is DeliveryMethod.TCatDeliveryFrozen ||
                 deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal ||
                 deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                 deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat).FirstOrDefault()?.IsEnabled ?? false;

        else return false;
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
        await Task.Delay(100);

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
                    Item = groupBuyItemGroup.Item,
                    ItemType = groupBuyItemGroup.ItemType,
                    Name = groupBuyItemGroup.Item.ItemName,
                    SetItem = groupBuyItemGroup.SetItem
                });
            }

            while (module.Selected.Count < 3)
            {
                module.Selected.Add(new());
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (isFirstRender)
        {
            try
            {
                await Loading.Show();
                await OnInitializedAsync();
                //await LoadHtmlContent();
                ExistingImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyCarouselImage);

                ExistingBannerImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyBannerImage);

                CarouselImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingImages);

                CarouselModules = CarouselImages.GroupBy(g => g.ModuleNumber).Select(s => s.ToList()).ToList();

                BannerImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingBannerImages);

                BannerModules = [.. BannerImages.GroupBy(g => g.ModuleNumber).Select(s => s.ToList())];

                GroupPurchaseOverviewModules = await _GroupPurchaseOverviewAppService.GetListByGroupBuyIdAsync(Id);

                GroupBuyOrderInstructionModules = await _GroupBuyOrderInstructionAppService.GetListByGroupBuyIdAsync(Id);

                await GetProductRankingCarouselsAsync();

                foreach (List<CreateImageDto> carouselImages in CarouselModules)
                {
                    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.CarouselImages))
                        CollapseItem.Add(new()
                        {
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                            GroupBuyModuleType = GroupBuyModuleType.CarouselImages
                        });

                    CarouselFilePickers.Add(new());
                }

                foreach (List<CreateImageDto> carouselImages in BannerModules)
                {
                    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.BannerImages))
                        CollapseItem.Add(new()
                        {
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                            GroupBuyModuleType = GroupBuyModuleType.BannerImages
                        });

                    BannerFilePickers.Add(new());
                }

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

                foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                {
                    if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule))
                        CollapseItem.Add(new()
                        {
                            Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                            SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                            GroupBuyModuleType = GroupBuyModuleType.OrderInstruction
                        });

                    ProductRankingCarouselPickers.Add(new());
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
        await GroupBuyHtml.LoadHTMLContent(EditGroupBuyDto.GroupBuyConditionDescription);
        await CustomerInformationHtml.LoadHTMLContent(EditGroupBuyDto.CustomerInformationDescription);
        await ExchangePolicyHtml.LoadHTMLContent(EditGroupBuyDto.ExchangePolicyDescription);
        await NotifyEmailHtml.LoadHTMLContent(EditGroupBuyDto.NotifyMessage);
    }

    private void LoadItemGroups()
    {
        if (CollapseItem.Count == 0)
        {
            var itemGroups = GroupBuy.ItemGroups;
            if (itemGroups.Any())
            {
                var i = 0;
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
                            Index = i++,
                            SortOrder = itemGroup.SortOrder,
                            GroupBuyModuleType = itemGroup.GroupBuyModuleType,
                            AdditionalInfo = itemGroup.AdditionalInfo
                        };
                    }

                    else
                    {
                        collapseItem = new CollapseItem
                        {
                            Id = itemGroup.Id,
                            Index = i++,
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
            if (CarouselModules.Count is 0)
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            CarouselFilePickers.Add(new());

            CarouselModules.Add([]);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.BannerImages)
        {
            if (BannerModules.Count is 0)
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            BannerFilePickers.Add(new());

            BannerModules.Add([]);
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

            GroupBuyOrderInstructionModules.Add(new());
        }

        else if (groupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
        {
            if (ProductRankingCarouselModules is { Count: 0 })
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

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
        //if (e.Files.Length > MaxAllowedFilesPerUpload)
        //{
        //    await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
        //    await CarouselPickerCustom.Clear();
        //    return;
        //}
        //if (CarouselImages.Count > TotalMaxAllowedFiles)
        //{
        //    await _uiMessageService.Error(L[PikachuDomainErrorCodes.AlreadyUploadMaxAllowedFiles]);
        //    await CarouselPickerCustom.Clear();
        //    return;
        //}
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

                    carouselImages.Add(new CreateImageDto
                    {
                        Name = file.Name,
                        BlobImageName = newFileName,
                        ImageUrl = url,
                        ImageType = imageType,
                        SortNo = sortNo + 1,
                        ModuleNumber = carouselModuleNumber
                    });

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
            if (IsUnableToSpecifyDuringPeakPeriodsForSelfPickups) IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = false;

            else IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = true;
        }

        bool value = (bool)(e?.Value ?? false);

        if (value) SelfPickupTimeList.Add(method);

        else SelfPickupTimeList.Remove(method);

        EditGroupBuyDto.SelfPickupDeliveryTime = JsonConvert.SerializeObject(SelfPickupTimeList);
    }
    void BlackCatDeliveryTimeCheckedChange(string method, ChangeEventArgs e)
    {
        var value = (bool)(e?.Value ?? false);
        if (value)
        {
            BlackCateDeliveryTimeList.Add(method);
        }
        else
        {
            BlackCateDeliveryTimeList.Remove(method);
        }

        EditGroupBuyDto.BlackCatDeliveryTime = JsonConvert.SerializeObject(BlackCateDeliveryTimeList);
    }
    void HomeDeliveryTimeCheckedChange(string method, ChangeEventArgs e)
    {
        if (method is PikachuResource.UnableToSpecifyDuringPeakPeriods)
        {
            if (IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery) IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;

            else IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = true;
        }

        bool value = (bool)(e?.Value ?? false);

        if (value) HomeDeliveryTimeList.Add(method);

        else HomeDeliveryTimeList.Remove(method);

        EditGroupBuyDto.HomeDeliveryDeliveryTime = JsonConvert.SerializeObject(HomeDeliveryTimeList);
    }
    void DeliveredByStoreTimeCheckedChange(string method, ChangeEventArgs e)
    {
        if (method is PikachuResource.UnableToSpecifyDuringPeakPeriods)
        {
            if (IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore) IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = false;

            else IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = true;
        }

        bool value = (bool)(e?.Value ?? false);

        if (value) DeliveredByStoreTimeList.Add(method);

        else DeliveredByStoreTimeList.Remove(method);

        EditGroupBuyDto.DeliveredByStoreDeliveryTime = JsonConvert.SerializeObject(DeliveredByStoreTimeList);
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
                        carouselModule.RemoveAll(r => r.BlobImageName == blobImageName);
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
                        bannerModule.RemoveAll(r => r.BlobImageName == blobImageName);
                    }

                    carouselImages = [.. carouselImages.Where(w => w.BlobImageName != blobImageName)];

                    ExistingBannerImages = [.. ExistingBannerImages.Where(w => w.BlobImageName != blobImageName)];
                }

                else if (imageType is ImageType.GroupBuyProductRankingCarousel)
                {

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
            //if (EditGroupBuyDto.GroupBuyName.IsNullOrWhiteSpace())
            //{
            //    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyNameCannotBeNull]);
            //    return;
            //}
            await Loading.Show();

            if (CollapseItem.Any(a => a.IsWarnedForInCompatible))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.InCompatibleModule]);
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

            if (CreditCard && BankTransfer && IsCashOnDelivery) EditGroupBuyDto.PaymentMethod = "Credit Card , Bank Transfer , Cash On Delivery";

            else if (CreditCard) EditGroupBuyDto.PaymentMethod = "Credit Card";

            else if (BankTransfer) EditGroupBuyDto.PaymentMethod = "Bank Transfer";

            else if (IsCashOnDelivery) EditGroupBuyDto.PaymentMethod = "Cash On Delivery";

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
            if (EditGroupBuyDto.IsEnterprise && (!EditGroupBuyDto.ExcludeShippingMethod.Contains("DeliveredByStore")))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.DeliverdByStoreMethodIsRequired]);
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

            EditGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
            EditGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();
            EditGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
            EditGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

            EditGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

            foreach (CollapseItem item in CollapseItem)
            {
                int j = 1;
                check = (item.IsModified && item.Id.HasValue)
                    || item.Selected.Any(x => x.Id != Guid.Empty || (item.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor && !x.Name.IsNullOrEmpty()));
                if (check)
                {
                    var itemGroup = new GroupBuyItemGroupCreateUpdateDto
                    {
                        Id = item.Id,
                        SortOrder = item.SortOrder,
                        GroupBuyModuleType = item.GroupBuyModuleType,
                        GroupBuyId = GroupBuy.Id,
                        AdditionalInfo = item.AdditionalInfo,
                        ProductGroupModuleTitle = item.ProductGroupModuleTitle,
                        ProductGroupModuleImageSize = item.ProductGroupModuleImageSize
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
                        AdditionalInfo = item.AdditionalInfo
                    };

                    EditGroupBuyDto.ItemGroups.Add(itemGroup);
                }
            }
            await Loading.Show();

            GroupBuyDto result = await _groupBuyAppService.UpdateAsync(Id, EditGroupBuyDto);

            if (EditGroupBuyDto.IsEnterprise) await _OrderAppService.UpdateOrdersIfIsEnterpricePurchaseAsync(Id);

            foreach (List<CreateImageDto> carouselImages in CarouselModules)
            {
                foreach (CreateImageDto carouselImage in carouselImages)
                {
                    if (!ExistingImages.Any(a => a.BlobImageName == carouselImage.BlobImageName))
                    {
                        carouselImage.TargetId = Id;

                        await _imageAppService.CreateAsync(carouselImage);
                    }
                }
            }

            foreach (List<CreateImageDto> bannerImages in BannerModules)
            {
                foreach (CreateImageDto carouselImage in bannerImages)
                {
                    if (!ExistingBannerImages.Any(a => a.BlobImageName == carouselImage.BlobImageName))
                    {
                        carouselImage.TargetId = Id;

                        await _imageAppService.CreateAsync(carouselImage);
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
                        if (!productRankingCarouselModule.Images.Any(a => a.BlobImageName == image.BlobImageName))
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

            await Loading.Hide();
            NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
        }
        catch (BusinessException ex)
        {
            await Loading.Hide();
            Console.WriteLine(ex.ToString());
            await _uiMessageService.Error(L[ex.Code]);
        }
        catch (Exception ex)
        {
            await Loading.Hide();
            Console.WriteLine(ex.ToString());
            await _uiMessageService.Error(ex.Message.GetType()?.ToString());
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
            var item = CollapseItem.Where(i => i.Index == index).FirstOrDefault();
            if (item?.Id != null)
            {
                var confirm = await _uiMessageService.Confirm(L["ThisDeleteActionCannotBeReverted"]);
                if (!confirm)
                {
                    return;
                }
                await Loading.Show();
                Guid GroupBuyId = Guid.Parse(id);
                await _groupBuyAppService.DeleteGroupBuyItemAsync(item.Id.Value, GroupBuyId);

                if (item.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview)
                    await _GroupPurchaseOverviewAppService.DeleteByGroupIdAsync(GroupBuyId);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction)
                    await _GroupBuyOrderInstructionAppService.DeleteByGroupBuyIdAsync(GroupBuyId);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
                    await _imageAppService.DeleteByGroupBuyIdAndImageTypeAsync(GroupBuyId, ImageType.GroupBuyCarouselImage);

                else if (item.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
                    await _imageAppService.DeleteByGroupBuyIdAndImageTypeAsync(GroupBuyId, ImageType.GroupBuyBannerImage);

                StateHasChanged();
            }
            CollapseItem.Remove(item);
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
                CollapseItem[i].Index = i;
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
    #endregion
}
