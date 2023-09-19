using Blazored.TextEditor;
using Blazorise;
using Blazorise.Components;
using Kooco.Pikachu.AzureStorage.Image;
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

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class CreateGroupBuy
    {
        bool paymentMethodCheck = false;
        private const int maxtextCount = 60;
        private const int MaxAllowedFilesPerUpload = 5;
        private const int TotalMaxAllowedFiles = 5;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private GroupBuyCreateDto CreateGroupBuyDto = new();
        public List<CreateImageDto> CarouselImages { get; set; }
        private string TagInputValue { get; set; }

        //private Input<string> inputTagRef; //used for create tag input
        private List<string> ItemTags { get; set; } = new List<string>(); //used for store item tags 
        private string? SelectedAutoCompleteText { get; set; }
        private List<KeyValueDto> ItemsList { get; set; } = new();
        private bool IsAllSelected { get; set; } = false;
        //private Input<string> inputPaymentMethodTagRef; //used for create tag input
        private List<string> PaymentMethodTags { get; set; } = new List<string>(); //used for store item tags 
        private string PaymentTagInputValue { get; set; }
        bool loading = false;
        private List<CollapseItem> CollapseItem = new List<CollapseItem>();
        int _value = 1;
        string logoBlobName;
        string bannerBlobName;
        private BlazoredTextEditor NotifyEmailHtml { get; set; }
        private BlazoredTextEditor GroupBuyHtml { get; set; }
        private BlazoredTextEditor CustomerInformationHtml { get; set; }
        private BlazoredTextEditor ExchangePolicyHtml { get; set; }

        public string _ProductPicture = "Product Picture";
        private readonly IImageBlobService _imageBlobService;
        private FilePicker LogoPickerCustom { get; set; }
        private FilePicker BannerPickerCustom { get; set; }
        private FilePicker CarouselPickerCustom { get; set; }
        private FilePicker FilePicker { get; set; }
        private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
        private readonly HttpClient _httpClient;
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IImageAppService _imageAppService;
        private readonly IItemAppService _itemAppService;
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };
        public CreateGroupBuy(IImageBlobService imageBlobService, HttpClient httpClient, IGroupBuyAppService groupBuyAppService,
            IImageAppService imageAppService, IUiMessageService uiMessageService, ImageContainerManager imageContainerManager, IItemAppService itemAppService)
        {
            _imageBlobService = imageBlobService;
            _httpClient = httpClient;
            _groupBuyAppService = groupBuyAppService;
            _imageAppService = imageAppService;
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;

            CarouselImages = new List<CreateImageDto>();
            _itemAppService = itemAppService;
        }
        protected override async Task OnInitializedAsync()
        {
            ItemsList = await _itemAppService.GetItemsLookupAsync();
        }

        async Task OnLogoUploadAsync(FileChangedEventArgs e)
        {
            if (e.Files.Count() > 1)
            {
                await _uiMessageService.Error("Select Only 1 Logo Upload");
                await LogoPickerCustom.Clear();
                return;
            }
            if (e.Files.Count() == 0)
            {

                return;

            }
            var count = 0;
            try
            {
                if (e.Files[0].Size > MaxAllowedFileSize)
                {

                    await LogoPickerCustom.RemoveFile(e.Files[0]);
                    return;
                }
                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(e.Files[0].Name));
                var stream = e.Files[0].OpenReadStream();
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

        async Task OnCarouselUploadAsync(FileChangedEventArgs e)
        {
            if (e.Files.Count() > MaxAllowedFilesPerUpload)
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
                    if (file.Size > MaxAllowedFileSize)
                    {
                        count++;
                        await CarouselPickerCustom.RemoveFile(file);
                        return;
                    }
                    string newFileName = Path.ChangeExtension(
                          Guid.NewGuid().ToString().Replace("-", ""),
                          Path.GetExtension(file.Name));
                    var stream = file.OpenReadStream();
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
            if (e.Files.Count() > 1)
            {
                await _uiMessageService.Error("Select Only 1 Logo Upload");
                await BannerPickerCustom.Clear();
                return;
            }
            if (e.Files.Count() == 0)
            {
                return;
            }
            var count = 0;
            try
            {

                if (e.Files[0].Size > MaxAllowedFileSize)
                {

                    await BannerPickerCustom.RemoveFile(e.Files[0]);
                    return;
                }
                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(e.Files[0].Name));
                var stream = e.Files[0].OpenReadStream();
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

        async Task DeleteLogoAsync(string blobImageName)
        {
            try
            {
                var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
                if (confirmed)
                {
                    confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                    if (confirmed)
                    {
                        CreateGroupBuyDto.LogoURL = null;
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
                    confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                    if (confirmed)
                    {
                        CreateGroupBuyDto.BannerURL = null;
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

        async Task DeleteImageAsync(CollapseItem collapse)
        {
            try
            {
                var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
                if (confirmed)
                {
                    confirmed = await _imageContainerManager.DeleteAsync(collapse.SelectedImage.BlobImageName);
                    collapse.SelectedImage = new();
                }
                else
                {
                    throw new BusinessException(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
            }
        }

        void AddProductItem(string title)
        {
            CollapseItem item = new()
            {
                Title = title,
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1
            };
            CollapseItem.Add(item);
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


        protected virtual async Task CreateEntityAsync()
        {
            try
            {
                if (CreateGroupBuyDto.GroupBuyName.IsNullOrWhiteSpace())
                {
                    await _uiMessageService.Warn(L[PikachuDomainErrorCodes.GroupBuyNameCannotBeNull]);
                    return;
                }
                CreateGroupBuyDto.GroupBuyNo = 0;
                CreateGroupBuyDto.Status = "New";
                CreateGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
                CreateGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
                CreateGroupBuyDto.NotifyMessage = await NotifyEmailHtml.GetHTML();
                CreateGroupBuyDto.GroupBuyConditionDescription = await GroupBuyHtml.GetHTML();
                CreateGroupBuyDto.ExchangePolicyDescription = await ExchangePolicyHtml.GetHTML();
                CreateGroupBuyDto.CustomerInformationDescription = await CustomerInformationHtml.GetHTML();

                CreateGroupBuyDto.ItemGroups = new List<GroupBuyItemGroupCreateUpdateDto>();

                int i = 1;
                foreach (var item in CollapseItem)
                {
                    int j = 1;
                    if (item.ItemDetails.Any())
                    {
                        var itemGroup = new GroupBuyItemGroupCreateUpdateDto
                        {
                            SortOrder = i++,
                            Title = item.Title
                        };

                        foreach (var itemDetail in item.ItemDetails)
                        {
                            itemGroup.ItemDetails.Add(new GroupBuyItemGroupDetailCreateUpdateDto
                            {
                                SortOrder = j++,
                                ItemDescription = itemDetail.ItemDescription,
                                ItemId = itemDetail.ItemId,
                                Image = new CreateImageDto(itemDetail.Image.ImageName, itemDetail.Image.BlobImageName, itemDetail.Image.ImageUrl, ImageType.GroupBuyItemGroup)
                            });
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
        private void IsDefaultPaymentChange(bool isChecked)
        {
            if (isChecked)
            {
                paymentMethodCheck = false; // Uncheck the second radio button
            }
        }

        private void PaymentMethodChange(bool isChecked)
        {
            if (isChecked)
            {
                CreateGroupBuyDto.IsDefaultPaymentGateWay = isChecked; // Uncheck the first radio button
            }
        }

        async Task OnFileUploadAsync(FileChangedEventArgs e, CollapseItem collapseItem)
        {
            try
            {
                foreach (var file in e.Files)
                {
                    if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name).ToLower()))
                    {
                        await FilePicker.RemoveFile(file);
                        return;
                    }
                    if (file.Size > MaxAllowedFileSize)
                    {
                        await FilePicker.RemoveFile(file);
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

                        collapseItem.SelectedImage = new ProductImage(file.Name, newFileName, url);

                        await FilePicker.Clear();
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
            }
        }
        private async void OnSelectedValueChanged(Guid? id, CollapseItem collapseItem)
        {
            try
            {
                if (id != null)
                {
                    collapseItem.SelectedItemId = id.Value;
                    collapseItem.SelectedItem = await _itemAppService.GetAsync(id.Value);

                    StateHasChanged();
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
    }

    public class CollapseItem
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public bool IsProductDescription { get; set; }

        public Guid? SelectedItemId { get; set; }
        public ProductImage SelectedImage { get; set; }
        public string? SelectedItemDescription { get; set; }
        public ItemDto SelectedItem { get; set; }
        public List<ProductPictureItem> ItemDetails { get; set; }
        public CollapseItem()
        {
            ItemDetails = new List<ProductPictureItem>();
        }
    }

    public class ProductPictureItem
    {
        public Guid? ItemId { get; set; }
        public string? ItemDescription { get; set; }
        public ProductImage Image { get; set; }
        public ItemDto Item { get; set; }
        public ProductPictureItem()
        {
            Image = new();
            Item = new();
        }
    }

    public class ProductImage
    {
        public ProductImage()
        {
        }
        public ProductImage(
            string name,
            string blobImageName,
            string url
            )
        {
            ImageName = name;
            BlobImageName = blobImageName;
            ImageUrl = url;
        }
        public string? ImageName { get; set; }
        public string? BlobImageName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
