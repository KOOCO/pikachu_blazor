using Blazored.TextEditor;
using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class CreateGroupBuy
    {
        private const int maxtextCount = 60;
        private const int MaxAllowedFilesPerUpload = 5;
        private const int TotalMaxAllowedFiles = 5;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;

        private GroupBuyCreateDto CreateGroupBuyDto = new();
        public List<CreateImageDto> CarouselImages { get; set; }
        private string TagInputValue { get; set; }

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
        public string _ProductPicture = "Product Picture";
        private FilePicker LogoPickerCustom { get; set; }
        private FilePicker BannerPickerCustom { get; set; }
        private FilePicker CarouselPickerCustom { get; set; }
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
        public CreateGroupBuy(
            IGroupBuyAppService groupBuyAppService,
            IImageAppService imageAppService,
            IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IItemAppService itemAppService,
            ISetItemAppService setItemAppService
            )
        {
            _groupBuyAppService = groupBuyAppService;
            _imageAppService = imageAppService;
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;

            CarouselImages = new List<CreateImageDto>();
            _itemAppService = itemAppService;
            _setItemAppService = setItemAppService;

        }
        protected override async Task OnInitializedAsync()
        {
            CreateGroupBuyDto.EntryURL = _configuration["EntryUrl"]?.TrimEnd('/');
            SetItemList = await _setItemAppService.GetItemsLookupAsync();
            ItemsList = await _itemAppService.GetItemsLookupAsync();
            ItemsList.AddRange(SetItemList);
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
        async Task DeleteImageAsync(string blobImageName)
        {
            try
            {
                var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
                if (confirmed)
                {
                    await _imageContainerManager.DeleteAsync(blobImageName);
                    CarouselImages = CarouselImages.Where(x => x.BlobImageName != blobImageName).ToList();
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

            CollapseItem collapseItem;
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
        private bool IsItemDisabled(string item)
        {
            if (CreateGroupBuyDto.AllowShipToOuterTaiwan)
            {
                if (item == Enum.GetName(DeliveryMethod.SevenToElevenC2C) || item == Enum.GetName(DeliveryMethod.BlackCat) || item == Enum.GetName(DeliveryMethod.SevenToEleven))
                {

                    if (item == Enum.GetName(DeliveryMethod.SevenToElevenC2C))
                    {

                        return ItemTags != null && ItemTags.Any(x => x == Enum.GetName(DeliveryMethod.SevenToEleven)) ? true : false;

                    }
                    else if (item == Enum.GetName(DeliveryMethod.SevenToEleven))
                    {

                        return ItemTags != null && ItemTags.Any(x => x == Enum.GetName(DeliveryMethod.SevenToElevenC2C)) ? true : false;

                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {

                    return true;
                }

            }
            else
            {
                if (item == Enum.GetName(DeliveryMethod.SevenToElevenC2C))
                {

                    return ItemTags != null && ItemTags.Any(x => x == Enum.GetName(DeliveryMethod.SevenToEleven)) ? true : false;

                }
                else if (item == Enum.GetName(DeliveryMethod.SevenToEleven))
                {

                    return ItemTags != null && ItemTags.Any(x => x == Enum.GetName(DeliveryMethod.SevenToElevenC2C)) ? true : false;

                }
                else if (item == Enum.GetName(DeliveryMethod.FamilyMart))
                {


                    return ItemTags != null && ItemTags.Any(x => x == Enum.GetName(DeliveryMethod.FamilyMartC2C)) ? true : false;


                }
                else if (item == Enum.GetName(DeliveryMethod.FamilyMartC2C))
                {

                    return ItemTags != null && ItemTags.Any(x => x == Enum.GetName(DeliveryMethod.FamilyMart)) ? true : false;


                }
                else
                {
                    return false;
                }
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

        protected virtual async Task CreateEntityAsync()
        {
            try
            {
                if (CreateGroupBuyDto.GroupBuyName.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyNameCannotBeNull]);
                    return;
                }
                if (CreateGroupBuyDto.ShortCode.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyShortCodeCannotBeNull]);
                    return;
                }

                string shortCode = CreateGroupBuyDto.ShortCode;
                string pattern = @"^[A-Za-z0-9]{4,12}$";

                bool isPatternValid = Regex.IsMatch(shortCode, pattern);

                if (!isPatternValid)
                {
                    await _uiMessageService.Warn(L["ShortCodePatternDoesnotMatch"]);
                    return;
                }
                var check = await _groupBuyAppService.CheckShortCodeForCreate(CreateGroupBuyDto.ShortCode);

                if (check)
                {
                    await _uiMessageService.Warn(L["Short Code Alredy Exist"]);
                    return;
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
                if (CreditCard && BankTransfer)
                {

                    CreateGroupBuyDto.PaymentMethod = "Credit Card , Bank Transfer";


                }
                else if(CreditCard) {

                    CreateGroupBuyDto.PaymentMethod = "Credit Card";


                }
                else if (BankTransfer)
                {

                    CreateGroupBuyDto.PaymentMethod = "Bank Transfer";


                }
                CreateGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
                CreateGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();
                CreateGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
                CreateGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

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
                        var itemGroup = new GroupBuyItemGroupCreateUpdateDto
                        {
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

                        CreateGroupBuyDto.ItemGroups.Add(itemGroup);
                    }
                }

                var result = await _groupBuyAppService.CreateAsync(CreateGroupBuyDto);
                foreach (var item in CarouselImages)
                {
                    item.TargetId = result.Id;
                    await _imageAppService.CreateAsync(item);
                }

                NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(L[ex.Code]);
            }
            catch (Exception ex)
            {
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

        private void RemoveCollapseItem(int index)
        {
            var item = CollapseItem.Where(i => i.Index == index).FirstOrDefault();
            CollapseItem.Remove(item);
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
    }

    public class CollapseItem
    {
        public Guid? Id { get; set; }
        public int Index { get; set; }
        public GroupBuyModuleType GroupBuyModuleType { get; set; }
        public int SortOrder { get; set; }
        public List<ItemWithItemTypeDto> Selected { get; set; }
        public bool IsModified = false;
        public CollapseItem()
        {
            Selected = [];
        }
    }
}
