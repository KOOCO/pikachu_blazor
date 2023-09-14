using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuys
{

    public class GroupBuyAppService : ApplicationService, IGroupBuyAppService
    {
        private readonly IGroupBuyRepositroy _groupBuyRepository;
        private readonly GroupBuyManager _groupBuyManager;
        private readonly IImageAppService _imageAppService;
        
        public GroupBuyAppService(IGroupBuyRepositroy groupBuyRepository, GroupBuyManager groupBuyManager, IImageAppService imageAppService)
        {
            _groupBuyManager = groupBuyManager;
            _groupBuyRepository = groupBuyRepository;
            _imageAppService = imageAppService;
        }

        public async Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
        {
            var sameName = await _groupBuyRepository.FirstOrDefaultAsync(x => x.GroupBuyName == input.GroupBuyName);

            if (sameName != null)
            {
                throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);
            }

            var result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                         , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.AllowShipToOuterTaiwan
                                                         , input.AllowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                         input.IssueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                         input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy, input.NotifyMessage,
                                                         input.ExcludeShippingMethod, input.IsDefaultPaymentGateWay, input.PaymentMethod, input.GroupBuyCondition, input.CustomerInformation, input.CustomerInformationDescription, input.GroupBuyConditionDescription, input.ExchangePolicyDescription);

            if (input.ItemGroups.Any())
            {
                foreach (var group in input.ItemGroups)
                {
                    var itemGroup = _groupBuyManager.AddItemGroup(
                        result,
                        group.SortOrder,
                        group.Title
                        );

                    if (group.ItemDetails.Any())
                    {
                        foreach (var item in group.ItemDetails)
                        {
                            Guid? imageId = null;
                            if (item.Image != null)
                            {
                                item.Image.TargetId = result.Id;
                                var image = await _imageAppService.CreateAsync(item.Image);
                                imageId = image?.Id;
                            }

                            _groupBuyManager.AddItemGroupDetail(
                                itemGroup,
                                item.SortOrder,
                                item.ItemDescription,
                                item.ItemId,
                                imageId
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
            if (item is null)
            {
                throw new BusinessException(PikachuDomainErrorCodes.EntityWithGivenIdDoesnotExist);
            }
            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(item);
        }

        public async Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
        {
            var count = await _groupBuyRepository.GetGroupBuyCountAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                         , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.allowShipToOuterTaiwan
                                                         , input.allowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                         input.issueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                         input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy, input.NotifyMessage
                                                         );
            var result = await _groupBuyRepository.GetGroupBuyListAsync(input.FilterText, input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                         , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.allowShipToOuterTaiwan
                                                         , input.allowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                         input.issueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                         input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy, input.NotifyMessage,
                                                         input.Sorting, input.MaxResultCount, input.SkipCount);
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

            var itemGroupIds = input.ItemGroups?.Select(x => x.Id).ToList();
            if (itemGroupIds.Any())
            {
                _groupBuyManager.RemoveItemGroups(groupBuy, itemGroupIds);
            }

            if (input.ItemGroups.Any())
            {
                foreach (var group in input.ItemGroups)
                {
                    if (group.Id.HasValue)
                    {
                        var itemGroup = groupBuy.ItemGroups.First(x => x.Id == group.Id);
                        itemGroup.SortOrder = group.SortOrder;
                        itemGroup.Title = group.Title;

                        if (group.ItemDetails.Any())
                        {
                            foreach (var item in group.ItemDetails)
                            {
                                if (item.Id.HasValue)
                                {
                                    var itemDetail = itemGroup.ItemGroupDetails.First(x => x.Id == item.Id);
                                    itemDetail.SortOrder = item.SortOrder;
                                    itemDetail.ItemDescription = item.ItemDescription;
                                    itemDetail.ItemId = item.ItemId;
                                }
                                else
                                {
                                    Guid? imageId = null;
                                    if (item.Image != null)
                                    {
                                        item.Image.TargetId = groupBuy.Id;
                                        var image = await _imageAppService.CreateAsync(item.Image);
                                        imageId = image?.Id;
                                    }

                                    _groupBuyManager.AddItemGroupDetail(
                                        itemGroup,
                                        item.SortOrder,
                                        item.ItemDescription,
                                        item.ItemId,
                                        imageId
                                        );
                                }
                            }
                        }
                    }
                    else
                    {
                        var itemGroup = _groupBuyManager.AddItemGroup(
                        groupBuy,
                        group.SortOrder,
                        group.Title
                        );

                        if (group.ItemDetails.Any())
                        {
                            foreach (var item in group.ItemDetails)
                            {
                                Guid? imageId = null;
                                if (item.Image != null)
                                {
                                    item.Image.TargetId = groupBuy.Id;
                                    var image = await _imageAppService.CreateAsync(item.Image);
                                    imageId = image?.Id;
                                }

                                _groupBuyManager.AddItemGroupDetail(
                                    itemGroup,
                                    item.SortOrder,
                                    item.ItemDescription,
                                    item.ItemId,
                                    imageId
                                    );
                            }
                        }
                    }
                }
            }


            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(groupBuy);
        }

        public async Task DeleteManyGroupBuyItemsAsync(List<Guid> groupBuyIds)
        {
            await _groupBuyRepository.DeleteManyAsync(groupBuyIds);
        }
    }
}
