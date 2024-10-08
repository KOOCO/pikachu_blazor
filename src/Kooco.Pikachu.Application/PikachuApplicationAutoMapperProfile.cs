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
using Kooco.Pikachu.TenantEmailing;
using Kooco.Pikachu.AutomaticEmails;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.DeliveryTempratureCosts;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.Members;
using Volo.Abp.Identity;
using Kooco.Pikachu.UserAddresses;
using System.Collections.Generic;
using Kooco.Pikachu.UserShoppingCredits;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeOrders;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.PikachuAccounts;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.ShopCarts;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.ShoppingCredits;
using Volo.Abp.Data;
using Kooco.Pikachu.GroupPurchaseOverviews;

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
        CreateMap<ItemListViewModel, ItemListDto>();
        CreateMap<ItemDto, UpdateItemDto>();
        CreateMap<UpdateItemDto, Item>();
        CreateMap<CreateItemDto, Item>();
        CreateMap<Item, KeyValueDto>().ForMember(dest => dest.Name, src => src.MapFrom(x => x.ItemName));
        CreateMap<Item, ItemWithItemTypeDto>()
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.ItemName))
            .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => ItemType.Item));

        CreateMap<ItemWithItemType, ItemWithItemTypeDto>();

        // ItemDetailDto EntityMapping
        CreateMap<ItemDetails, ItemDetailsDto>(MemberList.Source);
        CreateMap<CreateItemDetailsDto, ItemDetails>();

        //EnumValue EntityMapping
        CreateMap<EnumValueDto, EnumValue>();
        CreateMap<EnumValue, EnumValueDto>(MemberList.Source);
        CreateMap<CreateUpdateEnumValueDto, EnumValue>(MemberList.Source);

        //Image EntityMapping
        CreateMap<Image, ImageDto>();
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();

        CreateMap<OrderDeliveryDto, OrderDelivery>().ReverseMap();

        CreateMap<SetItem, SetItemDto>();
        CreateMap<CreateUpdateSetItemDto, SetItem>(MemberList.Source);
        CreateMap<SetItemDetails, SetItemDetailsDto>();
        CreateMap<CreateUpdateSetItemDetailsDto, SetItemDetails>(MemberList.Source);
        CreateMap<SetItem, ItemWithItemTypeDto>()
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.SetItemName))
            .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => ItemType.SetItem));

        //
        CreateMap<GroupBuy, GroupBuyDto>();
        CreateMap<GroupBuyList, GroupBuyDto>();
        CreateMap<GroupBuyItemGroup, GroupBuyItemGroupDto>();
        CreateMap<GroupBuyItemGroupDto, GroupBuyItemGroupCreateUpdateDto>();
        CreateMap<GroupBuyItemGroupDetails, GroupBuyItemGroupDetailsDto>();
        CreateMap<GroupBuy, KeyValueDto>().ForMember(dest => dest.Name, src => src.MapFrom(s => s.GroupBuyName));
        CreateMap<GroupBuyList, KeyValueDto>().ForMember(dest => dest.Name, src => src.MapFrom(s => s.GroupBuyName));
        CreateMap<GroupBuyItemGroupWithCount, GroupBuyItemGroupWithCountDto>();
        CreateMap<GroupBuyReportDetails, GroupBuyReportDetailsDto>();
        CreateMap<GroupBuyItemGroup, GroupBuyItemGroupModuleDetailsDto>().ReverseMap();

        CreateMap<Freebie, FreebieDto>();
        CreateMap<FreebieGroupBuys, FreebieGroupBuysDto>();

        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<StoreComment, StoreCommentDto>();

        CreateMap<OrderDto, CreateOrderDto>().ReverseMap();
        CreateMap<Refund, RefundDto>();

        CreateMap<PaymentGateway, PaymentGatewayDto>();
        CreateMap<PaymentGatewayDto, UpdateLinePayDto>();
        CreateMap<PaymentGatewayDto, UpdateChinaTrustDto>();
        CreateMap<PaymentGatewayDto, UpdateEcPayDto>();

        CreateMap<ElectronicInvoiceSetting, ElectronicInvoiceSettingDto>();
        CreateMap<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>();

        CreateMap<TenantEmailSettings, TenantEmailSettingsDto>();
        CreateMap<TenantEmailSettingsDto, CreateUpdateTenantEmailSettingsDto>();

        CreateMap<AutomaticEmail, AutomaticEmailDto>();
        CreateMap<AutomaticEmailGroupBuys, AutomaticEmailGroupBuysDto>();
        CreateMap<AutomaticEmailDto, AutomaticEmailCreateUpdateDto>();


        CreateMap<LogisticsProviderSettings, LogisticsProviderSettingsDto>();
        CreateMap<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, TCatLogisticsCreateUpdateDto>();
        CreateMap<LogisticsProviderSettings, TCatLogisticsCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, TCatNormalCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, TCatFreezeCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, TCatFrozenCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, TCat711NormalCreateUpdate>();
        CreateMap<LogisticsProviderSettingsDto, TCat711FreezeCreateUpdateDto>();
        CreateMap<LogisticsProviderSettingsDto, TCat711FrozenCreateUpdateDto>();

        CreateMap<DeliveryTemperatureCost, DeliveryTemperatureCostDto>();
        CreateMap<DeliveryTemperatureCostDto, UpdateDeliveryTemperatureCostDto>();

        CreateMap<OrderDelivery, OrderDeliveryDto>();
        //CreateMap<GroupBuyUpdateDto, GroupBuy>().ReverseMap();
        CreateMap<GroupBuyUpdateDto, GroupBuy>()
            .ForMember(dest => dest.ItemGroups, opt => opt.MapFrom(src => src.ItemGroups));

        CreateMap<GroupBuyItemGroupCreateUpdateDto, GroupBuyItemGroup>()
            .ForMember(dest => dest.ItemGroupDetails, opt => opt.MapFrom(src => src.ItemDetails));

        CreateMap<GroupBuyItemGroupDetailCreateUpdateDto, GroupBuyItemGroupDetails>();

        CreateMap<GroupBuyItemGroup, GroupBuyItemGroupDto>().ReverseMap();
        CreateMap<GroupBuyItemGroup, GroupBuyItemGroupCreateUpdateDto>().ReverseMap();
        CreateMap<GroupBuyItemGroupDto, GroupBuyItemGroupCreateUpdateDto>().ReverseMap();

        CreateMap<GroupBuyItemGroupDetails, GroupBuyItemGroupDetailsDto>().ReverseMap();
        CreateMap<GroupBuyItemGroupDetails, GroupBuyItemGroupDetailCreateUpdateDto>().ReverseMap();
        CreateMap<GroupBuyItemGroupDetailsDto, GroupBuyItemGroupDetailCreateUpdateDto>().ReverseMap();

        CreateMap<IdentityUser, MemberDto>()
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.ExtraProperties.GetValueOrDefault(Constant.Birthday)));
        CreateMap<IdentityUserDto, MemberDto>()
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.ExtraProperties.GetValueOrDefault(Constant.Birthday)));
        CreateMap<MemberDto, UpdateMemberDto>();
        CreateMap<MemberModel, MemberDto>();

        CreateMap<UserAddress, UserAddressDto>();
        CreateMap<UserAddressDto, UpdateUserAddressDto>();

        CreateMap<UserShoppingCredit, UserShoppingCreditDto>();
        CreateMap<UserShoppingCreditDto, UpdateUserShoppingCreditDto>();

        CreateMap<MemberCreditRecordModel, MemberCreditRecordDto>().ReverseMap();

        CreateMap<UserCumulativeCredit, UserCumulativeCreditDto>();
        CreateMap<UserCumulativeCreditDto, UpdateUserCumulativeCreditDto>();

        CreateMap<AddOnProduct, AddOnProductDto>().ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ItemName));
        CreateMap<AddOnProductSpecificGroupbuy, AddOnProductSpecificGroupbuyDto>();

        CreateMap<AddOnProductDto, CreateUpdateAddOnProductDto>();


        CreateMap<DiscountCode, DiscountCodeDto>();
        CreateMap<DiscountSpecificGroupbuy, DiscountCodeSpecificGroupbuyDto>();
        CreateMap<DiscountSpecificProduct, DiscountCodeSpecificProductDto>();
        CreateMap<DiscountCodeUsage, DiscountCodeUsageDto>();
        CreateMap<DiscountCodeDto, CreateUpdateDiscountCodeDto>();

        CreateMap<AddOnProductDto, CreateUpdateAddOnProductDto>();

        CreateMap<UserCumulativeOrder, UserCumulativeOrderDto>();
        CreateMap<UserCumulativeOrderDto, UpdateUserCumulativeOrderDto>();

        CreateMap<UserCumulativeFinancial, UserCumulativeFinancialDto>();
        CreateMap<UserCumulativeFinancialDto, UpdateUserCumulativeFinancialDto>();

        CreateMap<PikachuLoginResponseDto, MemberLoginResponseDto>();

        CreateMap<ShoppingCreditUsageSetting, ShoppingCreditUsageSettingDto>();
        CreateMap<ShoppingCreditUsageSettingDto, CreateUpdateShoppingCreditUsageSettingDto>();
        CreateMap<ShoppingCreditUsageSpecificGroupbuy, ShoppingCreditUsageSpecificGroupbuyDto>();
        CreateMap<ShoppingCreditUsageSpecificProduct, ShoppingCreditUsageSpecificProductDto>();

        CreateMap<ShoppingCreditEarnSetting, ShoppingCreditEarnSettingDto>();
        CreateMap<ShoppingCreditEarnSettingDto, CreateUpdateShoppingCreditEarnSettingDto>();
        CreateMap<ShoppingCreditEarnSpecificGroupbuy, ShoppingCreditEarnSpecificGroupbuyDto>();
        CreateMap<ShoppingCreditEarnSpecificProduct, ShoppingCreditEarnSpecificProductDto>();
        CreateMap<ShopCart, ShopCartDto>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
        CreateMap<CartItem, CartItemDto>().ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.ItemName));

        #region GroupPurchaseOverview Mappings
        CreateMap<GroupPurchaseOverview, GroupPurchaseOverviewDto>().ReverseMap();
        CreateMap<GroupPurchaseOverview, CreateUpdateGroupPurchaseOverviewDto>().ReverseMap();
        CreateMap<GroupPurchaseOverviewDto, CreateUpdateGroupPurchaseOverviewDto>().ReverseMap();
        #endregion

        CreateMap<TenantSettings, TenantSettingsDto>()
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.Tenant.ExtraProperties.GetValueOrDefault(Constant.Logo)))
            .ForMember(dest => dest.BannerUrl, opt => opt.MapFrom(src => src.Tenant.ExtraProperties.GetValueOrDefault(Constant.BannerUrl)));
        CreateMap<TenantSettingsDto, UpdateTenantSettingsDto>();

        CreateMap<OrderMessage, OrderMessageDto>();
    }
}
