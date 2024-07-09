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
namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class EditGroupBuy
{
    #region Inject
    [Parameter]
    public string id { get; set; }
    public Guid Id { get; set; }

    private const int MaxtextCount = 60;

    private GroupBuyDto GroupBuy { get; set; }
    private GroupBuyUpdateDto EditGroupBuyDto { get; set; }
    private List<CreateImageDto> CarouselImages { get; set; }
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
    private List<ItemWithItemTypeDto> ItemsList { get; set; } = [];
    private readonly ImageContainerManager _imageContainerManager;
    private readonly List<string> ValidFileExtensions = [".jpg", ".png", ".svg", ".jpeg", ".webp"];
    public readonly List<string> ValidPaymentMethods = ["ALL", "Credit", "WebATM", "ATM", "CVS", "BARCODE", "Alipay", "Tenpay", "TopUpUsed", "GooglePay"];
    private string? PaymentMethodError { get; set; } = null;
    private List<ImageDto> ExistingImages { get; set; } = [];
    private LoadingIndicator Loading { get; set; } = new();
    private bool LoadingItems { get; set; } = true;
    private int CurrentIndex { get; set; }

    public bool IsUnableToSpecifyDuringPeakPeriodsForSelfPickups = false;

    public bool IsUnableToSpecifyDuringPeakPeriodsForHomeDelivery = false;

    public bool IsUnableToSpecifyDuringPeakPeriodsForDeliveredByStore = false;
    #endregion

    #region Constructor
    public EditGroupBuy(
        IGroupBuyAppService groupBuyAppService,
        IImageAppService imageAppService,
        IObjectMapper objectMapper,
        IUiMessageService uiMessageService,
        ImageContainerManager imageContainerManager,
        IItemAppService itemAppService,
         ISetItemAppService setItemAppService
        )
    {
        _groupBuyAppService = groupBuyAppService;
        _imageAppService = imageAppService;
        _objectMapper = objectMapper;
        _uiMessageService = uiMessageService;
        _imageContainerManager = imageContainerManager;
        _itemAppService = itemAppService;
        _setItemAppService = setItemAppService;

        EditGroupBuyDto = new GroupBuyUpdateDto();
        CarouselImages = [];
        GroupBuy = new();
        Loading=new LoadingIndicator();
        id ??= "";
    }
    #endregion

    #region Methods
    protected override async Task OnInitializedAsync()
    {
        try
        {
            //await Loading.Show();
            Id = Guid.Parse(id);
            GroupBuy = await _groupBuyAppService.GetWithItemGroupsAsync(Id);
            EditGroupBuyDto = _objectMapper.Map<GroupBuyDto, GroupBuyUpdateDto>(GroupBuy);
            //await LoadHtmlContent();
            EditGroupBuyDto.ShortCode=EditGroupBuyDto.ShortCode==""?null:EditGroupBuyDto.ShortCode;
            if (!string.IsNullOrEmpty(GroupBuy.ExcludeShippingMethod))
            {
                EditGroupBuyDto.ShippingMethodList = JsonSerializer.Deserialize<List<string>>(GroupBuy.ExcludeShippingMethod);
            }
            if (!string.IsNullOrEmpty(GroupBuy.SelfPickupDeliveryTime))
            {
                SelfPickupTimeList = JsonSerializer.Deserialize<List<string>>(GroupBuy.SelfPickupDeliveryTime);
            }
            if (!string.IsNullOrEmpty(GroupBuy.BlackCatDeliveryTime))
            {
                BlackCateDeliveryTimeList = JsonSerializer.Deserialize<List<string>>(GroupBuy.BlackCatDeliveryTime);
            }
            if (!string.IsNullOrEmpty(GroupBuy.HomeDeliveryDeliveryTime))
            {
                HomeDeliveryTimeList = JsonSerializer.Deserialize<List<string>>(GroupBuy.HomeDeliveryDeliveryTime);
            }
            if (EditGroupBuyDto.FreeShipping && EditGroupBuyDto.FreeShippingThreshold == null)
            {
                EditGroupBuyDto.FreeShippingThreshold = 0;
               

            }
            if (!GroupBuy.PaymentMethod.IsNullOrEmpty())
            {
                var payments = GroupBuy.PaymentMethod.Split(",");
                if (payments.Length > 1)
                {
                    CreditCard = payments[0].Trim() == "Credit Card" ? true : false;
                    BankTransfer = payments[0].Trim() == "Credit Card" ? true : false;

                }
                else if (GroupBuy.PaymentMethod == "Credit Card")
                {

                    CreditCard = true;

                }
                else if (GroupBuy.PaymentMethod == "Bank Transfer")
                {

                    BankTransfer = true;

                }
            }
            
            EditGroupBuyDto.EntryURL = $"{_configuration["EntryUrl"]?.TrimEnd('/')}/{Id}";
            LoadItemGroups();
            if (EditValidationsRef != null)
            {
                await EditValidationsRef.ClearAll();
            }
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
                CarouselImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingImages);

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
                    var collapseItem = new CollapseItem
                    {
                        Id = itemGroup.Id,
                        Index = i++,
                        SortOrder = itemGroup.SortOrder,
                        GroupBuyModuleType = itemGroup.GroupBuyModuleType,
                        Selected = []
                    };

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

                    if (itemGroup.GroupBuyModuleType != GroupBuyModuleType.ProductDescriptionModule
                        && itemGroup.GroupBuyModuleType != GroupBuyModuleType.IndexAnchor
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
        CollapseItem collapseItem;

        if (CollapseItem.Count >= 20)
        {
            _uiMessageService.Error(L[PikachuDomainErrorCodes.CanNotAddMoreThan20Modules]);
            return;
        }

        if (groupBuyModuleType == GroupBuyModuleType.ProductDescriptionModule
            || groupBuyModuleType == GroupBuyModuleType.IndexAnchor)
        {
            collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected =
                [
                    new ItemWithItemTypeDto()
                ]
            };
        }

        else
        {
            collapseItem = new()
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
        CollapseItem.Add(collapseItem);
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

    async Task OnCarouselUploadAsync(FileChangedEventArgs e)
    {
        if (CarouselImages.Count >= 5)
        {
            await CarouselPickerCustom.Clear();
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
        var count = 0;
        try
        {
            foreach (var file in e.Files.Take(5))
            {
                if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name)))
                {
                    await CarouselPickerCustom.RemoveFile(file);
                    await _uiMessageService.Error(L["InvalidFileType"]);
                    continue;
                }
                if (file.Size > MaxAllowedFileSize)
                {
                    count++;
                    await CarouselPickerCustom.RemoveFile(file);
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

                    int sortNo = CarouselImages.LastOrDefault()?.SortNo ?? 0;

                    CarouselImages.Add(new CreateImageDto
                    {
                        Name = file.Name,
                        BlobImageName = newFileName,
                        ImageUrl = url,
                        ImageType = ImageType.GroupBuyCarouselImage,
                        SortNo = sortNo + 1
                    });

                    await CarouselPickerCustom.Clear();
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
        var value = (bool)(e?.Value ?? false);

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
    async Task DeleteImageAsync(string blobImageName)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                await Loading.Show();
                await _imageContainerManager.DeleteAsync(blobImageName);
                if (ExistingImages.Any(x => x.BlobImageName == blobImageName))
                {
                    var image = ExistingImages.Where(x => x.BlobImageName == blobImageName).FirstOrDefault();
                    await _imageAppService.DeleteAsync(image.Id);
                }
                CarouselImages = CarouselImages.Where(x => x.BlobImageName != blobImageName).ToList();
                ExistingImages = ExistingImages.Where(x => x.BlobImageName != blobImageName).ToList();
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
            else
            {

                EditGroupBuyDto.ShortCode = "";
            }
            //if (ItemTags.Any())
            //{
            //    EditGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
            //}
            //if (PaymentMethodTags.Any())
            //{
            //    EditGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
            //}
            if (CreditCard && BankTransfer)
            {

                EditGroupBuyDto.PaymentMethod = "Credit Card , Bank Transfer";


            }
            else if (CreditCard)
            {

                EditGroupBuyDto.PaymentMethod = "Credit Card";


            }
            else if (BankTransfer)
            {

                EditGroupBuyDto.PaymentMethod = "Bank Transfer";


            }
            else {

                EditGroupBuyDto.PaymentMethod = "";
            }
            if (EditGroupBuyDto.PaymentMethod.IsNullOrEmpty())
            {
                await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOnePaymentMethodIsRequired]);
                await Loading.Hide();
                return;
            }
            if (EditGroupBuyDto.ExcludeShippingMethod.IsNullOrEmpty()|| EditGroupBuyDto.ExcludeShippingMethod=="[]")
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
                ||EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFreeze")|| EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCatFrozen")
                || EditGroupBuyDto.ExcludeShippingMethod.Contains("SelfPickup") || EditGroupBuyDto.ExcludeShippingMethod.Contains("HomeDelivery")
                || EditGroupBuyDto.ExcludeShippingMethod.Contains("DeliveredByStore")))
            {
                if (EditGroupBuyDto.ExcludeShippingMethod.Contains("BlackCat1") && (EditGroupBuyDto.BlackCatDeliveryTime.IsNullOrEmpty()|| EditGroupBuyDto.BlackCatDeliveryTime=="[]"))
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
                else if (EditGroupBuyDto.ExcludeShippingMethod.Contains("SelfPickup") && (EditGroupBuyDto.SelfPickupDeliveryTime.IsNullOrEmpty()|| EditGroupBuyDto.SelfPickupDeliveryTime=="[]"))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.AtLeastOneDeliveryTimeIsRequiredForSelfPickup]);
                    await Loading.Hide();
                    return;
                }
                else if (EditGroupBuyDto.ExcludeShippingMethod.Contains("HomeDelivery") && (EditGroupBuyDto.HomeDeliveryDeliveryTime.IsNullOrEmpty()|| EditGroupBuyDto.HomeDeliveryDeliveryTime=="[]"))
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
            EditGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
            EditGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();
            EditGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
            EditGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

            EditGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

            foreach (var item in CollapseItem)
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
                        GroupBuyModuleType = item.GroupBuyModuleType
                    };

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

                    EditGroupBuyDto.ItemGroups.Add(itemGroup);
                }
            }
            await Loading.Show();

            GroupBuyDto result = await _groupBuyAppService.UpdateAsync(Id, EditGroupBuyDto);

            if (EditGroupBuyDto.IsEnterprise)
            {
                await _OrderAppService.UpdateOrdersIfIsEnterpricePurchaseAsync(Id);
            }

            foreach (var item in CarouselImages)
            {
                if (!ExistingImages.Any(x => x.BlobImageName == item.BlobImageName))
                {
                    item.TargetId = Id;
                    await _imageAppService.CreateAsync(item);
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
