using Blazorise;
//using Blazorise.RichTextEdit;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp; 

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class CreateItem
    {
        protected Validations CreateValidationsRef;
        protected CreateUpdateItemDto NewEntity = new();
        IReadOnlyList<ItemDto> itemList = Array.Empty<ItemDto>();
        public const int maxtextCount = 60;
        //protected RichTextEdit richTextEditRef;
        protected bool readOnly;
        protected string contentAsHtml;
        protected string contentAsDeltaJson;
        protected string contentAsText;
        protected string savedContent;
        bool previewVisible = false;
        string previewTitle = string.Empty;
        string imgUrl = string.Empty;

        public List<AntDesign.UploadFileItem> fileList = new List<AntDesign.UploadFileItem>();

      

        void HandleChange(AntDesign.UploadInfo fileinfo)
        {
            if (fileinfo.File.State == AntDesign.UploadState.Success)
            {
                fileinfo.File.Url = fileinfo.File.ObjectURL;
            }
        }

        public class ResponseModel
        {
            public string name { get; set; }

            public string status { get; set; }

            public string url { get; set; }

            public string thumbUrl { get; set; }
        }











        //public async Task OnContentChanged()
        //{
        //    contentAsHtml = await richTextEditRef.GetHtmlAsync();
        //    contentAsDeltaJson = await richTextEditRef.GetDeltaAsync();
        //    contentAsText = await richTextEditRef.GetTextAsync();
        //}

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (CreateValidationsRef != null)
                await CreateValidationsRef.ClearAll();
        }

        protected virtual async Task CreateEntityAsync()
        {
            try
            {
                await AppService.CreateAsync(NewEntity);
                NavigationManager.NavigateTo("Items");
            }
            catch (Exception ex)
            {
                //if the environment is debug then throw the exception
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    throw new UserFriendlyException("Unable to Save", ex.Source, ex.Message);
                else
                    throw new UserFriendlyException("Unable to Save");
            }
        }
        private async Task OnFileSelection(InputFileChangeEventArgs e)
        {
            try
            {
                IBrowserFile imgFile = e.File;
                var buffers = new byte[imgFile.Size];
                await imgFile.OpenReadStream().ReadAsync(buffers);
                string imageType = imgFile.ContentType;
                imgUrl = $"data:{imageType};base64,{Convert.ToBase64String(buffers)}";
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
