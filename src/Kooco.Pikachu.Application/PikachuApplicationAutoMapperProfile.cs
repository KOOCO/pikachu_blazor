using AutoMapper;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.AutomaticEmails;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Common;
using Kooco.Pikachu.Dashboards;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.DeliveryTempratureCosts;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.Domain.LogisticStatusRecords;
using Kooco.Pikachu.LogisticStatusRecords;
using Kooco.Pikachu.EdmManagement;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuyItemsPriceses;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.LoginConfigurations;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.LogisticsSettings;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Members.MemberTags;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.PikachuAccounts;
using Kooco.Pikachu.ProductCategories;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.SalesReports;
using Kooco.Pikachu.ShopCarts;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.StoreComments;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.TierManagement;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.UserCumulativeOrders;
using Kooco.Pikachu.Users;
using Kooco.Pikachu.UserShoppingCredits;
using Kooco.Pikachu.WebsiteManagement;
using Kooco.Pikachu.WebsiteManagement.FooterSettings;
using Kooco.Pikachu.WebsiteManagement.TopbarSettings;
using Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Kooco.Pikachu.InventoryManagement;

namespace Kooco.Pikachu;

public class PikachuApplicationAutoMapperProfile : Profile
{
    public PikachuApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        
        // 物流狀態記錄映射
        CreateMap<LogisticStatusRecord, LogisticStatusRecordDto>();
         

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
        CreateMap<Image, ImageDto>().ReverseMap();
        CreateMap<CreateImageDto, Image>().ReverseMap();
        CreateMap<ImageDto, CreateImageDto>().ReverseMap();
        CreateMap<UpdateImageDto, Image>();
        CreateMap<CreateImageDto, UpdateImageDto>().ReverseMap();

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
        CreateMap<OrderItem, OrderItemsCreateDto>().ReverseMap();

        CreateMap<OrderDto, CreateOrderDto>().ReverseMap();
        CreateMap<Refund, RefundDto>();

        CreateMap<PaymentGateway, PaymentGatewayDto>();
        CreateMap<PaymentGatewayDto, UpdateLinePayDto>();
        CreateMap<PaymentGatewayDto, UpdateChinaTrustDto>();
        CreateMap<PaymentGatewayDto, UpdateEcPayDto>();
        CreateMap<PaymentGatewayDto, UpdateOrderValidityDto>();

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
        CreateMap<LogisticsProviderSettingsDto, EcPayHomeDeliveryCreateUpdateDto>();

        CreateMap<DeliveryTemperatureCost, DeliveryTemperatureCostDto>();
        CreateMap<DeliveryTemperatureCostDto, UpdateDeliveryTemperatureCostDto>();

