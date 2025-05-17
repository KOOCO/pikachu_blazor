using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.EdmManagement;

public interface IEdmAppService : IApplicationService
{
    Task<EdmDto> CreateAsync(CreateEdmDto input);
    Task<EdmDto> GetAsync(Guid id);
    Task<EdmDto> UpdateAsync(Guid id, CreateEdmDto input);
    Task<PagedResultDto<EdmDto>> GetListAsync(GetEdmListDto input);
    Task DeleteAsync(Guid id);
}
