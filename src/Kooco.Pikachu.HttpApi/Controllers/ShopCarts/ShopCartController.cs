﻿using Asp.Versioning;
using Kooco.Pikachu.ShopCarts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.ShopCarts;

[RemoteService(IsEnabled = true)]
[ControllerName("ShopCarts")]
[Area("app")]
[Route("api/app/shop-carts")]
public class ShopCartController(IShopCartAppService shopCartAppService) : PikachuController
{
    [HttpPost]
    public Task<ShopCartDto> CreateAsync(CreateShopCartDto input)
    {
        return shopCartAppService.CreateAsync(input);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<ShopCartDto>> GetListAsync(GetShopCartListDto input)
    {
        return shopCartAppService.GetListAsync(input);
    }

    [HttpDelete("{userId}")]
    public Task DeleteByUserIdAsync(Guid userId)
    {
        return shopCartAppService.DeleteByUserIdAsync(userId);
    }

    [HttpGet("{userId}")]
    public Task<ShopCartDto?> FindByUserIdAsync(Guid userId)
    {
        return shopCartAppService.FindByUserIdAsync(userId);
    }

    [HttpPost("{userId}/cart-items")]
    public Task<ShopCartDto> AddCartItemAsync(Guid userId, CreateCartItemDto input)
    {
        return shopCartAppService.AddCartItemAsync(userId, input);
    }

    [HttpPut("cart-items/{cartItemId}")]
    public Task<ShopCartDto> UpdateCartItemAsync(Guid cartItemId, CreateCartItemDto input)
    {
        return shopCartAppService.UpdateCartItemAsync(cartItemId, input);
    }

    [HttpDelete("cart-items/{cartItemId}")]
    public Task<ShopCartDto> DeleteCartItemAsync(Guid cartItemId)
    {
        return shopCartAppService.DeleteCartItemAsync(cartItemId);
    }
}
