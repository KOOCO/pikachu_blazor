using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.AddOnProducts
{
    public interface IAddOnProductAppService:IApplicationService
    {
        Task<PagedResultDto<AddOnProductDto>> GetListAsync(GetAddOnProductListDto input);
        Task<AddOnProductDto> CreateAsync(CreateUpdateAddOnProductDto input);
        Task<AddOnProductDto> GetAsync(Guid Id);
        Task DeleteAsync(Guid Id);
        Task<AddOnProductDto> UpdateAsync(Guid Id, CreateUpdateAddOnProductDto input);
    }
}
