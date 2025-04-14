using AutoMapper;
using Kooco.Pikachu.Blazor.Pages.CashFlowManagement;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Tenants.Requests;

namespace Kooco.Pikachu.Blazor;
public class PikachuBlazorAutoMapperProfile : Profile
{
    public PikachuBlazorAutoMapperProfile()
    {
        CreateMap<GroupBuyDto, GroupBuyUpdateDto>();
        CreateMap<ImageDto, CreateImageDto>();
        CreateMap<CreateTenantTripartiteDto, TenantTripartiteResultDto>();
    }
}