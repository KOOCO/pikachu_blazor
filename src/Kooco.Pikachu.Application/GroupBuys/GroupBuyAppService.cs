using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.DeliveryTempratureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuyItemGroups;
using Kooco.Pikachu.GroupBuyItemGroupsDetails;
using Kooco.Pikachu.GroupBuyItemsPriceses;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupBuyOrderInstructions.Interface;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Groupbuys.Interface;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.GroupPurchaseOverviews.Interface;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Validation.Localization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kooco.Pikachu.GroupBuys;

[RemoteService(IsEnabled = false)]
public class GroupBuyAppService : ApplicationService, IGroupBuyAppService
{
    #region Inject
    private readonly IGroupBuyRepository _groupBuyRepository;
    private readonly IGroupBuyItemGroupsRepository _GroupBuyItemGroupsRepository;
    private readonly GroupBuyManager _groupBuyManager;
    private readonly GroupBuyItemsPriceManager _groupBuyItemsPriceManager;
    private readonly IRepository<Image, Guid> _imageRepository;
    private readonly ImageContainerManager _imageContainerManager;
    private readonly IFreebieRepository _freebieRepository;
    private readonly IDataFilter _dataFilter;
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<DeliveryTemperatureCost, Guid> _temperatureRepository;
    private readonly IStringLocalizer<PikachuResource> _l;
    private readonly IGroupBuyItemGroupDetailsRepository _GroupBuyItemGroupDetailsRepository;
    private readonly IGroupPurchaseOverviewAppService _GroupPurchaseOverviewAppService;
    private readonly IGroupBuyOrderInstructionAppService _GroupBuyOrderInstructionAppService;
    private readonly IDeliveryTemperatureCostAppService _DeliveryTemperatureCostAppService;
    private readonly IImageAppService _ImageAppService;
    private readonly IGroupBuyProductRankingAppService _groupBuyProductRankingAppService;
    private readonly UnitOfWorkManager _unitOfWorkManager;
    private readonly ILogisticsProvidersAppService _logisticsProvidersAppService;
    private readonly IGroupBuyItemsPriceAppService _groupBuyItemsPriceAppService;
    private readonly IGroupBuyItemsPriceRepository _groupBuyItemsPriceRepository;
    private readonly IRepository<GroupBuyItemGroupImageModule, Guid> _groupBuyItemGroupImageModuleRepository;
    #endregion

    #region Constructor
    public GroupBuyAppService(
        IGroupBuyRepository groupBuyRepository,
        GroupBuyManager groupBuyManager,
        ImageContainerManager imageContainerManager,
        IFreebieRepository freebieRepository,
        IRepository<Image, Guid> imageRepository,
        IDataFilter dataFilter,
        IOrderRepository orderRepository,
        IRepository<DeliveryTemperatureCost, Guid> temperatureRepository,
        IStringLocalizer<PikachuResource> l,
        IGroupBuyItemGroupsRepository GroupBuyItemGroupsRepository,
        IGroupBuyItemGroupDetailsRepository GroupBuyItemGroupDetailsRepository,
        IGroupPurchaseOverviewAppService GroupPurchaseOverviewAppService,
        IGroupBuyOrderInstructionAppService GroupBuyOrderInstructionAppService,
        IDeliveryTemperatureCostAppService DeliveryTemperatureCostAppService,
        IImageAppService ImageAppService,
        IGroupBuyProductRankingAppService groupBuyProductRankingAppService,
        UnitOfWorkManager unitOfWorkManager,
        ILogisticsProvidersAppService logisticsProvidersAppService,
        GroupBuyItemsPriceManager groupBuyItemsPriceManager,
        IGroupBuyItemsPriceAppService groupBuyItemsPriceAppService,
        IGroupBuyItemsPriceRepository groupBuyItemsPriceRepository,
        IRepository<GroupBuyItemGroupImageModule, Guid> groupBuyItemGroupImageModuleRepository
    )
    {
        _groupBuyManager = groupBuyManager;
        _groupBuyRepository = groupBuyRepository;
        _imageContainerManager = imageContainerManager;
        _imageRepository = imageRepository;
        _dataFilter = dataFilter;
        _freebieRepository = freebieRepository;
        _orderRepository = orderRepository;
        _temperatureRepository = temperatureRepository;
        _l = l;
        _GroupBuyItemGroupsRepository = GroupBuyItemGroupsRepository;
        _GroupBuyItemGroupDetailsRepository = GroupBuyItemGroupDetailsRepository;
        _GroupPurchaseOverviewAppService = GroupPurchaseOverviewAppService;
        _GroupBuyOrderInstructionAppService = GroupBuyOrderInstructionAppService;
        _DeliveryTemperatureCostAppService = DeliveryTemperatureCostAppService;
        _ImageAppService = ImageAppService;
        _groupBuyProductRankingAppService = groupBuyProductRankingAppService;
        _unitOfWorkManager = unitOfWorkManager;
        _logisticsProvidersAppService = logisticsProvidersAppService;
        _groupBuyItemsPriceManager = groupBuyItemsPriceManager;
        _groupBuyItemsPriceRepository = groupBuyItemsPriceRepository;
        _groupBuyItemsPriceAppService = groupBuyItemsPriceAppService;
        _groupBuyItemGroupImageModuleRepository = groupBuyItemGroupImageModuleRepository;
    }
    #endregion

    #region Methods
    public async Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
    {
        GroupBuy result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                    input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                    input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                    input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                    input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                    input.NotifyMessage, input.ExcludeShippingMethod, input.IsDefaultPaymentGateWay, input.PaymentMethod, input.GroupBuyCondition, input.CustomerInformation,
                                                    input.CustomerInformationDescription, input.GroupBuyConditionDescription, input.ExchangePolicyDescription, input.ShortCode, input.IsEnterprise, input.FreeShippingThreshold, input.SelfPickupDeliveryTime,
                                                    input.BlackCatDeliveryTime, input.HomeDeliveryDeliveryTime, input.DeliveredByStoreDeliveryTime, input.TaxType, input.ProductType,
                                                    input.ColorSchemeType, input.PrimaryColor, input.SecondaryColor, input.BackgroundColor, input.SecondaryBackgroundColor, input.AlertColor, input.BlockColor, input.ProductDetailsDisplayMethod, input.NotificationBar);
        result.AddOnProduct = input.AddOnProduct;
        result.InstallmentPeriodsJson = JsonSerializer.Serialize(input.InstallmentPeriods);

        if (!input.FacebookLink.IsNullOrEmpty()) result.FacebookLink = input.FacebookLink;

        if (!input.InstagramLink.IsNullOrEmpty()) result.InstagramLink = input.InstagramLink;

        if (!input.LINELink.IsNullOrEmpty()) result.LINELink = input.LINELink;

        result.TemplateType = input.TemplateType;

