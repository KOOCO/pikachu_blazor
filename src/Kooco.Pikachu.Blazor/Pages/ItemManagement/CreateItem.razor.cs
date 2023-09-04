using Blazored.TextEditor;
using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.ImageBlob;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Threading;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class CreateItem
    {
        private const int MaxTextCount = 60; //input max length
        private List<string> ItemTags { get; set; } = new List<string>(); //used for store item tags 
        private List<EnumValueDto> ShippingMethods { get; set; } // for bind all shippingMethods
        private List<EnumValueDto> TaxTypes { get; set; } // for bind all taxTypes
        private BlazoredTextEditor QuillHtml; //Item Discription Html
        private List<CreateItemDetailsDto> ItemDetailsList { get; set; } // List of CreateItemDetail dto to store PriceAndInventory
        private CreateItemDto CreateItemDto = new(); //Item Dto
        private List<string> ItemImageList = new(); //to store Item Images
        private List<Attributes> Attributes = new();
        private string TagInputValue { get; set; }
        private Modal GenerateSKUModal { get; set; }
        private SKUModel SKUModel { get; set; } = new();


        private readonly IItemAppService _itemAppService;
        private readonly IEnumValueAppService _enumValueService;
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
            TaxTypes = enumValues.Where(x => x.EnumType == EnumType.TaxType).ToList();
            CreateItemDto.TaxTypeId = TaxTypes.First().Id;

            ShippingMethods = enumValues.Where(x => x.EnumType == EnumType.ShippingMethod).ToList();
            CreateItemDto.ShippingMethodId = ShippingMethods.First().Id;

            ItemDetailsList = new List<CreateItemDetailsDto>();
            Attributes.Add(new Attributes
            {
                Id = 1,
                Name = "",
                ItemTags = new List<string>()
            });
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

        /// <summary>
        /// On Tag close button remove Tag for custom field
        /// </summary>
        /// <param name="item">Tag string</param>
        void OnAttributeFieldTagClose(int id, string item)
        {
            Attributes.First(x => x.Id == id).ItemTags.Remove(item);
            BindItemDetailList();
        }

        /// <summary>
        /// On custom Fields tag confirm
        /// </summary>
        void HandleAttributeTagInputConfirm(int id, string tag, KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                var attribute = Attributes.First(x => x.Id == id);
                if (string.IsNullOrEmpty(tag))
                {
                    attribute.InputTagValue = "";
                    return;
                }
                string? res = attribute.ItemTags.Find(s => s == tag);

                if (string.IsNullOrEmpty(res))
                {
                    Attributes.First(x => x.Id == id).ItemTags.Add(tag);
                    BindItemDetailList();
                }
                attribute.InputTagValue = "";
            }
        }

        void DeleteAttribute(Attributes attribute)
        {
            Attributes.Remove(attribute);
            BindItemDetailList();
        }

        void AddAttribute()
        {
            if (Attributes == null || Attributes.Count < 3)
            {
                var attribute = Attributes.OrderByDescending(x => x.Id).FirstOrDefault();
                Attributes.Add(new Attributes
                {
                    Id = attribute == null ? 1 : +attribute.Id + 1,
                    Name = "",
                    ItemTags = new List<string>()
                });
            }
        }

        void BindItemDetailList()
        {
            ItemDetailsList = new List<CreateItemDetailsDto>();
            if (!Attributes.Any())
                return;

            List<List<string>> permutations = GeneratePermutations(Attributes.Select(x => x.ItemTags.Any() ? x.ItemTags : new List<string> { "" }).ToList());

            foreach (List<string> permutation in permutations)
            {
                ItemDetailsList.Add(new CreateItemDetailsDto
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
                ValidateForm();
                GenerateAttributesForItemDetails();

                CreateItemDto.ItemDetails = ItemDetailsList;
                CreateItemDto.ItemDescription = await QuillHtml.GetHTML();
                CreateItemDto.ItemTags = string.Join(",", ItemTags);
                await _itemAppService.CreateAsync(CreateItemDto);
                NavigationManager.NavigateTo("Items");
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(ex.Code.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        private void GenerateSKU()
        {
            try
            {
                GenerateAttributesForItemDetails();
                ItemDetailsList.ForEach(item =>
                {
                    IDictionary<string, string?> itemOptions = new Dictionary<string, string?>();

                    if (!CreateItemDto.Attribute1Name.IsNullOrWhiteSpace())
                    {
                        itemOptions[CreateItemDto.Attribute1Name] = item.Attribute1Value;
                    }
                    if (!CreateItemDto.Attribute2Name.IsNullOrWhiteSpace())
                    {
                        itemOptions[CreateItemDto.Attribute2Name] = item.Attribute2Value;
                    }
                    if (!CreateItemDto.Attribute3Name.IsNullOrWhiteSpace())
                    {
                        itemOptions[CreateItemDto.Attribute3Name] = item.Attribute3Value;
                    }
                    item.Sku = GenerateItemSKU(itemOptions);
                    item.Sku = item.Sku.Trim(SKUModel.SeparatedBy[0]);
                });

                CloseGenerateSKUModal();
            }
            catch (Exception ex)
            {
                _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        private void ValidateForm()
        {
            if (CreateItemDto.ItemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
            }

            if (ItemDetailsList.Any(x => x.Sku.IsNullOrWhiteSpace()))
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SKUForItemDetailsCannotBeNull]);
            }

            if (ItemDetailsList.Any(x => ItemDetailsList.Any(y => y != x && y.Sku == x.Sku)))
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemWithSKUAlreadyExists]);
            }

            if (ItemDetailsList.Any(x => x.SellingPrice <= 0))
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SellingPriceForItemShouldBeGreaterThanZero]);
            }
        }

        private void GenerateAttributesForItemDetails()
        {
            int attributeCount = Attributes.Count;
            var customFields = Attributes.ToArray();
            // Generate all combinations of ItemTags
            var allCombinations = new List<List<string>>();
            for (int i = 0; i < attributeCount; i++)
            {
                allCombinations.Add(customFields[i].ItemTags);
            }
            var combinations = GenerateCombinations(allCombinations);

            for (int i = 0; i < ItemDetailsList.Count; i++)
            {
                var combination = combinations[i];
                ItemDetailsList[i].Attribute1Value = combination.Count > 0 ? combination[0] : null;
                ItemDetailsList[i].Attribute2Value = combination.Count > 1 ? combination[1] : null;
                ItemDetailsList[i].Attribute3Value = combination.Count > 2 ? combination[2] : null;
            }

            // Set AttributeNames in CreateItemDto
            if (attributeCount > 0)
            {
                CreateItemDto.Attribute1Name = customFields[0].Name;
            }
            if (attributeCount > 1)
            {
                CreateItemDto.Attribute2Name = customFields[1].Name;
            }
            if (attributeCount > 2)
            {
                CreateItemDto.Attribute3Name = customFields[2].Name;
            }
        }

        public List<List<string>> GenerateCombinations(List<List<string>> lists)
        {
            var combinations = new List<List<string>>();
            var combination = new List<string>();

            GenerateCombinationsHelper(lists, combinations, combination, 0);

            return combinations;
        }

        public void GenerateCombinationsHelper(List<List<string>> lists, List<List<string>> results, List<string> combination, int depth)
        {
            if (depth == lists.Count)
            {
                results.Add(new List<string>(combination));
                return;
            }

            for (int i = 0; i < lists[depth].Count; i++)
            {
                combination.Add(lists[depth][i]);
                GenerateCombinationsHelper(lists, results, combination, depth + 1);
                combination.RemoveAt(combination.Count - 1);
            }
        }

        private void CopyToAll(string propertyName)
        {
            try
            {
                if (ItemDetailsList != null && ItemDetailsList.Count > 0)
                {
                    if (ItemDetailsList != null && ItemDetailsList.Count > 0)
                    {
                        var prop = typeof(CreateItemDetailsDto).GetProperty(propertyName);
                        var firstItemValue = prop.GetValue(ItemDetailsList[0]);
                        foreach (var item in ItemDetailsList)
                        {
                            typeof(CreateItemDetailsDto).GetProperty(propertyName).SetValue(item, firstItemValue, null);
                        }
                    }
                }
            }
            catch
            {
                _uiMessageService.Error(L["SystemIsUnableToCopyAtTheMoment"]);
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

        private void OpenGenerateSKUModal()
        {
            GenerateSKUModal.Show();
            var options = GetSKUSampleDropdownOptions();
            var attributes = GetAttributesDictionary();
            SKUModel = new SKUModel(options, attributes);

            var dropdownOptions = SKUModel.DropdownOptions.ToList();

            for (int i = 0; i < SKUModel.SKUModelOptions.Count; i++)
            {
                // Only select a value if it's not the last item
                if (i != 0 && i < dropdownOptions.Count - 1)
                {
                    SKUModel.SKUModelOptions[i].SelectedSampleValue = dropdownOptions[i];
                }
            }
            GeneratePreview();


        }
        private void CloseGenerateSKUModal()
        {
            GenerateSKUModal.Hide();
        }

        private void UpdateSeparatedBy(ChangeEventArgs e)
        {
            SKUModel.SeparatedBy = e.Value.ToString();
            GeneratePreview();
        }

        private bool IsSeparatedBy(char value)
        {
            return SKUModel.SeparatedBy == value.ToString();
        }

        private void UpdateCaseUsed(ChangeEventArgs e)
        {
            SKUModel.CaseUsed = e.Value.ToString();
            if (SKUModel.CaseUsed == "U")
            {
                SKUModel.Preview = SKUModel.Preview.ToUpper();
            }
            else
            {
                SKUModel.Preview = SKUModel.Preview.ToLower();
            }
        }

        private bool IsCaseUsed(char value)
        {
            return SKUModel.CaseUsed == value.ToString();
        }

        private void UpdateDropdownDisplayValue(SKUModelOptions item, char value)
        {
            item.CharactersOrder = value.ToString();
            item.DropdownDisplayValue = item.CharactersOrder == "F" ? "First" : "Last";
            GeneratePreview();
        }

        public List<string> GetSKUSampleDropdownOptions()
        {
            List<string> options = new() { new("") };
            Attributes.ForEach(item =>
            {
                if (!item.Name.IsNullOrWhiteSpace())
                {
                    options.Add(item.Name);
                }
            });
            options.Add(new string("SelectCustomText"));
            return options;
        }

        public IDictionary<string, string?> GetAttributesDictionary()
        {
            IDictionary<string, string?> options = new Dictionary<string, string?>();
            Attributes.ForEach(item =>
            {
                if (!item.Name.IsNullOrWhiteSpace())
                {
                    options[item.Name] = item.ItemTags.FirstOrDefault();
                }
            });
            return options;
        }

        private void OnDropdownOptionsChanged(SKUModelOptions item, string value)
        {
            if (value == SKUModel.DropdownOptions.LastOrDefault())
            {
                item.CustomValueEnabled = true;
                item.SelectedSampleValue = value;
                GeneratePreview();
            }
            else
            {
                item.CustomValueEnabled = false;
                item.SelectedSampleValue = value;
                if (item.SelectedSampleValue.IsNullOrWhiteSpace())
                {
                    item.SampleDisplayValue = string.Empty;
                    GeneratePreview();
                    return;
                }
                item.SampleDisplayValue = SKUModel.AttributesDictionary[item.SelectedSampleValue];
                GeneratePreview();
            }
        }
        private void CharactersLengthInputChange(SKUModelOptions item, ChangeEventArgs e)
        {
            var value = e?.Value;
            if (int.TryParse(value?.ToString(), out int numericValue))
            {
                if (numericValue > 0 && numericValue < 7)
                {
                    item.CharactersLength = numericValue;
                    GeneratePreview();
                }
            }
        }

        private void GeneratePreview()
        {
            try
            {
                SKUModel.Preview = GenerateItemSKU(SKUModel.AttributesDictionary);
            }
            catch
            {
                SKUModel.Preview = "Error";
            }
        }

        private string? GenerateItemSKU(
            IDictionary<string, string?> attributesDictionary
            )
        {
            var preview = new StringBuilder();
            var lastItem = SKUModel.SKUModelOptions.LastOrDefault();

            foreach (var item in SKUModel.SKUModelOptions)
            {
                if (item.CustomValueEnabled)
                {
                    if (item.CustomValue?.Length > 6)
                    {
                        item.CustomValue = item.CustomValue[..6];
                    }
                    preview.Append(item.CustomValue);
                }

                else if (!string.IsNullOrWhiteSpace(item.SelectedSampleValue))
                {
                    var temp = attributesDictionary[item.SelectedSampleValue];

                    if (item.CharactersLength != null)
                    {
                        if (item.CharactersLength.Value > temp.Length)
                        {
                            preview.Append(temp);
                        }
                        else if (item.CharactersOrder == "F")
                        {
                            preview.Append(temp[..item.CharactersLength.Value]);
                        }
                        else
                        {
                            preview.Append(temp[(temp.Length - item.CharactersLength.Value)..]);
                        }
                    }
                }

                if (item != lastItem)
                {
                    preview.Append(SKUModel.SeparatedBy);
                }
            }
            return SKUModel.CaseUsed == "U" ? preview.ToString().ToUpper() : preview.ToString().ToLower();
        }

        private void CustomValueChanged(SKUModelOptions item, ChangeEventArgs e)
        {
            var value = e.Value.ToString();
            if (value.Length < 7)
            {
                item.CustomValue = value;
                GeneratePreview();
            }
        }
    }

    public class Attributes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ItemTags { get; set; }
        public string InputTagValue { get; set; }
        public bool TagInputVisible { get; set; }
    }

    public class SKUModel
    {
        public SKUModel() { }
        public SKUModel(
            List<string> options,
            IDictionary<string, string?> attributesDictionary
            )
        {
            DropdownOptions = options;
            AttributesDictionary = attributesDictionary;
        }
        public string? Preview { get; set; }
        public string? SeparatedBy { get; set; } = "/";
        public string? CaseUsed { get; set; } = "U";
        public List<SKUModelOptions> SKUModelOptions { get; set; }
                         = new List<SKUModelOptions>
                         {
                             new SKUModelOptions(),
                             new SKUModelOptions(),
                             new SKUModelOptions(),
                             new SKUModelOptions()
                         };
        public List<string> DropdownOptions { get; set; } = new();
        public IDictionary<string, string?> AttributesDictionary { get; set; } = new Dictionary<string, string?>();
    }

    public class SKUModelOptions
    {
        public string? SampleDisplayValue { get; set; }
        public string? SelectedSampleValue { get; set; }
        public string? CharactersOrder { get; set; } = "F";
        public int? CharactersLength { get; set; } = 3;
        public string? DropdownDisplayValue { get; set; } = "First";
        public bool CustomValueEnabled { get; set; } = false;
        public string? CustomValue { get; set; }
    }
}


