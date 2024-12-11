using Blazorise;
using Kooco.Pikachu.WebsiteManagement;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using Blazorise.Components;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.ProductCategories;
using Blazored.TextEditor;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class AddWebsiteSettings
{
    private CreateWebsiteSettingsDto NewEntity { get; set; }
    private Validations ValidationsRef;

    private bool IsLoading { get; set; }

    private string LogoBase64 { get; set; }
    private List<KeyValueDto> ProductCategoryLookup { get; set; } = [];
    private string SelectedAutoCompleteText { get; set; }
    private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
    private BlazoredTextEditor ArticlePageHtml;
    public AddWebsiteSettings()
    {
        NewEntity = new();
    }

    void NavigateToWebsiteSettings()
    {
        NavigationManager.NavigateTo("/Website-Settings");
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        var file = e.Files.FirstOrDefault();

        if (file != null)
        {
            string extension = Path.GetExtension(file.Name);

            if (file.Size > Constant.MaxImageSizeInBytes)
            {
                await UiNotificationService.Error(L["Pikachu:ImageSizeExceeds", Constant.MaxImageSizeInBytes.FromBytesToMB()]);
                return;
            }

            if (!Constant.ValidImageExtensions.Contains(extension))
            {
                await UiNotificationService.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", Constant.ValidImageExtensions)]);
                return;
            }

            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            LogoBase64 = Convert.ToBase64String(fileBytes);
            NewEntity.LogoName = file.Name;

            await InvokeAsync(StateHasChanged);
        }
    }

    async Task CreateAsync()
    {
        var validate = await ValidationsRef.ValidateAll();
        if (!validate) return;
        try
        {
            IsLoading = true;

            var bytes = Convert.FromBase64String(LogoBase64);
            NewEntity.LogoUrl = await ImageAppService.UploadImageAsync(NewEntity.LogoName, bytes);

            await WebsiteSettingsAppService.CreateAsync(NewEntity);
            NavigateToWebsiteSettings();
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task WebsitePageTypeChanged(WebsitePageType? websitePageType)
    {
        NewEntity.WebsitePageType = websitePageType;
    }

    async Task OnSelectedValueChanged(Guid? id)
    {
        //try
        //{
        //    if (id != null)
        //    {
        //        RowLoading = true;
        //        StateHasChanged();
        //        await AutocompleteField.Clear();
        //        var productCategory = ProductCategoryLookup.Where(x => x.Id == id).FirstOrDefault();

        //        if (productCategory == null) return;

        //        var itemCategory = new CreateUpdateItemCategoryDto
        //        {
        //            ProductCategoryId = productCategory.Id,
        //            ProductCategoryName = productCategory.Name,
        //            ImageUrl = await ProductCategoryAppService.GetDefaultImageUrlAsync(productCategory.Id)
        //        };

        //        CreateItemDto.ItemCategories.Add(itemCategory);
        //        RowLoading = false;

        //        ProductCategoryLookup = ProductCategoryLookup.Where(x => x.Id != id).ToList();
        //        StateHasChanged();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    RowLoading = false;
        //    await HandleErrorAsync(ex);
        //}
    }
}