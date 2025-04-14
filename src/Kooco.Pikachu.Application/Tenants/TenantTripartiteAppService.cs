using AutoMapper;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using Kooco.Pikachu.Tenants.Requests;
using Kooco.Pikachu.Tenants.Responses;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Tenants;

[RemoteService(IsEnabled = false)]
public class TenantTripartiteAppService : PikachuAppService, ITenantTripartiteAppService
{
    public async Task<TenantTripartiteDto?> FindAsync()
    {
        var tenantId = CurrentTenant.Id.Value;
        var query = await TenantTripartiteRepository.FindByTenantAsync(tenantId);
        if (query is not null) return ObjectMapper.Map<TenantTripartite, TenantTripartiteDto>(query);
        return default;
    }
    public async Task<TenantTripartiteDto> AddAsync(CreateTenantTripartiteDto input)
    {
        var tenantId = CurrentTenant.Id.Value;
        if (await TenantTripartiteRepository.AnyAsync(x => x.TenantId == tenantId))
        {
            throw new UserFriendlyException("已存在租戶三方資料，無法重複新增");
        }

        input.TenantId = tenantId;
        var entity = ObjectMapper.Map<CreateTenantTripartiteDto, TenantTripartite>(input);
        var result = await TenantTripartiteRepository.InsertAsync(entity);
        return ObjectMapper.Map<TenantTripartite, TenantTripartiteDto>(result);
    }
    public async Task<TenantTripartiteDto> PutAsync(UpdateTenantTripartiteDto input)
    {
        var tenantId = CurrentTenant.Id.Value;
        var entity = await TenantTripartiteRepository.FindByTenantAsync(tenantId)
            ?? throw new UserFriendlyException("不存在租戶三方資料，無法更新");

        input.TenantId = tenantId;
        ObjectMapper.Map(input, entity);
        var result = await TenantTripartiteRepository.UpsertAsync(entity);
        return ObjectMapper.Map<TenantTripartite, TenantTripartiteDto>(result);
    }

    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<CreateTenantTripartiteDto, TenantTripartite>();
            CreateMap<UpdateTenantTripartiteDto, TenantTripartite>();
            CreateMap<TenantTripartite, TenantTripartiteDto>();
        }
    }

    public required ITenantTripartiteRepository TenantTripartiteRepository { get; init; }
}