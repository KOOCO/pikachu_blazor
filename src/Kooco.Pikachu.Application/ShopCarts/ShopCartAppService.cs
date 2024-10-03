using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShopCarts;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.ShopCarts.Default)]
public class ShopCartAppService(ShopCartManager shopCartManager, IShopCartRepository shopCartRepository) : ApplicationService, IShopCartAppService
{
    public async Task<ShopCartDto> CreateAsync(CreateShopCartDto input)
    {
        Check.NotNull(input, nameof(input));
        var shopCart = await shopCartManager.CreateAsync(input.UserId.Value);
        input.CartItems ??= [];
        foreach (var cartItem in input.CartItems)
        {
            Check.NotNull(cartItem.ItemId, nameof(cartItem.ItemId));
            shopCartManager.AddCartItem(shopCart, cartItem.ItemId.Value, cartItem.Quantity, cartItem.UnitPrice);
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

        var totalCount = await shopCartRepository.GetCountAsync(input.Filter, input.UserId, input.IncludeDetails);

        var items = await shopCartRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.UserId, input.IncludeDetails);

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

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var shopCart = await shopCartRepository.FindByUserIdAsync(userId);
        await shopCartRepository.DeleteAsync(shopCart);
    }

    public async Task<ShopCartDto> AddCartItemAsync(Guid userId, CreateCartItemDto input)
    {
        Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.ItemId, nameof(input.ItemId));

        var shopCart = await shopCartRepository.FindByUserIdAsync(userId);
        if (shopCart is null)
        {
            shopCart = await shopCartManager.CreateAsync(userId);
        }
        else
        {
            await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        }
        shopCartManager.AddCartItem(shopCart, input.ItemId.Value, input.Quantity, input.UnitPrice);
        await shopCartRepository.UpdateAsync(shopCart);
        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }

    public async Task<ShopCartDto> UpdateCartItemAsync(Guid cartItemId, CreateCartItemDto input)
    {
        Check.NotDefaultOrNull<Guid>(cartItemId, nameof(cartItemId));
        Check.NotNull(input, nameof(input));
        Check.NotDefaultOrNull(input.ItemId, nameof(input.ItemId));

        var shopCart = await shopCartRepository.FindByCartItemIdAsync(cartItemId, exception: true);
        await shopCartRepository.EnsureCollectionLoadedAsync(shopCart, s => s.CartItems);
        shopCartManager.UpdateCartItem(shopCart, cartItemId, input.ItemId.Value, input.Quantity, input.UnitPrice);
        await shopCartRepository.UpdateAsync(shopCart);
        return ObjectMapper.Map<ShopCart, ShopCartDto>(shopCart);
    }
}
