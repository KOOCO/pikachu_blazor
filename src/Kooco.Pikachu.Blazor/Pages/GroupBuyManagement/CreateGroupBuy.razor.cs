
using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.ImageBlob;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections;
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
    public partial class CreateGroupBuy
    {
        bool paymentMethodCheck = false;
        [Inject] IJSRuntime JSRuntime { get; set; }
        private const int maxtextCount = 60;
        private const int MaxAllowedFilesPerUpload = 5;
        private const int TotalMaxAllowedFiles = 5;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private GroupBuyCreateDto createGroupBuyDto = new();

        public List<CreateImageDto> CarouselImages { get; set; }
        private string TagInputValue { get; set; }

        //private Input<string> inputTagRef; //used for create tag input
        private List<string> itemTags { get; set; } = new List<string>(); //used for store item tags 
     
        //private Input<string> inputPaymentMethodTagRef; //used for create tag input
        private List<string> paymentMethodTags { get; set; } = new List<string>(); //used for store item tags 
        private string PaymentTagInputValue { get; set; }
        bool loading = false;
        private List<CollapseItem> collapseItem = new List<CollapseItem>();
        int _value = 1;
        string logoBlobName;
        string bannerBlobName;
        private Blazored.TextEditor.BlazoredTextEditor quillHtml; //Item Discription Html
        public string _ProductPicture = "Product Picture";
        private readonly IImageBlobService _imageBlobService;
        private FilePicker LogoPickerCustom { get; set; }
        private FilePicker BannerPickerCustom { get; set; }
        private FilePicker CarouselPickerCustom { get; set; }
        private readonly HttpClient _httpClient;
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IImageAppService _imageAppService;
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        public CreateGroupBuy(IImageBlobService imageBlobService, HttpClient httpClient, IGroupBuyAppService groupBuyAppService,
            IImageAppService imageAppService, IUiMessageService uiMessageService, ImageContainerManager imageContainerManager)
        {
            _imageBlobService = imageBlobService;
            _httpClient = httpClient;
            _groupBuyAppService = groupBuyAppService;
            _imageAppService = imageAppService;
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            CarouselImages = new List<CreateImageDto>();
        }

        async Task OnLogoUploadAsync(FileChangedEventArgs e)
        {
            if (e.Files.Count() > 1)
            {
                await _uiMessageService.Error("Select Only 1 Logo Upload");
                await LogoPickerCustom.Clear();
                return;
            }
            if (e.Files.Count() == 0) {

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
                    logoBlobName = newFileName;


                    createGroupBuyDto.LogoURL = url;

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
                    bannerBlobName = newFileName;


                    createGroupBuyDto.BannerURL = url;

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
                        createGroupBuyDto.LogoURL = null;
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
                        createGroupBuyDto.BannerURL = null;
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
        void addProductItem(string title)
        {

            CollapseItem item = new CollapseItem();
            item.Title = title;
            item.Index = collapseItem.Count > 0 ? collapseItem.Count + 1 : 1;
            collapseItem.Add(item);


        }
     
        private void HandleItemTagInputKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (!TagInputValue.IsNullOrWhiteSpace() && !itemTags.Any(x => x == TagInputValue))
                {
                    itemTags.Add(TagInputValue);
                }
                TagInputValue = string.Empty;
            }
        }

        private void HandleItemTagDelete(string item)
        {
            itemTags.Remove(item);
        }
        private void HandlePaymentTagInputKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (!PaymentTagInputValue.IsNullOrWhiteSpace() && !paymentMethodTags.Any(x => x == PaymentTagInputValue))
                {
                    paymentMethodTags.Add(PaymentTagInputValue);
                }
                PaymentTagInputValue = string.Empty;
            }
        }

        private void HandlePaymentTagDelete(string item)
        {
            paymentMethodTags.Remove(item);
        }


      
        protected virtual async Task CreateEntityAsync()
        {
           

            createGroupBuyDto.GroupBuyNo = 0;
            createGroupBuyDto.Status = "New";
            createGroupBuyDto.ExcludeShippingMethod = string.Join(",", itemTags);
            createGroupBuyDto.PaymentMethod=string.Join(",", paymentMethodTags);
            createGroupBuyDto.NotifyMessage=  await quillHtml.GetHTML();
       var result=await _groupBuyAppService.CreateAsync(createGroupBuyDto);
            foreach (var item in CarouselImages)
            {
                item.TargetId = result.Id;

                await _imageAppService.CreateAsync(item);

            }
           


            NavigationManager.NavigateTo("GroupBuyManagement/GroupBuyList");
           

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
                createGroupBuyDto.IsDefaultPaymentGateWay = isChecked; // Uncheck the first radio button
            }
        }

    }
    public class CollapseItem {

        public int Index { get; set; }
        public string Title { get; set; }
        public bool IsProductDescription { get; set; }
       public List<ProductPictureItem> ItemDetails { get; set; }
        public CollapseItem() {

            ItemDetails = new List<ProductPictureItem>();


        }


    }
    public class ProductPictureItem {

        public string ImageUrl { get; set; }
        public string ItemDescription { get; set; }

    }
}
