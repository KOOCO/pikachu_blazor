using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Extensions;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.ProductCategories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.ProductCategories;

public partial class ProductCategories
{
    private IReadOnlyList<ProductCategoryDto> ProductCategoryList { get; set; }
    private IReadOnlyList<ProductCategoryDto> ProductSubCategoryList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private HashSet<Guid> rowsWithDetail = [];
    private bool CanCreateProductCategory { get; set; }
    private bool CanEditProductCategory { get; set; }
    private bool CanDeleteProductCategory { get; set; }

    private GetProductCategoryListDto Filters { get; set; }

    private Modal DescriptionModal;
    private Modal CarousalModal;
    private ProductCategoryDto Selected { get; set; }

    private int SelectedSlide { get; set; }

    public ProductCategories()
    {
        ProductCategoryList = [];
        ProductSubCategoryList = [];
        Filters = new();
        Selected = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await GetProductCategoriesAsync();
        await SetPermissionsAsync();
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProductCategory = await AuthorizationService.IsGrantedAsync(PikachuPermissions.ProductCategories.Create);
        CanEditProductCategory = await AuthorizationService.IsGrantedAsync(PikachuPermissions.ProductCategories.Edit);
        CanDeleteProductCategory = await AuthorizationService.IsGrantedAsync(PikachuPermissions.ProductCategories.Delete);
    }
    async Task<bool> DisplayDetailRow(ProductCategoryDto category)
    {
        ProductSubCategoryList = await ProductCategoryAppService.GetSubCategoryListAsync(category.Id);
        return ProductSubCategoryList.Count > 0 ? true : false;


    }
    

    async void RowClicked(DataGridRowMouseEventArgs<ProductCategoryDto> clickedRow)
    {
        var id = clickedRow.Item.Id;
        if (!rowsWithDetail.Add(id))
            rowsWithDetail.Remove(id);
      ProductSubCategoryList=  await ProductCategoryAppService.GetSubCategoryListAsync(id);
        await InvokeAsync(StateHasChanged);
    }
    private async Task GetProductCategoriesAsync()
    {
        try
        {
            var result = await ProductCategoryAppService.GetListAsync(
                new GetProductCategoryListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter
                }
            );

            ProductCategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {

            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProductCategoryDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetProductCategoriesAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void Create()
    {
        NavigationManager.NavigateTo("/Product-Categories/Create");
    }
    private void CreateSub()
    {
        NavigationManager.NavigateTo("/Product-Categories/Create"+true);
    }
    private void Edit(ProductCategoryDto productCategory)
    {
        NavigationManager.NavigateTo("/Product-Categories/Edit/" + productCategory.Id);
    }
    private void EditSub(ProductCategoryDto productCategory)
    {
        NavigationManager.NavigateTo("/Product-Categories/Edit/" + productCategory.Id+"/"+true);
    }
    private async Task DeleteAsync(ProductCategoryDto productCategory)
    {
        try
        {
            var confirmation = await Message.Confirm(L["AreYouSureToDeleteThisRecord"], L["AreYouSure"]);
            if (!confirmation) return;
            await ProductCategoryAppService.DeleteAsync(productCategory.Id);
            await GetProductCategoriesAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetProductCategoriesAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        Filters = new();

        await GetProductCategoriesAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void ViewDescription(ProductCategoryDto productCategory)
    {
        Selected = productCategory;
        DescriptionModal.Show();
    }

    private void ViewCarousal(ProductCategoryDto productCategory)
    {
        Selected = productCategory;
        SelectedSlide = 0;
        Selected.ProductCategoryImages = [.. Selected.ProductCategoryImages.OrderBy(i => i.SortNo)];
        CarousalModal.Show();
    }
   
}