using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.WebsiteManagement;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class WebsiteSettings
{
    private IReadOnlyList<WebsiteSettingsDto> WebsiteSettingsList { get; set; }
    private List<KeyValueDto> ProductCategories { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private GetWebsiteSettingsListDto Filters { get; set; }
    private bool FiltersVisible { get; set; }
    private bool CanCreateWebsiteSettings { get; set; }
    private bool CanEditWebsiteSettings { get; set; }
    private bool CanDeleteWebsiteSettings { get; set; }

    public WebsiteSettings()
    {
        WebsiteSettingsList = [];
        ProductCategories = [];
        Filters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetWebsiteSettingsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ProductCategories = await ProductCategoryAppService.GetProductCategoryLookupAsync();
        }
    }

    async Task SetPermissionsAsync()
    {
        CanCreateWebsiteSettings = await AuthorizationService
            .IsGrantedAsync(PikachuPermissions.WebsiteManagement.WebsiteSettings.Create);

        CanEditWebsiteSettings = await AuthorizationService
            .IsGrantedAsync(PikachuPermissions.WebsiteManagement.WebsiteSettings.Edit);

        CanDeleteWebsiteSettings = await AuthorizationService
            .IsGrantedAsync(PikachuPermissions.WebsiteManagement.WebsiteSettings.Delete);
    }

    async Task GetWebsiteSettingsAsync()
    {
        try
        {
            var result = await WebsiteSettingsAppService.GetListAsync(
                new GetWebsiteSettingsListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    PageTitle = Filters.PageTitle,
                    PageType = Filters.PageType,
                    ProductCategoryId = Filters.ProductCategoryId,
                    TemplateType = Filters.TemplateType,
                    SetAsHomePage = Filters.SetAsHomePage
                });

            WebsiteSettingsList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task OnDataGridReadAsync(DataGridReadDataEventArgs<WebsiteSettingsDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetWebsiteSettingsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task DeleteAsync(WebsiteSettingsDto websiteSettings)
    {
        try
        {
            var confirm = await Message.Confirm("AreYouSureToDeleteThis?");
            if (confirm)
            {
                await WebsiteSettingsAppService.DeleteAsync(websiteSettings.Id);
                await GetWebsiteSettingsAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetWebsiteSettingsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        Filters = new();

        await GetWebsiteSettingsAsync();

        await InvokeAsync(StateHasChanged);
    }
}