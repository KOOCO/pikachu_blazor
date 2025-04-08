using AutoMapper;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
            CreateMap<FreebieDto, UpdateFreebieDto>()
                .ForMember(dest => dest.FreebieGroupBuys, opt => opt.MapFrom<FreebieGroupBuysResolver>());

            CreateMap<FreebieGroupBuysDto, CreateFreebieGroupBuysDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, CreateOrderDto>();
            CreateMap<GroupBuyReport, GroupBuyReportDto>();

            CreateMap<OrderDeliveryDto, OrderDelivery>().ReverseMap();
        }
        
    }

    public class FreebieGroupBuysResolver : IValueResolver<FreebieDto, UpdateFreebieDto, List<Guid>>
    {
        public List<Guid> Resolve(FreebieDto source, UpdateFreebieDto destination, List<Guid> destMember, ResolutionContext context)
        {
            return source.FreebieGroupBuys.Select(x => x.GroupBuyId).ToList();
        }
    }
}