        if (input.ItemGroups is not null && input.ItemGroups.Any())
        {
            foreach (var group in input.ItemGroups)
            {
                var itemGroup = _groupBuyManager.AddItemGroup(
                    result,
                    group.SortOrder,
                    group.GroupBuyModuleType,
                    group.AdditionalInfo,
                    group.ProductGroupModuleTitle,
                    group.ProductGroupModuleImageSize,
                    group.Title,
                    group.Text,
                    group.Url
                );

                itemGroup.ModuleNumber = group.ModuleNumber;

                await ProcessImageModules(itemGroup, group.ImageModules);

                if (group.ItemDetails != null && group.ItemDetails.Any())
                {
                    foreach (var item in group.ItemDetails.DistinctBy(x => x.ItemId))
                    {
                        _groupBuyManager.AddItemGroupDetail(
                            itemGroup,
                            item.SortOrder,
                            item.ItemId,
                            item.SetItemId,
                            item.ItemType,
                            item.DisplayText,
                            item.ModuleNumber
                        );

                    }
                }
            }
        }

        await _groupBuyRepository.InsertAsync(result);
        if (input.ItemGroups is not null && input.ItemGroups.Any())
        {
            foreach (var group in input.ItemGroups)
            {


                if (group.ItemDetails != null && group.ItemDetails.Any())
                {

                    foreach (var item in group.ItemDetails.DistinctBy(x => x.ItemDetailId))
                    {


                        if (group.GroupBuyModuleType == GroupBuyModuleType.ProductGroupModule)
                        {
                            if (item.ItemType == ItemType.Item)
                            {
                                await _groupBuyItemsPriceManager.CreateAsync(null, result.Id, item.Price, item.ItemDetailId);
                            }
                            else
                            {
                                await _groupBuyItemsPriceManager.CreateAsync(item.SetItemId, result.Id, item.Price, null);
                            }

                        }
                    }
                }
            }
        }
        return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
    }

    public async Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input)
    {
        using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            try
            {
                bool sameName = await _groupBuyRepository.AnyAsync(x => x.GroupBuyName == input.GroupBuyName && x.Id != id);
                if (sameName) throw new BusinessException(PikachuDomainErrorCodes.GroupBuyWithSameNameAlreadyExists);

                var groupBuy = await _groupBuyRepository.GetAsync(id);

                await _groupBuyRepository.EnsureCollectionLoadedAsync(groupBuy, x => x.ItemGroups);
                ObjectMapper.Map(input, groupBuy);
                groupBuy.InstallmentPeriodsJson = JsonSerializer.Serialize(input.InstallmentPeriods);

                await ProcessItemGroups(groupBuy, input.ItemGroups.ToList());

                groupBuy.ItemGroups.RemoveAll(w => w.Id == Guid.Empty);

                await _groupBuyRepository.UpdateAsync(groupBuy, true);

                await uow.CompleteAsync(); // Commit unit of work

                return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupBuy);
            }
            catch (AbpDbConcurrencyException)
            {
                uow.Dispose(); // Dispose current unit of work
                return await HandleConcurrencyAsync(id, input); // Retry update in new UOW
            }
            catch (Exception e)
            {
                uow.Dispose(); // Ensure rollback on failure
                throw;
            }
        }
    }

    private async Task<GroupBuyDto> HandleConcurrencyAsync(Guid id, GroupBuyUpdateDto input)
    {
        using (var retryUow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            try
            {
                var groupBuy = await _groupBuyRepository.GetAsync(id);
                await _groupBuyRepository.EnsureCollectionLoadedAsync(groupBuy, x => x.ItemGroups);

                ObjectMapper.Map(input, groupBuy);

                await ProcessItemGroups(groupBuy, input.ItemGroups.ToList());

                groupBuy.ItemGroups.RemoveAll(w => w.Id == Guid.Empty);

                await _groupBuyRepository.UpdateAsync(groupBuy);

                await retryUow.CompleteAsync(); // Commit new UOW

                return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupBuy);
            }
            catch (Exception)
            {
                retryUow.Dispose(); // Ensure rollback if retry fails
                throw;
            }
        }
    }


    private async Task ProcessItemGroups(GroupBuy groupBuy, List<GroupBuyItemGroupCreateUpdateDto> itemGroups)
    {
        if (itemGroups is { Count: > 0 })
        {
            foreach (GroupBuyItemGroupCreateUpdateDto group in itemGroups)
            {
                if (group.Id.HasValue)
                {
                    GroupBuyItemGroup? itemGroup = groupBuy.ItemGroups.FirstOrDefault(x => x.Id == group.Id);
                    if (itemGroup is not null)
                    {
                        itemGroup.SortOrder = group.SortOrder;
                        itemGroup.GroupBuyModuleType = group.GroupBuyModuleType;
                        itemGroup.AdditionalInfo = group.AdditionalInfo;
                        itemGroup.ProductGroupModuleTitle = group.ProductGroupModuleTitle;
                        itemGroup.ProductGroupModuleImageSize = group.ProductGroupModuleImageSize;
                        itemGroup.ModuleNumber = group.ModuleNumber;
                        itemGroup.Title = group.Title;
                        itemGroup.Text = group.Text;
                        itemGroup.Url = group.Url;
                        itemGroup.TenantId = CurrentTenant.Id;

                        if (group.ItemDetails.Count is 0) groupBuy.ItemGroups.Remove(itemGroup);

                        await _GroupBuyItemGroupsRepository.UpdateAsync(itemGroup);

                        if (group.ItemDetails is { Count: > 0 })
                            await _GroupBuyItemGroupDetailsRepository.DeleteAsync(d => d.GroupBuyItemGroupId == itemGroup.Id);

                        ProcessItemDetails(itemGroup, group.ItemDetails);
                        await ProcessImageModules(itemGroup, group.ImageModules);

                        itemGroup.ItemGroupDetails ??= [];
                        foreach (GroupBuyItemGroupDetails itemGroupDetails in itemGroup.ItemGroupDetails)
                        {
                            await _GroupBuyItemGroupDetailsRepository.UpdateAsync(itemGroupDetails);
                        }
                    }
                }
                else
                {
                    GroupBuyItemGroup itemGroup = _groupBuyManager.AddItemGroup(
                        groupBuy,
                        group.SortOrder,
                        group.GroupBuyModuleType,
                        group.AdditionalInfo,
                        group.ProductGroupModuleTitle,
                        group.ProductGroupModuleImageSize,
                        group.Title,
                        group.Text,
                        group.Url
                    );

                    itemGroup.ModuleNumber = group.ModuleNumber;

                    await _GroupBuyItemGroupsRepository.InsertAsync(itemGroup);

                    ProcessItemDetails(itemGroup, group.ItemDetails);
                    await ProcessImageModules(itemGroup, group.ImageModules);

                    foreach (GroupBuyItemGroupDetails itemGroupDetails in itemGroup.ItemGroupDetails)
                    {
                        await _GroupBuyItemGroupDetailsRepository.InsertAsync(itemGroupDetails);
                    }
                }
            }
        }
    }

    private async Task ProcessImageModules(GroupBuyItemGroup itemGroup, ICollection<GroupBuyItemGroupImageModuleDto> imageModules)
    {
        foreach (var module in imageModules)
        {
            var existing = await _groupBuyItemGroupImageModuleRepository.FirstOrDefaultAsync(im => im.Id == module.Id);
            if (module.Id != Guid.Empty && existing != null)
            {
                await _groupBuyItemGroupImageModuleRepository.EnsureCollectionLoadedAsync(existing, e => e.Images);
                existing.Images = [.. module.Images
                    .Select(image => new GroupBuyItemGroupImage(GuidGenerator.Create())
                    {
                        GroupBuyItemGroupImageModuleId = existing.Id,
                        Url = image.Url,
                        SortNo = image.SortNo,
                        BlobImageName = image.BlobImageName
                    })];
            }
            else
            {
                var newImageModule = new GroupBuyItemGroupImageModule(GuidGenerator.Create())
                {
                    GroupBuyItemGroupId = itemGroup.Id
                };

                newImageModule.Images = [.. module.Images
                    .Select(image => new GroupBuyItemGroupImage(GuidGenerator.Create())
                    {
                        GroupBuyItemGroupImageModuleId = newImageModule.Id,
                        Url = image.Url,
                        SortNo = image.SortNo,
                        BlobImageName = image.BlobImageName
                    })];

                itemGroup.ImageModules.Add(newImageModule);
            }
        }
    }

    public async Task<GroupBuyDto> CopyAsync(Guid Id)
    {
        var query = await _groupBuyRepository.GetQueryableAsync();
        var input = await _groupBuyRepository.GetWithDetailsAsync(Id);
        var count = query.Where(x => x.GroupBuyName.Contains(input.GroupBuyName)).Count();
        var Name = input.GroupBuyName + "(" + count + ")";
        var ShortCode = "";
        var result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, Name, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                    input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                    input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                    input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                    input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                    input.NotifyMessage, input.ExcludeShippingMethod, input.IsDefaultPaymentGateWay, input.PaymentMethod, input.GroupBuyCondition, input.CustomerInformation,
                                                    input.CustomerInformationDescription, input.GroupBuyConditionDescription, input.ExchangePolicyDescription, ShortCode, input.IsEnterprise, input.FreeShippingThreshold,
                                                    input.SelfPickupDeliveryTime, input.BlackCatDeliveryTime, input.HomeDeliveryDeliveryTime, input.DeliveredByStoreDeliveryTime, input.TaxType, input.ProductType,
                                                    input.ColorSchemeType, input.PrimaryColor, input.SecondaryColor, input.BackgroundColor, input.SecondaryBackgroundColor, input.AlertColor, input.BlockColor, input.ProductDetailsDisplayMethod, input.NotificationBar);

        if (input.ItemGroups != null && input.ItemGroups.Any())
        {
            foreach (var group in input.ItemGroups)
            {

                var itemGroup = _groupBuyManager.AddItemGroup(
                   result,
                   group.SortOrder,
                   group.GroupBuyModuleType,
                   group.AdditionalInfo,
                   group.ProductGroupModuleTitle,
                   group.ProductGroupModuleImageSize,
                   group.Title,
                   group.Text,
                   group.Url
               );

                itemGroup.ModuleNumber = group.ModuleNumber;

                if (group.ItemGroupDetails != null && group.ItemGroupDetails.Any())
                {
                    foreach (var item in group.ItemGroupDetails)
                    {
                        _groupBuyManager.AddItemGroupDetail(
                            itemGroup,
                            item.SortOrder,
                            item.ItemId,
                            item.SetItemId,
                            item.ItemType,
                            item.DisplayText,
                            item.ModuleNumber
                        );
                    }
                }
            }
        }

        List<ImageDto> images = await _ImageAppService.GetGroupBuyImagesAsync(input.Id);

        foreach (ImageDto image in images)
        {
            CreateImageDto newImage = new();

            newImage = ObjectMapper.Map<ImageDto, CreateImageDto>(image);

            newImage.Id = GuidGenerator.Create();

            newImage.TargetId = result.Id;

            await _ImageAppService.CreateAsync(newImage);
        }

        await _groupBuyRepository.InsertAsync(result);

        var GroupPurchaseOverviewModules = await _GroupPurchaseOverviewAppService.GetListByGroupBuyIdAsync(Id);
        if (GroupPurchaseOverviewModules is { Count: > 0 })
        {
            foreach (GroupPurchaseOverviewDto groupPurchaseOverview in GroupPurchaseOverviewModules)
            {

                groupPurchaseOverview.GroupBuyId = result.Id;
                groupPurchaseOverview.Id = Guid.Empty;
                await _GroupPurchaseOverviewAppService.CreateGroupPurchaseOverviewAsync(groupPurchaseOverview);

            }
        }
        var GroupBuyOrderInstructionModules = await _GroupBuyOrderInstructionAppService.GetListByGroupBuyIdAsync(Id);

        if (GroupBuyOrderInstructionModules is { Count: > 0 })
        {
            foreach (GroupBuyOrderInstructionDto groupBuyOrderInstruction in GroupBuyOrderInstructionModules)
            {

                groupBuyOrderInstruction.GroupBuyId = result.Id;
                groupBuyOrderInstruction.Id = Guid.Empty;

                await _GroupBuyOrderInstructionAppService.CreateGroupBuyOrderInstructionAsync(groupBuyOrderInstruction);

            }
        }
        var ProductRankingCarouselModules = await _groupBuyProductRankingAppService.GetListByGroupBuyIdAsync(Id);
        if (ProductRankingCarouselModules is { Count: > 0 })
        {
            foreach (var productRankingCarouselModule in ProductRankingCarouselModules)
            {



                await _groupBuyProductRankingAppService.CreateGroupBuyProductRankingAsync(new()
                {
                    GroupBuyId = result.Id,
                    Title = productRankingCarouselModule.Title,
                    SubTitle = productRankingCarouselModule.SubTitle,
                    Content = productRankingCarouselModule.Content,
                    ModuleNumber = ProductRankingCarouselModules.IndexOf(productRankingCarouselModule) + 1
                });

            }
        }

        return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
    }
    public async Task DeleteAsync(Guid id)
    {
        var check = (await _orderRepository.GetQueryableAsync()).Any(x => x.GroupBuyId == id);
        if (check)
        {
            throw new UserFriendlyException(_l["Groupbuyhaveorderitsnotdeleteable"]);

        }
        await _groupBuyRepository.DeleteAsync(id);
    }

    public async Task<GroupBuyDto> GetAsync(Guid id, bool includeDetails = false)
    {
        var item = await _groupBuyRepository.GetAsync(id);
        if (includeDetails)
        {
            await _groupBuyRepository.EnsureCollectionLoadedAsync(item, i => i.ItemGroups);
        }

        return ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);
    }
    public async Task<GroupBuyDto> GetWithDetailsAsync(Guid id)
    {
        var item = await _groupBuyRepository.GetWithDetailsAsync(id);
        return item is null
            ? throw new BusinessException(PikachuDomainErrorCodes.EntityWithGivenIdDoesnotExist)
            : ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);
    }

    public async Task DeleteGroupBuyItemAsync(Guid id, Guid GroupBuyID)
    {
        var groupbuy = await _groupBuyRepository.GetAsync(GroupBuyID);
        await _groupBuyRepository.EnsureCollectionLoadedAsync(groupbuy, i => i.ItemGroups);
        var itemGroup = groupbuy.ItemGroups.Where(i => i.Id == id).First();
        groupbuy.ItemGroups.Remove(itemGroup);
        await _groupBuyRepository.UpdateAsync(groupbuy);
    }

    public async Task GroupBuyItemModuleNoReindexingAsync(Guid groupBuyId, GroupBuyModuleType groupBuyModuleType)
    {
        GroupBuy groupbuy = await _groupBuyRepository.GetAsync(groupBuyId);

        await _groupBuyRepository.EnsureCollectionLoadedAsync(groupbuy, i => i.ItemGroups);

        int moduleNumber = 0;

        foreach (GroupBuyItemGroup itemGroup in groupbuy.ItemGroups.Where(w => w.GroupBuyModuleType == groupBuyModuleType).ToList())
        {
            int oldModuleNumber = (int)itemGroup.ModuleNumber;

            itemGroup.ModuleNumber = moduleNumber + 1;

            int newModuleNumber = (int)itemGroup.ModuleNumber;

            await _ImageAppService.ImagesModuleNoReindexingAsync(groupBuyId, GetImageType(groupBuyModuleType), oldModuleNumber, newModuleNumber);

            await _GroupBuyItemGroupsRepository.UpdateAsync(itemGroup);
        }
    }

    private ImageType GetImageType(GroupBuyModuleType groupBuyModuleType)
    {
        return groupBuyModuleType switch
        {
            GroupBuyModuleType.CarouselImages => ImageType.GroupBuyCarouselImage,
            GroupBuyModuleType.BannerImages => ImageType.GroupBuyBannerImage,
            _ => ImageType.GroupBuyCarouselImage,
        };
    }

    public async Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
    {
        var sorting = "CreationTime DESC";
        var count = await _groupBuyRepository.GetGroupBuyCountAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                     , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan
                                                     , input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                     input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                     input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                     input.NotifyMessage, input.ExcludeShippingMethod, input.PaymentMethod, input.IsInviteCode, input.IsEnterprise, input.IsGroupBuyAvaliable
                                                     );
        var result = await _groupBuyRepository.GetGroupBuyListAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                    input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                    input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                    input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                    input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                    input.NotifyMessage, input.ExcludeShippingMethod, input.PaymentMethod, input.IsInviteCode, input.IsEnterprise, input.IsGroupBuyAvaliable, sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<GroupBuyDto>
        {
            TotalCount = count,
            Items = ObjectMapper.Map<List<GroupBuyList>, List<GroupBuyDto>>(result)
        };
    }
    public async Task UpdateItemProductPrice(Guid groupbuyId, ICollection<GroupBuyItemGroupDetailCreateUpdateDto> itemDetails)
    {
        await _groupBuyItemsPriceAppService.DeleteAllGroupByItemAsync(groupbuyId);

        foreach (var item in itemDetails.DistinctBy(x => x.ItemDetailId))
        {



            if (item.ItemType == ItemType.Item)
            {


                await _groupBuyItemsPriceManager.CreateAsync(null, groupbuyId, item.Price, item.ItemDetailId);
            }
            else
            {



                await _groupBuyItemsPriceManager.CreateAsync(item.SetItemId, groupbuyId, item.Price, null);
            }


        }


    }
    private void ProcessItemDetails(GroupBuyItemGroup itemGroup, ICollection<GroupBuyItemGroupDetailCreateUpdateDto> itemDetails)
    {
        itemGroup.ItemGroupDetails?.Clear();
        foreach (var item in itemDetails.DistinctBy(x => x.ItemId))
        {
            _groupBuyManager.AddItemGroupDetail(
                itemGroup,
                item.SortOrder,
                item.ItemId,
                item.SetItemId,
                item.ItemType,
                item.DisplayText,
                item.ModuleNumber
            );
        }

    }
    public async Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds)
    {
        foreach (var id in groupBuyIds)
        {
            var check = (await _orderRepository.GetQueryableAsync()).Any(x => x.GroupBuyId == id);
            if (check)
            {
                throw new UserFriendlyException(_l["Groupbuyhaveorderitsnotdeleteable"]);

            }
            var images = await _imageRepository.GetListAsync(x => x.TargetId == id);
            if (images == null) continue;

            foreach (var image in images)
            {
                await _imageContainerManager.DeleteAsync(image.BlobImageName);
            }
        }
        await _groupBuyRepository.DeleteManyAsync(groupBuyIds);
    }

    public async Task ChangeGroupBuyAvailability(Guid groupBuyID)
    {
        var groupBuy = await _groupBuyRepository.FindAsync(x => x.Id == groupBuyID);
        groupBuy.IsGroupBuyAvaliable = !groupBuy.IsGroupBuyAvaliable;
        await _groupBuyRepository.UpdateAsync(groupBuy);
    }

    public async Task<GroupBuyItemGroupDto> GetGroupBuyItemGroupAsync(Guid id)
    {
        var itemGroup = await _groupBuyRepository.GetGroupBuyItemGroupAsync(id);
        return ObjectMapper.Map<GroupBuyItemGroup, GroupBuyItemGroupDto>(itemGroup);
    }

    public async Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
    {
        var groupbuys = (await _groupBuyRepository.GetQueryableAsync())
                        .Where(g => g.IsGroupBuyAvaliable)
                        .Select(x => new GroupBuyList { Id = x.Id, GroupBuyName = x.GroupBuyName })
                        .OrderBy(x => x.GroupBuyName)
                        .ToList();
        return ObjectMapper.Map<List<GroupBuyList>, List<KeyValueDto>>(groupbuys);
    }
    public async Task<List<KeyValueDto>> GetAllGroupBuyLookupAsync()
    {
        var groupbuys = (await _groupBuyRepository.GetQueryableAsync())
            .Select(x => new GroupBuyList { Id = x.Id, GroupBuyName = x.GroupBuyName })
            .OrderBy(x => x.GroupBuyName)
            .ToList();
        return ObjectMapper.Map<List<GroupBuyList>, List<KeyValueDto>>(groupbuys);
    }

    public async Task<ShippingMethodResponse> GetGroupBuyShippingMethodAsync(Guid id)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            GroupBuy item = await _groupBuyRepository.GetAsync(id);
            var response = new ShippingMethodResponse();

            // Deserialize relevant data
            List<string>? shippingMethods = !string.IsNullOrEmpty(item?.ExcludeShippingMethod)
                ? JsonSerializer.Deserialize<List<string>>(item.ExcludeShippingMethod)
                : [];

            List<string>? homeDeliveryTimes = !string.IsNullOrEmpty(item?.HomeDeliveryDeliveryTime)
                ? JsonSerializer.Deserialize<List<string>>(item.HomeDeliveryDeliveryTime)
                : [];

            List<string>? convenienceStoreTimes = !string.IsNullOrEmpty(item?.DeliveredByStoreDeliveryTime)
                ? JsonSerializer.Deserialize<List<string>>(item.DeliveredByStoreDeliveryTime)
                : [];

            List<string>? selfPickupTimes = !string.IsNullOrEmpty(item?.SelfPickupDeliveryTime)
                ? JsonSerializer.Deserialize<List<string>>(item.SelfPickupDeliveryTime)
                : [];

            List<string>? blackCatTCatPickupTimes = !string.IsNullOrEmpty(item?.BlackCatDeliveryTime)
                ? JsonSerializer.Deserialize<List<string>>(item.BlackCatDeliveryTime)
                : [];

            // Get all logistics providers
            using (CurrentTenant.Change(item.TenantId))
            {
                var logisticsProviders = await _logisticsProvidersAppService.GetAllAsync();


                var providerMapping = new Dictionary<DeliveryMethod, LogisticProviders>
                {
                    { DeliveryMethod.HomeDelivery, LogisticProviders.HomeDelivery },
                    { DeliveryMethod.PostOffice, LogisticProviders.PostOffice },
                    { DeliveryMethod.FamilyMart1, LogisticProviders.FamilyMart },
                    { DeliveryMethod.SevenToEleven1, LogisticProviders.SevenToEleven },
                    { DeliveryMethod.SevenToElevenFrozen, LogisticProviders.SevenToElevenFrozen },
                    { DeliveryMethod.BlackCat1, LogisticProviders.BNormal },
                    { DeliveryMethod.BlackCatFreeze, LogisticProviders.BFreeze },
                    { DeliveryMethod.BlackCatFrozen, LogisticProviders.BFrozen },
                    { DeliveryMethod.FamilyMartC2C, LogisticProviders.FamilyMartC2C },
                    { DeliveryMethod.SevenToElevenC2C, LogisticProviders.SevenToElevenC2C },
                    { DeliveryMethod.TCatDeliveryNormal, LogisticProviders.TCatNormal },
                    { DeliveryMethod.TCatDeliveryFreeze, LogisticProviders.TCat711Freeze },
                    { DeliveryMethod.TCatDeliveryFrozen, LogisticProviders.TCatFrozen },
                    { DeliveryMethod.TCatDeliverySevenElevenNormal, LogisticProviders.TCat711Normal },
                    { DeliveryMethod.TCatDeliverySevenElevenFreeze, LogisticProviders.TCat711Freeze },
                    { DeliveryMethod.TCatDeliverySevenElevenFrozen, LogisticProviders.TCat711Frozen }
                };

                bool IsMethodEnabled(string method)
                {
                    foreach (var pair in providerMapping)
                    {
                        if (method.Contains(pair.Key.ToString()))
                        {
                            var matched = logisticsProviders.FirstOrDefault(p => p.LogisticProvider == pair.Value && p.TenantId == item.TenantId);
                            return matched?.IsEnabled == true;
                        }
                    }
                    return false;
                }

                foreach (string method in shippingMethods ?? [])
                {
                    if (IsMethodEnabled(method))
                    {
                        if (method.Contains("HomeDelivery"))
                        {
                            var matchingTimes = homeDeliveryTimes.Where(t => !string.IsNullOrEmpty(t)).ToList();
                            response.HomeDeliveryType[method] = matchingTimes.Count > 0 ? matchingTimes : ["No time preference"];
                        }
                        else if (method.Contains("TCatDelivery") || method.Contains("BlackCat"))
                        {
                            var matchingTimes = blackCatTCatPickupTimes.Where(t => !string.IsNullOrEmpty(t)).ToList();
                            response.HomeDeliveryType[method] = matchingTimes.Count > 0 ? matchingTimes : ["No time preference"];
                        }
                        else if (method.Contains("PostOffice"))
                        {
                            response.HomeDeliveryType[method] = ["Not Specified"];
                        }
                        else if (method.Contains("SevenToEleven") || method.Contains("FamilyMart") || method.Contains("TCatDeliverySevenEleven"))
                        {
                            response.ConvenienceStoreType[method] = [string.Empty];
                        }
                    }
                    else if (method.Contains("SelfPickup"))
                    {
                        var matchingTimes = selfPickupTimes.Where(t => !string.IsNullOrEmpty(t)).ToList();
                        response.SelfPickupType[method] = matchingTimes.Count > 0 ? matchingTimes : ["No time preference"];
                    }
                    else if (method.Contains("DeliveredByStore"))
                    {
                        using (_dataFilter.Enable<IMultiTenant>())
                        {
                            var deliveryTemperatureCosts = await _DeliveryTemperatureCostAppService.GetListAsync();
                            var enabled = deliveryTemperatureCosts.GroupBy(g => g.IsLogisticProviderActivated).Select(s => s.Key).FirstOrDefault();
                            if (enabled && item.ExcludeShippingMethod?.Contains("DeliveredByStore") == true)
                            {
                                foreach (var deliveryCost in deliveryTemperatureCosts)
                                {
                                    var key = deliveryCost.Temperature.ToString();
                                    response.DeliveredByStoreType.TryAdd(key, new());

                                    response.DeliveredByStoreType[key].DeliveryMethod = deliveryCost.DeliveryMethod?.ToString();

                                    if (deliveryCost.DeliveryMethod is DeliveryMethod.BlackCat1 or DeliveryMethod.BlackCatFreeze or DeliveryMethod.BlackCatFrozen or
                                        DeliveryMethod.TCatDeliveryNormal or DeliveryMethod.TCatDeliveryFreeze or DeliveryMethod.TCatDeliveryFrozen)
                                    {
                                        var matchingTimes = convenienceStoreTimes.Where(t => !string.IsNullOrEmpty(t)).ToList();
                                        response.DeliveredByStoreType[key].DeliveryTime = matchingTimes.Count > 0 ? matchingTimes : ["No time preference"];
                                        response.DeliveredByStoreType[key].DeliveryType = 0;
                                    }
                                    else if (deliveryCost.DeliveryMethod is DeliveryMethod.HomeDelivery or DeliveryMethod.PostOffice)
                                    {
                                        response.DeliveredByStoreType[key].DeliveryType = 0;
                                    }
                                    else if (deliveryCost.DeliveryMethod is DeliveryMethod.SevenToEleven1 or DeliveryMethod.SevenToElevenC2C or
                                             DeliveryMethod.FamilyMart1 or DeliveryMethod.FamilyMartC2C or
                                             DeliveryMethod.TCatDeliverySevenElevenNormal or DeliveryMethod.TCatDeliverySevenElevenFreeze or
                                             DeliveryMethod.TCatDeliverySevenElevenFrozen or DeliveryMethod.SevenToElevenFrozen)
                                    {
                                        response.DeliveredByStoreType[key].DeliveryType = 1;
                                    }
                                }
                            }
                        }
                    }
                }
                return response;
            }
        }
    }

    /// <summary>
    /// This Method Returns the Desired Result For the Store Front End.
    /// Do not change unless you want to make changes in the Store Front End Code
    /// </summary>
    /// <returns></returns>
    public async Task<GroupBuyDto> GetForStoreAsync(Guid id)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            GroupBuy item = await _groupBuyRepository.GetAsync(id);

            GroupBuyDto result = ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);

            if (result.TemplateType is not null) result.TemplateTypeName = result.TemplateType.ToString();

            if (result.ColorSchemeType is not null) result.ColorSchemeTypeName = result.ColorSchemeType.ToString();

            if (result.IsGroupBuyAvaliable)
            {
                if (result.StartTime != null && result.StartTime <= DateTime.Now && result.EndTime != null && result.StartTime >= DateTime.Now)
                    result.Status = "Open";

                else if (result.StartTime == null && result.EndTime == null)
                    result.Status = "Open";

                else result.Status = "Expired";
            }

            else result.Status = "Closed";

            return result;
        }
    }
    /// <summary>
    /// This Method Returns the Desired Result For the Store Front End.
    /// Do not change unless you want to make changes in the Store Front End Code
    /// </summary>
    /// <returns></returns>
    public async Task<GroupBuyItemGroupWithCountDto> GetPagedItemGroupAsync(Guid id, int skipCount)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var item = await _groupBuyRepository.GetPagedItemGroupAsync(id, skipCount);
            return item == null ?
                throw new BusinessException(PikachuDomainErrorCodes.EntityWithGivenIdDoesnotExist)
                : ObjectMapper.Map<GroupBuyItemGroupWithCount, GroupBuyItemGroupWithCountDto>(item);

        }
    }

    public async Task<List<GroupBuyItemGroupModuleDetailsDto>> GetGroupBuyModulesAsync(Guid groupBuyId)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var moduless = await _groupBuyRepository.GetGroupBuyItemGroupBuyGroupBuyIdAsync(groupBuyId);
            List<GroupBuyItemGroupModuleDetailsDto> modules = ObjectMapper.Map<List<GroupBuyItemGroup>, List<GroupBuyItemGroupModuleDetailsDto>>(
                moduless
            );

            foreach (GroupBuyItemGroupModuleDetailsDto module in modules)
            {
                if (module.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
                {
                    foreach (GroupBuyItemGroupDetailsDto itemDetail in module.ItemGroupDetails)
                    {
                        if (itemDetail.ItemType == ItemType.Item)
                        {
                            if (!itemDetail.Item.Attribute1Name.IsNullOrEmpty())
                            {
                                itemDetail.Item.AttributeNameOptions ??= [];

                                itemDetail.Item.AttributeNameOptions.Add(new()
                                {
                                    AttributeName = itemDetail.Item.Attribute1Name,
                                    AttributeOptions = [.. itemDetail.Item.ItemDetails.DistinctBy(x=>x.Attribute1Value).Where(w => !w.Attribute1Value.IsNullOrEmpty())
                                                                                  .Select(s => s.Attribute1Value)]
                                });
                            }

                            if (!itemDetail.Item.Attribute2Name.IsNullOrEmpty())
                            {
                                itemDetail.Item.AttributeNameOptions ??= [];

                                itemDetail.Item.AttributeNameOptions.Add(new()
                                {
                                    AttributeName = itemDetail.Item.Attribute2Name,
                                    AttributeOptions = [.. itemDetail.Item.ItemDetails.DistinctBy(x=>x.Attribute2Value).Where(w => !w.Attribute2Value.IsNullOrEmpty())
                                                                                  .Select(w => w.Attribute2Value)]
                                });
                            }

                            if (!itemDetail.Item.Attribute3Name.IsNullOrEmpty())
                            {
                                itemDetail.Item.AttributeNameOptions ??= [];

                                itemDetail.Item.AttributeNameOptions.Add(new()
                                {
                                    AttributeName = itemDetail.Item.Attribute3Name,
                                    AttributeOptions = [.. itemDetail.Item.ItemDetails.Where(w => !w.Attribute3Value.IsNullOrEmpty())
                                                                                  .Select(w => w.Attribute3Value)]
                                });
                            }
                        }
                        if (itemDetail.ItemType == ItemType.SetItem)
                        {
                            foreach (var setItemDetail in itemDetail.SetItem.SetItemDetails)
                            {
                                if (!setItemDetail.Item.Attribute1Name.IsNullOrEmpty())
                                {
                                    setItemDetail.Item.AttributeNameOptions ??= [];

                                    setItemDetail.Item.AttributeNameOptions.Add(new()
                                    {
                                        AttributeName = setItemDetail.Item.Attribute1Name,
                                        AttributeOptions = [.. setItemDetail.Item.ItemDetails.Where(w => !w.Attribute1Value.IsNullOrEmpty())
                                                                                  .Select(s => s.Attribute1Value)]
                                    });
                                }

                                if (!setItemDetail.Item.Attribute2Name.IsNullOrEmpty())
                                {
                                    setItemDetail.Item.AttributeNameOptions ??= [];

                                    setItemDetail.Item.AttributeNameOptions.Add(new()
                                    {
                                        AttributeName = setItemDetail.Item.Attribute2Name,
                                        AttributeOptions = [.. setItemDetail.Item.ItemDetails.Where(w => !w.Attribute2Value.IsNullOrEmpty())
                                                                                  .Select(s => s.Attribute2Value)]
                                    });
                                }

                                if (!setItemDetail.Item.Attribute3Name.IsNullOrEmpty())
                                {
                                    setItemDetail.Item.AttributeNameOptions ??= [];

                                    setItemDetail.Item.AttributeNameOptions.Add(new()
                                    {
                                        AttributeName = setItemDetail.Item.Attribute3Name,
                                        AttributeOptions = [.. setItemDetail.Item.ItemDetails.Where(w => !w.Attribute3Value.IsNullOrEmpty())
                                                                                  .Select(s => s.Attribute3Value)]
                                    });
                                }
                            }
                        }
                    }
                }


                if (module.GroupBuyModuleType is GroupBuyModuleType.ProductGroupModule)
                {
                    foreach (GroupBuyItemGroupDetailsDto itemDetail in module.ItemGroupDetails)
                    {
                        if (itemDetail.ItemType == ItemType.Item && itemDetail.ItemId != null)
                        {
                            List<ItemDetailsDto> removeItems = new List<ItemDetailsDto>();
                            foreach (var detailitem in itemDetail.Item?.ItemDetails)
                            {
                                var checkPrice = await _groupBuyItemsPriceAppService.GetByItemIdAndGroupBuyIdAsync(detailitem.Id, groupBuyId);
                                if (checkPrice is not null)
                                {
                                    detailitem.GroupBuyPrice = checkPrice.GroupBuyPrice;

                                }
                                else
                                {

                                    removeItems.Add(detailitem);
                                }

                            }
                            foreach (var removeItem in removeItems)
                            {
                                itemDetail.Item?.ItemDetails.Remove(removeItem);
                            }
                        }
                        if (itemDetail.ItemType == ItemType.SetItem && itemDetail.SetItemId != null)
                        {
                            itemDetail.SetItem.SetItemDetails = [];
                            var checkPrice = await _groupBuyItemsPriceAppService.GetBySetItemIdAndGroupBuyIdAsync(itemDetail.SetItemId.Value, groupBuyId);
                            if (checkPrice is not null)
                            {
                                itemDetail.SetItem.GroupBuyPrice = checkPrice.GroupBuyPrice;

                            }
                            else
                            {

                                itemDetail.SetItem = null;
                                itemDetail.SetItemId = null;
                            }


                        }
                    }
                }
                module.GroupBuyModuleTypeName = module.GroupBuyModuleType.ToString();

                if (module.GroupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
                {
                    var productRanking = await _groupBuyProductRankingAppService.GetListByGroupBuyIdAsync(groupBuyId);
                    module.GroupBuyProductRankingModules = new();
                    module.GroupBuyProductRankingModules = productRanking.Where(x => x.ModuleNumber == module.ModuleNumber).FirstOrDefault();
                    module.GroupBuyProductRankingModules.CarouselImages = (await _ImageAppService.GetGroupBuyImagesAsync(groupBuyId, ImageType.GroupBuyProductRankingCarousel)).Where(x => x.ModuleNumber == module.ModuleNumber).Select(x => x.ImageUrl).ToList();




                }
                if (module.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
                {
                    Tuple<List<string>, string?> tuple = await GetCarouselImagesModuleWiseAsync(groupBuyId, module.ModuleNumber!.Value);

                    module.CarouselModulesImages = tuple.Item1;

                    module.CarouselModuleStyle = tuple.Item2;
                }

                if (module.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
                    module.BannerModulesImages = await GetBannerImagesModuleWiseAsync(groupBuyId, module.ModuleNumber!.Value);

                if (module.GroupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview)
                    module.GroupPurchaseOverviewModules = await GetGroupPurchaseOverviewsAsync(groupBuyId);

                if (module.GroupBuyModuleType is GroupBuyModuleType.OrderInstruction)
                    module.GetGroupBuyOrderInstructionModules = await GetGroupBuyOrderInstructionsAsync(groupBuyId);
            }

            return modules;
        }
    }

    public async Task<List<GroupBuyOrderInstructionDto>> GetGroupBuyOrderInstructionsAsync(Guid groupBuyId)
    {
        return await _GroupBuyOrderInstructionAppService.GetListByGroupBuyIdAsync(groupBuyId);
    }

    public async Task<List<GroupBuyItemGroupDto>> GetGroupBuyItemGroupsAsync(Guid groupBuyId)
    {
        return ObjectMapper.Map<List<GroupBuyItemGroup>, List<GroupBuyItemGroupDto>>(
            await _groupBuyRepository.GetGroupBuyItemGroupBuyGroupBuyIdAsync(groupBuyId)
        );
    }

    /// <summary>
    /// This Method Returns the Desired Result For the Store Front End.
    /// Do not change unless you want to make changes in the Store Front End Code
    /// </summary>
    /// <returns></returns>
    public async Task<List<ImageWithLinkDto>> GetCarouselImagesAsync(Guid id)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var data = await _imageRepository.GetListAsync(x => x.TargetId == id && x.ImageType == ImageType.GroupBuyCarouselImage);
            return ObjectMapper.Map<List<Image>, List<ImageWithLinkDto>>(data);
        }
    }

    public async Task<Tuple<List<string>, string?>> GetCarouselImagesModuleWiseAsync(Guid id, int moduleNumber)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            List<Image> images = await _imageRepository.GetListAsync(x => x.TargetId == id && x.ImageType == ImageType.GroupBuyCarouselImage && x.ModuleNumber == moduleNumber);

            List<string> imageUrls = [.. images.Select(s => s.ImageUrl)];

            string? carouselStyle = images.GroupBy(g => g.CarouselStyle).Select(s => s.Key).FirstOrDefault().ToString();

            return Tuple.Create(
                imageUrls,
                carouselStyle
            );
        }
    }

    public async Task<List<string>> GetBannerImagesModuleWiseAsync(Guid id, int moduleNumber)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            List<Image> images = await _imageRepository.GetListAsync(x => x.TargetId == id && x.ImageType == ImageType.GroupBuyBannerImage && x.ModuleNumber == moduleNumber);

            return [.. images.Select(s => s.ImageUrl)];
        }
    }

    public async Task<List<GroupPurchaseOverviewDto>> GetGroupPurchaseOverviewsAsync(Guid groupBuyId)
    {
        return await _GroupPurchaseOverviewAppService.GetListByGroupBuyIdAsync(groupBuyId);
    }

    public async Task<List<ImageDto>> GetBannerImagesAsync(Guid id)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            return ObjectMapper.Map<List<Image>, List<ImageDto>>(
                await _imageRepository.GetListAsync(x => x.TargetId == id && x.ImageType == ImageType.GroupBuyBannerImage)
            );
        }
    }

    /// <summary>
    /// This Method Returns the Desired Result For the Store Front End.
    /// Do not change unless you want to make changes in the Store Front End Code
    /// </summary>
    /// <returns></returns>
    public async Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId)
    {
        List<Freebie> freebie = await _freebieRepository.GetFreebieStoreAsync(groupBuyId);

        return ObjectMapper.Map<List<Freebie>, List<FreebieDto>>(freebie);
    }

    public async Task<bool> CheckShortCodeForCreate(string shortCode)
    {

        var query = await _groupBuyRepository.GetQueryableAsync();
        var check = query.Any(x => x.ShortCode == shortCode);
        return check;
    }

    public async Task<bool> CheckShortCodeForEdit(string shortCode, Guid Id)
    {

        var query = await _groupBuyRepository.GetQueryableAsync();
        var check = query.Any(x => x.ShortCode == shortCode && x.Id != Id);
        return check;
    }

    public async Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string ShortCode)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var query = await _groupBuyRepository.GetQueryableAsync();
            var groupbuy = query.Where(x => x.ShortCode == ShortCode).ToList();
            return ObjectMapper.Map<List<GroupBuy>, List<GroupBuyDto>>(groupbuy);
        }

    }

    public async Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyReportListAsync(GetGroupBuyReportListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(GroupBuyReport.GroupBuyName);
        }
        var totalCount = await _groupBuyRepository.GetGroupBuyReportCountAsync();

        var items = await _groupBuyRepository.GetGroupBuyReportListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);

        return new PagedResultDto<GroupBuyReportDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<GroupBuyReport>, List<GroupBuyReportDto>>(items)
        };
    }
    public async Task<PagedResultDto<GroupBuyReportDto>> GetGroupBuyTenantReportListAsync(GetGroupBuyReportListDto input)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(GroupBuyReport.GroupBuyName);
            }
            var totalCount = await _groupBuyRepository.GetGroupBuyTenantReportCountAsync();

            var items = await _groupBuyRepository.GetGroupBuyTenantReportListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);

            return new PagedResultDto<GroupBuyReportDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<GroupBuyReport>, List<GroupBuyReportDto>>(items)
            };
        }
    }

    public async Task<GroupBuyDto> GetGroupBuyofTenant(string ShortCode, Guid TenantId)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var query = await _groupBuyRepository.GetQueryableAsync();
            var groupbuy = query.Where(x => x.ShortCode == ShortCode && x.TenantId == TenantId).FirstOrDefault();
            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupbuy);
        }

    }

    public async Task<GroupBuyDto> GetWithItemGroupsAsync(Guid id)
    {
        var groupbuy = await _groupBuyRepository.GetWithItemGroupsAsync(id);
        return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupbuy);
    }

    public async Task<GroupBuyReportDetailsDto> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null)
    {
        var data = await _groupBuyRepository.GetGroupBuyReportDetailsAsync(id, startDate, endDate, orderStatus);
        return ObjectMapper.Map<GroupBuyReportDetails, GroupBuyReportDetailsDto>(data);
    }

    public async Task<GroupBuyReportDetailsDto> GetGroupBuyTenantReportDetailsAsync(Guid id)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var data = await _groupBuyRepository.GetGroupBuyReportDetailsAsync(id);
            return ObjectMapper.Map<GroupBuyReportDetails, GroupBuyReportDetailsDto>(data);
        }
    }

    public async Task<IRemoteStreamContent> GetTenantsListAsExcelFileAsync(Guid id)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var items = await _orderRepository.GetListAsync(0, int.MaxValue, nameof(Order.CreationTime), null, id, new List<Guid>());

            // Create a dictionary for localized headers
            var headers = new Dictionary<string, string>
            {
                { "OrderNo", _l["OrderNo"] },
                { "OrderDate", _l["OrderDate"] },
                { "CustomerName", _l["CustomerName"] },
                { "Email", _l["Email"] },
                { "OrderStatus", _l["OrderStatus"] },
                { "ShippingStatus", _l["ShippingStatus"] },
                { "PaymentMethod", _l["PaymentMethod"] },
                { "CheckoutAmount", _l["CheckoutAmount"] }
            };

            var excelData = items.Select(x => new Dictionary<string, object>
            {
                { headers["OrderNo"], x.OrderNo },
                { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
                { headers["CustomerName"], x.CustomerName },
                { headers["Email"], x.CustomerEmail },
                { headers["OrderStatus"], _l[x.OrderStatus.ToString()] },
                { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
                { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
                { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(excelData);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }

    public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null, bool isChinese = false)
    {
        var groupBuy = await _groupBuyRepository.FirstOrDefaultAsync(x => x.Id == id);
        var items = await _orderRepository.GetAllListAsync(0, int.MaxValue, "CreationTime desc", null, id, null, startDate, endDate, orderStatus);
        var data = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

        if (groupBuy.ProtectPrivacyData)
        {
            data = data.HideCredentials();
        }

        // Create a dictionary for localized headers
        var headers = isChinese ? new Dictionary<string, string>
        {
            { "OrderNo", "訂單號" },
            { "OrderDate", "訂單日期" },
            { "CustomerName", "客戶名稱" },
            { "Email", "電子郵件" },
            { "OrderStatus", "訂單狀態" },
            { "ShippingStatus", "運輸狀態" },
            { "PaymentMethod", "付款方式" },
            { "CheckoutAmount", "結帳金額" }
        } :
        new Dictionary<string, string>
        {
            { "OrderNo", _l["OrderNo"] },
            { "OrderDate", _l["OrderDate"] },
            { "CustomerName", _l["CustomerName"] },
            { "Email", _l["Email"] },
            { "OrderStatus", _l["OrderStatus"] },
            { "ShippingStatus", _l["ShippingStatus"] },
            { "PaymentMethod", _l["PaymentMethod"] },
            { "CheckoutAmount", _l["CheckoutAmount"] }
        };

        var excelData = data.Select(x => new Dictionary<string, object>
        {
            { headers["OrderNo"], x.OrderNo },
            { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
            { headers["CustomerName"], x.CustomerName },
            { headers["Email"], x.CustomerEmail },
            { headers["OrderStatus"], _l[x.OrderStatus.ToString()] },
            { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
            { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
            { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelData);
        memoryStream.Seek(0, SeekOrigin.Begin);

        string fileName = groupBuy?.GroupBuyName ?? "GroupBuyReport";
        return new RemoteStreamContent(memoryStream, $"{fileName}.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task<IRemoteStreamContent> GetAttachmentAsync(Guid id, Guid? tenantId, DateTime sendTime, RecurrenceType recurrenceType)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, sendTime.Hour, sendTime.Minute, sendTime.Second);
            DateTime endDate = date.AddMinutes(-1);
            var startDate = recurrenceType switch
            {
                RecurrenceType.Daily => date.AddDays(-1),
                RecurrenceType.Weekly => date.AddDays(-7),
                _ => throw new ArgumentException("Invalid RecurrenceType"),
            };
            return await GetListAsExcelFileAsync(id, startDate, endDate, null, true);
        }
    }

    public async Task UpdateSortOrderAsync(Guid id, List<GroupBuyItemGroupCreateUpdateDto> itemGroups)
    {
        GroupBuy groupbuy = await _groupBuyRepository.GetAsync(id);

        await _groupBuyRepository.EnsureCollectionLoadedAsync(groupbuy, g => g.ItemGroups);

        foreach (var item in itemGroups)
        {
            GroupBuyItemGroup? itemGroup = groupbuy.ItemGroups.FirstOrDefault(x => x.Id == item.Id);

            if (itemGroup is not null) itemGroup.SortOrder = item.SortOrder;
        }

        await _groupBuyRepository.UpdateAsync(groupbuy);
    }

    public async Task<DeliveryTemperatureCostDto> GetTemperatureCostAsync(ItemStorageTemperature itemStorageTemperature)
    {
        var query = await _temperatureRepository.GetQueryableAsync();
        var cost = query.Where(x => x.Temperature == itemStorageTemperature).FirstOrDefault();
        return ObjectMapper.Map<DeliveryTemperatureCost, DeliveryTemperatureCostDto>(cost);
    }

    public async Task<Guid?> GetGroupBuyIdAsync(string shortCode)
    {
        var queryable = await _groupBuyRepository.GetQueryableAsync();
        return queryable
            .Where(x => x.ShortCode == shortCode)
            .Select(x => x.Id)
            .FirstOrDefault();
    }
    #endregion
}
