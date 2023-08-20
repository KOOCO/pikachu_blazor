using AntDesign;
using AntDesign.Locales;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

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
        private List<CustomeField> customeFields = new List<CustomeField>();
        private readonly IItemAppService _itemAppService;
        private readonly IEnumValueAppService _enumValueService;

        public CreateItem(IEnumValueAppService enumValueService, IItemAppService itemAppService)
        {
            _enumValueService = enumValueService;
            _itemAppService = itemAppService;
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
                Id = customeField == null ? 1 : +customeField.Id + 1,
                Name = "",
                ItemTags = new List<string>()
            });
        }

        void BindItemDetailList()
        {
            itemDetailList = new List<CreateItemDetailsDto>();
            List<List<string>> permutations = GeneratePermutations(customeFields.Select(x=>x.ItemTags).ToList());

            foreach (List<string> permutation in permutations)
            {
                itemDetailList.Add(new CreateItemDetailsDto
                {
                    ItemName = string.Join("/", permutation)
                });
            }
        }

        static List<List<string>> GeneratePermutations(List<List<string>> sets)
        {
            List<List<string>> permutations = new List<List<string>>();
            GeneratePermutationsRecursive(sets, new List<string>(), 0, permutations);
            return permutations;
        }

        static void GeneratePermutationsRecursive(List<List<string>> sets, List<string> currentPermutation, int currentIndex, List<List<string>> permutations)
        {
            if (currentIndex == sets.Count)
            {
                permutations.Add(new List<string>(currentPermutation));
                return;
            }

            foreach (string item in sets[currentIndex])
            {
                currentPermutation.Add(item);
                GeneratePermutationsRecursive(sets, currentPermutation, currentIndex + 1, permutations);
                currentPermutation.RemoveAt(currentPermutation.Count - 1);
            }
        }

        protected virtual async Task CreateEntityAsync()
        {
            int customeFieldCount = customeFields.Count;
            var customFields = customeFields.ToArray();
            if (customeFieldCount>0)
            {
                createItemDto.CustomField1Name = customFields[0].Name;
                createItemDto.CustomField10Value = string.Join(",", customFields[0].ItemTags);
            }
            if (customeFieldCount > 1)
            {
                createItemDto.CustomField2Name = customFields[1].Name;
                createItemDto.CustomField1Value = string.Join(",", customFields[1].ItemTags);
            }
            if (customeFieldCount > 2)
            {
                createItemDto.CustomField3Name = customFields[2].Name;
                createItemDto.CustomField2Value = string.Join(",", customFields[2].ItemTags);
            }
            if (customeFieldCount > 3)
            {
                createItemDto.CustomField4Name = customFields[3].Name;
                createItemDto.CustomField3Value = string.Join(",", customFields[3].ItemTags);
            }
            if (customeFieldCount > 4)
            {
                createItemDto.CustomField5Name = customFields[4].Name;
                createItemDto.CustomField4Value = string.Join(",", customFields[4].ItemTags);
            }
            if (customeFieldCount > 5)
            {
                createItemDto.CustomField6Name = customFields[5].Name;
                createItemDto.CustomField5Value = string.Join(",", customFields[5].ItemTags);
            }
            if (customeFieldCount > 6)
            {
                createItemDto.CustomField7Name = customFields[6].Name;
                createItemDto.CustomField6Value = string.Join(",", customFields[6].ItemTags);
            }
            if (customeFieldCount > 7)
            {
                createItemDto.CustomField8Name = customFields[7].Name;
                createItemDto.CustomField7Value = string.Join(",", customFields[7].ItemTags);
            }
            if (customeFieldCount > 8)
            {
                createItemDto.CustomField9Name = customFields[8].Name;
                createItemDto.CustomField8Value = string.Join(",", customFields[8].ItemTags);
            }
            if (customeFieldCount > 9)
            {
                createItemDto.CustomField10Name = customFields[9].Name;
                createItemDto.CustomField9Value = string.Join(",", customFields[9].ItemTags);
            }

            createItemDto.ItemDetails = itemDetailList;
            createItemDto.ItemDescription = await quillHtml.GetHTML();
            createItemDto.ItemTags = string.Join(",", inputTagRef);
            await _itemAppService.CreateAsync(createItemDto);
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
