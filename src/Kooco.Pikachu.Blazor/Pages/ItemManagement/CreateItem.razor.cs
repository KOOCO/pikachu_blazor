using AntDesign;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Linq;
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
        private int _selectedShippingMethodValue;
        private int _selectedTaxTypeValue;
        private bool loading = false;
        private bool tagInputVisible { get; set; } = false;

        private CreateItemDto createItemDto = new();
        List<UploadFileItem> itemImageList   = new List<UploadFileItem>();
        List<string> itemTags { get; set; } = new List<string>();
        List<EnumValueDto> shippingMethods { get; set; }
        List<EnumValueDto> taxTypes { get; set; }

        private readonly IEnumValueAppService _enumValueService;
        public CreateItem(IEnumValueAppService enumValueService)
        {
            _enumValueService = enumValueService;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var enumValues = (await _enumValueService.GetEnums(new List<EnumType> { 
                                                             EnumType.ShippingMethod,
                                                             EnumType.TaxType
                                                         })).ToList();
            taxTypes = enumValues.Where(x=>x.EnumType == EnumType.TaxType).ToList();
            shippingMethods = enumValues.Where(x=>x.EnumType == EnumType.ShippingMethod).ToList();
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
        ///bind Shipping method value
        /// </summary>
        /// <param name="fileinfo">Selected File</param>
        void ItemSippingMethodHandleChange(EnumValueDto SippingMethod)
        {
            _selectedShippingMethodValue = SippingMethod.Id;
        }
        
        /// <summary>
        ///bind Tax Type value
        /// </summary>
        /// <param name="fileinfo">Selected File</param>
        void ItemTaxTypeHandleChange(EnumValueDto TaxType)
        {
            _selectedTaxTypeValue = TaxType.Id;
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

    public class ResponseModel
    {
        public string name { get; set; }

        public string status { get; set; }

        public string url { get; set; }

        public string thumbUrl { get; set; }
    }
}
