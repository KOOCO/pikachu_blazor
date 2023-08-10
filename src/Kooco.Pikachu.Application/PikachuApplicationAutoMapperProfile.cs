using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using AutoMapper;

namespace Kooco.Pikachu;

public class PikachuApplicationAutoMapperProfile : Profile
{
    public PikachuApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Item, ItemDto>();
        CreateMap<CreateUpdateItemDto, Item>(MemberList.Source);
        CreateMap<ItemDto, UpdateItemDto>();
        CreateMap<UpdateItemDto, Item>();
        CreateMap<ItemDetails, ItemDetailsDto>();
        CreateMap<CreateUpdateItemDetailsDto, ItemDetails>(MemberList.Source);
        CreateMap<SetItem, SetItemDto>();
        CreateMap<CreateUpdateSetItemDto, SetItem>(MemberList.Source);
        CreateMap<SetItemDetails, SetItemDetailsDto>();
        CreateMap<CreateUpdateSetItemDetailsDto, SetItemDetails>(MemberList.Source);
    }
}
