using Blazored.TextEditor;
using Blazorise;
using Blazorise.Components;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.ProductCategories;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ProductCategories;

public partial class EditProductCategory
{
    [Parameter]
    public Guid Id { get; set; }
    private ProductCategoryDto Selected { get; set; }
    private UpdateProductCategoryDto EditingEntity { get; set; }
    private Validations ValidationsRef;
    private BlazoredTextEditor DescriptionHtml { get; set; }
    private bool IsLoading { get; set; }
    private FilePicker FilePicker;
    private int CurrentIndex { get; set; }

    private Autocomplete<ItemWithItemTypeDto, Guid?> AutocompleteField { get; set; }
    private string? SelectedAutoCompleteText { get; set; }
    private List<ItemWithItemTypeDto> ItemsLookup { get; set; }
    private bool RowLoading { get; set; } = false;

    public EditProductCategory()
    {
        EditingEntity = new();
        ItemsLookup = [];
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Selected = await ProductCategoryAppService.GetAsync(Id, true);
            EditingEntity = ObjectMapper.Map<ProductCategoryDto, UpdateProductCategoryDto>(Selected);
            await LoadHtmlContent();
            await PopulateItems();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                ItemsLookup = await ItemAppService.GetItemsLookupAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private async Task PopulateItems()
    {
        var itemIds = EditingEntity.CategoryProducts.Where(x => x.ItemId.HasValue).Select(x => x.ItemId!.Value).ToList();

        var items = await ItemAppService.GetManyAsync(itemIds);

        EditingEntity.CategoryProducts.ForEach(product =>
        {
            var item = items.Where(i => i.Id == product.ItemId).FirstOrDefault();
            product.Item = item;
            product.ItemImageUrl = item?.Images?.FirstOrDefault()?.ImageUrl;
        });

        ItemsLookup.RemoveAll(item => itemIds.Contains(item.Id));
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadHtmlContent()
    {
        await Task.Delay(200);
        await DescriptionHtml.LoadHTMLContent(EditingEntity.Description);
    }

    private void NavigateToProductCategory()
    {
        NavigationManager.NavigateTo("Product-Categories");
    }
    private string LocalizeFilePicker(string key, object[] args)
    {
        return L[key];
    }
    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        if (e?.Files == null)
        {
            await FilePicker.Clear();
            return;
        }

        if ((EditingEntity.ProductCategoryImages.Count + e.Files.Length) > ProductCategoryConsts.MaxImageLimit)
        {
            await Message.Error(L[PikachuDomainErrorCodes.ProductCategoryImageMaxLimit]);
            return;
        }

        if (e.Files.Any(file => file.Size > Constant.MaxImageSizeInBytes))
        {
            await Message.Error(L["Pikachu:ImageSizeExceeds", Constant.MaxImageSizeInBytes.FromBytesToMB()]);
            return;
        }

        if (e.Files.Any(file => !Constant.ValidImageExtensions.Contains(Path.GetExtension(file.Name))))
        {
            await Message.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", Constant.ValidImageExtensions)]);
            return;
        }

        foreach (var file in e.Files)
        {
            if (file != null)
            {
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var sortNo = EditingEntity.ProductCategoryImages.Count > 0 ? EditingEntity.ProductCategoryImages.Max(i => i.SortNo) : 1;

                var base64 = Convert.ToBase64String(fileBytes);
                EditingEntity.ProductCategoryImages.Add(new CreateUpdateProductCategoryImageDto
                {
                    Base64 = base64,
                    Name = file.Name,
                    SortNo = sortNo
                });
            }
        }

        await InvokeAsync(StateHasChanged);
    }

    async Task UpdateAsync()
    {
        await ValidationsRef.ValidateAll();

        var validate = await ValidationsRef.ValidateAll();
        if (!validate) return;

        try
        {
            IsLoading = true;
            EditingEntity.Description = await DescriptionHtml.GetHTML();

            List<string> oldBlobNames = [];
            foreach (var image in EditingEntity.ProductCategoryImages.Where(i => !i.Base64.IsNullOrWhiteSpace()))
            {
                oldBlobNames.Add(image.BlobName);
                var bytes = Convert.FromBase64String(image.Base64);
                image.BlobName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(image.Name);
                image.Url = await ImageContainerManager.SaveAsync(image.BlobName, bytes);
            }

            await ProductCategoryAppService.UpdateAsync(Id, EditingEntity);
            if (oldBlobNames.Count > 0)
            {
                await DeleteOldImagesAsync(oldBlobNames).ConfigureAwait(false);
            }
            NavigateToProductCategory();
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    void StartDrag(CreateUpdateProductCategoryImageDto image)
    {
        CurrentIndex = EditingEntity.ProductCategoryImages.IndexOf(image);
    }

    void Drop(CreateUpdateProductCategoryImageDto image)
    {
        if (image != null)
        {
            var index = EditingEntity.ProductCategoryImages.IndexOf(image);

            var current = EditingEntity.ProductCategoryImages[CurrentIndex];

            EditingEntity.ProductCategoryImages.RemoveAt(CurrentIndex);
            EditingEntity.ProductCategoryImages.Insert(index, current);

            CurrentIndex = index;

            for (int i = 0; i < EditingEntity.ProductCategoryImages.Count; i++)
            {
                EditingEntity.ProductCategoryImages[i].SortNo = i + 1;
            }
            StateHasChanged();
        }
    }

    void DeleteImage(CreateUpdateProductCategoryImageDto image)
    {
        EditingEntity.ProductCategoryImages.Remove(image);
        StateHasChanged();
    }

    private async Task DeleteOldImagesAsync(List<string> oldBlobNames)
    {
        foreach (var oldBlobName in oldBlobNames)
        {
            try
            {
                await ImageContainerManager.DeleteAsync(oldBlobName);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }

    private async Task OnSelectedValueChanged(Guid? id)
    {
        try
        {
            if (id != null)
            {
                RowLoading = true;
                await AutocompleteField.Clear();
                var item = await ItemAppService.GetAsync(id.Value);

                var categoryProduct = new CreateUpdateCategoryProductDto
                {
                    ItemId = item.Id,
                    Item = item,
                    ItemImageUrl = await ItemAppService.GetFirstImageUrlAsync(id.Value)
                };

                EditingEntity.CategoryProducts.Add(categoryProduct);
                RowLoading = false;

                ItemsLookup = ItemsLookup.Where(x => x.Id != id).ToList();
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            RowLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    private void RemoveCategoryProduct(CreateUpdateCategoryProductDto categoryProduct)
    {
        if (categoryProduct != null)
        {
            EditingEntity.CategoryProducts.Remove(categoryProduct);
            ItemsLookup.Add(new ItemWithItemTypeDto
            {
                Id = categoryProduct.ItemId!.Value,
                Name = categoryProduct.Item?.ItemName
            });

            StateHasChanged();
        }
    }
}