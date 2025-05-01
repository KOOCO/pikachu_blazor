using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.ShopCarts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.ShopCarts;

public partial class ShopCarts
{
    private IReadOnlyList<ShopCartListWithDetailsDto> ShopCartsList { get; set; }
    private IReadOnlyList<string> MemberStatusOptions { get; set; }
    private IReadOnlyList<string> VipTierOptions { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }

    private GetShopCartListDto Filters { get; set; }

    private ShopCartListWithDetailsDto SelectedShopCart { get; set; }
    private List<ShopCartListWithDetailsDto> SelectedShopCarts { get; set; }
    private bool FiltersVisible { get; set; } = false;
    private bool IsClearing { get; set; }

    private CartItemsModal CartItemsModal;

    public ShopCarts()
    {
        ShopCartsList = [];
        MemberStatusOptions = [];
        Filters = new();
        SelectedShopCarts = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await GetShopCartsAsync();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                MemberStatusOptions = [.. MemberConsts.MemberTags.Names];
                VipTierOptions = await VipTierSettingAppService.GetVipTierNamesAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private async Task GetShopCartsAsync()
    {
        try
        {
            SelectedShopCarts = [];

            var result = await ShopCartAppService.GetListWithDetailsAsync(
                new GetShopCartListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    UserId = Filters.UserId,
                    GroupBuyId = Filters.GroupBuyId,
                    MinItems = Filters.MinItems,
                    MaxItems = Filters.MaxItems,
                    MinAmount = Filters.MinAmount,
                    MaxAmount = Filters.MaxAmount,
                    VipTier = Filters.VipTier,
                    MemberStatus = Filters.MemberStatus
                }
            );

            ShopCartsList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ShopCartListWithDetailsDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetShopCartsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task ClearAsync()
    {
        try
        {
            var confirm = await Message.Confirm(L["AreYouSureYouWantToClearTheCarts"]);
            if (!confirm) return;

            IsClearing = true;
            StateHasChanged();

            var ids = SelectedShopCarts.Select(sc => sc.Id).ToList();

            await ShopCartAppService.ClearCartItemsAsync(ids);

            IsClearing = false;

            await GetShopCartsAsync();
        }
        catch (Exception ex)
        {
            IsClearing = false;
            await HandleErrorAsync(ex);
        }
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetShopCartsAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetFilters()
    {
        CurrentPage = 1;

        Filters = new();

        await GetShopCartsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task Edit(ShopCartListWithDetailsDto shopCart)
    {
        SelectedShopCart = shopCart;
        await CartItemsModal.Show(shopCart);
    }

    private void EditMember(ShopCartListWithDetailsDto shopCart)
    {
        NavigationManager.NavigateTo("/Members/Details/" + shopCart.UserId);
    }

    private static bool RowSelectableHandler(RowSelectableEventArgs<ShopCartListWithDetailsDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
}