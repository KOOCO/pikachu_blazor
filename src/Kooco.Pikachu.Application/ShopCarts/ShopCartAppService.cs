using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShopCarts;

[RemoteService(IsEnabled = false)]
//[Authorize]
public class ShopCartAppService(ShopCartManager shopCartManager, IShopCartRepository shopCartRepository) : ApplicationService, IShopCartAppService
{
    public async Task<ShopCartDto> CreateAsync(CreateShopCartDto input)
    {
        Check.NotNull(input, nameof(input));

        ShopCart shopCart = await shopCartManager.CreateAsync(input.UserId.Value, input.GroupBuyId);

        await shopCartRepository.EnsurePropertyLoadedAsync(shopCart, x => x.User);

        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);

        input.CartItems ??= [];

        foreach (CreateCartItemDto inputItem in input.CartItems)
        {
            if (shopCart.CartItems.Any(ci => inputItem.ItemId.HasValue && ci.ItemId == inputItem.ItemId.Value && ci.ItemDetailId == inputItem.ItemDetailId.Value))
            {
                foreach (CartItem cartItem in shopCart.CartItems.Where(ci => ci.ItemId == inputItem.ItemId.Value && ci.ItemDetailId == inputItem.ItemDetailId.Value))
                {
                    cartItem.Quantity += inputItem.Quantity;
                }

                await shopCartRepository.UpdateAsync(shopCart);
            }

            else if (shopCart.CartItems.Any(ci => inputItem.SetItemId.HasValue && ci.SetItemId == inputItem.SetItemId))
            {
                foreach (CartItem cartItem in shopCart.CartItems.Where(w => w.SetItemId == inputItem.SetItemId))
                {
                    cartItem.Quantity += inputItem.Quantity;
                }
            }

            else
            {
                await shopCartManager.AddCartItem(shopCart, inputItem.Quantity, inputItem.GroupBuyPrice, inputItem.SellingPrice, inputItem.ItemId, inputItem.ItemDetailId, inputItem.SetItemId);
            }
        }

        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task DeleteAsync(Guid id)
    {
        var shopCart = await shopCartRepository.GetAsync(id);
        await shopCartRepository.DeleteAsync(shopCart);
    }

    public async Task<ShopCartDto> GetAsync(Guid id)
    {
        var shopCart = await shopCartRepository.GetAsync(id, true);
        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task<PagedResultDto<ShopCartDto>> GetListAsync(GetShopCartListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(ShopCart.CreationTime) + " DESC";
        }

        var totalCount = await shopCartRepository.GetCountAsync(input.Filter, input.UserId, input.GroupBuyId, input.IncludeDetails);

        var items = await shopCartRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.UserId, input.GroupBuyId, input.IncludeDetails);

