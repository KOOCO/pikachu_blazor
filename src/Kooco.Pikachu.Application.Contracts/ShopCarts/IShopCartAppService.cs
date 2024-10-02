using System;
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
    Task DeleteByUserIdAsync(Guid userId);
    Task<ShopCartDto> AddCartItemAsync(Guid userId, CreateCartItemDto input);
}
