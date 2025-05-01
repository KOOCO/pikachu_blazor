using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ShopCarts;

public interface IShopCartAppService : IApplicationService
{
    Task<ShopCartDto> CreateAsync(CreateShopCartDto input);
    Task<ShopCartDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<PagedResultDto<ShopCartDto>> GetListAsync(GetShopCartListDto input);
    Task<ShopCartDto?> FindByUserIdAsync(Guid userId);
    Task<ShopCartDto?> FindByUserIdAndGroupBuyIdAsync(Guid userId, Guid groupBuyId);
    Task DeleteByUserIdAsync(Guid userId);
    Task DeleteByUserIdAndGroupBuyIdAsync(Guid userId, Guid groupBuyId);
    Task<ShopCartDto> AddCartItemAsync(Guid userId, Guid groupBuyId, CreateCartItemDto input);
    Task<ShopCartDto> UpdateCartItemAsync(Guid cartItemId, CreateCartItemDto input);
    Task<ShopCartDto> DeleteCartItemAsync(Guid cartItemId);
    Task<PagedResultDto<ShopCartListWithDetailsDto>> GetListWithDetailsAsync(GetShopCartListDto input);
    Task ClearCartItemsAsync(List<Guid> ids);
    Task<List<CartItemWithDetailsDto>> GetCartItemsListAsync(Guid shopCartId);
}
