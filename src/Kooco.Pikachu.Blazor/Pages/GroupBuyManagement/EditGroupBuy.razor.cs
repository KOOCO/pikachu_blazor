using Blazored.TextEditor;
using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.ImageBlob;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class EditGroupBuy
    {
        [Parameter]
        public string id { get; set; }
        public Guid Id { get; set; }

        private const int MaxtextCount = 60;
        private GroupBuyUpdateDto EditGroupBuyDto { get; set; }
        private List<CreateImageDto> CarouselImages { get; set; }
        private const int MaxAllowedFilesPerUpload = 5;
        private const int TotalMaxAllowedFiles = 5;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private string? TagInputValue { get; set; }
        private List<string> ItemTags { get; set; } = new List<string>(); //used for store item tags 
        private List<string> PaymentMethodTags { get; set; } = new List<string>(); //used for store item tags 
        private string PaymentTagInputValue { get; set; }
        private List<CollapseItem> CollapseItem = new();

        public string _ProductPicture = "Product Picture";
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IImageAppService _imageAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IUiMessageService _uiMessageService;
        private readonly IItemAppService _itemAppService;
        private BlazoredTextEditor GroupBuyHtml { get; set; }

        private BlazoredTextEditor CustomerInformationHtml { get; set; }
        private BlazoredTextEditor ExchangePolicyHtml { get; set; }
        private BlazoredTextEditor NotifyEmailHtml { get; set; }

        private FilePicker LogoPickerCustom { get; set; }
        private FilePicker BannerPickerCustom { get; set; }
        private FilePicker CarouselPickerCustom { get; set; }
        private List<KeyValueDto> ItemsList { get; set; } = new();
        private readonly ImageContainerManager _imageContainerManager;
        private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };

        private List<ImageDto> ExistingImages { get; set; } = new();

        public EditGroupBuy(
            IGroupBuyAppService groupBuyAppService,
            IImageAppService imageAppService, 
            IObjectMapper objectMapper, 
            IUiMessageService uiMessageService, 
            ImageContainerManager imageContainerManager, 
            IItemAppService itemAppService
            )
        {
            _groupBuyAppService = groupBuyAppService;
            _imageAppService = imageAppService;
            _objectMapper = objectMapper;
            EditGroupBuyDto = new GroupBuyUpdateDto();
            CarouselImages = new List<CreateImageDto>();
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            _itemAppService = itemAppService;
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                id ??= "";
                Id = Guid.Parse(id);
                var groupbuy = await _groupBuyAppService.GetWithDetailsAsync(Id);
                EditGroupBuyDto = _objectMapper.Map<GroupBuyDto, GroupBuyUpdateDto>(groupbuy);
                ExistingImages = await _imageAppService.GetGroupBuyImagesAsync(Id, ImageType.GroupBuyCarouselImage);
                List<string> paymentMethotTagArray = EditGroupBuyDto.PaymentMethod?.Split(',')?.ToList() ?? new List<string>();
                PaymentMethodTags = paymentMethotTagArray != null ? paymentMethotTagArray.ToList() : new List<string>();
                List<string> excludeShippingMethodArray = EditGroupBuyDto.ExcludeShippingMethod?.Split(',')?.ToList() ?? new();
                ItemTags = excludeShippingMethodArray != null ? excludeShippingMethodArray.ToList() : new List<string>();
                CarouselImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(ExistingImages);
                ItemsList = await _itemAppService.GetItemsLookupAsync();

                if (!EditGroupBuyDto.GroupBuyConditionDescription.IsNullOrWhiteSpace())
                {
                    await GroupBuyHtml.LoadHTMLContent(EditGroupBuyDto.GroupBuyConditionDescription);
                }

                if (!EditGroupBuyDto.CustomerInformationDescription.IsNullOrWhiteSpace())
                {
                    await CustomerInformationHtml.LoadHTMLContent(EditGroupBuyDto.CustomerInformationDescription);
                }

                if (!EditGroupBuyDto.ExchangePolicyDescription.IsNullOrWhiteSpace())
                {
                    await ExchangePolicyHtml.LoadHTMLContent(EditGroupBuyDto.ExchangePolicyDescription);
                }

                if (!EditGroupBuyDto.NotifyMessage.IsNullOrWhiteSpace())
                {
                    await NotifyEmailHtml.LoadHTMLContent(EditGroupBuyDto.NotifyMessage);
                }

                var itemGroups = groupbuy.ItemGroups;
                if (itemGroups.Any())
                {
                    var i = 0;
                    foreach (var itemGroup in itemGroups)
                    {
                        var collapseItem = new CollapseItem
                        {
                            Index = i++,
                            SortOrder = itemGroup.SortOrder,
                            GroupBuyModuleType = itemGroup.GroupBuyModuleType
                        };
                        if (itemGroup.ItemGroupDetails.Any())
                        {
                            foreach (var item in itemGroup.ItemGroupDetails)
                            {
                                collapseItem.SelectedItems.Add(item.Item);
                            }
                        }

                        CollapseItem.Add(collapseItem);
                    }
                }

                StateHasChanged();
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(L[ex.Code]);
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        void AddProductItem(GroupBuyModuleType groupBuyModuleType)
        {
            CollapseItem collapseItem;
            if (groupBuyModuleType == GroupBuyModuleType.ProductDescription)
            {
                collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType,
                    SelectedItems = new()
                    {
                        new ItemDto()
                    }
                };
            }

            else
            {
                collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType,
                    SelectedItems = new()
                    {
                        new ItemDto(),
                        new ItemDto(),
                        new ItemDto()
                    }
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
                    var memoryStream = new MemoryStream();

                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);
                    EditGroupBuyDto.LogoURL = url;
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
            if (e.Files.Length > MaxAllowedFilesPerUpload)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
                await CarouselPickerCustom.Clear();
                return;
            }
            if (CarouselImages.Count > TotalMaxAllowedFiles)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.AlreadyUploadMaxAllowedFiles]);
                await CarouselPickerCustom.Clear();
                return;
            }
            var count = 0;
            try
            {
                foreach (var file in e.Files)
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

                    EditGroupBuyDto.BannerURL = url;

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
                    await _imageContainerManager.DeleteAsync(blobImageName);
                    if (ExistingImages.Any(x => x.BlobImageName == blobImageName))
                    {
                        var image = ExistingImages.Where(x => x.BlobImageName == blobImageName).FirstOrDefault();
                        await _imageAppService.DeleteAsync(image.Id);
                    }
                    CarouselImages = CarouselImages.Where(x => x.BlobImageName != blobImageName).ToList();
                    ExistingImages = ExistingImages.Where(x => x.BlobImageName != blobImageName).ToList();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
            }
        }

        private static void OnProductGroupValueChange(ChangeEventArgs e, CollapseItem collapseItem)
        {
            int takeCount = int.Parse(e?.Value.ToString());
            if (collapseItem.SelectedItems.Count > takeCount)
            {
                collapseItem.SelectedItems = collapseItem.SelectedItems.Take(takeCount).ToList();
            }
            else
            {
                collapseItem.SelectedItems.Add(new ItemDto());
            }
        }

        private void HandleItemTagInputKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (!TagInputValue.IsNullOrWhiteSpace() && !ItemTags.Any(x => x == TagInputValue))
                {
                    ItemTags.Add(TagInputValue);
                }
                TagInputValue = string.Empty;
            }
        }

        private void HandleItemTagDelete(string item)
        {
            ItemTags.Remove(item);
        }

        private void HandlePaymentTagInputKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (!PaymentTagInputValue.IsNullOrWhiteSpace() && !PaymentMethodTags.Any(x => x == PaymentTagInputValue))
                {
                    PaymentMethodTags.Add(PaymentTagInputValue);
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
                if (EditGroupBuyDto.GroupBuyName.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyNameCannotBeNull]);
                    return;
                }
                if (CollapseItem.Any(c => c.SelectedItems.Any(s => s.Id == Guid.Empty)))
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyModuleCannotBeEmpty]);
                    return;
                }
                if (ItemTags.Any())
                {
                    EditGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
                }
                if (PaymentMethodTags.Any())
                {
                    EditGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
                }

                EditGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
                EditGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();
                EditGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
                EditGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

                EditGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

                int i = 1;
                foreach (var item in CollapseItem)
                {
                    int j = 1;
                    if (item.SelectedItems.Any())
                    {
                        var itemGroup = new GroupBuyItemGroupCreateUpdateDto
                        {
                            SortOrder = i++,
                            GroupBuyModuleType = item.GroupBuyModuleType
                        };

                        foreach (var itemDetail in item.SelectedItems)
                        {
                            itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                            {
                                SortOrder = j++,
                                ItemId = itemDetail.Id
                            });
                        }

                        EditGroupBuyDto.ItemGroups.Add(itemGroup);
                    }
                }

                var result = await _groupBuyAppService.UpdateAsync(Id, EditGroupBuyDto);

                foreach (var item in CarouselImages)
                {
                    if (!ExistingImages.Any(x => x.BlobImageName == item.BlobImageName))
                    {
                        item.TargetId = Id;
                        await _imageAppService.CreateAsync(item);
                    }
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

        private async Task OnSelectedValueChanged(Guid? id, CollapseItem collapseItem, ItemDto? selectedItem = null)
        {
            try
            {
                var index = collapseItem.SelectedItems.IndexOf(selectedItem);
                if (id != null)
                {
                    collapseItem.SelectedItems[index] = await _itemAppService.GetAsync(id.Value, true);
                }
                else
                {
                    collapseItem.SelectedItems[index] = new();
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

        private void RemoveCollapseItem(int index)
        {
            var item = CollapseItem.Where(i => i.Index == index).FirstOrDefault();
            CollapseItem.Remove(item);
        }
    }

}