        CreateMap<OrderDelivery, OrderDeliveryDto>();
        //CreateMap<GroupBuyUpdateDto, GroupBuy>().ReverseMap();
        CreateMap<GroupBuyUpdateDto, GroupBuy>()
            .Ignore(dest => dest.ItemGroups);

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
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.GetBirthday()))
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.GetMobileNumber()))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.GetGender()));

        CreateMap<IdentityUserDto, MemberDto>()
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.GetBirthday()))
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.GetMobileNumber()))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.GetGender()));

        CreateMap<MemberDto, UpdateMemberDto>();
        CreateMap<MemberModel, MemberDto>();
        CreateMap<MemberOrderInfoModel, MemberOrderInfoDto>();

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


        CreateMap<DiscountCode, DiscountCodeDto>().ForMember(x => x.TotalQuantity, y => y.MapFrom(z => z.DiscountCodeUsages.Sum(a => a.TotalOrders)));
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
        CreateMap<ShopCartListWithDetailsModel, ShopCartListWithDetailsDto>();
        CreateMap<CartItemWithDetailsModel, CartItemWithDetailsDto>();
        CreateMap<ItemWithDetailsModel, ItemWithDetailsDto>();
        CreateMap<CartItemDetailsModel, CartItemDetailsDto>();

        #region GroupPurchaseOverview Mappings
        CreateMap<GroupPurchaseOverview, GroupPurchaseOverviewDto>().ReverseMap();
        CreateMap<GroupPurchaseOverview, CreateUpdateGroupPurchaseOverviewDto>().ReverseMap();
        CreateMap<GroupPurchaseOverviewDto, CreateUpdateGroupPurchaseOverviewDto>().ReverseMap();
        #endregion

        #region GroupBuyOrderInstruction Mappings
        CreateMap<GroupBuyOrderInstruction, GroupBuyOrderInstructionDto>().ReverseMap();
        CreateMap<GroupBuyOrderInstruction, CreateUpdateGroupBuyOrderInstructionDto>().ReverseMap();
        CreateMap<GroupBuyOrderInstructionDto, CreateUpdateGroupBuyOrderInstructionDto>().ReverseMap();
        #endregion

        #region GroupBuyProductRanking Mappings
        CreateMap<GroupBuyProductRanking, GroupBuyProductRankingDto>().ReverseMap();
        CreateMap<GroupBuyProductRanking, CreateUpdateGroupBuyProductRankingDto>().ReverseMap();
        CreateMap<GroupBuyProductRankingDto, CreateUpdateGroupBuyProductRankingDto>().ReverseMap();
        #endregion

        CreateMap<TenantSettings, TenantSettingsDto>()
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.Logo)))
            .ForMember(dest => dest.BannerUrl, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.BannerUrl)))
            .ForMember(dest => dest.TenantOwner, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantOwner)))
            .ForMember(dest => dest.TenantContactTitle, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantContactTitle)))
            .ForMember(dest => dest.TenantContactPerson, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantContactPerson)))
            .ForMember(dest => dest.TenantContactEmail, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantContactEmail)))
            .ForMember(dest => dest.TenantUrl, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantUrl)))
            .ForMember(dest => dest.Domain, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.Domain)))
            .ForMember(dest => dest.ShortCode, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.ShortCode)))
            .ForMember(dest => dest.ShareProfitPercent, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.ShareProfitPercent)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.Status)));
        CreateMap<TenantSettingsDto, UpdateTenantSettingsDto>();

        CreateMap<TenantSettings, TenantInformationDto>()
            .ForMember(dest => dest.ShortCode, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.ShortCode)))
            .ForMember(dest => dest.TenantContactTitle, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantContactTitle)))
            .ForMember(dest => dest.TenantContactPerson, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantContactPerson)))
            .ForMember(dest => dest.TenantContactEmail, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantContactEmail)))
            .ForMember(dest => dest.TenantUrl, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.TenantUrl)))
            .ForMember(dest => dest.Domain, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.Domain)));
        CreateMap<TenantInformationDto, UpdateTenantInformationDto>();

        CreateMap<TenantSettings, TenantCustomerServiceDto>();
        CreateMap<TenantCustomerServiceDto, UpdateTenantCustomerServiceDto>();

        CreateMap<TenantSettings, TenantFrontendInformationDto>()
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.Logo)))
            .ForMember(dest => dest.BannerUrl, opt => opt.MapFrom(src => src.Tenant!.ExtraProperties.GetValueOrDefault(Constant.BannerUrl)));
        CreateMap<TenantFrontendInformationDto, UpdateTenantFrontendInformationDto>();

        CreateMap<TenantSettings, TenantSocialMediaDto>();
        CreateMap<TenantSocialMediaDto, UpdateTenantSocialMediaDto>();

        CreateMap<TenantSettings, TenantSocialMediaDto>();
        CreateMap<TenantSocialMediaDto, UpdateTenantSocialMediaDto>();

        CreateMap<TenantSettings, TenantGoogleTagManagerDto>();
        CreateMap<TenantGoogleTagManagerDto, UpdateTenantGoogleTagManagerDto>();

        CreateMap<OrderMessage, OrderMessageDto>();

        CreateMap<WebsiteSettings, WebsiteSettingsDto>();
        CreateMap<WebsiteSettingsDto, UpdateWebsiteSettingsDto>();
        CreateMap<WebsiteSettingsModule, WebsiteSettingsModuleDto>();
        CreateMap<WebsiteSettingsModuleItem, WebsiteSettingsModuleItemDto>();
        CreateMap<WebsiteSettingsOverviewModule, WebsiteSettingsOverviewModuleDto>();
        CreateMap<WebsiteSettingsInstructionModule, WebsiteSettingsInstructionModuleDto>();
        CreateMap<WebsiteSettingsProductRankingModule, WebsiteSettingsProductRankingModuleDto>();

        CreateMap<WebsiteBasicSetting, WebsiteBasicSettingDto>();
        CreateMap<WebsiteBasicSettingDto, UpdateWebsiteBasicSettingDto>();

        CreateMap<FooterSetting, FooterSettingDto>();
        CreateMap<FooterSettingDto, UpdateFooterSettingDto>();
        CreateMap<FooterSettingSection, FooterSettingSectionDto>();
        CreateMap<FooterSettingSectionDto, UpdateFooterSettingSectionDto>();
        CreateMap<FooterSettingLink, FooterSettingLinkDto>();
        CreateMap<FooterSettingLinkDto, UpdateFooterSettingLinkDto>();

        CreateMap<TopbarSetting, TopbarSettingDto>();
        CreateMap<TopbarSettingDto, UpdateTopbarSettingDto>();
        CreateMap<TopbarSettingLink, TopbarSettingLinkDto>();
        CreateMap<TopbarSettingLinkDto, UpdateTopbarSettingLinkDto>();
        CreateMap<TopbarSettingCategoryOption, TopbarSettingCategoryOptionDto>();
        CreateMap<TopbarSettingCategoryOptionDto, UpdateTopbarSettingCategoryOptionDto>();

        CreateMap<LoginConfiguration, LoginConfigurationDto>();
        CreateMap<LoginConfigurationDto, UpdateLoginConfigurationDto>();

        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategoryDto, UpdateProductCategoryDto>();
        CreateMap<ProductCategoryImage, ProductCategoryImageDto>()
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory!.Name));
        CreateMap<ProductCategoryImageDto, CreateUpdateProductCategoryImageDto>();
        CreateMap<CategoryProduct, CategoryProductDto>()
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory!.Name))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item!.ItemName))
            .ForMember(dest => dest.ProductCategoryFirstImageUrl, opt => opt.MapFrom(src => src.ProductCategory!.ProductCategoryImages.FirstOrDefault() != null ? src.ProductCategory.ProductCategoryImages.OrderBy(x => x.SortNo).FirstOrDefault()!.Url : null));
        CreateMap<CategoryProductDto, CreateUpdateCategoryProductDto>();
        CreateMap<CategoryProductDto, CreateUpdateItemCategoryDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ProductCategoryFirstImageUrl));

        CreateMap<SalesReportModel, SalesReportDto>();

        CreateMap<Image, ImageWithLinkDto>();

        CreateMap<OrderHistory, OrderHistoryDto>();

        CreateMap<OrderTransaction, OrderTransactionDto>();

        CreateMap<GroupBuyReportOrderModel, GroupBuyReportOrderDto>();
        CreateMap<GroupBuyReportOrderItemsModel, GroupBuyReportOrderItemsDto>();

        CreateMap<DashboardStatsModel, DashboardStatsDto>();
        CreateMap<DashboardChartsModel, DashboardChartsDto>();
        CreateMap<DashboardDonutChartModel, DashboardDonutChartDto>();
        CreateMap<DashboardBarChartModel, DashboardBarChartDto>();
        CreateMap<DashboardOrdersModel, DashboardOrdersDto>();
        CreateMap<DashboardBestSellerModel, DashboardBestSellerDto>();

        CreateMap<VipTierSetting, VipTierSettingDto>();
        CreateMap<VipTierSettingDto, UpdateVipTierSettingDto>();
        CreateMap<VipTier, VipTierDto>();
        CreateMap<VipTierDto, UpdateVipTierDto>();

        CreateMap<MemberTag, MemberTagDto>();
        CreateMap<MemberTagFilter, MemberTagFilterDto>();
        CreateMap<MemberTagFilterDto, AddTagForUsersDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag))
            .ForMember(dest => dest.RegistrationDateRange, opt => opt.MapFrom(src =>
                new DateTime?[]
                {
                    src.MinRegistrationDate,
                    src.MaxRegistrationDate
                }));

        CreateMap<GroupBuyItemsPrice, GroupBuyItemsPriceDto>();
        CreateMap<GroupBuyItemGroupImageModule, GroupBuyItemGroupImageModuleDto>();
        CreateMap<GroupBuyItemGroupImage, GroupBuyItemGroupImageDto>();

        CreateMap<Campaign, CampaignDto>();
        CreateMap<CampaignDiscount, CampaignDiscountDto>();
        CreateMap<CampaignShoppingCredit, CampaignShoppingCreditDto>();
        CreateMap<CampaignAddOnProduct, CampaignAddOnProductDto>();
        CreateMap<CampaignGroupBuy, CampaignGroupBuyDto>();
        CreateMap<CampaignProduct, CampaignProductDto>();
        CreateMap<CampaignStageSetting, CampaignStageSettingDto>();

        CreateMap<CampaignDto, CreateCampaignDto>()
            .ForMember(dest => dest.GroupBuyIds, opt => opt.MapFrom(src => src.GroupBuys != null ? src.GroupBuys.Select(gb => gb.GroupBuyId) : new List<Guid>()))
            .ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.Products != null ? src.Products.Select(gb => gb.ProductId) : new List<Guid>()));
        CreateMap<CampaignDiscountDto, CreateCampaignDiscountDto>();
        CreateMap<CampaignShoppingCreditDto, CreateCampaignShoppingCreditDto>();
        CreateMap<CampaignAddOnProductDto, CreateCampaignAddOnProductDto>();
        CreateMap<CampaignStageSettingDto, CreateCampaignStageSettingDto>();

        CreateMap<Edm, EdmDto>()
            .ForMember(dest => dest.CampaignName, opt => opt.MapFrom(src => src.Campaign != null ? src.Campaign.Name : null))
            .ForMember(dest => dest.GroupBuyName, opt => opt.MapFrom(src => src.GroupBuy != null ? src.GroupBuy.GroupBuyName : null));
        CreateMap<EdmDto, CreateEdmDto>();

        CreateMap<InventoryModel, InventoryDto>();
        CreateMap<InventoryLog, InventoryLogDto>();
    }
}