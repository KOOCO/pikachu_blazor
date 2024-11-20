using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq;

namespace Kooco.Pikachu.ShopCarts;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.ShopCarts.Default)]
public class ShopCartAppService(ShopCartManager shopCartManager, IShopCartRepository shopCartRepository) : ApplicationService, IShopCartAppService
{
    public async Task<ShopCartDto> CreateAsync(CreateShopCartDto input)
    {
        Check.NotNull(input, nameof(input));
        var shopCart = await shopCartManager.CreateAsync(input.UserId.Value, input.GroupBuyId);
        input.CartItems ??= [];
        foreach (var cartItem in input.CartItems)
        {
            Check.NotNull(cartItem.ItemId, nameof(cartItem.ItemId));
            Check.NotNull(cartItem.ItemDetailId, nameof(cartItem.ItemDetailId));
            await shopCartManager.AddCartItem(shopCart, cartItem.ItemId.Value, cartItem.Quantity, cartItem.UnitPrice, cartItem.ItemDetailId.Value);
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
        return ObjectMapper.Map<ShopCart?, ShopCartDto?>(
            await shopCartRepository.FindByUserIdAndGroupBuyIdAsync(userId, groupBuyId, true)
        );
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
        Check.NotDefaultOrNull(input.ItemId, nameof(input.ItemId));
        Check.NotDefaultOrNull(input.ItemDetailId, nameof(input.ItemDetailId));

        ShopCart shopCart = await shopCartRepository.FindByUserIdAndGroupBuyIdAsync(userId, groupBuyId);
        
        if (shopCart is null) shopCart = await shopCartManager.CreateAsync(userId, groupBuyId);

        else
        {
            await shopCartRepository.EnsurePropertyLoadedAsync(shopCart, x => x.User);
            await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        }

        if (shopCart.CartItems.Any(a => a.ItemId == input.ItemId.Value && a.ItemDetailId == input.ItemDetailId.Value))
        {
            foreach (CartItem cartItem in shopCart.CartItems.Where(w => w.ItemId == input.ItemId.Value && w.ItemDetailId == input.ItemDetailId.Value))
            {
                cartItem.Quantity += input.Quantity;
            }
        }

        else await shopCartManager.AddCartItem(shopCart, input.ItemId.Value, input.Quantity, input.UnitPrice, input.ItemDetailId.Value);

        await shopCartRepository.UpdateAsync(shopCart);

        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task<ShopCartDto> UpdateCartItemAsync(Guid cartItemId, CreateCartItemDto input)
    {
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.ItemId, nameof(input.ItemId));
        Check.NotDefaultOrNull(input.ItemDetailId, nameof(input.ItemDetailId));

        var shopCart = await shopCartRepository.FindByCartItemIdAsync(cartItemId, exception: true);
        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        await shopCartManager.UpdateCartItemAsync(shopCart, cartItemId, input.ItemId.Value,
            input.Quantity, input.UnitPrice, input.ItemDetailId.Value);
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
}
