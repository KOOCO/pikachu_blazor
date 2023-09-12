using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using AutoMapper;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.GroupBuys;

namespace Kooco.Pikachu;

public class PikachuApplicationAutoMapperProfile : Profile
{
    public PikachuApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //Item Dto EntityMapping
        CreateMap<Item, ItemDto>();
        CreateMap<ItemDto, UpdateItemDto>();
        CreateMap<UpdateItemDto, Item>();
        CreateMap<CreateItemDto, Item>();
        CreateMap<Item, KeyValueDto>().ForMember(dest => dest.Name, src => src.MapFrom(x => x.ItemName));

        // ItemDetailDto EntityMapping
        CreateMap<ItemDetails, ItemDetailsDto>();
        CreateMap<CreateItemDetailsDto, ItemDetails>();

        //EnumValue EntityMapping
        CreateMap<EnumValueDto, EnumValue>();
        CreateMap<EnumValue, EnumValueDto>(MemberList.Source);
        CreateMap<CreateUpdateEnumValueDto, EnumValue>(MemberList.Source);

        //Image EntityMapping
        CreateMap<Image, ImageDto>();
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();


        CreateMap<SetItem, SetItemDto>();
        CreateMap<CreateUpdateSetItemDto, SetItem>(MemberList.Source);
        CreateMap<SetItemDetails, SetItemDetailsDto>();
        CreateMap<CreateUpdateSetItemDetailsDto, SetItemDetails>(MemberList.Source);
        //
        CreateMap<GroupBuy, GroupBuyDto>();
        CreateMap<GroupBuyItemGroup, GroupBuyItemGroupDto>();
    }
}
