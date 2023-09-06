using AutoMapper;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;

namespace Kooco.Pikachu.Blazor
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ItemDto, UpdateItemDto>();
            CreateMap<ItemDetailsDto, CreateItemDetailsDto>();
            CreateMap<ImageDto, CreateImageDto>();
        }
    }
}
