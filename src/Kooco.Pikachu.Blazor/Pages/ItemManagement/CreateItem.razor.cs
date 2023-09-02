
using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.ImageBlob;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class CreateItem
    {
        private const int maxtextCount = 60; //input max length
        private string inputTagValue;  //used for item store tag value 
        //private Input<string> inputTagRef; //used for create tag input
        private List<string> itemTags { get; set; } = new List<string>(); //used for store item tags 
        private bool tagInputVisible { get; set; } = false; //For show/hide item add tag input
        private List<EnumValueDto> shippingMethods { get; set; } // for bind all shippingMethods
        private List<EnumValueDto> taxTypes { get; set; } // for bind all taxTypes
        private Blazored.TextEditor.BlazoredTextEditor quillHtml; //Item Discription Html
        private List<CreateItemDetailsDto> itemDetailList { get; set; } // List of CreateItemDetail dto to store PriceAndInventory
        private CreateItemDto createItemDto = new(); //Item Dto
        private List<string> ItemImageList = new(); //to store Item Images
        private List<CustomFields> customFields = new();
        private readonly IItemAppService _itemAppService;
        private readonly IEnumValueAppService _enumValueService;
        private string TagInputValue { get; set; }
        private Dictionary<IFileEntry, string> FileDataUrls = new();
        private readonly IImageBlobService _imageBlobService;
        private readonly IUiMessageService _uiMessageService;

        public CreateItem(
            IEnumValueAppService enumValueService, 
            IItemAppService itemAppService, 
            IImageBlobService imageBlobService,
            IUiMessageService uiMessageService
            )
        {
            _enumValueService = enumValueService;
            _itemAppService = itemAppService;
            _imageBlobService = imageBlobService;
            _uiMessageService = uiMessageService;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var enumValues = (await _enumValueService.GetEnums(new List<EnumType> {
                                                             EnumType.ShippingMethod,
                                                             EnumType.TaxType
                                                         })).ToList();
            taxTypes = enumValues.Where(x => x.EnumType == EnumType.TaxType).ToList();
            createItemDto.TaxTypeId = taxTypes.First().Id;

            shippingMethods = enumValues.Where(x => x.EnumType == EnumType.ShippingMethod).ToList();
            createItemDto.ShippingMethodId = shippingMethods.First().Id;

            itemDetailList = new List<CreateItemDetailsDto>();
            customFields.Add(new CustomFields
            {
                Id = 1,
                Name = "",
                ItemTags = new List<string>()
            });
        }

        /// <summary>
        /// Show Image in Div After selection
        /// </summary>
        ///// <param name="fileinfo">Selected File</param>
        async void ItemImageChangeEvent(InputFileChangeEventArgs e)
        {
            var file = e.GetMultipleFiles(1).FirstOrDefault();
            var byteArray = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(byteArray);

            var savedFile = await _imageBlobService.UploadFileToBlob(file.Name, byteArray, file.ContentType, file.Name);


            var format = "image/png"; var imageFile = (e.GetMultipleFiles(1)).FirstOrDefault();
            var resizedImageFile = await imageFile.RequestImageFileAsync(format, 100, 100);
            var buffer = new byte[resizedImageFile.Size]; await resizedImageFile.OpenReadStream().ReadAsync(buffer);
            var url = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
        }
        async void ItemImageUploadEvent(FileUploadEventArgs e)
        {
            var file = e.File;
            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var imageBase64 = $"data:image/png;base64,{Convert.ToBase64String(fileBytes)}";
            FileDataUrls[file] = imageBase64;
        }
        void RemoveImage(IFileEntry file)
        {
            filePickerCustom.RemoveFile(file);
        }

        /// <summary>
        ///bind Shipping method value
        /// </summary>
        /// <param name="fileinfo">Selected File</param>
        void ItemSippingMethodHandleChange(EnumValueDto SippingMethod)
        {
            createItemDto.ShippingMethodId = SippingMethod.Id;
        }

        /// <summary>
        ///bind Tax Type value
        /// </summary>
        /// <param name="fileinfo">Selected File</param>
        void ItemTaxTypeHandleChange(EnumValueDto TaxType)
        {
            createItemDto.TaxTypeId = TaxType.Id;
        }

        /// <summary>
        /// On Tag close button remove Tag
        /// </summary>
        /// <param name="item">Tag string</param>
        void OnTagClose(string item)
        {
            itemTags.Remove(item);
        }
        private void HandleTagInputKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (TagInputValue.IsNullOrWhiteSpace())
                {
                    TagInputValue = string.Empty;
                    return;
                }
                itemTags.Add(TagInputValue);
                TagInputValue = string.Empty;
            }
        }

        private void HandleTagDelete(string item)
        {
            itemTags.Remove(item);
        }

        /// <summary>
        /// On Tag close button remove Tag for custom field
        /// </summary>
        /// <param name="item">Tag string</param>
        void OnCustomFieldTagClose(int id, string item)
        {
            customFields.First(x => x.Id == id).ItemTags.Remove(item);
            BindItemDetailList();
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
        void HandleCustomFieldTagInputConfirm(int id, string tag, KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                var customeField = customFields.First(x => x.Id == id);
                if (string.IsNullOrEmpty(tag))
                {
                    customeField.InputTagValue = "";
                    customeField.TagInputVisible = false;
                    return;
                }
                string? res = customeField.ItemTags.Find(s => s == tag);

                if (string.IsNullOrEmpty(res))
                {
                    customFields.First(x => x.Id == id).ItemTags.Add(tag);
                    BindItemDetailList();
                }
                customeField.InputTagValue = "";
                customeField.TagInputVisible = false;
            }
        }

        /// <summary>
        /// On tag cancel
        /// </summary>
        void CancelInput()
        {
            this.inputTagValue = "";
            this.tagInputVisible = false;
        }

        void DeleteCustomeField(CustomFields customeField)
        {
            customFields.Remove(customeField);
            BindItemDetailList();
        }
        void AddCustomeField()
        {
            var customeField = customFields.OrderByDescending(x => x.Id).FirstOrDefault();
            customFields.Add(new CustomFields
            {
                Id = customeField == null ? 1 : +customeField.Id + 1,
                Name = "",
                ItemTags = new List<string>()
            });
        }

        void BindItemDetailList()
        {
            itemDetailList = new List<CreateItemDetailsDto>();
            if (!customFields.Any())
                return;

            List<List<string>> permutations = GeneratePermutations(customFields.Select(x => x.ItemTags.Any() ? x.ItemTags : new List<string> { "" }).ToList());

            foreach (List<string> permutation in permutations)
            {
                itemDetailList.Add(new CreateItemDetailsDto
                {
                    ItemName = string.Join("/", permutation).TrimEnd('/')
                });
            }
        }

        static List<List<string>> GeneratePermutations(List<List<string>> sets)
        {
            List<List<string>> permutations = new();
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
            try
            {
                int customeFieldCount = this.customFields.Count;
                var customFields = this.customFields.ToArray();
                if (customeFieldCount > 0)
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
                createItemDto.ItemTags = string.Join(",", itemTags);
                await _itemAppService.CreateAsync(createItemDto);
                NavigationManager.NavigateTo("Items");
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.Message, ex.GetType().ToString());
            }
        }

        public async Task DeleteAllAsync()
        {
            try
            {
                await _itemAppService.DeleteAllAsync();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.Message, ex.GetType().ToString());
            }
        }
    }

    public class CustomFields
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ItemTags { get; set; }
        //public Input<string> InputTagRef { get; set; }
        public string InputTagValue { get; set; }
        public bool TagInputVisible { get; set; }
    }
}

public class ResponseModel
{
    public string name { get; set; }
    public string status { get; set; }
    public string url { get; set; }
    public string thumbUrl { get; set; }
}

