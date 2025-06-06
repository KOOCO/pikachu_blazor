﻿using AutoMapper;
using Blazored.TextEditor;
using Blazorise;
using Blazorise.Components;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ProductCategories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement;

public partial class EditItem
{
    #region Inject
    [Parameter]
    public string Id { get; set; }
    private const int MaxTextCount = 60; //input max length
    private const int MaxItemBadgeCount = 4; //input max length
    private const int MaxAllowedFilesPerUpload = 10;
    private const int TotalMaxAllowedFiles = 50;
    private const int MaxAllowedFileSize = 1024 * 1024 * 10;
    private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };

    private Guid EditingId { get; set; }
    private ItemDto ExistingItem { get; set; }

    private List<string> ItemTags { get; set; } = new List<string>(); //used for store item tags 
    private List<EnumValueDto> ShippingMethods { get; set; } // for bind all shippingMethods
    private List<EnumValueDto> TaxTypes { get; set; } // for bind all taxTypes
    private BlazoredTextEditor QuillHtml; //Item Discription Html
    private List<CreateItemDetailsDto> ItemDetailsList { get; set; } = new(); // List of CreateItemDetail dto to store PriceAndInventory
    private UpdateItemDto UpdateItemDto = new();
    private List<Attributes> Attributes = [];
    private string TagInputValue { get; set; }
    private Modal GenerateSKUModal { get; set; }
    private SKUModel SKUModel { get; set; } = new();
    private FilePicker FilePickerCustom { get; set; }
    private string QuillReadOnlyContent { get; set; } = "";
    private CreateItemDetailsDto draggedItem;
    private readonly IItemAppService _itemAppService;
    private readonly IEnumValueAppService _enumValueService;
    private readonly IUiMessageService _uiMessageService;
    private readonly ImageContainerManager _imageContainerManager;

    LoadingIndicator Loading { get; set; }
    int CurrentIndex { get; set; }

    private List<BlazoredTextEditor> ItemDetailsQuillHtml = [];

    private List<FilePicker> ItemDetailPickers = [];

    private bool OnDetailClose = false;

    private bool _isRenderComplete = false;

    private List<KeyValueDto> ProductCategoryLookup { get; set; } = [];
    private bool RowLoading { get; set; } = false;
    private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
    private string SelectedAutoCompleteText { get; set; }
    private Dictionary<CreateItemDetailsDto, string> ValidationErrors = new();
    private const int MaxBadgeTextCount = 4;
    private List<ItemBadgeDto> ItemBadgeList { get; set; } = [];
    private ItemBadgeDto NewItemBadge { get; set; } = new();
    private bool SelectOpen { get; set; } = false;
    private object TooltipVisibleItem;
    #endregion

    #region Constructor
    public EditItem(
        IEnumValueAppService enumValueService,
        IItemAppService itemAppService,
        IUiMessageService uiMessageService,
        ImageContainerManager imageContainerManager
    )
    {
        _enumValueService = enumValueService;
        _itemAppService = itemAppService;
        _uiMessageService = uiMessageService;
        _imageContainerManager = imageContainerManager;
    }
    #endregion

    #region Methods
    private void NavigateToItemList()
    {
        NavigationManager.NavigateTo("/items");
    }
    protected override async Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (isFirstRender)
        {
            try
            {
                await Loading.Show();
                await JS.InvokeVoidAsync("updateDropText");
                EditingId = Guid.Parse(Id);
                ExistingItem = await _itemAppService.GetAsync(EditingId, true);
                await GetItemBadgeListAsync();
                var enumValues = (await _enumValueService.GetEnums(new List<EnumType> {
                                                         EnumType.ShippingMethod,
                                                         EnumType.TaxType
                                                     })).ToList();
                TaxTypes = enumValues.Where(x => x.EnumType == EnumType.TaxType).ToList();
                ShippingMethods = enumValues.Where(x => x.EnumType == EnumType.ShippingMethod).ToList();
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
                // Create a new mapper
                var mapper = config.CreateMapper();
                // Map ItemDto to UpdateItemDto
                UpdateItemDto = mapper.Map<UpdateItemDto>(ExistingItem);
                UpdateItemDto.ItemBadgeDto = new ItemBadgeDto { ItemBadge = ExistingItem.ItemBadge, ItemBadgeColor = ExistingItem.ItemBadgeColor };
                UpdateItemDto.Images = UpdateItemDto.Images.OrderBy(x => x.SortNo).ToList();
                ItemDetailsList = mapper.Map<List<CreateItemDetailsDto>>(ExistingItem.ItemDetails);
                foreach (var item in ItemDetailsList.Where(x => x.StockOnHand < x.SaleableQuantity))
                {
                    ValidateQuantity(item);
                }
                ItemTags = ExistingItem.ItemTags?.Split(',').ToList() ?? [];
                if (!ExistingItem.Attribute1Name.IsNullOrWhiteSpace())
                {
                    Attributes.Add(
                        new Attributes
                        {
                            Id = 1,
                            Name = L[ExistingItem.Attribute1Name],
                            ItemTags = ItemDetailsList.Where(x => x.Attribute1Value != null).Select(x => x.Attribute1Value).Distinct().ToList()
                        });
                }
                if (!ExistingItem.Attribute2Name.IsNullOrWhiteSpace())
                {
                    Attributes.Add(
                        new Attributes
                        {
                            Id = 2,
                            Name = L[ExistingItem.Attribute2Name],
                            ItemTags = ItemDetailsList.Where(x => x.Attribute2Value != null).Select(x => x.Attribute2Value).Distinct().ToList()

                        });
                }
                if (!ExistingItem.Attribute3Name.IsNullOrWhiteSpace())
                {
                    Attributes.Add(
                        new Attributes
                        {
                            Id = 3,
                            Name = L[ExistingItem.Attribute3Name],
                            ItemTags = ItemDetailsList.Where(x => x.Attribute3Value != null).Select(x => x.Attribute3Value).Distinct().ToList()
                        });
                }
                if (ExistingItem.Attribute1Name.IsNullOrWhiteSpace() && ExistingItem.Attribute2Name.IsNullOrWhiteSpace() && ExistingItem.Attribute3Name.IsNullOrWhiteSpace())
                {
                    Attributes.Add(new Attributes
                    {
                        Id = 1,
                        //Name = L["ItemStyle1"],
                        ItemTags = []
                    });
                }
                await LoadHtmlContent();

                var itemCategories = await _itemAppService.GetItemCategoriesAsync(EditingId);
                UpdateItemDto.ItemCategories = ObjectMapper.Map<List<CategoryProductDto>, List<CreateUpdateItemCategoryDto>>(itemCategories);
                ProductCategoryLookup = await ProductCategoryAppService.GetProductCategoryLookupAsync();

                var itemCategoryIds = itemCategories.Select(ic => ic.ProductCategoryId);
                ProductCategoryLookup.RemoveAll(pc => itemCategoryIds.Contains(pc.Id));
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.Message.ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                await Loading.Hide();
            }
        }
    }

    private async Task LoadHtmlContent()
    {
        await Task.Delay(1);
        await QuillHtml.LoadHTMLContent(ExistingItem.ItemDescription);

        if (ItemDetailsList is { Count: > 0 })
        {
            for (int i = 0; i < ItemDetailsList.Count; i++)
            {
                CreateItemDetailsDto item = ItemDetailsList[i];

                ItemDetailPickers.Add(new());

                BlazoredTextEditor textEditor = new();

                ItemDetailsQuillHtml.Add(textEditor);

                await InvokeAsync(StateHasChanged);
            }
        }
    }

    protected virtual async Task UpdateEntityAsync()
    {
        try
        {
            await Loading.Show();
            ValidateForm();
            GenerateAttributesForItemDetails();

            UpdateItemDto.ItemDetails = ItemDetailsList;
            UpdateItemDto.ItemDescription = await QuillHtml.GetHTML();

            UpdateItemDto.ItemTags = ItemTags.Any() ? string.Join(",", ItemTags) : null;

            await _itemAppService.UpdateAsync(EditingId, UpdateItemDto);
            NavigationManager.NavigateTo("Items");
        }
        catch (BusinessException ex)
        {
            await _uiMessageService.Error(ex.Code.ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    private string LocalizeFilePicker(string key, object[] args)
    {
        return L[key];
    }
    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        if (e.Files != null && e.Files.Length == 0) return;

        try
        {
            if (e.Files.Length > MaxAllowedFilesPerUpload)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
                await FilePickerCustom.Clear();
                return;
            }
            if (UpdateItemDto.Images.Count > TotalMaxAllowedFiles)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.AlreadyUploadMaxAllowedFiles]);
                await FilePickerCustom.Clear();
                return;
            }
            var count = 0;

            await Loading.Show();

            foreach (var file in e.Files)
            {
                if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name).ToLower()))
                {
                    await FilePickerCustom.RemoveFile(file);
                    return;
                }

                string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(file.Name));

                var bytes = await file.GetBytes();

                var compressed = await ImageCompressorService.CompressAsync(bytes);

                if (compressed.CompressedSize > MaxAllowedFileSize)
                {
                    count++;
                    await FilePickerCustom.RemoveFile(file);
                    return;
                }

                var url = await _imageContainerManager.SaveAsync(newFileName, compressed.CompressedBytes);

                int sortNo = 0;
                if (UpdateItemDto.Images.Any())
                {
                    sortNo = UpdateItemDto.Images.Max(x => x.SortNo);
                }

                UpdateItemDto.Images.Add(new CreateImageDto
                {
                    Name = file.Name,
                    BlobImageName = newFileName,
                    ImageUrl = url,
                    ImageType = ImageType.Item,
                    SortNo = sortNo + 1
                });

                await FilePickerCustom.Clear();
            }
            if (count > 0)
            {
                await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
            await FilePickerCustom.Clear();
            await Loading.Hide();
        }
    }

    async Task DeleteImageAsync(string blobImageName)
    {
        try
        {
            var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                await Loading.Show();
                confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                await _itemAppService.DeleteSingleImageAsync(EditingId, blobImageName);

                UpdateItemDto.Images = UpdateItemDto.Images.Where(x => x.BlobImageName != blobImageName).ToList();
            }
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
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
    async Task OnAttributeFieldTagClose(int id, string item)
    {
        Attributes.First(x => x.Id == id).ItemTags.Remove(item);
        await BindItemDetailList();
    }

    /// <summary>
    /// On custom Fields tag confirm
    /// </summary>
    async Task HandleAttributeTagInputConfirm(int id, string tag, KeyboardEventArgs e)
    {
        if (e is not null && e.Key != "Enter")
            return;

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
            await BindItemDetailList();
        }
        attribute.InputTagValue = "";

    }

    async Task DeleteAttribute(Attributes attribute)
    {
        int index = Attributes.IndexOf(attribute);
        Attributes.Remove(attribute);
        await BindItemDetailList();
    }

    void AddAttribute()
    {
        if (Attributes.Count == 0 || (Attributes[0].ItemTags.Count > 0 && Attributes.Count < 3))
        {
            var attribute = Attributes.OrderByDescending(x => x.Id).FirstOrDefault();
            Attributes.Add(new Attributes
            {
                Id = attribute == null ? 1 : +attribute.Id + 1,
                //Name =L["ItemStyle" + (attribute == null ? 1 : +attribute.Id + 1)],
                ItemTags = new List<string>()
            });
        }
    }
    async Task BindItemDetailList()
    {
        //if (ItemDetailsList.Any(a => !a.Image.IsNullOrEmpty()))
        //{
        //    bool confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);

        //    if (!confirmed) return;

        //    foreach (CreateItemDetailsDto item in ItemDetailsList.Where(w => !w.Image.IsNullOrEmpty()).ToList())
        //    {
        //        await _imageContainerManager.DeleteAsync(item.Image);
        //    }
        //}

        ItemDetailsQuillHtml = []; ItemDetailPickers = [];

        ItemDetailsList ??= [];
        if (Attributes.Count == 0)
        {
            ItemDetailsList.Clear(); // If no attributes exist, clear the list
            return;
        }

        // Generate all valid permutations based on current attributes
        List<List<string>> permutations = GeneratePermutations(Attributes
            .Select(x => x.ItemTags.Count != 0 ? x.ItemTags : [""])
            .ToList());

        HashSet<string> validItemNames = permutations
            .Select(permutation => string.Join("/", permutation).TrimEnd('/'))
            .ToHashSet();

        // Remove items that are no longer valid
        ItemDetailsList.RemoveAll(x => !validItemNames.Contains(x.ItemName));

        // Set sortNo based on the updated list
        int sortNo = ItemDetailsList.Count != 0 ? ItemDetailsList.Max(x => x.SortNo) + 1 : 1;

        foreach (string itemName in validItemNames)
        {
            // Add new items that don't already exist
            if (!ItemDetailsList.Any(x => x.ItemName == itemName))
            {
                ItemDetailsList.Add(new CreateItemDetailsDto
                {
                    ItemName = itemName,
                    Status = true,
                    StockOnHand = 0,
                    SortNo = sortNo
                });
                sortNo++;

                ItemDetailsQuillHtml.Add(new());
                ItemDetailPickers.Add(new());
            }
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

    private void GenerateSKU()
    {
        try
        {
            GenerateAttributesForItemDetails();
            ItemDetailsList.ForEach(item =>
            {
                IDictionary<string, string?> itemOptions = new Dictionary<string, string?>();

                if (!UpdateItemDto.Attribute1Name.IsNullOrWhiteSpace())
                {
                    itemOptions[UpdateItemDto.Attribute1Name] = item.Attribute1Value;
                }
                if (!UpdateItemDto.Attribute2Name.IsNullOrWhiteSpace())
                {
                    itemOptions[UpdateItemDto.Attribute2Name] = item.Attribute2Value;
                }
                if (!UpdateItemDto.Attribute3Name.IsNullOrWhiteSpace())
                {
                    itemOptions[UpdateItemDto.Attribute3Name] = item.Attribute3Value;
                }
                item.Sku = GenerateItemSKU(itemOptions);
                item.Sku = item.Sku.Trim(SKUModel.SeparatedBy[0]);
            });

            CloseGenerateSKUModal();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    private void ValidateForm()
    {
        if (UpdateItemDto.ItemName.IsNullOrWhiteSpace())
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
        }

        if (ItemDetailsList.Any(x => x.Sku.IsNullOrWhiteSpace()))
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.SKUForItemDetailsCannotBeNull]);
        }
        //if (Attributes.Any(x => x.Name.IsNullOrEmpty()))
        //{
        //    throw new BusinessException(L[PikachuDomainErrorCodes.ItemStyleKeyCannotBeNull]);

        //}
        if (Attributes.GroupBy(x => x.Name).Any(g => g.Count() > 1))
        {

            throw new BusinessException(L[PikachuDomainErrorCodes.ItemStyleKeyCannotBeDuplicate]);

        }
        if (ItemDetailsList.Any(x => ItemDetailsList.Any(y => y != x && y.Sku == x.Sku)))
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.ItemWithSKUAlreadyExists]);
        }

        if (ItemDetailsList.Any(x => x.SellingPrice < 0))
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.SellingPriceForItemShouldBeGreaterThanZero]);
        }
    }

    private void GenerateAttributesForItemDetails()
    {
        int attributeCount = Attributes.Count;
        var customFields = Attributes.ToArray();

        var validItemNames = new Dictionary<string, List<string>>();
        var allCombinations = new List<List<string>>();

        for (int i = 0; i < attributeCount; i++)
        {
            if (customFields[i].ItemTags.Any())
            {
                allCombinations.Add(customFields[i].ItemTags);
            }
        }

        var combinations = GenerateCombinations(allCombinations);

        // Create a mapping of itemName → attribute combination
        foreach (var combination in combinations)
        {
            string itemName = string.Join("/", combination).TrimEnd('/');
            validItemNames[itemName] = combination;
        }

        // Assign attributes correctly based on item names
        foreach (var item in ItemDetailsList)
        {
            if (validItemNames.TryGetValue(item.ItemName, out var combination))
            {
                item.Attribute1Value = combination.Count > 0 ? combination[0] : null;
                item.Attribute2Value = combination.Count > 1 ? combination[1] : null;
                item.Attribute3Value = combination.Count > 2 ? combination[2] : null;
            }
        }

        // Set AttributeNames in CreateItemDto
        if (attributeCount > 0)
        {
            UpdateItemDto.Attribute1Name = customFields[0].Name;
        }
        else
        {
            UpdateItemDto.Attribute1Name = null;
        }

        if (attributeCount > 1)
        {
            UpdateItemDto.Attribute2Name = customFields[1].Name;
        }
        else
        {
            UpdateItemDto.Attribute2Name = null;
        }

        if (attributeCount > 2)
        {
            UpdateItemDto.Attribute3Name = customFields[2].Name;
        }
        else
        {
            UpdateItemDto.Attribute3Name = null;
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
                        if (propertyName == "StockOnHand" || propertyName == "SaleableQuantity")
                        {
                            ValidateQuantity(item);
                        }
                    }
                }
            }
        }
        catch
        {
            _uiMessageService.Error(L[PikachuDomainErrorCodes.SystemIsUnableToCopyAtTheMoment]);
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
        options.Add(L["ItemName"]);
        options.Add(L["SelectCustomText"]);
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
            if (value == L["ItemName"])
            {
                item.CustomValueEnabled = false;
                item.SelectedSampleValue = value;
                item.SampleDisplayValue = UpdateItemDto.ItemName;
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
                var temp = "";
                if (item.SelectedSampleValue == L["ItemName"])
                {
                    temp = UpdateItemDto.ItemName;
                }
                else
                {
                    temp = attributesDictionary[item.SelectedSampleValue];
                }
                if (temp != null && item.CharactersLength != null)
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

    private void CancelToItemList()
    {
        NavigationManager.NavigateTo("Items");
    }

    void StartDrag(CreateImageDto image)
    {
        CurrentIndex = UpdateItemDto.Images.IndexOf(image);
    }

    void Drop(CreateImageDto image)
    {
        if (image != null)
        {
            var index = UpdateItemDto.Images.IndexOf(image);

            var current = UpdateItemDto.Images[CurrentIndex];

            UpdateItemDto.Images.RemoveAt(CurrentIndex);
            UpdateItemDto.Images.Insert(index, current);

            CurrentIndex = index;

            for (int i = 0; i < UpdateItemDto.Images.Count; i++)
            {
                UpdateItemDto.Images[i].SortNo = i + 1;
            }
            StateHasChanged();
        }
    }

    public async Task ToggleExpanded(CreateItemDetailsDto item)
    {
        item.IsExpanded = !item.IsExpanded;

        _isRenderComplete = item.IsExpanded;

        StateHasChanged();
        await JS.InvokeVoidAsync("updateDropText");
    }

    public async Task OnImageUploadAsync(
        FileChangedEventArgs e,
        CreateItemDetailsDto item,
        FilePicker filePicker
    )
    {
        try
        {
            if (e.Files.Length is 0) return;

            if (e.Files.Length > 1)
            {
                await _uiMessageService.Error("Cannot add more 1 image.");

                await filePicker.Clear();

                return;
            }

            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await _uiMessageService.Error(L["InvalidFileType"]);

                await filePicker.Clear();

                return;
            }

            string newFileName = Path.ChangeExtension(
                Guid.NewGuid().ToString().Replace("-", ""),
                Path.GetExtension(e.Files[0].Name)
            );

            var bytes = await e.Files[0].GetBytes();

            var compressed = await ImageCompressorService.CompressAsync(bytes);

            string url = await _imageContainerManager.SaveAsync(newFileName, compressed.CompressedBytes);

            item.Image = url;

            await filePicker.Clear();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public async Task DeleteImageAsync(MouseEventArgs e, CreateItemDetailsDto item)
    {
        bool confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);

        if (confirmed)
        {
            await _imageContainerManager.DeleteAsync(item.Image);

            item.Image = string.Empty;

            StateHasChanged();
        }
    }

    private async Task OnSelectedValueChanged(Guid? id)
    {
        try
        {
            if (id != null)
            {
                RowLoading = true;
                StateHasChanged();
                await AutocompleteField.Clear();
                var productCategory = ProductCategoryLookup.Where(x => x.Id == id).FirstOrDefault();

                if (productCategory == null) return;

                var itemCategory = new CreateUpdateItemCategoryDto
                {
                    ProductCategoryId = productCategory.Id,
                    ProductCategoryName = productCategory.Name,
                    ImageUrl = await ProductCategoryAppService.GetDefaultImageUrlAsync(productCategory.Id)
                };

                UpdateItemDto.ItemCategories.Add(itemCategory);
                RowLoading = false;

                ProductCategoryLookup = ProductCategoryLookup.Where(x => x.Id != id).ToList();
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            RowLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    private void RemoveItemCategory(CreateUpdateItemCategoryDto itemCategory)
    {
        if (itemCategory != null)
        {
            UpdateItemDto.ItemCategories.Remove(itemCategory);
            ProductCategoryLookup.Add(new KeyValueDto
            {
                Id = itemCategory.ProductCategoryId!.Value,
                Name = itemCategory.ProductCategoryName
            });

            StateHasChanged();
        }
    }
    private void OnStockOnHandChanged(ChangeEventArgs e, CreateItemDetailsDto item)
    {
        if (int.TryParse(e.Value?.ToString(), out var value))
        {
            item.StockOnHand = value; // Update the model
            ValidateQuantity(item);  // Validate the row
        }
    }

    private void OnSaleableQuantityChanged(ChangeEventArgs e, CreateItemDetailsDto item)
    {
        if (int.TryParse(e.Value?.ToString(), out var value))
        {
            item.SaleableQuantity = value; // Update the model
            ValidateQuantity(item);       // Validate the row
        }
    }

    private void ValidateQuantity(CreateItemDetailsDto item)
    {
        if (item.SaleableQuantity > item.StockOnHand)
        {
            ValidationErrors[item] = L["SaleableQuantitycannotbegreaterthanCurrentStock"];
        }
        else
        {
            ValidationErrors.Remove(item);
        }

        StateHasChanged(); // Refresh UI
    }

    private void OnDragStart(DragEventArgs e, CreateItemDetailsDto item)
    {
        draggedItem = item; // Store the item being dragged
    }

    private async Task OnDragOver(DragEventArgs e)
    {
        // Call JavaScript to prevent default drag behavior
        await JS.InvokeVoidAsync("preventDefaultDrag", e);
    }

    private void OnDrop(DragEventArgs e, CreateItemDetailsDto targetItem)
    {
        if (draggedItem == null || draggedItem == targetItem)
            return;

        int fromIndex = ItemDetailsList.IndexOf(draggedItem);
        int toIndex = ItemDetailsList.IndexOf(targetItem);

        // Rearrange items in the list
        ItemDetailsList.RemoveAt(fromIndex);
        ItemDetailsList.Insert(toIndex, draggedItem);


        for (int i = 0; i < ItemDetailsList.Count; i++)
        {
            ItemDetailsList[i].SortNo = i + 1;
        }
        StateHasChanged();
        draggedItem = null; // Reset dragged item
    }

    private void AddItem()
    {
        if (!string.IsNullOrWhiteSpace(NewItemBadge?.ItemBadge))
        {
            var newItemBadge = new ItemBadgeDto { ItemBadge = NewItemBadge.ItemBadge, ItemBadgeColor = NewItemBadge.ItemBadgeColor };
            ItemBadgeList.Add(newItemBadge);
            UpdateItemDto.ItemBadgeDto = newItemBadge;
            NewItemBadge = new();
        }
    }

    async Task DeleteItemBadge(ItemBadgeDto itemBadge)
    {
        try
        {
            var confirmation = await Message.Confirm(L["AreYouSureToDeleteThisBadge"]);
            if (confirmation)
            {
                await _itemAppService.DeleteItemBadgeAsync(itemBadge);
                await GetItemBadgeListAsync();
            }

            SelectOpen = true;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task GetItemBadgeListAsync()
    {
        ItemBadgeList = await _itemAppService.GetItemBadgesAsync();
    }
    private void ShowTooltipFor(object item)
    {
        TooltipVisibleItem = item;
    }

    private void HideTooltip()
    {
        TooltipVisibleItem = null;
    }

    private string GetInputClass(CreateItemDetailsDto item)
    {
        return ValidationErrors.ContainsKey(item)
            ? "form-control form-control-sm border-danger"
            : "form-control form-control-sm";
    }
    #endregion
}
