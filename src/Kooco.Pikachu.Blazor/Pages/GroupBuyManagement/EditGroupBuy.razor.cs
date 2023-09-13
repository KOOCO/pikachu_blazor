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
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class EditGroupBuy
    {
        [Parameter]
        public string id { get; set; }
        public Guid Id { get; set; }

        bool PaymentMethodCheck = false;
        private const int MaxtextCount = 60;
        private GroupBuyUpdateDto EditGroupBuyDto { get; set; }
        private List<CreateImageDto> CarouselImages { get; set; }
        private const int MaxAllowedFilesPerUpload = 5;
        private const int TotalMaxAllowedFiles = 5;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private string? TagInputValue { get; set; }

        //private Input<string> inputTagRef; //used for create tag input
        private List<string> ItemTags { get; set; } = new List<string>(); //used for store item tags 

        //private Input<string> inputPaymentMethodTagRef; //used for create tag input
        private List<string> PaymentMethodTags { get; set; } = new List<string>(); //used for store item tags 
        private string PaymentTagInputValue { get; set; }
        bool loading = false;
        private List<CollapseItem> CollapseItem = new();
        int _value = 1;
        private Blazored.TextEditor.BlazoredTextEditor quillHtml; //Item Discription Html
        public string _ProductPicture = "Product Picture";
        private readonly IImageBlobService _imageBlobService;
        private readonly HttpClient _httpClient;
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IImageAppService _imageAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IUiMessageService _uiMessageService;
        private readonly IItemAppService _itemAppService;
        private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
        private FilePicker LogoPickerCustom { get; set; }
        private FilePicker BannerPickerCustom { get; set; }
        private FilePicker CarouselPickerCustom { get; set; }
        private List<KeyValueDto> ItemsList { get; set; } = new();
        private FilePicker FilePicker { get; set; }
        private string? SelectedAutoCompleteText { get; set; }
        private readonly ImageContainerManager _imageContainerManager;
        private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };
        public EditGroupBuy(IImageBlobService imageBlobService, HttpClient httpClient, IGroupBuyAppService groupBuyAppService,
            IImageAppService imageAppService, IObjectMapper objectMapper, IUiMessageService uiMessageService, ImageContainerManager imageContainerManager, IItemAppService itemAppService)
        {
            _imageBlobService = imageBlobService;
            _httpClient = httpClient;
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
                var images = await _imageAppService.GetGroupBuyImagesAsync(Id);
                string[] paymentMethotTagArray = EditGroupBuyDto.PaymentMethod != null ? EditGroupBuyDto.PaymentMethod.Split(',') : null;
                PaymentMethodTags = PaymentMethodTags != null ? PaymentMethodTags.ToList() : new List<string>();
                string[] excludeShippingMethodArray = EditGroupBuyDto.ExcludeShippingMethod != null ? EditGroupBuyDto.ExcludeShippingMethod.Split(',') : null;
                ItemTags = excludeShippingMethodArray != null ? excludeShippingMethodArray.ToList() : new List<string>();
                CarouselImages = _objectMapper.Map<List<ImageDto>, List<CreateImageDto>>(images);
                ItemsList = await _itemAppService.GetItemsLookupAsync();

                var itemGroups = groupbuy.ItemGroups;
                if (itemGroups.Any())
                {
                    var i = 0;
                    foreach (var itemGroup in itemGroups)
                    {
                        var collapseItem = new CollapseItem
                        {
                            Index = i++,
                            Title = itemGroup.Title
                        };
                        if (itemGroup.ItemGroupDetails.Any())
                        {

                            foreach (var item in itemGroup.ItemGroupDetails)
                            {
                                collapseItem.ItemDetails.Add(new ProductPictureItem
                                {
                                    ItemId = item.ItemId,
                                    ItemDescription = item.ItemDescription,
                                    Item = item.Item,
                                    Image = new ProductImage(item.Image.Name, item.Image.BlobImageName, item.Image.ImageUrl)
                                });
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

        void addProductItem(string title)
        {
            CollapseItem item = new CollapseItem();
            item.Title = title;
            item.Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1;
            CollapseItem.Add(item);
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
                      Path.GetRandomFileName(),
                      Path.GetExtension(e.Files[0].Name));
                var stream = e.Files[0].OpenReadStream();
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
                          Path.GetRandomFileName(),
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
                            ImageType = ImageType.Item,
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
                      Path.GetRandomFileName(),
                      Path.GetExtension(e.Files[0].Name));
                var stream = e.Files[0].OpenReadStream();
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
                    confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                    if (confirmed)
                    {
                        CarouselImages = CarouselImages.Where(x => x.BlobImageName != blobImageName).ToList();
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



            EditGroupBuyDto.ExcludeShippingMethod = string.Join(",", ItemTags);
            EditGroupBuyDto.PaymentMethod = string.Join(",", PaymentMethodTags);
            EditGroupBuyDto.NotifyMessage = await quillHtml.GetHTML();



            var result = await _groupBuyAppService.UpdateAsync(Id, EditGroupBuyDto);
            await _imageAppService.DeleteGroupBuyImagesAsync(Id);
            foreach (var item in CarouselImages)
            {

                item.TargetId = Id;
                await _imageAppService.CreateAsync(item);

            }

            NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");


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

        private void CloseGroupBuyEdit()
        {
            NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
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


    }

}
