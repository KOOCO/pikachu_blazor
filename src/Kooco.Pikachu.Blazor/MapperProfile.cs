using AutoMapper;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;

namespace Kooco.Pikachu.Blazor
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<ItemDto, UpdateItemDto>();
            CreateMap<ItemDetailsDto, CreateItemDetailsDto>();

            CreateMap<ImageDto, CreateImageDto>();
            
            CreateMap<SetItem, SetItemDto>();
            CreateMap<SetItemDetails, SetItemDetailsDto>();
            CreateMap<SetItemDto, CreateUpdateSetItemDto>();
            CreateMap<SetItemDetailsDto, CreateUpdateSetItemDetailsDto>();
            CreateMap<FreebieDto, UpdateFreebieDto>();

            CreateMap<FreebieGroupBuysDto, CreateFreebieGroupBuysDto>();
        }
        
    }
}
