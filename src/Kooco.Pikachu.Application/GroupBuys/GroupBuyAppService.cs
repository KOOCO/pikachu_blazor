using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.AzureStorage.Image;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyAppService : ApplicationService, IGroupBuyAppService
    {
        private readonly IGroupBuyRepository _groupBuyRepository;
        private readonly GroupBuyManager _groupBuyManager;
        private readonly IRepository<Image, Guid> _imageRepository;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieRepository _freebieRepository;
        private readonly IDataFilter _dataFilter;
        private readonly ISetItemRepository _setItemRepository;

        public GroupBuyAppService(
            IGroupBuyRepository groupBuyRepository,
            GroupBuyManager groupBuyManager,
            ImageContainerManager imageContainerManager,
            IFreebieRepository freebieRepository,
            IRepository<Image, Guid> imageRepository,
            IDataFilter dataFilter,
            ISetItemRepository setItemRepository
            )
        {
            _groupBuyManager = groupBuyManager;
            _groupBuyRepository = groupBuyRepository;
            _imageContainerManager = imageContainerManager;
            _imageRepository = imageRepository;
            _dataFilter = dataFilter;
            _freebieRepository = freebieRepository;
            _setItemRepository = setItemRepository;
        }

        public async Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
        {
            var result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                        input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                        input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                        input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                        input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                        input.NotifyMessage, input.ExcludeShippingMethod, input.IsDefaultPaymentGateWay, input.PaymentMethod, input.GroupBuyCondition, input.CustomerInformation,
                                                        input.CustomerInformationDescription, input.GroupBuyConditionDescription, input.ExchangePolicyDescription, input.ShortCode);

            if (input.ItemGroups != null && input.ItemGroups.Any())
            {
                foreach (var group in input.ItemGroups)
                {
                    var itemGroup = _groupBuyManager.AddItemGroup(
                        result,
                        group.SortOrder,
                        group.GroupBuyModuleType
                        );

                    if (group.ItemDetails != null && group.ItemDetails.Any())
                    {
                        foreach (var item in group.ItemDetails)
                        {
                            _groupBuyManager.AddItemGroupDetail(
                                itemGroup,
                                item.SortOrder,
                                item.ItemId,
                                item.SetItemId,
                                item.ItemType
                                );
                        }
                    }
                }
            }

            await _groupBuyRepository.InsertAsync(result);

            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
        }

        public async Task DeleteAsync(Guid id)
        {
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

        public async Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
        {
            var count = await _groupBuyRepository.GetGroupBuyCountAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                         , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan
                                                         , input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                         input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                         input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                         input.NotifyMessage
                                                         );
            var result = await _groupBuyRepository.GetGroupBuyListAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine,
                                                        input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan,
                                                        input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                        input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                        input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy,
                                                        input.NotifyMessage, input.Sorting, input.MaxResultCount, input.SkipCount);
            return new PagedResultDto<GroupBuyDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<GroupBuy>, List<GroupBuyDto>>(result)
            };
        }

        public async Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input)
        {
            var sameName = await _groupBuyRepository.FirstOrDefaultAsync(x => x.GroupBuyName == input.GroupBuyName);

            if (sameName != null && sameName.Id != id)
            {
                throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);
            }

            var groupBuy = await _groupBuyRepository.GetWithDetailsAsync(id);

            groupBuy.GroupBuyNo = input.GroupBuyNo;
            groupBuy.Status = input.Status;
            groupBuy.GroupBuyName = input.GroupBuyName;
            groupBuy.EntryURL = input.EntryURL;
            groupBuy.EntryURL2 = input.EntryURL2;
            groupBuy.SubjectLine = input.SubjectLine;
            groupBuy.ShortName = input.ShortName;
            groupBuy.LogoURL = input.LogoURL;
            groupBuy.BannerURL = input.BannerURL;
            groupBuy.StartTime = input.StartTime;
            groupBuy.EndTime = input.EndTime;
            groupBuy.FreeShipping = input.FreeShipping;
            groupBuy.AllowShipToOuterTaiwan = input.AllowShipToOuterTaiwan;
            groupBuy.AllowShipOversea = input.AllowShipOversea;
            groupBuy.ExpectShippingDateFrom = input.ExpectShippingDateFrom;
            groupBuy.ExpectShippingDateTo = input.ExpectShippingDateTo;
            groupBuy.MoneyTransferValidDayBy = input.MoneyTransferValidDayBy;
            groupBuy.MoneyTransferValidDays = input.MoneyTransferValidDays;
            groupBuy.IssueInvoice = input.IssueInvoice;
            groupBuy.AutoIssueTriplicateInvoice = input.AutoIssueTriplicateInvoice;
            groupBuy.InvoiceNote = input.InvoiceNote;
            groupBuy.ProtectPrivacyData = input.ProtectPrivacyData;
            groupBuy.InviteCode = input.InviteCode;
            groupBuy.ProfitShare = input.ProfitShare;
            groupBuy.MetaPixelNo = input.MetaPixelNo;
            groupBuy.FBID = input.FBID;
            groupBuy.IGID = input.IGID;
            groupBuy.LineID = input.LineID;
            groupBuy.GAID = input.GAID;
            groupBuy.GTM = input.GTM;
            groupBuy.WarningMessage = input.WarningMessage;
            groupBuy.OrderContactInfo = input.OrderContactInfo;
            groupBuy.ExchangePolicy = input.ExchangePolicy;
            groupBuy.NotifyMessage = input.NotifyMessage;
            groupBuy.ExcludeShippingMethod = input.ExcludeShippingMethod;
            groupBuy.IsDefaultPaymentGateWay = input.IsDefaultPaymentGateWay;
            groupBuy.PaymentMethod = input.PaymentMethod;
            groupBuy.GroupBuyCondition = input.GroupBuyCondition;
            groupBuy.CustomerInformation = input.CustomerInformation;
            groupBuy.CustomerInformationDescription = input.CustomerInformationDescription;
            groupBuy.GroupBuyConditionDescription = input.GroupBuyConditionDescription;
            groupBuy.ExchangePolicyDescription = input.ExchangePolicyDescription;
            groupBuy.ShortCode = input.ShortCode;

            var itemGroupIds = input.ItemGroups?.Select(x => x.Id).ToList();
            if (itemGroupIds != null && itemGroupIds.Any())
            {
                _groupBuyManager.RemoveItemGroups(groupBuy, itemGroupIds);
            }

            if (input?.ItemGroups != null)
            {
                foreach (var group in input.ItemGroups)
                {
                    if (group.Id.HasValue)
                    {
                        var itemGroup = groupBuy.ItemGroups.FirstOrDefault(x => x.Id == group.Id);
                        if (itemGroup != null)
                        {
                            itemGroup.SortOrder = group.SortOrder;
                            itemGroup.GroupBuyModuleType = group.GroupBuyModuleType;

                            ProcessItemDetails(itemGroup, group.ItemDetails);
                        }
                    }
                    else
                    {
                        var itemGroup = _groupBuyManager.AddItemGroup(
                                groupBuy,
                                group.SortOrder,
                                group.GroupBuyModuleType
                         );

                        ProcessItemDetails(itemGroup, group.ItemDetails);
                    }
                }
            }


            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupBuy);
        }
        private void ProcessItemDetails(GroupBuyItemGroup itemGroup, ICollection<GroupBuyItemGroupDetailCreateUpdateDto> itemDetails)
        {
            foreach (var item in itemDetails)
            {
                if (item.Id.HasValue)
                {
                    var itemDetail = itemGroup.ItemGroupDetails.FirstOrDefault(x => x.Id == item.Id);
                    if (itemDetail != null)
                    {
                        itemDetail.SortOrder = item.SortOrder;
                        itemDetail.ItemId = item.ItemId;
                        itemDetail.SetItemId = item.SetItemId;
                        itemDetail.ItemType = item.ItemType;
                    }
                }
                else
                {
                    _groupBuyManager.AddItemGroupDetail(
                        itemGroup,
                        item.SortOrder,
                        item.ItemId,
                        item.SetItemId,
                        item.ItemType
                    );
                }
            }
        }
        public async Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds)
        {
            foreach (var id in groupBuyIds)
            {
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


        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<GroupBuyDto> GetForStoreAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var item = await _groupBuyRepository.GetAsync(id);
                return ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);

            }
        }
        
        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<GroupBuyDto> GetWithDetailsForStoreAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var item = await _groupBuyRepository.GetWithDetailsAsync(id);
                return item == null ?
                    throw new BusinessException(PikachuDomainErrorCodes.EntityWithGivenIdDoesnotExist)
                    : ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);

            }
        }

        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetCarouselImagesAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var data = await _imageRepository.GetListAsync(x => x.TargetId == id && x.ImageType == ImageType.GroupBuyCarouselImage);
                return data.Select(i => i.ImageUrl).ToList();
            }
        }

        /// <summary>
        /// This Method Returns the Desired Result For the Store Front End.
        /// Do not change unless you want to make changes in the Store Front End Code
        /// </summary>
        /// <returns></returns>
        public async Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var freebie = await _freebieRepository.GetFreebieStoreAsync(groupBuyId);
                return ObjectMapper.Map<List<Freebie>, List<FreebieDto>>(freebie);
            }
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

        }
        public async Task<List<GroupBuyDto>> GetGroupBuyByShortCode(string ShortCode) {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var query = await _groupBuyRepository.GetQueryableAsync();
                var groupbuy = query.Where(x => x.ShortCode == ShortCode).ToList();
                return ObjectMapper.Map<List<GroupBuy>, List<GroupBuyDto>>(groupbuy);
            }

        }
        public async Task<GroupBuyDto> GetGroupBuyofTenant(string ShortCode,Guid TenantId)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var query = await _groupBuyRepository.GetQueryableAsync();
                var groupbuy = query.Where(x => x.ShortCode == ShortCode && x.TenantId==TenantId).FirstOrDefault();
                return ObjectMapper.Map<GroupBuy,GroupBuyDto>(groupbuy);
            }

        }
    }
}
