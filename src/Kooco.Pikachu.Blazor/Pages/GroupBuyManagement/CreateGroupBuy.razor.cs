using AntDesign;
using Kooco.Pikachu.GroupBuys;
using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class CreateGroupBuy
    {
        private const int maxtextCount = 60;
        private GroupBuyCreateDto createGroupBuyDto = new();
        private UploadFileItem groupBuyLogo = new UploadFileItem();
        private UploadFileItem productPicture = new UploadFileItem();
        private UploadFileItem groupBuyBanner = new UploadFileItem();
        private string inputTagValue;  //used for item store tag value 
        private Input<string> inputTagRef; //used for create tag input
        private List<string> itemTags { get; set; } = new List<string>(); //used for store item tags 
        private bool tagInputVisible { get; set; } = false; //For show/hide item add tag input
        bool loading = false;
        private List<CollapseItem> collapseItem = new List<CollapseItem>();
        int _value = 1;
        private Blazored.TextEditor.BlazoredTextEditor quillHtml; //Item Discription Html
        public string _ProductPicture = "Product Picture";
        public CreateGroupBuy()
        {


        }
        void GroupBuyLogoHandleChange(UploadInfo fileinfo)
        {
          
                if (fileinfo.File.State == UploadState.Success)
                {
                    fileinfo.File.Url = fileinfo.File.ObjectURL;
                    
                   groupBuyLogo.Size = fileinfo.File.Size;
                    groupBuyLogo.Url = fileinfo.File.Url;
                    groupBuyLogo.ObjectURL= fileinfo.File.ObjectURL;
                    groupBuyLogo.Percent = fileinfo.File.Percent;
                    groupBuyLogo.Response=fileinfo.File.Response;
                    groupBuyLogo.State = UploadState.Success;
                    groupBuyLogo.FileName=fileinfo.File.FileName;
                    groupBuyLogo.Type = fileinfo.File.Type;
                    groupBuyLogo.Ext=fileinfo.File.Ext;
                  
                }
            }
        void addProductItem(string title)
        {

            CollapseItem item = new CollapseItem();
            item.Title = title;
            item.Index = collapseItem.Count > 0 ? collapseItem.Count + 1 : 1;
            collapseItem.Add(item);


        }
        void GroupBuyBannerHandleChange(UploadInfo fileinfo)
        {

            if (fileinfo.File.State == UploadState.Success)
            {
                fileinfo.File.Url = fileinfo.File.ObjectURL;

                groupBuyBanner.Size = fileinfo.File.Size;
                groupBuyBanner.Url = fileinfo.File.Url;
                groupBuyBanner.ObjectURL = fileinfo.File.ObjectURL;
                groupBuyBanner.Percent = fileinfo.File.Percent;
                groupBuyBanner.Response = fileinfo.File.Response;
                groupBuyBanner.State = UploadState.Success;
                groupBuyBanner.FileName = fileinfo.File.FileName;
                groupBuyBanner.Type = fileinfo.File.Type;
                groupBuyBanner.Ext = fileinfo.File.Ext;

            }
        }
        void ProductPictureHandleChange(UploadInfo fileinfo)
        {

            if (fileinfo.File.State == UploadState.Success)
            {
                fileinfo.File.Url = fileinfo.File.ObjectURL;

                productPicture.Size = fileinfo.File.Size;
                productPicture.Url = fileinfo.File.Url;
                productPicture.ObjectURL = fileinfo.File.ObjectURL;
                productPicture.Percent = fileinfo.File.Percent;
                productPicture.Response = fileinfo.File.Response;
                productPicture.State = UploadState.Success;
                productPicture.FileName = fileinfo.File.FileName;
                productPicture.Type = fileinfo.File.Type;
                productPicture.Ext = fileinfo.File.Ext;

            }
        }
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

        public UploadFileItem ImageDetail { get; set; }
        public string ItemDescription { get; set; }

    }
}