        return new PagedResultDto<ShopCartDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<ShopCart>, List<ShopCartDto>>(items)
        };
    }

    public async Task<ShopCartDto?> FindByUserIdAsync(Guid userId)
    {
        var shopCart = await shopCartRepository.FindByUserIdAsync(userId, true);
        return ObjectMapper.Map<ShopCart?, ShopCartDto?>(shopCart);
    }

    public async Task<ShopCartDto?> FindByUserIdAndGroupBuyIdAsync(Guid userId, Guid groupBuyId)
    {
        var shopCart = await shopCartRepository.FindByUserIdAndGroupBuyIdAsync(userId, groupBuyId);
        if (shopCart is not null)
        {
            await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        }
        return ObjectMapper.Map<ShopCart?, ShopCartDto?>(shopCart);
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var shopCart = await shopCartRepository.FindByUserIdAsync(userId);
        await shopCartRepository.DeleteAsync(shopCart);
    }

    public async Task DeleteByUserIdAndGroupBuyIdAsync(Guid userId, Guid groupBuyId)
    {
        ShopCart shopCart = await shopCartRepository.FindByUserIdAndGroupBuyIdAsync(userId, groupBuyId);

        await shopCartRepository.DeleteAsync(shopCart);
    }

    public async Task<ShopCartDto> AddCartItemAsync(Guid userId, Guid groupBuyId, CreateCartItemDto input)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.NotDefaultOrNull<Guid>(groupBuyId, nameof(groupBuyId));
        Check.NotNull(input, nameof(input));

        ShopCart shopCart = await shopCartRepository.FindByUserIdAndGroupBuyIdAsync(userId, groupBuyId);

        if (shopCart is null) shopCart = await shopCartManager.CreateAsync(userId, groupBuyId);

        else
        {
            await shopCartRepository.EnsurePropertyLoadedAsync(shopCart, x => x.User);
            await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        }

        if (shopCart.CartItems.Any(a => input.ItemId.HasValue && a.ItemId == input.ItemId && a.ItemDetailId == input.ItemDetailId))
        {
            foreach (CartItem cartItem in shopCart.CartItems.Where(w => w.ItemId == input.ItemId.Value && w.ItemDetailId == input.ItemDetailId.Value))
            {
                cartItem.Quantity = input.Quantity;
            }
        }
        else if (shopCart.CartItems.Any(x => input.SetItemId.HasValue && x.SetItemId == input.SetItemId))
        {
            foreach (CartItem cartItem in shopCart.CartItems.Where(w => w.SetItemId == input.SetItemId))
            {
                cartItem.Quantity = input.Quantity;
            }
        }
        else
        {
            await shopCartManager.AddCartItem(shopCart, input.Quantity, input.GroupBuyPrice,input.SellingPrice, input.ItemId, input.ItemDetailId, input.SetItemId);
        }
        await shopCartRepository.UpdateAsync(shopCart);

        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task<ShopCartDto> UpdateCartItemAsync(Guid cartItemId, CreateCartItemDto input)
    {
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));
        Check.NotNull(input, nameof(input));

        ShopCart shopCart = await shopCartRepository.FindByCartItemIdAsync(cartItemId, exception: true);

        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);

        shopCartManager.UpdateQuantityAsync(shopCart, cartItemId, input.Quantity);

        await shopCartRepository.UpdateAsync(shopCart);

        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task<ShopCartDto> DeleteCartItemAsync(Guid cartItemId)
    {
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));

        var shopCart = await shopCartRepository.FindByCartItemIdAsync(cartItemId, exception: true);
        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        shopCartManager.RemoveCartItem(shopCart, cartItemId);
        await shopCartRepository.UpdateAsync(shopCart);
        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task<PagedResultDto<ShopCartListWithDetailsDto>> GetListWithDetailsAsync(GetShopCartListDto input)
    {
        var shopCarts = await shopCartRepository.GetListWithDetailsAsync(input.SkipCount, input.MaxResultCount, input.Sorting,
            input.Filter, input.UserId, input.GroupBuyId, input.MinItems, input.MaxItems, input.MinAmount, input.MaxAmount,
            input.VipTier, input.MemberStatus);

        return new PagedResultDto<ShopCartListWithDetailsDto>
        {
            TotalCount = shopCarts.TotalCount,
            Items = ObjectMapper.Map<List<ShopCartListWithDetailsModel>, List<ShopCartListWithDetailsDto>>(shopCarts.Items)
        };
    }

    public async Task<List<CartItemWithDetailsDto>> GetCartItemsListAsync(Guid shopCartId)
    {
        var cartItems = await shopCartRepository.GetCartItemsListAsync(shopCartId);
        return ObjectMapper.Map<List<CartItemWithDetailsModel>, List<CartItemWithDetailsDto>>(cartItems);
    }

    public async Task ClearCartItemsAsync(List<Guid> ids)
    {
        var shopCarts = await shopCartRepository.GetListWithCartItemsAsync(ids);
        shopCarts.ForEach(shopCart => shopCart.CartItems.Clear());
        await shopCartRepository.UpdateManyAsync(shopCarts);
    }

    public async Task ClearCartItemsAsync(Guid userId, Guid groupBuyId)
    {
        var shopCart = await shopCartRepository.FindByUserIdAndGroupBuyIdAsync(userId, groupBuyId)
            ?? throw new EntityNotFoundException(typeof(ShopCart));

        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        shopCart.CartItems.Clear();
        await shopCartRepository.UpdateAsync(shopCart);
    }

    public async Task<List<ItemWithItemTypeDto>> GetGroupBuyProductsLookupAsync(Guid groupBuyId)
    {
        var data = await shopCartRepository.GetGroupBuyProductsLookupAsync(groupBuyId);
        return ObjectMapper.Map<List<ItemWithItemType>, List<ItemWithItemTypeDto>>(data);
    }

    public async Task<ItemWithDetailsDto> GetItemWithDetailsAsync(Guid groupBuyId, Guid id, ItemType itemType)
    {
        var data = await shopCartRepository.GetItemWithDetailsAsync(groupBuyId, id, itemType);
        return ObjectMapper.Map<ItemWithDetailsModel, ItemWithDetailsDto>(data);
    }

    public async Task UpdateShopCartAsync(Guid id, List<CartItemWithDetailsDto> cartItems)
    {
        var shopCart = await shopCartRepository.GetAsync(id);
        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, sc => sc.CartItems);

        var existingIds = cartItems.Where(ci => ci.Id.HasValue).Select(ci => ci.Id.Value).ToList();
        shopCart.CartItems.RemoveAll(ci => !existingIds.Contains(ci.Id));

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Id.HasValue)
            {
                var existingItem = shopCart.CartItems.First(ci => ci.Id == cartItem.Id);
                existingItem.SetQuantity(cartItem.Quantity);
                existingItem.ChangeGroupBuyPrice(cartItem.GroupBuyPrice);
            }
            else
            {
                await shopCartManager.AddCartItem(shopCart, cartItem.Quantity, cartItem.GroupBuyPrice,cartItem.SellingPrice, cartItem.ItemId, cartItem.ItemDetailId, cartItem.SetItemId);
            }
        }

        await shopCartRepository.UpdateAsync(shopCart);
    }

    [AllowAnonymous]
    public async Task<List<VerifyCartItemDto>> VerifyCartItemsAsync(Guid userId, Guid groupBuyId)
    {
        var cartItems = await shopCartRepository.VerifyCartItemsAsync(userId, groupBuyId);
        return ObjectMapper.Map<List<VerifyCartItemModel>, List<VerifyCartItemDto>>(cartItems);
    }
}
