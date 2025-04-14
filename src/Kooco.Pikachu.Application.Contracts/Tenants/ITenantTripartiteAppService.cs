using Kooco.Pikachu.Tenants.Requests;
using Kooco.Pikachu.Tenants.Responses;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Tenants;
public interface ITenantTripartiteAppService : IApplicationService
{
    Task<TenantTripartiteDto?> FindAsync();
    Task<TenantTripartiteDto> AddAsync(CreateTenantTripartiteDto input);
    Task<TenantTripartiteDto> PutAsync(UpdateTenantTripartiteDto input);
}