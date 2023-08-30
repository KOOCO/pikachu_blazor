
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.ImageBlob;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;



namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class EditGroupBuy
    {
        [Parameter]
        public string id { get; set; }
        public Guid Id { get; set; }
     
        bool paymentMethodCheck = false;
        [Inject] IJSRuntime JSRuntime { get; set; }
        private const int maxtextCount = 60;
        private GroupBuyUpdateDto updateGroupBuyDto { get; set; }
        //private UploadFileItem groupBuyLogo = new UploadFileItem();
        //private UploadFileItem productPicture = new UploadFileItem();
        //private UploadFileItem groupBuyBanner = new UploadFileItem();
        //private UploadFileItem groupBuyCarousel1 = new UploadFileItem();
        //private UploadFileItem groupBuyCarousel2 = new UploadFileItem();
        //private UploadFileItem groupBuyCarousel3 = new UploadFileItem();
        //private UploadFileItem groupBuyCarousel4 = new UploadFileItem();
        //private UploadFileItem groupBuyCarousel5 = new UploadFileItem();
        private string inputTagValue;  //used for item store tag value 
        //private Input<string> inputTagRef; //used for create tag input
        private List<string> itemTags { get; set; } = new List<string>(); //used for store item tags 
        private bool tagInputVisible { get; set; } = false; //For show/hide item add tag input
        private string inputPaymentMethodTagValue;  //used for item store tag value 
        //private Input<string> inputPaymentMethodTagRef; //used for create tag input
        private List<string> paymentMethodTags { get; set; } = new List<string>(); //used for store item tags 
        private bool tagInputPaymentMethodVisible { get; set; } = false; //For show/hide item add tag input
        bool loading = false;
        private List<CollapseItem> collapseItem = new List<CollapseItem>();
        int _value = 1;
        private Blazored.TextEditor.BlazoredTextEditor quillHtml; //Item Discription Html
        public string _ProductPicture = "Product Picture";
        private readonly IImageBlobService _imageBlobService;
        private readonly HttpClient _httpClient;
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IImageAppService _imageAppService;
        private readonly IObjectMapper _objectMapper;
        List<ImageDto> Images = new List<ImageDto>();
        CreateImageDto imageDto = new CreateImageDto();
        CreateImageDto imageDto2 = new CreateImageDto();
        CreateImageDto imageDto3 = new CreateImageDto();
        CreateImageDto imageDto4 = new CreateImageDto();
        CreateImageDto imageDto5 = new CreateImageDto();
        public EditGroupBuy(IImageBlobService imageBlobService, HttpClient httpClient, IGroupBuyAppService groupBuyAppService, IImageAppService imageAppService, IObjectMapper objectMapper)
        {
            _imageBlobService = imageBlobService;
            _httpClient = httpClient;
            _groupBuyAppService = groupBuyAppService;
            _imageAppService = imageAppService;
            _objectMapper = objectMapper;
            updateGroupBuyDto = new GroupBuyUpdateDto();
        }
        protected override async void OnInitialized()
        {
            id = id ?? "";
            Id=Guid.Parse(id);
          updateGroupBuyDto=  _objectMapper.Map<GroupBuyDto, GroupBuyUpdateDto>(await _groupBuyAppService.GetAsync(Id));
            Images = await _imageAppService.GetGroupBuyImagesAsync(Id);
            string[] paymentMethotTagArray  = updateGroupBuyDto.PaymentMethod != null ? updateGroupBuyDto.PaymentMethod.Split(','):null;
           paymentMethodTags= paymentMethodTags!=null? paymentMethodTags.ToList():new List<string>();
            string[] excludeShippingMethodArray = updateGroupBuyDto.ExcludeShippingMethod != null ? updateGroupBuyDto.ExcludeShippingMethod.Split(',') : null;
            itemTags = excludeShippingMethodArray!=null? excludeShippingMethodArray.ToList():new List<string>();
            //groupBuyLogo.Url = updateGroupBuyDto.LogoURL;
            //groupBuyBanner.Url = updateGroupBuyDto.BannerURL;
            


            //    groupBuyCarousel1.Url = (Images.Count - 1) >= 0 ? Images[0]?.ImagePath : null;
            //groupBuyCarousel2.Url = (Images.Count - 2) >= 0 ? Images[1]?.ImagePath : null;
            //groupBuyCarousel3.Url = (Images.Count - 3) >= 0 ? Images[2]?.ImagePath : null;
            //groupBuyCarousel4.Url = (Images.Count - 4) >= 0 ? Images[3]?.ImagePath : null;
            //groupBuyCarousel5.Url = (Images.Count - 5) >= 0 ? Images[4]?.ImagePath : null;

            imageDto = (Images.Count - 1) >= 0 ?_objectMapper.Map<ImageDto, CreateImageDto>(Images[0]) : new CreateImageDto();
            imageDto2= (Images.Count - 2) >= 0 ? _objectMapper.Map<ImageDto, CreateImageDto>(Images[1]) : new CreateImageDto();
            imageDto3 = (Images.Count - 3) >= 0 ? _objectMapper.Map<ImageDto, CreateImageDto>(Images[2]) : new CreateImageDto();
            imageDto4 = (Images.Count - 4) >= 0 ? _objectMapper.Map<ImageDto, CreateImageDto>(Images[3]) : new CreateImageDto();
           imageDto5 = (Images.Count - 5) >= 0 ? _objectMapper.Map<ImageDto, CreateImageDto>(Images[4]) : new CreateImageDto();

            StateHasChanged();
        }
        //void GroupBuyLogoHandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyLogo.Size = fileinfo.File.Size;
        //        groupBuyLogo.Url = fileinfo.File.Url;
        //        groupBuyLogo.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyLogo.Percent = fileinfo.File.Percent;
        //        groupBuyLogo.Response = fileinfo.File.Response;
        //        groupBuyLogo.State = UploadState.Success;
        //        groupBuyLogo.FileName = fileinfo.File.FileName;
        //        groupBuyLogo.Type = fileinfo.File.Type;
        //        groupBuyLogo.Ext = fileinfo.File.Ext;

        //    }
        //}
        void addProductItem(string title)
        {

            CollapseItem item = new CollapseItem();
            item.Title = title;
            item.Index = collapseItem.Count > 0 ? collapseItem.Count + 1 : 1;
            collapseItem.Add(item);


        }
        //void GroupBuyBannerHandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyBanner.Size = fileinfo.File.Size;
        //        groupBuyBanner.Url = fileinfo.File.Url;
        //        groupBuyBanner.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyBanner.Percent = fileinfo.File.Percent;
        //        groupBuyBanner.Response = fileinfo.File.Response;
        //        groupBuyBanner.State = UploadState.Success;
        //        groupBuyBanner.FileName = fileinfo.File.FileName;
        //        groupBuyBanner.Type = fileinfo.File.Type;
        //        groupBuyBanner.Ext = fileinfo.File.Ext;

        //    }
        //}
        //void GroupBuyCarouselImage2HandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyCarousel2.Size = fileinfo.File.Size;
        //        groupBuyCarousel2.Url = fileinfo.File.Url;
        //        groupBuyCarousel2.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyCarousel2.Percent = fileinfo.File.Percent;
        //        groupBuyCarousel2.Response = fileinfo.File.Response;
        //        groupBuyCarousel2.State = UploadState.Success;
        //        groupBuyCarousel2.FileName = fileinfo.File.FileName;
        //        groupBuyCarousel2.Type = fileinfo.File.Type;
        //        groupBuyCarousel2.Ext = fileinfo.File.Ext;

        //    }
        //}
        //void GroupBuyCarouselImage1HandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyCarousel1.Size = fileinfo.File.Size;
        //        groupBuyCarousel1.Url = fileinfo.File.Url;
        //        groupBuyCarousel1.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyCarousel1.Percent = fileinfo.File.Percent;
        //        groupBuyCarousel1.Response = fileinfo.File.Response;
        //        groupBuyCarousel1.State = UploadState.Success;
        //        groupBuyCarousel1.FileName = fileinfo.File.FileName;
        //        groupBuyCarousel1.Type = fileinfo.File.Type;
        //        groupBuyCarousel1.Ext = fileinfo.File.Ext;

        //    }
        //}
        //void GroupBuyCarouselImage3HandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyCarousel3.Size = fileinfo.File.Size;
        //        groupBuyCarousel3.Url = fileinfo.File.Url;
        //        groupBuyCarousel3.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyCarousel3.Percent = fileinfo.File.Percent;
        //        groupBuyCarousel3.Response = fileinfo.File.Response;
        //        groupBuyCarousel3.State = UploadState.Success;
        //        groupBuyCarousel3.FileName = fileinfo.File.FileName;
        //        groupBuyCarousel3.Type = fileinfo.File.Type;
        //        groupBuyCarousel3.Ext = fileinfo.File.Ext;

        //    }
        //}
        //void GroupBuyCarouselImage4HandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyCarousel4.Size = fileinfo.File.Size;
        //        groupBuyCarousel4.Url = fileinfo.File.Url;
        //        groupBuyCarousel4.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyCarousel4.Percent = fileinfo.File.Percent;
        //        groupBuyCarousel4.Response = fileinfo.File.Response;
        //        groupBuyCarousel4.State = UploadState.Success;
        //        groupBuyCarousel4.FileName = fileinfo.File.FileName;
        //        groupBuyCarousel4.Type = fileinfo.File.Type;
        //        groupBuyCarousel4.Ext = fileinfo.File.Ext;

        //    }
        //}
        //void GroupBuyCarouselImage5HandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        groupBuyCarousel5.Size = fileinfo.File.Size;
        //        groupBuyCarousel5.Url = fileinfo.File.Url;
        //        groupBuyCarousel5.ObjectURL = fileinfo.File.ObjectURL;
        //        groupBuyCarousel5.Percent = fileinfo.File.Percent;
        //        groupBuyCarousel5.Response = fileinfo.File.Response;
        //        groupBuyCarousel5.State = UploadState.Success;
        //        groupBuyCarousel5.FileName = fileinfo.File.FileName;
        //        groupBuyCarousel5.Type = fileinfo.File.Type;
        //        groupBuyCarousel5.Ext = fileinfo.File.Ext;

        //    }
        //}
        //void ProductPictureHandleChange(UploadInfo fileinfo)
        //{

        //    if (fileinfo.File.State == UploadState.Success)
        //    {
        //        fileinfo.File.Url = fileinfo.File.ObjectURL;

        //        productPicture.Size = fileinfo.File.Size;
        //        productPicture.Url = fileinfo.File.Url;
        //        productPicture.ObjectURL = fileinfo.File.ObjectURL;
        //        productPicture.Percent = fileinfo.File.Percent;
        //        productPicture.Response = fileinfo.File.Response;
        //        productPicture.State = UploadState.Success;
        //        productPicture.FileName = fileinfo.File.FileName;
        //        productPicture.Type = fileinfo.File.Type;
        //        productPicture.Ext = fileinfo.File.Ext;

        //    }
        //}
        void HandleTagInputConfirm()
        {
            if (string.IsNullOrEmpty(inputTagValue))
            {
                CancelInput();
                return;
            }
            string res = itemTags.Find(s => s == inputTagValue);

            if (string.IsNullOrEmpty(res))
                itemTags.Add(inputTagValue);
            CancelInput();
        }
        void CancelInput()
        {
            this.inputTagValue = "";
            this.tagInputVisible = false;
        }
        /// <param name="item">Tag string</param>
        void OnTagClose(string item)
        {
            itemTags.Remove(item);
        }
        void HandlePaymentMethodTagInputConfirm()
        {
            if (string.IsNullOrEmpty(inputPaymentMethodTagValue))
            {
                CancelPaymentMethodInput();
                return;
            }
            string res = paymentMethodTags.Find(s => s == inputPaymentMethodTagValue);

            if (string.IsNullOrEmpty(res))
                paymentMethodTags.Add(inputPaymentMethodTagValue);
            CancelPaymentMethodInput();
        }
        void CancelPaymentMethodInput()
        {
            this.inputPaymentMethodTagValue = "";
            this.tagInputPaymentMethodVisible = false;
        }

        void OnPaymentMethodTagClose(string item)
        {
            paymentMethodTags.Remove(item);
        }
        protected virtual async Task CreateEntityAsync()
        {
         

           
            updateGroupBuyDto.ExcludeShippingMethod = string.Join(",", itemTags);
            updateGroupBuyDto.PaymentMethod = string.Join(",", paymentMethodTags);
            updateGroupBuyDto.NotifyMessage = await quillHtml.GetHTML();

            //if (groupBuyLogo.ObjectURL != null)
            //{
            //    await _imageBlobService.DeleteBlobData(updateGroupBuyDto.LogoURL);
            //    var blobUrl = groupBuyLogo.ObjectURL;

            //    var base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", blobUrl);

            //    // Convert the base64-encoded string to a byte array
            //    var byteArray = Convert.FromBase64String(base64String);
            //    updateGroupBuyDto.LogoURL = await _imageBlobService.UploadFileToBlob(groupBuyLogo.FileName, byteArray, groupBuyLogo.Type, groupBuyLogo.FileName);
            //}
            //if (groupBuyBanner.ObjectURL != null)
            //{
            //    await _imageBlobService.DeleteBlobData(updateGroupBuyDto.BannerURL);
            //    var base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", groupBuyBanner.ObjectURL);

            //    // Convert the base64-encoded string to a byte array
            //   var byteArray = Convert.FromBase64String(base64String);
            //    updateGroupBuyDto.BannerURL = await _imageBlobService.UploadFileToBlob(groupBuyBanner.FileName, byteArray, groupBuyBanner.Type, groupBuyBanner.FileName);
            //}
               
            //if (groupBuyCarousel1.ObjectURL != null)
            //{
            //    if (imageDto?.ImagePath != null)
            //    {
            //        await _imageBlobService.DeleteBlobData(imageDto?.ImagePath);
            //    }
            //    var  base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", groupBuyCarousel1.ObjectURL);

            //    // Convert the base64-encoded string to a byte array
            //   var byteArray = Convert.FromBase64String(base64String);

            //    imageDto.ImageType = ImageType.Item;
            //    imageDto.ImagePath = await _imageBlobService.UploadFileToBlob(groupBuyCarousel1.FileName, byteArray, groupBuyCarousel1.Type, groupBuyCarousel1.FileName);

            //}

            //if (groupBuyCarousel4.ObjectURL != null)
            //{
            //    if (imageDto4?.ImagePath != null)
            //    {
            //        await _imageBlobService.DeleteBlobData(imageDto4?.ImagePath);
            //    }
            //    var  base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", groupBuyCarousel4.ObjectURL);

            //    // Convert the base64-encoded string to a byte array
            //   var byteArray = Convert.FromBase64String(base64String);

            //    imageDto.ImageType = ImageType.Item;
            //    imageDto.ImagePath = await _imageBlobService.UploadFileToBlob(groupBuyCarousel4.FileName, byteArray, groupBuyCarousel4.Type, groupBuyCarousel4.FileName);

            //}
            //if (groupBuyCarousel2.ObjectURL != null)
            //{
            //    if (imageDto2?.ImagePath != null)
            //    {
            //        await _imageBlobService.DeleteBlobData(imageDto2?.ImagePath);
            //    }
            //   var base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", groupBuyCarousel2.ObjectURL);

            //    // Convert the base64-encoded string to a byte array
            //   var byteArray = Convert.FromBase64String(base64String);

            //    imageDto.ImageType = ImageType.Item;
            //    imageDto.ImagePath = await _imageBlobService.UploadFileToBlob(groupBuyCarousel2.FileName, byteArray, groupBuyCarousel2.Type, groupBuyCarousel2.FileName);

            //}
            //if (groupBuyCarousel3.ObjectURL != null)
            //{
            //    if (imageDto3?.ImagePath != null)
            //    {
            //        await _imageBlobService.DeleteBlobData(imageDto3?.ImagePath);
            //    }
            //    var base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", groupBuyCarousel3.ObjectURL);

            //    // Convert the base64-encoded string to a byte array
            //  var  byteArray = Convert.FromBase64String(base64String);

            //    imageDto.ImageType = ImageType.Item;
            //    imageDto.ImagePath = await _imageBlobService.UploadFileToBlob(groupBuyCarousel3.FileName, byteArray, groupBuyCarousel3.Type, groupBuyCarousel3.FileName);

            //}
            //if (groupBuyCarousel5.Url != null)
            //{
            //    if (imageDto5?.ImagePath != null)
            //    {
            //        await _imageBlobService.DeleteBlobData(imageDto5?.ImagePath);
            //    }

            //    var base64String = await JSRuntime.InvokeAsync<string>("convertBlobUrlToByteArray", groupBuyCarousel5.ObjectURL);

            //    // Convert the base64-encoded string to a byte array
            //   var byteArray = Convert.FromBase64String(base64String);

            //    imageDto.ImageType = ImageType.Item;
            //    imageDto.ImagePath = await _imageBlobService.UploadFileToBlob(groupBuyCarousel5.FileName, byteArray, groupBuyCarousel5.Type, groupBuyCarousel5.FileName);

            //}

            var result = await _groupBuyAppService.UpdateAsync(Id,updateGroupBuyDto);
            await _imageAppService.DeleteGroupBuyImagesAsync(Id);
            if (imageDto.ImagePath != null)
            {
                imageDto.TargetID = result.Id;

                await _imageAppService.CreateAsync(imageDto);

            }
            if (imageDto2.ImagePath != null)
            {
                imageDto2.TargetID = result.Id;

                await _imageAppService.CreateAsync(imageDto2);

            }
            if (imageDto3.ImagePath != null)
            {
                imageDto3.TargetID = result.Id;

                await _imageAppService.CreateAsync(imageDto3);

            }
            if (imageDto4.ImagePath != null)
            {
                imageDto2.TargetID = result.Id;

                await _imageAppService.CreateAsync(imageDto4);

            }
            if (imageDto5.ImagePath != null)
            {
                imageDto5.TargetID = result.Id;

                await _imageAppService.CreateAsync(imageDto5);

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
                updateGroupBuyDto.IsDefaultPaymentGateWay = isChecked; // Uncheck the first radio button
            }
        }

    }
}
