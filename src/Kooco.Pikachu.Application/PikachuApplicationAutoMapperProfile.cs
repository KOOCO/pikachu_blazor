using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using AutoMapper;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.StoreComments;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.ElectronicInvoiceSettings;

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
        CreateMap<Item, ItemWithItemTypeDto>()
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.ItemName))
            .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => ItemType.Item));

        CreateMap<ItemWithItemType, ItemWithItemTypeDto>();

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
        CreateMap<SetItem, ItemWithItemTypeDto>()
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.SetItemName))
            .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => ItemType.SetItem));

        //
        CreateMap<GroupBuy, GroupBuyDto>();
        CreateMap<GroupBuyItemGroup, GroupBuyItemGroupDto>();
        CreateMap<GroupBuyItemGroupDto, GroupBuyItemGroupCreateUpdateDto>();
        CreateMap<GroupBuyItemGroupDetails, GroupBuyItemGroupDetailsDto>();
        CreateMap<GroupBuy, KeyValueDto>().ForMember(dest => dest.Name, src => src.MapFrom(s => s.GroupBuyName));
        CreateMap<GroupBuyItemGroupWithCount, GroupBuyItemGroupWithCountDto>();
        CreateMap<GroupBuyReportDetails, GroupBuyReportDetailsDto>();

        CreateMap<Freebie, FreebieDto>();
        CreateMap<FreebieGroupBuys, FreebieGroupBuysDto>();

        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<StoreComment, StoreCommentDto>();


        CreateMap<Refund, RefundDto>();

        CreateMap<PaymentGateway, PaymentGatewayDto>();
        CreateMap<PaymentGatewayDto, UpdateLinePayDto>();
        CreateMap<PaymentGatewayDto, UpdateChinaTrustDto>();
        CreateMap<PaymentGatewayDto, UpdateEcPayDto>();

        CreateMap<ElectronicInvoiceSetting, ElectronicInvoiceSettingDto>();
        CreateMap<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>();
    }
}
