using AntDesign;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class CreateItem
    {
        private const int maxtextCount = 60;
        private bool previewVisible = false;
        private string previewTitle = string.Empty;
        private string imgUrl = string.Empty;
        private string inputTagValue;
        private Input<string> inputTagRef;
        private string _selectedValue;
        private bool loading = false;
        private bool tagInputVisible { get; set; } = false;

        private CreateUpdateItemDto createItemDto = new();
        List<UploadFileItem> itemImageList = new List<UploadFileItem>();
        List<string> itemTags { get; set; } = new List<string>();
        List<ShippingMethod> shippingMethods { get; set; }
        List<TaxType> taxTypes { get; set; }


        public CreateItem()
        {

        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await GetAllTaxTape();
            await GetAllShippingMethod();
        }

        /// <summary>
        /// Show Image in Div After selection
        /// </summary>
        /// <param name="fileinfo">Selected File</param>
        void ItemImageHandleChange(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                fileinfo.File.Url = fileinfo.File.ObjectURL;
            }
        }

        /// <summary>
        /// On Tag close button remove Tag
        /// </summary>
        /// <param name="item">Tag string</param>
        void OnTagClose(string item)
        {
            itemTags.Remove(item);
        }
        /// <summary>
        /// On tag confirm
        /// </summary>
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

        /// <summary>
        /// On tag cancel
        /// </summary>
        void CancelInput()
        {
            this.inputTagValue = "";
            this.tagInputVisible = false;
        }


        /// <summary>
        /// Get All Type of Tax
        /// </summary>
        /// <returns></returns>
        async Task GetAllTaxTape()
        {
            taxTypes = new List<TaxType>
            {
                new TaxType { Value = "jack", Name = "Jack" },
                new TaxType { Value = "lucy", Name = "Lucy" },
                new TaxType { Value = "tom" , Name = "Tom" }
            };
        }

        /// <summary>
        /// Get All type of ShippingMethod
        /// </summary>
        /// <returns></returns>
        async Task GetAllShippingMethod()
        {
            shippingMethods = new List<ShippingMethod>
            {
                new ShippingMethod { Value = "ShippingMethod1", Name = "ShippingMethod1" },
                new ShippingMethod { Value = "ShippingMethod2", Name = "ShippingMethod2" },
                new ShippingMethod { Value = "ShippingMethod3" , Name = "ShippingMethod3" }
            };
        }

        protected virtual async Task CreateEntityAsync()
        {
        }


    }
    public class Item
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    class ShippingMethod
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    class TaxType
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class ResponseModel
    {
        public string name { get; set; }

        public string status { get; set; }

        public string url { get; set; }

        public string thumbUrl { get; set; }
    }
}
