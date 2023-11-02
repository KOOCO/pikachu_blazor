using AutoMapper;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;

namespace Kooco.Pikachu.Blazor
{
    public class PikachuBlazorAutoMapperProfile:Profile
    {
        public PikachuBlazorAutoMapperProfile()
        {
            CreateMap<GroupBuyDto, GroupBuyUpdateDto>();
            CreateMap<ImageDto, CreateImageDto>();
            CreateMap<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>();
        }
    }
}
