using AngleSharp.Dom;
using AntDesign;
using Blazored.TextEditor;
using Blazorise;
using Blazorise.Cropper;
using Blazorise.Extensions;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuyItemsPriceses;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Requests;
using Kooco.Pikachu.Tenants.Responses;
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
using Volo.Abp.AspNetCore.Components.Messages;
using Modal = Blazorise.Modal;


namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class CreateGroupBuy
{
    #region Inject
    private bool IsRender = false;
    private Modal AddLinkModal { get; set; }
    private CreateImageDto SelectedImageDto = new();
    private const int maxtextCount = 60;
    private Modal CropperModal;
    private Modal BannerCropperModal;
    private Modal CarouselCropperModal;
    private Modal PurchaseOverviewCropperModal;
    private Modal OrderInstructionCropperModal;
    private Cropper LogoCropper;
    private Cropper BannerCropper;
    private Cropper CarouselCropper;
    private Cropper PurchaseOverviewCropper;
    private Cropper OrderInstructionCropper;
    private FilePicker LogoPickerCustom;
    private string imageToCrop;
    private IFileEntry selectedFile;
    private string logoBlobName;
    private string croppedImage;
    private bool cropButtonDisabled = true;
    private const int MaxAllowedFilesPerUpload = 5;
    private const int TotalMaxAllowedFiles = 5;
    private const int MaxAllowedFileSize = 1024 * 1024 * 10;
    public List<string> SelfPickupTimeList = new List<string>();
    public List<string> BlackCateDeliveryTimeList = new List<string>();
    public List<string> BlackCatFreezeDeliveryTimeList = new List<string>();
    public List<string> BlackCatFrozenDeliveryTimeList = new List<string>();
    public List<string> HomeDeliveryTimeList = new List<string>();
    public List<string> DeliverdByStoreTimeList = new List<string>();
    private GroupBuyCreateDto CreateGroupBuyDto = new();
    public List<CreateImageDto> CarouselImages { get; set; }
    public List<StyleForCarouselImages> StyleForCarouselImages { get; set; } = new List<StyleForCarouselImages>();
    private string TagInputValue { get; set; }
    private LoadingIndicator Loading { get; set; } = new();
    private IReadOnlyList<string> ItemTags { get; set; } = new List<string>(); //used for store item tags 
    private string? SelectedAutoCompleteText { get; set; }
    private List<ItemWithItemTypeDto> ItemsList { get; set; } = [];
    private List<ItemWithItemTypeDto> SetItemList { get; set; } = [];
    private List<string> PaymentMethodTags { get; set; } = [];
    private string PaymentTagInputValue { get; set; }
    private List<CollapseItem> CollapseItem = [];

    string bannerBlobName;
    protected Validations EditValidationsRef;
    private BlazoredTextEditor NotifyEmailHtml { get; set; }
    private BlazoredTextEditor GroupBuyHtml { get; set; }
    private BlazoredTextEditor CustomerInformationHtml { get; set; }
    private BlazoredTextEditor ExchangePolicyHtml { get; set; }
    bool CreditCard { get; set; }
    bool BankTransfer { get; set; }
    bool IsCashOnDelivery { get; set; }
    bool IsLinePay { get; set; }

    public string _ProductPicture = "Product Picture";

    private FilePicker BannerPickerCustom { get; set; }
    private FilePicker CarouselPickerCustom { get; set; }
    private List<FilePicker> CarouselFilePickers = [];
    private List<FilePicker> BannerFilePickers = [];
    private List<FilePicker> GroupPurchaseOverviewFilePickers = [];
    private List<FilePicker> GroupBuyOrderInstructionPickers = [];
    private List<FilePicker> ProductRankingCarouselPickers = [];
    private List<string> ShippingMethods { get; set; } = Enum.GetNames(typeof(DeliveryMethod)).ToList();
    private readonly IGroupBuyAppService _groupBuyAppService;
    private readonly IImageAppService _imageAppService;
    private readonly IItemAppService _itemAppService;
    private readonly ISetItemAppService _setItemAppService;
    private readonly IUiMessageService _uiMessageService;
    private readonly ImageContainerManager _imageContainerManager;
    private readonly List<string> ValidFileExtensions = [".jpg", ".png", ".svg", ".jpeg", ".webp"];
    public readonly List<string> ValidPaymentMethods = ["ALL", "Credit", "WebATM", "ATM", "CVS", "BARCODE", "Alipay", "Tenpay", "TopUpUsed", "GooglePay"];
    private string? PaymentMethodError { get; set; } = null;
    int CurrentIndex { get; set; }

    public bool IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = false;

    public bool IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;

    public bool IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = false;

    public GroupBuyTemplateType SelectedTemplate;

    public bool IsSelectedModule = true;

    private bool IsShowCarouselImageModule = false;

    public List<List<CreateImageDto>> CarouselModules = [];
    public List<List<CreateImageDto>> BannerModules = [];
    public List<GroupPurchaseOverviewDto> GroupPurchaseOverviewModules = [];
    public List<GroupBuyOrderInstructionDto> GroupBuyOrderInstructionModules = [];
    public List<ProductRankingCarouselModule> ProductRankingCarouselModules = [];

    private readonly IGroupPurchaseOverviewAppService _GroupPurchaseOverviewAppService;

    private readonly IGroupBuyOrderInstructionAppService _GroupBuyOrderInstructionAppService;

    private readonly IGroupBuyProductRankingAppService _GroupBuyProductRankingAppService;
    private readonly ITenantTripartiteAppService _electronicInvoiceSettingAppService;

    public List<LogisticsProviderSettingsDto> LogisticsProviders = [];
    public TenantTripartiteDto TenantTripartiteDto = new();
    public List<PaymentGatewayDto> PaymentGateways = [];
    private readonly ILogisticsProvidersAppService _LogisticsProvidersAppService;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService;
    private readonly IGroupBuyItemsPriceAppService _groupBuyItemsPriceAppService;

    private bool IsColorPickerOpen = false;
    public bool HasDifferentItemTemperatures = false;

    private List<IFileEntry> selectedFiles = new List<IFileEntry>(); // List of selected files
    private List<CreateImageDto> uploadedCarouselImages = new List<CreateImageDto>(); // List for storing image data
    private int CurrentFileIndex = 0;
    FilePicker CarouselPicker = new FilePicker();
    FilePicker BannerPicker = new FilePicker();
    ImageType ImageType;
    int CarouselModuleNumber;
    int BannerModuleNumber;
    private List<CreateImageDto> uploadBannerImages = new List<CreateImageDto>(); // List for storing image data
    public GroupPurchaseOverviewDto GroupPurchaseOverviewModule = new GroupPurchaseOverviewDto();
    public GroupBuyOrderInstructionDto GroupOrderInstructionModule = new GroupBuyOrderInstructionDto();
    FilePicker PurchaseOverviewPicker = new FilePicker();
    FilePicker OrderInstructionPicker = new FilePicker();
    #endregion

    #region Constructor
    public CreateGroupBuy(
        IGroupBuyAppService groupBuyAppService,
        IImageAppService imageAppService,
        IUiMessageService uiMessageService,
        ImageContainerManager imageContainerManager,
        IItemAppService itemAppService,
        ISetItemAppService setItemAppService,
        IGroupPurchaseOverviewAppService GroupPurchaseOverviewAppService,
        IGroupBuyOrderInstructionAppService GroupBuyOrderInstructionAppService,
        ILogisticsProvidersAppService LogisticsProvidersAppService,
        IGroupBuyProductRankingAppService GroupBuyProductRankingAppService,
        ITenantTripartiteAppService electronicInvoiceSettingAppService,
        IPaymentGatewayAppService paymentGatewayAppService,
        IGroupBuyItemsPriceAppService groupBuyItemsPriceAppService
    )
    {
        _groupBuyAppService = groupBuyAppService;
        _imageAppService = imageAppService;
        _uiMessageService = uiMessageService;
        _imageContainerManager = imageContainerManager;

        CarouselImages = new List<CreateImageDto>();
        _itemAppService = itemAppService;
        _setItemAppService = setItemAppService;

        _GroupPurchaseOverviewAppService = GroupPurchaseOverviewAppService;
        _GroupBuyOrderInstructionAppService = GroupBuyOrderInstructionAppService;
        _LogisticsProvidersAppService = LogisticsProvidersAppService;
        _GroupBuyProductRankingAppService = GroupBuyProductRankingAppService;
        _electronicInvoiceSettingAppService = electronicInvoiceSettingAppService;
        _paymentGatewayAppService = paymentGatewayAppService;
        _groupBuyItemsPriceAppService = groupBuyItemsPriceAppService;
    }
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

    #region Methods
    private void NavigateToGroupBuyList()
    {
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList");
    }
    protected override async Task OnInitializedAsync()
    {
        CreateGroupBuyDto.EntryURL = (await MyTenantAppService.FindTenantUrlAsync(CurrentTenant.Id)) + "groupBuy/";
        SetItemList = await _setItemAppService.GetItemsLookupAsync();
        ItemsList = await _itemAppService.GetItemsLookupAsync();
        ItemsList.AddRange(SetItemList);

        LogisticsProviders = await _LogisticsProvidersAppService.GetAllAsync();
        TenantTripartiteDto = await _electronicInvoiceSettingAppService.FindAsync();
        PaymentGateways = await _paymentGatewayAppService.GetAllAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("updateDropText");
                SelectTemplate(new ChangeEventArgs { Value = GroupBuyTemplateType.PikachuTwo });
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private void SelectTemplate(ChangeEventArgs e)
    {
        SelectedTemplate = Enum.Parse<GroupBuyTemplateType>(e.Value.ToString());

        CreateGroupBuyDto.TemplateType = SelectedTemplate;

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

    public bool CheckForShippingMethodData(string method)
    {
        if (LogisticsProviders is { Count: 0 }) return true;

        DeliveryMethod deliveryMethod = Enum.Parse<DeliveryMethod>(method);
        if (deliveryMethod is DeliveryMethod.PostOffice)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.PostOffice).FirstOrDefault();
            if (data == null || (data.Freight <= 0 && data.Weight <= 0))
            {
                return true;
            }
            else
            {
                return false;


            }

        }
        else if (deliveryMethod is DeliveryMethod.FamilyMart1)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.FamilyMart).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.SevenToEleven1)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.SevenToEleven).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.SevenToElevenFrozen)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.SevenToElevenFrozen).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.BlackCat1)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.BNormal).FirstOrDefault();
            if (data == null || (data.Freight < 0 || data.OuterIslandFreight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.BlackCatFreeze)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.BFreeze).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.BlackCatFrozen)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.BFrozen).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.SevenToElevenC2C)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.SevenToElevenC2C).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.FamilyMartC2C)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.FamilyMartC2C).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat711Normal).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat711Freeze).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat711Frozen).FirstOrDefault();
            if (data == null || (data.Freight < 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.TCatDeliveryNormal)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCatNormal).FirstOrDefault();
            if (data == null || (data.Freight < 0 || data.OuterIslandFreight < 0 || data.Size <= 0 || data.TCatPaymentMethod == null || data.TCatPaymentMethod <= 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.TCatDeliveryFreeze)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat711Freeze).FirstOrDefault();
            if (data == null || (data.Freight < 0 || data.OuterIslandFreight < 0 || data.Size <= 0 || data.TCatPaymentMethod == null || data.TCatPaymentMethod <= 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.TCatDeliveryFrozen)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat711Frozen).FirstOrDefault();
            if (data == null || (data.Freight < 0 || data.OuterIslandFreight < 0 || data.Size <= 0 || data.TCatPaymentMethod == null || data.TCatPaymentMethod <= 0))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        else if (deliveryMethod is DeliveryMethod.DeliveredByStore)
        {
            var data = LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.HomeDelivery).FirstOrDefault();
            if (data == null || data.MainIslands.IsNullOrEmpty() || data.Freight < 0 || (data.IsOuterIslands && data.OuterIslands.IsNullOrEmpty()))
            {
                return true;
            }
            else
            {
                return false;


            }
        }
        return true;
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

    public bool isPaymentMethodEnabled()
    {
        var ecpay = PaymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).FirstOrDefault();

        if (ecpay == null)
        {
            return true;
        }
        else if (!ecpay.IsCreditCardEnabled) return true;
        else if (ecpay.HashIV.IsNullOrEmpty() || ecpay.HashKey.IsNullOrEmpty() || ecpay.MerchantId.IsNullOrEmpty() || ecpay.TradeDescription.IsNullOrEmpty() || ecpay.CreditCheckCode.IsNullOrEmpty()) return true;
        else
        {
            return false;

        }
    }
    public bool isLinePayPaymentMethodEnabled()
    {
        var linePay = PaymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay).FirstOrDefault();

        if (linePay == null)
        {
            return true;
        }
        else if (!linePay.IsEnabled) return true;
        else if (linePay.ChannelId.IsNullOrEmpty() || linePay.ChannelSecretKey.IsNullOrEmpty()) return true;
        else
        {
            return false;

        }
    }

    public bool IsInstallmentPeriodEnabled(string period)
    {
        var ecpay = PaymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).FirstOrDefault();
        if (!CreditCard || ecpay == null) return true;

        return !ecpay.InstallmentPeriods.Contains(period);
    }

    void OnInstallmentPeriodChange(bool value, string period)
    {
        if (value)
        {
            CreateGroupBuyDto.InstallmentPeriods.Add(period);
        }
        else
        {
            CreateGroupBuyDto.InstallmentPeriods.Remove(period);
        }
    }

    void OnCreditCardCheckedChange(bool value)
    {
        CreditCard = value;
        if (!CreditCard)
        {
            CreateGroupBuyDto.InstallmentPeriods = [];
        }
    }

    public bool IsInvoiceEnable()
    {
        if (TenantTripartiteDto == null)
        {
            return true;
        }
        return !TenantTripartiteDto.IsEnable;


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
    private Task OnModalClosing(Blazorise.ModalClosingEventArgs e)
    {
        return Task.CompletedTask;
    }
    
 
    private async Task CloseCropModal()
    {
        await LogoPickerCustom.Clear();
        await CropperModal.Hide();

       
        await CarouselCropperModal.Hide();
    }
    private async Task ClosePurchaseOverviewCropModal()
    {
        await PurchaseOverviewCropperModal.Hide();
        imageToCrop = "";
        PurchaseOverviewCropper.Source = "";
        await PurchaseOverviewCropper.ResetSelection();
        await PurchaseOverviewPicker.Clear();
    }
    private async Task CloseBannerCropModal()
    {
        
        await BannerCropperModal.Hide();
        imageToCrop = "";
         BannerCropper.Source = "";
        await BannerCropper.ResetSelection();
        await BannerPicker.Clear();


    }
    private async Task CloseOrderInstructionCropModal()
    {
        await OrderInstructionCropperModal.Hide();
        imageToCrop = "";
        OrderInstructionCropper.Source = "";
        await OrderInstructionCropper.ResetSelection();
        await OrderInstructionPicker.Clear();

    }
    private string LocalizeFilePicker(string key, object[] args)
    {
        return L[key];
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
            GroupBuyModuleType.ProductRankingCarouselModule,
            GroupBuyModuleType.CustomTextModule,
            GroupBuyModuleType.PartnershipModule,
            GroupBuyModuleType.VideoUpload
        ];
    }

    public void OnProductTypeChange(ChangeEventArgs e)
    {
        CreateGroupBuyDto.ProductType = Enum.TryParse<ProductType>(
            Convert.ToString(e.Value),
            out ProductType selectedValue
        ) ? selectedValue : null;
    }

    private void OnColorSchemeChange(ChangeEventArgs e)
    {

        string? selectedTheme = e.Value.ToString() != "Choose Color Theme" ? e.Value.ToString() : null;

        CreateGroupBuyDto.ColorSchemeType = !selectedTheme.IsNullOrEmpty() ? Enum.Parse<ColorScheme>(selectedTheme) : null;

        IsColorPickerOpen = true;
        if (GroupBuyOrderInstructionModules.Count > 0)
        {
            string[] fileNames = { "Forest Dawn.png", "Tropical Sunset.png", "Deep Sea Night.png", "Sweet Apricot Cream.png", "Desert Dawn.png" };
            foreach (var item in GroupBuyOrderInstructionModules)
            {
                if (fileNames.Any(fileName => item.Image.Contains(fileName)))
                {
                    item.Image = "https://pikachublobs.blob.core.windows.net/images/" + L["Enum:ColorSchemeFile." + (int)CreateGroupBuyDto.ColorSchemeType.Value] + ".png";
                }

            }

        }

        switch (CreateGroupBuyDto.ColorSchemeType)
        {
            case ColorScheme.ForestDawn:
                CreateGroupBuyDto.PrimaryColor = "#23856D";
                CreateGroupBuyDto.SecondaryColor = "#FFD057";
                CreateGroupBuyDto.BackgroundColor = "#FFFFFF";
                CreateGroupBuyDto.SecondaryBackgroundColor = "#C9D6BD";
                CreateGroupBuyDto.AlertColor = "#FF902A";
                CreateGroupBuyDto.BlockColor = "#EFF4EB";

                break;

            case ColorScheme.TropicalSunset:
                CreateGroupBuyDto.PrimaryColor = "#FF902A";
                CreateGroupBuyDto.SecondaryColor = "#BDDA8D";
                CreateGroupBuyDto.BackgroundColor = "#FFFFFF";
                CreateGroupBuyDto.SecondaryBackgroundColor = "#E5D19A";
                CreateGroupBuyDto.AlertColor = "#FF902A";
                CreateGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.DeepSeaNight:
                CreateGroupBuyDto.PrimaryColor = "#133854";
                CreateGroupBuyDto.SecondaryColor = "#CAE28D";
                CreateGroupBuyDto.BackgroundColor = "#FFFFFF";
                CreateGroupBuyDto.SecondaryBackgroundColor = "#DCD6D0";
                CreateGroupBuyDto.AlertColor = "#A1E82D";
                CreateGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.SweetApricotCream:
                CreateGroupBuyDto.PrimaryColor = "#FFA085";
                CreateGroupBuyDto.SecondaryColor = "#BDDA8D";
                CreateGroupBuyDto.BackgroundColor = "#FFFFFF";
                CreateGroupBuyDto.SecondaryBackgroundColor = "#DCBFC3";
                CreateGroupBuyDto.AlertColor = "#FFC123";
                CreateGroupBuyDto.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.DesertDawn:
                CreateGroupBuyDto.PrimaryColor = "#C08C5D";
                CreateGroupBuyDto.SecondaryColor = "#E7AD99";
                CreateGroupBuyDto.BackgroundColor = "#FFFFFF";
                CreateGroupBuyDto.SecondaryBackgroundColor = "#EBC7AD";
                CreateGroupBuyDto.AlertColor = "#FF902A";
                CreateGroupBuyDto.BlockColor = "#EFF4EB";
                break;


            default:
                CreateGroupBuyDto.PrimaryColor = string.Empty;
                CreateGroupBuyDto.SecondaryColor = string.Empty;
                CreateGroupBuyDto.BackgroundColor = string.Empty;
                CreateGroupBuyDto.SecondaryBackgroundColor = string.Empty;
                CreateGroupBuyDto.AlertColor = string.Empty;
                CreateGroupBuyDto.BlockColor = string.Empty;
                IsColorPickerOpen = false;
                break;
        }
    }

    private void OnProductDetailsDisplayMethodChange(ChangeEventArgs e)
    {
        string? selectedMethod = e.Value.ToString();

        CreateGroupBuyDto.ProductDetailsDisplayMethod = !selectedMethod.IsNullOrEmpty() ?
                                                        Enum.Parse<ProductDetailsDisplayMethod>(selectedMethod) :
                                                        null;
    }

    //async Task OnLogoUploadAsync(FileChangedEventArgs e)
    //{
    //    if (e.Files.Length > 1)
    //    {
    //        await _uiMessageService.Error("Select Only 1 Logo Upload");
    //        await LogoPickerCustom.Clear();
    //        return;
    //    }
    //    if (e.Files.Length == 0)
    //    {
    //        return;
    //    }
    //    try
    //    {
    //        if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
    //        {
    //            await _uiMessageService.Error(L["InvalidFileType"]);
    //            await LogoPickerCustom.Clear();
    //            return;
    //        }
    //        if (e.Files[0].Size > MaxAllowedFileSize)
    //        {
    //            await LogoPickerCustom.RemoveFile(e.Files[0]);
    //            await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
    //            return;
    //        }
    //        string newFileName = Path.ChangeExtension(
    //              Guid.NewGuid().ToString().Replace("-", ""),
    //              Path.GetExtension(e.Files[0].Name));
    //        var stream = e.Files[0].OpenReadStream(long.MaxValue);
    //        try
    //        {
    //            var memoryStream = new MemoryStream();

    //            await stream.CopyToAsync(memoryStream);
    //            memoryStream.Position = 0;
    //            var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);
    //            logoBlobName = newFileName;

    //            CreateGroupBuyDto.LogoURL = url;

    //            await LogoPickerCustom.Clear();
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //        finally
    //        {
    //            stream.Close();
    //        }
    //    }
    //    catch (Exception exc)
    //    {
    //        Console.WriteLine(exc.Message);
    //        await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
    //    }
    //}
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
        selectedFile = e.Files[0];

        if (!ValidFileExtensions.Contains(Path.GetExtension(selectedFile.Name)))
        {
            await _uiMessageService.Error(L["InvalidFileType"]);
            await LogoPickerCustom.Clear();
            return;
        }

        if (selectedFile.Size > MaxAllowedFileSize)
        {
            await LogoPickerCustom.RemoveFile(selectedFile);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            return;
        }

        // Read image to Data URL for cropping preview
        using var stream = selectedFile.OpenReadStream(long.MaxValue);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());
        var fileExt = Path.GetExtension(selectedFile.Name).ToLowerInvariant().TrimStart('.');
        imageToCrop = $"data:image/{fileExt};base64,{base64}";
        stream.Close();
        // Show cropper modal
        croppedImage = "";
        await CropperModal.Show();
    }
    private Task OnSelectionChanged(CropperSelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Width != 0)
        {
            cropButtonDisabled = false;

            return InvokeAsync(StateHasChanged);
        }

        return Task.CompletedTask;
    }
    private async Task ResetSelection()
    {
        cropButtonDisabled = true;
        croppedImage = "";
        await LogoCropper.ResetSelection();
    }
    private async Task GetCroppedImage()
    {
        var options = new CropperCropOptions
        {
            Width = 300,       // Set desired width of the cropped image
            Height = 300,      // Set desired height of the cropped image
            ImageQuality = 0.9, // Set image quality (if using JPEG)
            ImageType = "image/png" // Specify the image format (use PNG in this case)
        };
        var base64Image = await LogoCropper.CropAsBase64ImageAsync(options);
        croppedImage = base64Image;

    }
    private async Task CropImageAsync()
    {
        try
        {


            // Strip the prefix
            var base64Data = croppedImage.Substring(croppedImage.IndexOf(",") + 1);

            // Convert to byte array
            var croppedBytes = Convert.FromBase64String(base64Data);

            // Save the image to the server
            string newFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";
            using var croppedStream = new MemoryStream(croppedBytes);

            var url = await _imageContainerManager.SaveAsync(newFileName, croppedStream);
            logoBlobName = newFileName;
            CreateGroupBuyDto.LogoURL = url;

            await _uiMessageService.Success("Logo uploaded successfully!");
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error("Something went wrong during cropping/upload.");
            Console.WriteLine(ex);
        }
        finally
        {
            await LogoCropper.ResetSelection();
            await LogoPickerCustom.Clear();
            await CropperModal.Hide();
        }
    }

    //async Task OnImageModuleUploadAsync(
    //    FileChangedEventArgs e,
    //    List<CreateImageDto> carouselImages,
    //    int carouselModuleNumber,
    //    FilePicker carouselPicker,
    //    ImageType imageType
    //)
    //{
    //    if (carouselImages.Count >= 5)
    //    {
    //        await carouselPicker.Clear();
    //        return;
    //    }

    //    int count = 0;
    //    try
    //    {
    //        foreach (var file in e.Files.Take(5))
    //        {
    //            if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name)))
    //            {
    //                await carouselPicker.RemoveFile(file);
    //                await _uiMessageService.Error(L["InvalidFileType"]);
    //                continue;
    //            }
    //            if (file.Size > MaxAllowedFileSize)
    //            {
    //                count++;
    //                await carouselPicker.RemoveFile(file);
    //                continue;
    //            }
    //            string newFileName = Path.ChangeExtension(
    //                  Guid.NewGuid().ToString().Replace("-", ""),
    //                  Path.GetExtension(file.Name));
    //            var stream = file.OpenReadStream(long.MaxValue);
    //            try
    //            {
    //                var memoryStream = new MemoryStream();

    //                await stream.CopyToAsync(memoryStream);
    //                memoryStream.Position = 0;
    //                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

    //                int sortNo = carouselImages.LastOrDefault()?.SortNo ?? 0;

    //                if (sortNo is 0 && carouselImages.Any(a => a.Id != Guid.Empty && a.CarouselStyle != null && a.BlobImageName == string.Empty))
    //                {
    //                    carouselImages[0].Name = file.Name;
    //                    carouselImages[0].BlobImageName = newFileName;
    //                    carouselImages[0].ImageUrl = url;
    //                    carouselImages[0].ImageType = imageType;
    //                    carouselImages[0].SortNo = sortNo + 1;
    //                    carouselImages[0].ModuleNumber = carouselModuleNumber;
    //                }

    //                else
    //                {
    //                    //int indexInCarouselModules = CarouselModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));
    //                    int indexInCarouselModules = 0;

    //                    if (imageType is ImageType.GroupBuyCarouselImage)
    //                        indexInCarouselModules = CarouselModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));

    //                    else if (imageType is ImageType.GroupBuyBannerImage)
    //                        indexInCarouselModules = BannerModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));
    //                    else if (imageType is ImageType.GroupBuyProductRankingCarousel)
    //                        indexInCarouselModules = ProductRankingCarouselModules.FindIndex(img => img.ModuleNumber == carouselModuleNumber);
    //                    if (indexInCarouselModules >= 0)
    //                    {
    //                        // List<CreateImageDto> originalCarouselImages = CarouselModules[indexInCarouselModules];

    //                        List<CreateImageDto> originalCarouselImages = new();

    //                        if (imageType is ImageType.GroupBuyCarouselImage)
    //                            originalCarouselImages = CarouselModules[indexInCarouselModules];

    //                        else if (imageType is ImageType.GroupBuyBannerImage)
    //                            originalCarouselImages = BannerModules[indexInCarouselModules];
    //                        else if (imageType is ImageType.GroupBuyProductRankingCarousel)
    //                            originalCarouselImages = ProductRankingCarouselModules[indexInCarouselModules].Images;

    //                        if (originalCarouselImages.Any(a => a.SortNo is 0))
    //                        {
    //                            int index = originalCarouselImages.IndexOf(originalCarouselImages.First(f => f.SortNo == 0));

    //                            originalCarouselImages[index].Name = file.Name;
    //                            originalCarouselImages[index].BlobImageName = newFileName;
    //                            originalCarouselImages[index].ImageUrl = url;
    //                            originalCarouselImages[index].ImageType = imageType;
    //                            originalCarouselImages[index].SortNo = sortNo + 1;
    //                            originalCarouselImages[index].ModuleNumber = carouselModuleNumber;
    //                        }

    //                        else
    //                        {
    //                            originalCarouselImages.Add(new CreateImageDto
    //                            {
    //                                Name = file.Name,
    //                                BlobImageName = newFileName,
    //                                ImageUrl = url,
    //                                ImageType = imageType,
    //                                SortNo = sortNo + 1,
    //                                ModuleNumber = carouselModuleNumber,
    //                                CarouselStyle = originalCarouselImages.FirstOrDefault(f => f.CarouselStyle != null)?.CarouselStyle ?? null
    //                            });
    //                        }
    //                    }
    //                }

    //                await carouselPicker.Clear();
    //            }
    //            finally
    //            {
    //                stream.Close();
    //            }
    //        }
    //        if (count > 0)
    //        {
    //            await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
    //        }
    //    }
    //    catch (Exception exc)
    //    {
    //        Console.WriteLine(exc.Message);
    //        await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
    //    }
    //}
    // Handles file selection from FilePicker
    async Task OnImageModuleUploadAsync(FileChangedEventArgs e, List<CreateImageDto> carouselImages, int carouselModuleNumber, FilePicker carouselPicker, ImageType imageType)
    {
        if (carouselImages.Count >= 5)
        {
            await carouselPicker.Clear();
            return;
        }

        selectedFiles = e.Files.ToList(); // Get the list of selected files
        CurrentFileIndex = 0;
        // Show the cropper for the first file
        if (selectedFiles.Any())
        {
            await ShowCropperForFileAsync(CurrentFileIndex, carouselImages, carouselModuleNumber, carouselPicker, imageType);
        }
    }

    private Task OnCarouselSelectionChanged(CropperSelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Width != 0)
        {
            cropButtonDisabled = false;

            return InvokeAsync(StateHasChanged);
        }

        return Task.CompletedTask;
    }
    private async Task ResetCarouselSelection()
    {
        cropButtonDisabled = true;
        croppedImage = "";
        await CarouselCropper.ResetSelection();
    }
    private async Task GetCarouselCroppedImage()
    {
        var options = new CropperCropOptions
        {
            Width = 300,       // Set desired width of the cropped image
            Height = 300,      // Set desired height of the cropped image
            ImageQuality = 0.9, // Set image quality (if using JPEG)
            ImageType = "image/png" // Specify the image format (use PNG in this case)
        };
        var base64Image = await CarouselCropper.CropAsBase64ImageAsync(options);
        croppedImage = base64Image;

    }
    // Show the cropper for the selected file
    private async Task ShowCropperForFileAsync(int fileIndex, List<CreateImageDto> carouselImages, int carouselModuleNumber, FilePicker carouselPicker, ImageType imageType)
    {
        selectedFile = selectedFiles[fileIndex];
        uploadedCarouselImages = carouselImages;
        CarouselPicker = carouselPicker;
        ImageType = imageType;
        CarouselModuleNumber = carouselModuleNumber;
        // Convert selected file to base64 for preview in the cropper
        using var stream = selectedFile.OpenReadStream(long.MaxValue);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        var fileExt = Path.GetExtension(selectedFile.Name).ToLowerInvariant().TrimStart('.');
        imageToCrop = $"data:image/{fileExt};base64,{base64}";

        croppedImage = ""; // Clear previous cropped image
        cropButtonDisabled = true; // Disable crop button until selection is made

        // Show the cropper modal
        await CarouselCropperModal.Show();
    }

    // Crops the image and uploads it
    private async Task CropImageAsync(List<CreateImageDto> carouselImages, int carouselModuleNumber, FilePicker carouselPicker, ImageType imageType)
    {
        try
        {
            // Strip the prefix from the base64 string
            var base64Data = croppedImage.Substring(croppedImage.IndexOf(",") + 1);

            // Convert base64 string to byte array
            var croppedBytes = Convert.FromBase64String(base64Data);

            // Generate a new unique file name for the cropped image
            string newFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";

            // Save the cropped image to the server
            using var croppedStream = new MemoryStream(croppedBytes);
            var url = await _imageContainerManager.SaveAsync(newFileName, croppedStream);

            int sortNo = carouselImages.LastOrDefault()?.SortNo ?? 0;

            if (sortNo is 0 && carouselImages.Any(a => a.Id != Guid.Empty && a.CarouselStyle != null && a.BlobImageName == string.Empty))
            {
                carouselImages[0].Name = selectedFile.Name;
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
                    // List<CreateImageDto> originalCarouselImages = CarouselModules[indexInCarouselModules];

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

                        originalCarouselImages[index].Name = selectedFile.Name;
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
                            Name = selectedFile.Name,
                            BlobImageName = newFileName,
                            ImageUrl = url,
                            ImageType = imageType,
                            SortNo = sortNo + 1,
                            ModuleNumber = carouselModuleNumber,
                            CarouselStyle = originalCarouselImages.FirstOrDefault(f => f.CarouselStyle != null)?.CarouselStyle ?? null
                        });
                    }
                }
            }

            // Move to the next image in the list, if any
            if (CurrentFileIndex < selectedFiles.Count - 1)
            {
                CurrentFileIndex++;
                await ShowCropperForFileAsync(CurrentFileIndex, carouselImages, carouselModuleNumber, carouselPicker, imageType);
            }
            else
            {
                // All images are processed, hide the modal and show success message
                await _uiMessageService.Success("All images cropped and uploaded successfully!");
                await CarouselCropperModal.Hide();
                await carouselPicker.Clear();
                await InvokeAsync(StateHasChanged);

            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error("An error occurred during cropping/upload.");
            Console.WriteLine(ex);
        }
        finally
        {
            // Reset cropper and picker
            await CarouselCropper.ResetSelection();
            await carouselPicker.Clear();

        }
    }
    //async Task OnBannerImageModuleUploadAsync(
    //    FileChangedEventArgs e,
    //    List<CreateImageDto> bannerImages,
    //    int carouselModuleNumber,
    //    FilePicker bannerPicker,
    //    ImageType imageType
    //)
    //{

    //    if (e.Files.Length > 1)
    //    {
    //        await _uiMessageService.Error("Select Only 1 Banner to Upload");
    //        await LogoPickerCustom.Clear();
    //        return;
    //    }
    //    if (e.Files.Length == 0)
    //    {
    //        return;
    //    }

    //    int count = 0;
    //    try
    //    {
    //        if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
    //        {
    //            await _uiMessageService.Error(L["InvalidFileType"]);
    //            await LogoPickerCustom.Clear();
    //            return;
    //        }
    //        if (e.Files[0].Size > MaxAllowedFileSize)
    //        {
    //            await LogoPickerCustom.RemoveFile(e.Files[0]);
    //            await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
    //            return;
    //        }
    //        string newFileName = Path.ChangeExtension(
    //                  Guid.NewGuid().ToString().Replace("-", ""),
    //                  Path.GetExtension(e.Files[0].Name));
    //        var stream = e.Files[0].OpenReadStream(long.MaxValue);
    //        try
    //        {
    //            var memoryStream = new MemoryStream();

    //            await stream.CopyToAsync(memoryStream);
    //            memoryStream.Position = 0;
    //            var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

    //            int sortNo = bannerImages[0].SortNo is 0 ? bannerImages[0].SortNo + 1 : 1;

    //            bannerImages[0].Name = e.Files[0].Name;
    //            bannerImages[0].BlobImageName = newFileName;
    //            bannerImages[0].ImageUrl = url;
    //            bannerImages[0].ImageType = imageType;
    //            bannerImages[0].SortNo = sortNo;
    //            bannerImages[0].ModuleNumber = carouselModuleNumber;

    //            SelectedImageDto.Link = string.Empty;

    //            await bannerPicker.Clear();
    //        }
    //        finally
    //        {
    //            stream.Close();
    //        }

    //        if (count > 0)
    //        {
    //            await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
    //        }
    //    }
    //    catch (Exception exc)
    //    {
    //        Console.WriteLine(exc.Message);
    //        await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
    //    }
    //}
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
            await bannerPicker.Clear();
            return;
        }
        if (e.Files.Length == 0)
        {
            return;
        }

        // Handle the selected file
        selectedFile = e.Files[0];

        if (!ValidFileExtensions.Contains(Path.GetExtension(selectedFile.Name)))
        {
            await _uiMessageService.Error(L["InvalidFileType"]);
            await bannerPicker.Clear();
            return;
        }
        if (selectedFile.Size > MaxAllowedFileSize)
        {
            await bannerPicker.RemoveFile(selectedFile);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            return;
        }

        // Convert the selected file to base64 for preview in the cropper
        using var stream = selectedFile.OpenReadStream(long.MaxValue);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        var fileExt = Path.GetExtension(selectedFile.Name).ToLowerInvariant().TrimStart('.');
        imageToCrop = $"data:image/{fileExt};base64,{base64}";
        BannerModuleNumber = carouselModuleNumber;
        BannerPicker = bannerPicker;
        uploadBannerImages = bannerImages;
        ImageType = imageType;
        croppedImage = ""; // Reset cropped image
        cropButtonDisabled = true; // Disable crop button until selection is made

        // Show the cropper modal for cropping
        await BannerCropperModal.Show();
    }
    private Task OnBannerSelectionChanged(CropperSelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Width != 0)
        {
            cropButtonDisabled = false;

            return InvokeAsync(StateHasChanged);
        }

        return Task.CompletedTask;
    }
    private async Task ResetBannerSelection()
    {
        cropButtonDisabled = true;
        croppedImage = "";
        await BannerCropper.ResetSelection();
    }
    private async Task GetBannerCroppedImage()
    {
        var options = new CropperCropOptions
        {
            Width = 300,       // Set desired width of the cropped image
            Height = 300,      // Set desired height of the cropped image
            ImageQuality = 0.9, // Set image quality (if using JPEG)
            ImageType = "image/png" // Specify the image format (use PNG in this case)
        };
        var base64Image = await BannerCropper.CropAsBase64ImageAsync(options);
        croppedImage = base64Image;

    }
      // Handle cropping and uploading the image after cropping
    private async Task CropBannerImageAsync(List<CreateImageDto> bannerImages, int bannerModuleNumber, FilePicker bannerPicker, ImageType imageType)
    {
        try
        {
            // Strip the base64 prefix
            var base64Data = croppedImage.Substring(croppedImage.IndexOf(",") + 1);

            // Convert base64 string to byte array
            var croppedBytes = Convert.FromBase64String(base64Data);

            // Generate a new unique file name for the cropped image
            string newFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";

            // Save the cropped image to the server
            using var croppedStream = new MemoryStream(croppedBytes);
            var url = await _imageContainerManager.SaveAsync(newFileName, croppedStream);

            // Determine the sort number for the banner images
            int sortNo = bannerImages.FirstOrDefault()?.SortNo ?? 0;

            // Update the banner image list
            bannerImages[0].Name = selectedFile.Name;
            bannerImages[0].BlobImageName = newFileName;
            bannerImages[0].ImageUrl = url;
            bannerImages[0].ImageType = imageType;
            bannerImages[0].SortNo = sortNo + 1;
            bannerImages[0].ModuleNumber = bannerModuleNumber;

            // Clear the picker after uploading the image
            await bannerPicker.Clear();

            await _uiMessageService.Success("Banner image cropped and uploaded successfully!");
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error("Something went wrong during cropping/upload.");
            Console.WriteLine(ex);
        }
        finally
        {
            // Reset the selection in the cropper and hide the modal
            await BannerCropper.ResetSelection();
            await BannerCropperModal.Hide();
        }
    }
    async Task OnPartnershipImageUploadAsync(FileChangedEventArgs e, MultiImageModuleItem item)
    {
        if (e.Files.Length == 0) return;

        try
        {
            if (e.Files.Any(file => !ValidFileExtensions.Contains(Path.GetExtension(file.Name))))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);
                return;
            }
            if (e.Files.Any(file => file.Size > MaxAllowedFileSize))
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                return;
            }

            foreach (var file in e.Files.Take(5))
            {
                if(item.Images.Count >= 5) return;
                var stream = file.OpenReadStream(long.MaxValue);

                try
                {
                    string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(e.Files[0].Name));

                    var memoryStream = new MemoryStream();

                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                    int sortNo = item.Images.OrderByDescending(i => i.SortNo).Select(i => i.SortNo).FirstOrDefault() + 1;
                    item.Images.Add(new GroupBuyItemGroupImageDto
                    {
                        BlobImageName = newFileName,
                        Url = url,
                        SortNo = sortNo
                    });

                    item.FilePicker?.Clear();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    stream.Close();
                }
            }

        }
        catch (Exception exc)
        {
            await HandleErrorAsync(exc);
        }
    }

    async Task DeletePartnershipImage(MultiImageModuleItem item, GroupBuyItemGroupImageDto image)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                await _imageContainerManager.DeleteAsync(image.BlobImageName);
                item.Images.Remove(image);
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
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
                var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);
                bannerBlobName = newFileName;
                CreateGroupBuyDto.BannerURL = url;
                await BannerPickerCustom.Clear();
            }
            finally
            {
                stream.Close();
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    async Task DeleteLogoAsync(string blobImageName)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                await _imageContainerManager.DeleteAsync(blobImageName);
                CreateGroupBuyDto.LogoURL = null;
                StateHasChanged();
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
                confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                CreateGroupBuyDto.BannerURL = null;
                StateHasChanged();
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
                await _imageContainerManager.DeleteAsync(blobImageName);

                if (imageType is ImageType.GroupBuyBannerImage)
                {
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
                }

                else if (imageType is ImageType.GroupBuyCarouselImage)
                {
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
                }

                else if (imageType is ImageType.GroupBuyProductRankingCarousel)
                {
                    foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                    {
                        module.Images.RemoveAll(r => r.BlobImageName == blobImageName);
                    }
                }

                carouselImages = carouselImages.Where(x => x.BlobImageName != blobImageName).ToList();
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
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
                ModuleNumber = CollapseItem.Any(c => c.GroupBuyModuleType is GroupBuyModuleType.CarouselImages) ?
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
            if (CreateGroupBuyDto.ColorSchemeType.HasValue)
            {
                img = "https://pikachublobs.blob.core.windows.net/images/" + L["Enum:ColorSchemeFile." + (int)CreateGroupBuyDto.ColorSchemeType.Value] + ".png";
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

        else if (groupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
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
        }

        else if (groupBuyModuleType == GroupBuyModuleType.CustomTextModule || groupBuyModuleType == GroupBuyModuleType.VideoUpload)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected = []
            };

            CollapseItem.Add(collapseItem);
        }

        else if (groupBuyModuleType == GroupBuyModuleType.PartnershipModule)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected = [],
                ImageModules = [new MultiImageModuleItem(), new MultiImageModuleItem(), new MultiImageModuleItem()]
            };

            CollapseItem.Add(collapseItem);
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

        CreateGroupBuyDto.SelfPickupDeliveryTime = JsonConvert.SerializeObject(SelfPickupTimeList.Distinct());
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

        CreateGroupBuyDto.BlackCatDeliveryTime = JsonConvert.SerializeObject(BlackCateDeliveryTimeList.Distinct());

    }
    void DeliverdByStoreDeliveryTimeCheckedChange(string method, ChangeEventArgs e, bool clearAll = false)
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

        if (value) DeliverdByStoreTimeList.Add(method);

        else DeliverdByStoreTimeList.Remove(method);

        CreateGroupBuyDto.DeliveredByStoreDeliveryTime = JsonConvert.SerializeObject(DeliverdByStoreTimeList.Distinct());

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

        bool value = (bool)(e?.Value ?? false);

        if (!IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery || clearAll)
        {
            if (value) HomeDeliveryTimeList.Add(method);

            else HomeDeliveryTimeList.Remove(method);

            if (clearAll)
            {
                IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;
            }
        }

        CreateGroupBuyDto.HomeDeliveryDeliveryTime = JsonConvert.SerializeObject(HomeDeliveryTimeList.Distinct());
    }

    void OnShippingMethodCheckedChange(string method, ChangeEventArgs e)
    {
        var value = (bool)(e?.Value ?? false);

        if (value)
        {
            // If the selected method is SevenToEleven or FamilyMart, uncheck the corresponding C2C method
            if (method == "SevenToEleven1" && CreateGroupBuyDto.ShippingMethodList.Contains("SevenToElevenC2C"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("SevenToElevenC2C");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenC2C");
            }
            else if (method is "SevenToElevenFrozen" && CreateGroupBuyDto.ShippingMethodList.Contains("SevenToElevenC2C"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("SevenToElevenC2C");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenC2C");
            }
            else if (method == "FamilyMart1" && CreateGroupBuyDto.ShippingMethodList.Contains("FamilyMartC2C"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("FamilyMartC2C");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "FamilyMartC2C");
            }
            else if (method == "SevenToElevenC2C" && CreateGroupBuyDto.ShippingMethodList.Contains("SevenToEleven1"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("SevenToEleven1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToEleven1");
            }
            else if (method is "SevenToElevenC2C" && CreateGroupBuyDto.ShippingMethodList.Contains("SevenToElevenFrozen"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("SevenToElevenFrozen");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenFrozen");
            }
            else if (method == "FamilyMartC2C" && CreateGroupBuyDto.ShippingMethodList.Contains("FamilyMart1"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("FamilyMart1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "FamilyMart1");
            }
            else if (method == "BlackCat1" && CreateGroupBuyDto.ShippingMethodList.Contains("BlackCatFreeze"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("BlackCatFreeze");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFreeze");
            }
            else if (method == "BlackCat1" && CreateGroupBuyDto.ShippingMethodList.Contains("BlackCatFrozen"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("BlackCatFrozen");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFrozen");
            }
            else if (method == "BlackCatFreeze" && CreateGroupBuyDto.ShippingMethodList.Contains("BlackCat1"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("BlackCat1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCat1");
            }
            else if (method == "BlackCatFreeze" && CreateGroupBuyDto.ShippingMethodList.Contains("BlackCatFrozen"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("BlackCatFrozen");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFrozen");
            }
            else if (method == "BlackCatFrozen" && CreateGroupBuyDto.ShippingMethodList.Contains("BlackCat1"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("BlackCat1");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCat1");
            }
            else if (method == "BlackCatFrozen" && CreateGroupBuyDto.ShippingMethodList.Contains("BlackCatFreeze"))
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("BlackCatFreeze");
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFreeze");
            }
        }

        // Update the selected method in the CreateGroupBuyDto.ShippingMethodList
        if (value)
        {
            if (method == "DeliveredByStore")
            {
                foreach (var item in CreateGroupBuyDto.ShippingMethodList)
                {
                    JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);

                }
                CreateGroupBuyDto.ShippingMethodList = new List<string>();
            }
            else
            {
                CreateGroupBuyDto.ShippingMethodList.Remove("DeliveredByStore");

            }
            CreateGroupBuyDto.ShippingMethodList.Add(method);
        }
        else
        {
            CreateGroupBuyDto.ShippingMethodList.Remove(method);
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

        if (!CreateGroupBuyDto.ShippingMethodList.Any(x => blackCat.Contains(x)))
        {
            DeliveryTimeConts.BlackCat.ForEach(item =>
            {

                BlackCatDeliveryTimeCheckedChange(item, new ChangeEventArgs { Value = false });
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);


            });
        }

        if (!CreateGroupBuyDto.ShippingMethodList.Contains(DeliveryMethod.HomeDelivery.ToString()))
        {
            DeliveryTimeConts.HomeDelivery.ForEach(item =>
            {
                HomeDeliveryTimeCheckedChange(item, new ChangeEventArgs { Value = false }, true);
                if (!(CreateGroupBuyDto.ShippingMethodList.Any(x => blackCat.Contains(x)) && item == "Inapplicable"))
                    JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);
            });
        }

        if (!CreateGroupBuyDto.ShippingMethodList.Contains(DeliveryMethod.DeliveredByStore.ToString()))
        {
            DeliveryTimeConts.DeliveredByStore.ForEach(item =>
            {
                DeliverdByStoreDeliveryTimeCheckedChange(item, new ChangeEventArgs { Value = false }, true);
                if (!(CreateGroupBuyDto.ShippingMethodList.Any(x => blackCat.Contains(x)) && item == "Inapplicable"))
                    JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);
            });
        }

        // Serialize the updated list and assign it to ExcludeShippingMethod
        CreateGroupBuyDto.ExcludeShippingMethod = JsonConvert.SerializeObject(CreateGroupBuyDto.ShippingMethodList);
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

    //private void HandleItemTagInputChange(ChangeEventArgs e)
    //{
    //    var selectedOptions = (IEnumerable<string>)e.Value;
    //    ItemTags=new List<string>();
    //    foreach (var item in selectedOptions.ToList())
    //    {

    //        if (!ItemTags.Any(x => x == item))
    //        {

    //            ItemTags.Add(item);

    //        }

    //    }


    //}

    //private void HandleItemTagDelete(string item)
    //{
    //    ItemTags.Remove(item);
    //}

    public void OnEnterPriseChange(bool e)
    {
        CreateGroupBuyDto.IsEnterprise = e;

        if (CreateGroupBuyDto.IsEnterprise)
        {
            IsCashOnDelivery = true; CreditCard = false; BankTransfer = false; IsLinePay = false;
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

    private void HandlePaymentTagInputKeyUp(KeyboardEventArgs e)
    {
        PaymentMethodError = null;
        if (e.Key == "Enter")
        {
            if (!PaymentTagInputValue.IsNullOrWhiteSpace() && !PaymentMethodTags.Any(x => x == PaymentTagInputValue))
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
            }
            PaymentTagInputValue = string.Empty;
        }
    }

    private void HandlePaymentTagDelete(string item)
    {
        PaymentMethodTags.Remove(item);
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

    //public async Task OnImageUploadAsync(
    //    FileChangedEventArgs e,
    //    GroupPurchaseOverviewDto module,
    //    FilePicker filePicker
    //)
    //{
    //    try
    //    {
    //        if (e.Files.Length is 0) return;

    //        if (e.Files.Length > 1)
    //        {
    //            await _uiMessageService.Error("Cannot add more 1 image.");

    //            await filePicker.Clear();

    //            return;
    //        }

    //        if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
    //        {
    //            await _uiMessageService.Error(L["InvalidFileType"]);

    //            await filePicker.Clear();

    //            return;
    //        }

    //        string newFileName = Path.ChangeExtension(
    //            Guid.NewGuid().ToString().Replace("-", ""),
    //            Path.GetExtension(e.Files[0].Name)
    //        );

    //        Stream stream = e.Files[0].OpenReadStream(long.MaxValue);

    //        try
    //        {
    //            MemoryStream memoryStream = new();

    //            await stream.CopyToAsync(memoryStream);

    //            memoryStream.Position = 0;

    //            string url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

    //            module.Image = url;

    //            await filePicker.Clear();
    //        }
    //        finally
    //        {
    //            stream.Close();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);

    //        await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
    //    }
    //}
    public async Task OnImageUploadAsync(
        FileChangedEventArgs e,
        GroupPurchaseOverviewDto module,
        FilePicker filePicker
    )
    {
        try
        {
            if (e.Files.Length == 0) return;

            if (e.Files.Length > 1)
            {
                await _uiMessageService.Error("Cannot add more than 1 image.");
                await filePicker.Clear();
                return;
            }

            selectedFile = e.Files[0];

            // Validate file type
            if (!ValidFileExtensions.Contains(Path.GetExtension(selectedFile.Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);
                await filePicker.Clear();
                return;
            }

            // Convert the selected file to base64 for preview in the cropper
            using var stream = selectedFile.OpenReadStream(long.MaxValue);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            var fileExt = Path.GetExtension(selectedFile.Name).ToLowerInvariant().TrimStart('.');
            imageToCrop = $"data:image/{fileExt};base64,{base64}";

            croppedImage = ""; // Reset cropped image
            cropButtonDisabled = true; // Disable crop button until selection is made
            GroupPurchaseOverviewModule = module;
            PurchaseOverviewPicker = filePicker;
            // Show the cropper modal for cropping
            await PurchaseOverviewCropperModal.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }
    // Handle the cropper selection changed event
    private Task OnPurchaseOverviewSelectionChanged(CropperSelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Width != 0)
        {
            cropButtonDisabled = false;
            return InvokeAsync(StateHasChanged);
        }

        return Task.CompletedTask;
    }
    // Reset the cropper selection
    private async Task ResetPurchaseOverviewSelection()
    {
        cropButtonDisabled = true;
        croppedImage = "";
        await PurchaseOverviewCropper.ResetSelection();
    }
    // Get the cropped image in base64 format
    private async Task GetPurchaseOverviewCroppedImage()
    {
        var options = new CropperCropOptions
        {
            Width = 300,       // Set desired width of the cropped image
            Height = 300,      // Set desired height of the cropped image
            ImageQuality = 0.9, // Set image quality (if using JPEG)
            ImageType = "image/png" // Specify the image format (use PNG in this case)
        };
        var base64Image = await PurchaseOverviewCropper.CropAsBase64ImageAsync(options);
        croppedImage = base64Image;
    }
    // Crops the image and uploads it
    private async Task CropPurchaseOverviewImageAsync(GroupPurchaseOverviewDto module, FilePicker filePicker)
    {
        try
        {
            // Strip the prefix from the base64 string
            var base64Data = croppedImage.Substring(croppedImage.IndexOf(",") + 1);

            // Convert base64 string to byte array
            var croppedBytes = Convert.FromBase64String(base64Data);

            // Generate a new unique file name for the cropped image
            string newFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";

            // Save the cropped image to the server
            using var croppedStream = new MemoryStream(croppedBytes);
            var url = await _imageContainerManager.SaveAsync(newFileName, croppedStream);

            // Update the module with the image URL
            module.Image = url;

            // Clear the picker after uploading the image
            await filePicker.Clear();

            await _uiMessageService.Success("Banner image cropped and uploaded successfully!");
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error("Something went wrong during cropping/upload.");
            Console.WriteLine(ex);
        }
        finally
        {
            // Reset the selection in the cropper and hide the modal
            await PurchaseOverviewCropper.ResetSelection();
            await PurchaseOverviewCropperModal.Hide();
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
    //public async Task OnImageUploadAsync(
    //    FileChangedEventArgs e,
    //    GroupBuyOrderInstructionDto module,
    //    FilePicker filePicker
    //)
    //{
    //    try
    //    {
    //        if (e.Files.Length is 0) return;

    //        if (e.Files.Length > 1)
    //        {
    //            await _uiMessageService.Error("Cannot add more 1 image.");

    //            await filePicker.Clear();

    //            return;
    //        }

    //        if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
    //        {
    //            await _uiMessageService.Error(L["InvalidFileType"]);

    //            await filePicker.Clear();

    //            return;
    //        }

    //        string newFileName = Path.ChangeExtension(
    //            Guid.NewGuid().ToString().Replace("-", ""),
    //            Path.GetExtension(e.Files[0].Name)
    //        );

    //        Stream stream = e.Files[0].OpenReadStream(long.MaxValue);

    //        try
    //        {
    //            MemoryStream memoryStream = new();

    //            await stream.CopyToAsync(memoryStream);

    //            memoryStream.Position = 0;

    //            string url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

    //            module.Image = url;

    //            await filePicker.Clear();
    //        }
    //        finally
    //        {
    //            stream.Close();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);

    //        await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
    //    }
    //}
    public async Task OnImageUploadAsync(
        FileChangedEventArgs e,
        GroupBuyOrderInstructionDto module,
        FilePicker filePicker
    )
    { 
    try
        {
            if (e.Files.Length == 0) return;

            if (e.Files.Length > 1)
            {
                await _uiMessageService.Error("Cannot add more than 1 image.");
    await filePicker.Clear();
                return;
            }

selectedFile = e.Files[0];

            // Validate file type
            if (!ValidFileExtensions.Contains(Path.GetExtension(selectedFile.Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);
await filePicker.Clear();
                return;
            }

            // Convert the selected file to base64 for preview in the cropper
            using var stream = selectedFile.OpenReadStream(long.MaxValue);
using var memoryStream = new MemoryStream();
await stream.CopyToAsync(memoryStream);
var base64 = Convert.ToBase64String(memoryStream.ToArray());
var fileExt = Path.GetExtension(selectedFile.Name).ToLowerInvariant().TrimStart('.');
imageToCrop = $"data:image/{fileExt};base64,{base64}";

croppedImage = ""; // Reset cropped image
cropButtonDisabled = true; // Disable crop button until selection is made
            GroupOrderInstructionModule = module;
            OrderInstructionPicker = filePicker;
// Show the cropper modal for cropping
await OrderInstructionCropperModal.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }
    // Handle the cropper selection changed event
    private Task OnOrderInstructionSelectionChanged(CropperSelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Width != 0)
        {
            cropButtonDisabled = false;
            return InvokeAsync(StateHasChanged);
        }

        return Task.CompletedTask;
    }
    // Reset the cropper selection
    private async Task ResetOrderInstructionSelection()
    {
        cropButtonDisabled = true;
        croppedImage = "";
        await OrderInstructionCropper.ResetSelection();
    }
    // Get the cropped image in base64 format
    private async Task GetOrderInstructionCroppedImage()
    {
        var options = new CropperCropOptions
        {
            Width = 300,       // Set desired width of the cropped image
            Height = 300,      // Set desired height of the cropped image
            ImageQuality = 0.9, // Set image quality (if using JPEG)
            ImageType = "image/png" // Specify the image format (use PNG in this case)
        };
        var base64Image = await OrderInstructionCropper.CropAsBase64ImageAsync(options);
        croppedImage = base64Image;
    }
    // Crops the image and uploads it
    private async Task CropOrderInstructionImageAsync(GroupBuyOrderInstructionDto module, FilePicker filePicker)
    {
        try
        {
            // Strip the prefix from the base64 string
            var base64Data = croppedImage.Substring(croppedImage.IndexOf(",") + 1);

            // Convert base64 string to byte array
            var croppedBytes = Convert.FromBase64String(base64Data);

            // Generate a new unique file name for the cropped image
            string newFileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.png";

            // Save the cropped image to the server
            using var croppedStream = new MemoryStream(croppedBytes);
            var url = await _imageContainerManager.SaveAsync(newFileName, croppedStream);

            // Update the module with the image URL
            module.Image = url;

            // Clear the picker after uploading the image
            await filePicker.Clear();

            await _uiMessageService.Success("Banner image cropped and uploaded successfully!");
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error("Something went wrong during cropping/upload.");
            Console.WriteLine(ex);
        }
        finally
        {
            // Reset the selection in the cropper and hide the modal
            await PurchaseOverviewCropper.ResetSelection();
            await PurchaseOverviewCropperModal.Hide();
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

    protected virtual async Task CreateEntityAsync()
    {
        try
        {
            await Loading.Show();

            if (CollapseItem.Any(a => a.IsWarnedForInCompatible))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.InCompatibleModule]);
                await Loading.Hide();
                return;
            }

            if (CreateGroupBuyDto.GroupBuyName.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyNameCannotBeNull]);
                await Loading.Hide();
                return;
            }
            if (CreateGroupBuyDto.ShortCode.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyShortCodeCannotBeNull]);
                await Loading.Hide();
                return;
            }
            if (CreateGroupBuyDto.LogoURL.IsNullOrEmpty())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.LogoIsRequired]);
                await Loading.Hide();
                return;
            }
            string shortCode = CreateGroupBuyDto.ShortCode;
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
                var check = await _groupBuyAppService.CheckShortCodeForCreate(CreateGroupBuyDto.ShortCode);

                if (check)
                {
                    await _uiMessageService.Warn(L["Short Code Alredy Exist"]);
                    await Loading.Hide();
                    return;
                }
            }
            else
            {

                CreateGroupBuyDto.ShortCode = "";
            }
            CreateGroupBuyDto.GroupBuyNo = 0;
            CreateGroupBuyDto.Status = "New";
            if (CreateGroupBuyDto.FreeShipping && CreateGroupBuyDto.FreeShippingThreshold is null)
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOnePaymentMethodIsRequired]);
                await Loading.Hide();
                return;
            }

            if (ItemTags.Any())
            {
                CreateGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
            }
            //if (PaymentMethodTags.Any())
            //{
            //    CreateGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
            //}
            List<string> paymentMethods = new List<string>();

            if (CreditCard) paymentMethods.Add("Credit Card");
            if (BankTransfer) paymentMethods.Add("Bank Transfer");
            if (IsCashOnDelivery) paymentMethods.Add("Cash On Delivery");
            if (IsLinePay) paymentMethods.Add("LinePay");
            if (paymentMethods.Count > 0) CreateGroupBuyDto.PaymentMethod = string.Join(" , ", paymentMethods);
            else CreateGroupBuyDto.PaymentMethod = string.Empty;

            if (CreditCard)

                if (CreateGroupBuyDto.ProductType is null)
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ProductTypeIsRequired]);
                    await Loading.Hide();
                    return;
                }
            if (CreateGroupBuyDto.PaymentMethod.IsNullOrEmpty())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOnePaymentMethodIsRequired]);
                await Loading.Hide();
                return;
            }
            if (CreateGroupBuyDto.ExcludeShippingMethod.IsNullOrEmpty())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneShippingMethodIsRequired]);
                await Loading.Hide();
                return;
            }
            if (CreateGroupBuyDto.IsEnterprise && (CreateGroupBuyDto.ExcludeShippingMethod is not "[\"SelfPickup\"]"))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.EnterprisePurchaseCanOnlyUseSelfPickup]);
                await Loading.Hide();
                return;
            }
            if (CreateGroupBuyDto.FreeShipping && CreateGroupBuyDto.FreeShippingThreshold == null)
            {
                await _uiMessageService.Warn(L["PleaseEnterThresholdAmount"]);
                await Loading.Hide();
                return;
            }
            if ((!CreateGroupBuyDto.ExcludeShippingMethod.IsNullOrEmpty()) && (CreateGroupBuyDto.ExcludeShippingMethod.Contains("BlackCat1")
                || CreateGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFreeze") || CreateGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFrozen")
                || CreateGroupBuyDto.ExcludeShippingMethod.Contains("SelfPickup") || CreateGroupBuyDto.ExcludeShippingMethod.Contains("HomeDelivery")
                || CreateGroupBuyDto.ExcludeShippingMethod.Contains("DeliveredByStore")))
            {
                if (CreateGroupBuyDto.ExcludeShippingMethod.Contains("BlackCat1") && (CreateGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty() || CreateGroupBuyDto.BlackCatDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForBlackCat]);
                    await Loading.Hide();
                    return;
                }
                if (CreateGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFreeze") && (CreateGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty() || CreateGroupBuyDto.BlackCatDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForBlackCatFreeze]);
                    await Loading.Hide();
                    return;
                }
                if (CreateGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFrozen") && (CreateGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty() || CreateGroupBuyDto.BlackCatDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForBlackCatFrozen]);
                    await Loading.Hide();
                    return;
                }
                else if (CreateGroupBuyDto.ExcludeShippingMethod.Contains("SelfPickup") && CreateGroupBuyDto.SelfPickupDeliveryTime.IsNullOrEmpty() || (CreateGroupBuyDto.SelfPickupDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForSelfPickup]);
                    await Loading.Hide();
                    return;
                }
                else if (CreateGroupBuyDto.ExcludeShippingMethod.Contains("HomeDelivery") && (CreateGroupBuyDto.HomeDeliveryDeliveryTime.IsNullOrEmpty() || CreateGroupBuyDto.HomeDeliveryDeliveryTime == "[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForHomeDelivery]);
                    await Loading.Hide();
                    return;
                }
            }
            else if (CreateGroupBuyDto.ExcludeShippingMethod.Contains("DeliveredByStore") && (CreateGroupBuyDto.HomeDeliveryDeliveryTime.IsNullOrEmpty() || CreateGroupBuyDto.HomeDeliveryDeliveryTime == "[]"))
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForDeliverdByStore]);
                await Loading.Hide();
                return;
            }
            if (CreateGroupBuyDto.ColorSchemeType == null || CreateGroupBuyDto.ColorSchemeType == 0)
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ColorSchemeRequired]);
                await Loading.Hide();
                return;

            }
            if (CreateGroupBuyDto.ProductDetailsDisplayMethod == null || CreateGroupBuyDto.ProductDetailsDisplayMethod == 0)
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.ProductDetailsDisplayMethodRequired]);
                await Loading.Hide();
                return;

            }
            if (CreateGroupBuyDto.GroupBuyConditionDescription.IsNullOrWhiteSpace())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyConditionRequired]);
                await Loading.Hide();
                return;

            }
            CreateGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
            if (CreateGroupBuyDto.ExchangePolicyDescription.IsNullOrWhiteSpace())
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
                        await _uiMessageService.Error("Title Cannot be empty in GroupBuy Order Instruction Module");

                        await Loading.Hide();

                        return;
                    }

                    if (groupBuyOrderInstruction.Image.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("Please Add Image in GroupBuy Order Instruction Module");

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
                        await _uiMessageService.Error("Title Cannot be empty in Product Ranking Carousel Module");

                        await Loading.Hide();

                        return;
                    }

                    if (productRankingCarouselModule.SubTitle.IsNullOrEmpty())
                    {
                        await _uiMessageService.Error("SubTitle Cannot be empty in Product Ranking Carousel Modules");

                        await Loading.Hide();

                        return;
                    }
                }
            }

            if (CarouselModules is { Count: > 0 })
            {
                foreach (var carouselImages in CarouselModules)
                {
                    foreach (var carouselImage in carouselImages)
                    {
                        if (carouselImage.CarouselStyle == null)
                        {
                            await _uiMessageService.Error(L["CarouselModuleStyleCannotBeEmpty"]);

                            await Loading.Hide();

                            return;
                        }
                    }
                }
            }

            CreateGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
            //CreateGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();

            //CreateGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

            CreateGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

            foreach (var item in CollapseItem)
            {
                if (item.Selected.Any(s => s.Id == Guid.Empty && item.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor && s.Name.IsNullOrEmpty()))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyModuleCannotBeEmpty]);
                    await Loading.Hide();
                    return;
                }

                int j = 1;
                if (item.Selected.Any())
                {
                    GroupBuyItemGroupCreateUpdateDto itemGroup = new();

                    if (item.GroupBuyModuleType is GroupBuyModuleType.ProductGroupModule)
                    {
                        itemGroup = new()
                        {
                            SortOrder = item.SortOrder,
                            GroupBuyModuleType = item.GroupBuyModuleType,
                            ProductGroupModuleTitle = item.ProductGroupModuleTitle,
                            ProductGroupModuleImageSize = item.ProductGroupModuleImageSize
                        };
                    }

                    else
                    {
                        itemGroup = new()
                        {
                            SortOrder = item.SortOrder,
                            GroupBuyModuleType = item.GroupBuyModuleType
                        };
                    }

                    foreach (var itemDetail in item.Selected)
                    {
                        if (itemDetail.Id != Guid.Empty || (item.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor && !itemDetail.Name.IsNullOrEmpty()))
                        {
                            if (item.GroupBuyModuleType == GroupBuyModuleType.ProductGroupModule)
                            {
                                if (itemDetail.ItemType == ItemType.Item)
                                {
                                    if (itemDetail.SelectedItemDetailIds == null || !itemDetail.SelectedItemDetailIds.Any())
                                    {
                                        await _uiMessageService.Error($"Item '{itemDetail.Name}' must have at least one variant selected.");
                                        await Loading.Hide();
                                        return;
                                    }

                                    foreach (var detailId in itemDetail.SelectedItemDetailIds)
                                    {
                                        if (!itemDetail.ItemDetailsWithPrices.TryGetValue(detailId, out var labelAndPrice))
                                        {
                                            await _uiMessageService.Error($"Price missing for one or more item variants in '{itemDetail.Name}'.");
                                            await Loading.Hide();
                                            return;
                                        }

                                        itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                                        {
                                            SortOrder = j++,
                                            ItemId = itemDetail.ItemType == ItemType.Item ? itemDetail.Id : null,
                                            SetItemId = itemDetail.ItemType == ItemType.SetItem ? itemDetail.Id : null,
                                            ItemType = itemDetail.ItemType,
                                            ItemDetailId = itemDetail.ItemType == ItemType.Item ? detailId : null, // ✅ only for ItemType.Item
                                            Price = labelAndPrice.Price
                                        });
                                    }
                                }
                                else
                                {

                                    if (itemDetail.Price is null)
                                    {
                                        await _uiMessageService.Error($"Price missing for one or more item variants in '{itemDetail.Name}'.");
                                        await Loading.Hide();
                                        return;
                                    }
                                    itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                                    {
                                        SortOrder = j++,
                                        ItemId = itemDetail.ItemType == ItemType.Item ? itemDetail.Id : null,
                                        SetItemId = itemDetail.ItemType == ItemType.SetItem ? itemDetail.Id : null,
                                        ItemType = itemDetail.ItemType,

                                        Price = itemDetail.Price.Value
                                    });
                                }
                            }
                            else
                            {
                                // Existing logic for IndexAnchor and others
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

                    CreateGroupBuyDto.ItemGroups.Add(itemGroup);
                }

                if (item.GroupBuyModuleType is GroupBuyModuleType.CarouselImages ||
                    item.GroupBuyModuleType is GroupBuyModuleType.BannerImages ||
                    item.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview ||
                    item.GroupBuyModuleType is GroupBuyModuleType.CountdownTimer ||
                    item.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction ||
                    item.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
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

                    CreateGroupBuyDto.ItemGroups.Add(itemGroup);
                }

                if (item.GroupBuyModuleType == GroupBuyModuleType.CustomTextModule 
                    || item.GroupBuyModuleType == GroupBuyModuleType.PartnershipModule
                    || item.GroupBuyModuleType == GroupBuyModuleType.VideoUpload)
                {
                    GroupBuyItemGroupCreateUpdateDto itemGroup = new()
                    {
                        SortOrder = item.SortOrder,
                        GroupBuyModuleType = item.GroupBuyModuleType,
                        Title = item.Title,
                        Text = item.Text,
                        Url = item.Url,
                        ImageModules = []
                    };

                    foreach(var imageModule in item.ImageModules)
                    {
                        itemGroup.ImageModules.Add(new()
                        {
                            Images = imageModule.Images
                        });
                    }

                    CreateGroupBuyDto.ItemGroups.Add(itemGroup);
                }
            }

            GroupBuyDto result = await _groupBuyAppService.CreateAsync(CreateGroupBuyDto);

            List<List<List<CreateImageDto>>> imageModules = [CarouselModules, BannerModules];

            IEnumerable<CreateImageDto> allImages = imageModules.SelectMany(module => module.SelectMany(images => images.Where(w => !w.ImageUrl.IsNullOrEmpty() && !w.BlobImageName.IsNullOrEmpty())));

            foreach (CreateImageDto image in allImages)
            {
                image.TargetId = result.Id;

                await _imageAppService.CreateAsync(image);
            }

            if (GroupPurchaseOverviewModules is { Count: > 0 })
            {
                foreach (GroupPurchaseOverviewDto groupPurchaseOverview in GroupPurchaseOverviewModules)
                {
                    groupPurchaseOverview.GroupBuyId = result.Id;

                    await _GroupPurchaseOverviewAppService.CreateGroupPurchaseOverviewAsync(groupPurchaseOverview);
                }
            }

            if (GroupBuyOrderInstructionModules is { Count: > 0 })
            {
                foreach (GroupBuyOrderInstructionDto groupBuyOrderInstruction in GroupBuyOrderInstructionModules)
                {
                    groupBuyOrderInstruction.GroupBuyId = result.Id;

                    await _GroupBuyOrderInstructionAppService.CreateGroupBuyOrderInstructionAsync(groupBuyOrderInstruction);
                }
            }

            if (ProductRankingCarouselModules is { Count: > 0 })
            {
                foreach (ProductRankingCarouselModule productRankingCarouselModule in ProductRankingCarouselModules)
                {
                    foreach (CreateImageDto image in productRankingCarouselModule.Images)
                    {
                        image.TargetId = result.Id;

                        await _imageAppService.CreateAsync(image);
                    }

                    await _GroupBuyProductRankingAppService.CreateGroupBuyProductRankingAsync(new()
                    {
                        GroupBuyId = result.Id,
                        Title = productRankingCarouselModule.Title,
                        SubTitle = productRankingCarouselModule.SubTitle,
                        Content = productRankingCarouselModule.Content,
                        ModuleNumber = ProductRankingCarouselModules.IndexOf(productRankingCarouselModule) + 1
                    });
                }
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
            }
            var isDisableShipping = CollapseItem.Where(x => x.GroupBuyModuleType == GroupBuyModuleType.ProductGroupModule).ToList();
            HasDifferentItemTemperatures = isDisableShipping
       .SelectMany(x => x.Selected)
       .Where(x =>
           (x.ItemType == ItemType.Item && x.Item != null) ||
           (x.ItemType == ItemType.SetItem && x.SetItem != null))
       .Select(x => x.ItemType == ItemType.Item
           ? x.Item.ItemStorageTemperature
           : x.SetItem.ItemStorageTemperature)
       .Where(temp => temp != null) // Filter out null temperatures
       .Distinct()
       .Count() > 1;
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }
    private void UpdatePrice(Guid detailId, ItemWithItemTypeDto selectedItem, double price)
    {
        if (selectedItem.ItemDetailsWithPrices.ContainsKey(detailId))
        {
            selectedItem.ItemDetailsWithPrices[detailId] = (selectedItem.ItemDetailsWithPrices[detailId].Label, (float)price);
        }
    }
    private void OnSelectedItemDetailsChanged(IEnumerable<Guid> selectedValues, ItemWithItemTypeDto selectedItem)
    {
        var itemDetails = selectedItem.Item?.ItemDetails;

        // Remove unselected items
        var removed = selectedItem.ItemDetailsWithPrices.Keys.Except(selectedValues).ToList();
        foreach (var key in removed)
        {
            selectedItem.ItemDetailsWithPrices.Remove(key);
        }

        // Add newly selected items
        foreach (var id in selectedValues)
        {
            if (!selectedItem.ItemDetailsWithPrices.ContainsKey(id))
            {
                var itemDetail = itemDetails?.FirstOrDefault(x => x.Id == id);
                if (itemDetail != null)
                {
                    var label = itemDetail.Attribute1Value;

                    if (!string.IsNullOrWhiteSpace(itemDetail.Attribute2Value))
                        label += " / " + itemDetail.Attribute2Value;

                    if (!string.IsNullOrWhiteSpace(itemDetail.Attribute3Value))
                        label += " / " + itemDetail.Attribute3Value;

                    selectedItem.ItemDetailsWithPrices[id] = (label, 0); // default price
                }
            }
        }

        selectedItem.SelectedItemDetailIds = selectedValues.ToList();
        StateHasChanged();
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

    private void RemoveCollapseItem(int index)
    {
        CollapseItem? collapseItem = CollapseItem.FirstOrDefault(f => f.Index == index);

        int moduleNumber = collapseItem.ModuleNumber.HasValue ? collapseItem.ModuleNumber.Value : 0;

        if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
        {
            CarouselFilePickers.RemoveAt(moduleNumber - 1);

            CarouselModules.RemoveAll(r => r.Any(w => w.ModuleNumber == moduleNumber));
        }

        else if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
        {
            BannerFilePickers.RemoveAt(moduleNumber - 1);

            BannerModules.RemoveAll(r => r.Any(w => w.ModuleNumber == moduleNumber));
        }
        if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction)
        {
            GroupBuyOrderInstructionModules = new List<GroupBuyOrderInstructionDto>();

        }
        CollapseItem.Remove(collapseItem);

        if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.CarouselImages ||
            collapseItem.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
            ReindexingCollapseItem(moduleNumber, collapseItem.GroupBuyModuleType);
    }

    private void ReindexingCollapseItem(int moduleNumber, GroupBuyModuleType groupBuyModuleType)
    {
        foreach (CollapseItem collapseItem in CollapseItem.Where(w => w.GroupBuyModuleType == groupBuyModuleType && w.ModuleNumber > moduleNumber).ToList())
        {
            int oldModuleNumber = (int)collapseItem.ModuleNumber!;

            collapseItem.ModuleNumber = collapseItem.ModuleNumber - 1;

            if (groupBuyModuleType is GroupBuyModuleType.CarouselImages)
            {
                foreach (List<CreateImageDto> images in CarouselModules.Select(s => s.Where(w => w.ModuleNumber == oldModuleNumber && s.Count > 0).ToList()).ToList())
                {
                    foreach (CreateImageDto image in images)
                    {
                        image.ModuleNumber = image.ModuleNumber - 1;
                    }
                }
            }

            else if (groupBuyModuleType is GroupBuyModuleType.BannerImages)
            {
                foreach (List<CreateImageDto> images in BannerModules.Select(s => s.Where(w => w.ModuleNumber == oldModuleNumber && s.Count > 0).ToList()).ToList())
                {
                    foreach (CreateImageDto image in images)
                    {
                        image.ModuleNumber = image.ModuleNumber - 1;
                    }
                }
            }

        }
    }
    private void BackToGroupBuyList()
    {
        NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
    }
    void StartDrag(CollapseItem item)
    {
        CurrentIndex = CollapseItem.IndexOf(item);
    }

    void Drop(CollapseItem item)
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
            StateHasChanged();
        }
    }

    async Task CollapseToggled()
    {
        await JSRuntime.InvokeVoidAsync("updateDropText");
    }
    #endregion
}

public class CollapseItem
{
    public Guid? Id { get; set; }
    public int Index { get; set; }
    public GroupBuyModuleType GroupBuyModuleType { get; set; }
    public int SortOrder { get; set; }
    public List<ItemWithItemTypeDto> Selected { get; set; }
    public List<List<CreateImageDto>> CarouselModule { get; set; }
    public bool IsModified = false;
    public bool IsWarnedForInCompatible = false;
    public string? AdditionalInfo { get; set; }
    public string? ProductGroupModuleTitle { get; set; }
    public string? ProductGroupModuleImageSize { get; set; }
    public int? ModuleNumber { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }
    public List<MultiImageModuleItem> ImageModules { get; set; } = [];
    public CollapseItem()
    {
        Selected = [];
    }
}

public class MultiImageModuleItem
{
    public Guid? Id { get; set; }
    public FilePicker FilePicker { get; set; }
    public List<GroupBuyItemGroupImageDto> Images { get; set; } = [];
}

public class ProductRankingCarouselModule
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Content { get; set; }
    public int? ModuleNumber { get; set; }
    public List<ItemWithItemTypeDto> Selected { get; set; } = [];
    public List<CreateImageDto> Images { get; set; } = [];
}
