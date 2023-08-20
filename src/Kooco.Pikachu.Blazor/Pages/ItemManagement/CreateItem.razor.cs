using AntDesign;
using AntDesign.Locales;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class CreateItem
    {
        private const int maxtextCount = 60; //input max length
        private string inputTagValue;  //used for item store tag value 
        private Input<string> inputTagRef; //used for create tag input
        private List<string> itemTags { get; set; } = new List<string>(); //used for store item tags 
        private bool tagInputVisible { get; set; } = false; //For show/hide item add tag input
        private List<EnumValueDto> shippingMethods { get; set; } // for bind all shippingMethods
        private List<EnumValueDto> taxTypes { get; set; } // for bind all taxTypes
        private Blazored.TextEditor.BlazoredTextEditor quillHtml; //Item Discription Html
        private List<CreateItemDetailsDto> itemDetailList { get; set; } // List of CreateItemDetail dto to store PriceAndInventory
        private CreateItemDto createItemDto = new(); //Item Dto
        private List<UploadFileItem> itemImageList = new List<UploadFileItem>(); //to store Item Images
        private readonly IEnumValueAppService _enumValueService;
        private List<CustomeField> customeFields = new List<CustomeField>();

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
            taxTypes = enumValues.Where(x => x.EnumType == EnumType.TaxType).ToList();
            shippingMethods = enumValues.Where(x => x.EnumType == EnumType.ShippingMethod).ToList();
            itemDetailList = new List<CreateItemDetailsDto>();
            customeFields.Add(new CustomeField
            {
                Id = 1,
                Name = "",
                ItemTags = new List<string>()
            });
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
            createItemDto.ShippingMethod = SippingMethod;
        }

        /// <summary>
        ///bind Tax Type value
        /// </summary>
        /// <param name="fileinfo">Selected File</param>
        void ItemTaxTypeHandleChange(EnumValueDto TaxType)
        {
            createItemDto.TaxType = TaxType;
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
        /// On Tag close button remove Tag for custom field
        /// </summary>
        /// <param name="item">Tag string</param>
        void OnCustomFieldTagClose(int id, string item)
        {
            customeFields.First(x => x.Id == id).ItemTags.Remove(item);
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
        /// On custom Fields tag confirm
        /// </summary>
        void HandleCustomFieldTagInputConfirm(int id, string tag)
        {
            var customeField = customeFields.First(x => x.Id == id);
            if (string.IsNullOrEmpty(tag))
            {
                customeField.InputTagValue = "";
                customeField.TagInputVisible = false;
                return;
            }
            string? res = customeField.ItemTags.Find(s => s == tag);

            if (string.IsNullOrEmpty(res))
            {
                customeFields.First(x => x.Id == id).ItemTags.Add(tag);
                BindItemDetailList();
            }
            customeField.InputTagValue = "";
            customeField.TagInputVisible = false;
        }

        /// <summary>
        /// On tag cancel
        /// </summary>
        void CancelInput()
        {
            this.inputTagValue = "";
            this.tagInputVisible = false;
        }

        void DeleteCustomeField(CustomeField customeField)
        {
             customeFields.Remove(customeField);
            BindItemDetailList();
        }
        void AddCustomeField()
        {
            var customeField = customeFields.OrderByDescending(x => x.Id).FirstOrDefault();
            customeFields.Add(new CustomeField
            {
                Id = customeField == null ? 1 : + customeField.Id + 1,
                Name = "",
                ItemTags = new List<string>()
            });
        }

        void BindItemDetailList()
        {
            var localref = customeFields.ToList();
            foreach (var customeField in localref)
            {
                foreach (var item in customeField.ItemTags)
                {
                    itemDetailList.Add(new CreateItemDetailsDto
                    {
                        ItemName = item
                    });
                }
            }
        }

        //void UpdateItemDetail(List<>localref)
        //{
        //    itemDetailList.Add(new CreateItemDetailsDto
        //    {
        //        ItemName = tag
        //    });
        //}


        protected virtual async Task CreateEntityAsync()
        {
            createItemDto.ItemDescription = await quillHtml.GetHTML();
            createItemDto.ItemTags = string.Join(",", inputTagRef);
        }
    }

    public class CustomeField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ItemTags { get; set; }
        public Input<string> InputTagRef { get; set; }
        public string InputTagValue { get; set; }
        public bool TagInputVisible { get; set; }
    }
    public class ResponseModel
    {
        public string name { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string thumbUrl { get; set; }
    }
}
