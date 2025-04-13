using AutoMapper;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Tenants.ElectronicInvoiceSettings;

namespace Kooco.Pikachu.Blazor;
public class PikachuBlazorAutoMapperProfile : Profile
{
    public PikachuBlazorAutoMapperProfile()
    {
        CreateMap<GroupBuyDto, GroupBuyUpdateDto>();
        CreateMap<ImageDto, CreateImageDto>();
        CreateMap<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>();
    }
}