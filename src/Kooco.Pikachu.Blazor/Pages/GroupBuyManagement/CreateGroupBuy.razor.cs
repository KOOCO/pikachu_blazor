using Blazored.TextEditor;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.EnumValues;
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
using Volo.Abp.Domain.Entities;


namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class CreateGroupBuy
{
    #region Inject
    private bool IsRender = false;
    private Modal AddLinkModal { get; set; }
    private CreateImageDto SelectedImageDto = new();
    private const int maxtextCount = 60;

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
    string logoBlobName;
    string bannerBlobName;
    protected Validations EditValidationsRef;
    private BlazoredTextEditor NotifyEmailHtml { get; set; }
    private BlazoredTextEditor GroupBuyHtml { get; set; }
    private BlazoredTextEditor CustomerInformationHtml { get; set; }
    private BlazoredTextEditor ExchangePolicyHtml { get; set; }
    bool CreditCard { get; set; }
    bool BankTransfer { get; set; }
    bool IsCashOnDelivery { get; set; }
    public string _ProductPicture = "Product Picture";
    private FilePicker LogoPickerCustom { get; set; }
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
    private readonly IElectronicInvoiceSettingAppService _electronicInvoiceSettingAppService;

    public List<LogisticsProviderSettingsDto> LogisticsProviders = [];
    public ElectronicInvoiceSettingDto ElectronicInvoiceSetting = new ElectronicInvoiceSettingDto();
    public List<PaymentGatewayDto> PaymentGateways = [];
    private readonly ILogisticsProvidersAppService _LogisticsProvidersAppService;
    private readonly IPaymentGatewayAppService _paymentGatewayAppService;

    private bool IsColorPickerOpen = false;

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
        IElectronicInvoiceSettingAppService electronicInvoiceSettingAppService,
        IPaymentGatewayAppService paymentGatewayAppService
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
    protected override async Task OnInitializedAsync()
    {
        CreateGroupBuyDto.EntryURL = await MyTenantAppService.FindTenantUrlAsync(CurrentTenant.Id);
        SetItemList = await _setItemAppService.GetItemsLookupAsync();
        ItemsList = await _itemAppService.GetItemsLookupAsync();
        ItemsList.AddRange(SetItemList);

        LogisticsProviders = await _LogisticsProvidersAppService.GetAllAsync();
        ElectronicInvoiceSetting = await _electronicInvoiceSettingAppService.GetSettingAsync();
        PaymentGateways = await _paymentGatewayAppService.GetAllAsync();
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
    public bool isPaymentMethodEnabled()
    {
        var ecpay = PaymentGateways.Where(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).FirstOrDefault();
        if (ecpay == null)
        {
            return true;
        }
        else if (!ecpay.IsEnabled) return true;
        else if (ecpay.HashIV.IsNullOrEmpty() || ecpay.HashKey.IsNullOrEmpty() || ecpay.MerchantId.IsNullOrEmpty() || ecpay.TradeDescription.IsNullOrEmpty() || ecpay.CreditCheckCode.IsNullOrEmpty()) return true;
        else
        {
            return false;

        }
    }
    public bool IsInvoiceEnable()
    {
        return ElectronicInvoiceSetting.IsEnable;


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
            GroupBuyModuleType.ProductRankingCarouselModule
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
        string? selectedTheme = e.Value.ToString();

        CreateGroupBuyDto.ColorSchemeType = !selectedTheme.IsNullOrEmpty() ? Enum.Parse<ColorScheme>(selectedTheme) : null;

        IsColorPickerOpen = true;

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
                var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);
                logoBlobName = newFileName;

                CreateGroupBuyDto.LogoURL = url;

                await LogoPickerCustom.Clear();
            }
            catch (Exception ex)
            {

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
                    continue;
                }
                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(file.Name));
                var stream = file.OpenReadStream(long.MaxValue);
                try
                {
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
                                    CarouselStyle = originalCarouselImages.FirstOrDefault(f => f.CarouselStyle != null)?.CarouselStyle ?? null
                                });
                            }
                        }
                    }

                    await carouselPicker.Clear();
                }
                finally
                {
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

            GroupBuyOrderInstructionModules.Add(new());
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

        CreateGroupBuyDto.SelfPickupDeliveryTime = JsonConvert.SerializeObject(SelfPickupTimeList);
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

        CreateGroupBuyDto.BlackCatDeliveryTime = JsonConvert.SerializeObject(BlackCateDeliveryTimeList);

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

        CreateGroupBuyDto.DeliveredByStoreDeliveryTime = JsonConvert.SerializeObject(DeliverdByStoreTimeList);

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

        CreateGroupBuyDto.HomeDeliveryDeliveryTime = JsonConvert.SerializeObject(HomeDeliveryTimeList);
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
            DeliveryMethod.TCatDeliverySevenElevenFrozen.ToString()
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
                JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);
            });
        }

        if (!CreateGroupBuyDto.ShippingMethodList.Contains(DeliveryMethod.DeliveredByStore.ToString()))
        {
            DeliveryTimeConts.DeliveredByStore.ForEach(item =>
            {
                DeliverdByStoreDeliveryTimeCheckedChange(item, new ChangeEventArgs { Value = false }, true);
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
            IsCashOnDelivery = true; CreditCard = false; BankTransfer = false;
            OnShippingMethodCheckedChange("SelfPickup", new ChangeEventArgs { Value = true });
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
            //if (CreateGroupBuyDto.ShortCode.IsNullOrWhiteSpace())
            //{
            //    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyShortCodeCannotBeNull]);
            //    return;
            //}

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
            if (ItemTags.Any())
            {
                CreateGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
            }
            //if (PaymentMethodTags.Any())
            //{
            //    CreateGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
            //}
            if (CreditCard && BankTransfer && IsCashOnDelivery) CreateGroupBuyDto.PaymentMethod = "Credit Card , Bank Transfer , Cash On Delivery";

            else if (CreditCard) CreateGroupBuyDto.PaymentMethod = "Credit Card";

            else if (BankTransfer) CreateGroupBuyDto.PaymentMethod = "Bank Transfer";

            else if (IsCashOnDelivery) CreateGroupBuyDto.PaymentMethod = "Cash On Delivery";

            else CreateGroupBuyDto.PaymentMethod = string.Empty;

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

            CreateGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
            //CreateGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();
            CreateGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
            //CreateGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

            CreateGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

            foreach (var item in CollapseItem)
            {
                if (item.Selected.Any(s => s.Id == Guid.Empty && item.GroupBuyModuleType == GroupBuyModuleType.IndexAnchor && s.Name.IsNullOrEmpty()))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyModuleCannotBeEmpty]);
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
        catch (BusinessException ex)
        {
            await Loading.Hide();
            await _uiMessageService.Error(L[ex.Code]);
        }
        catch (Exception ex)
        {
            await Loading.Hide();
            await _uiMessageService.Error(ex.Message.GetType()?.ToString());
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
    public CollapseItem()
    {
        Selected = [];
    }
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
