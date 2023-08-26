using Kooco.Pikachu.Groupbuys;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuys
{
    
    public class GroupBuyAppService : ApplicationService, IGroupBuyAppService
    {
        private readonly IGroupBuyRepositroy _groupBuyRepository;
        private readonly GroupBuyManager _groupBuyManager;
        public GroupBuyAppService(IGroupBuyRepositroy groupBuyRepository, GroupBuyManager groupBuyManager) { 
        
        _groupBuyManager = groupBuyManager;
           _groupBuyRepository = groupBuyRepository;
        
        }

        public async Task<GroupBuyDto> CreateAsync(GroupBuyCreateDto input)
        {
            var result = await _groupBuyManager.CreateAsync(input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                         , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.allowShipToOuterTaiwan
                                                         , input.allowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                         input.issueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                         input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy, input.NotifyMessage);
            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _groupBuyRepository.DeleteAsync(id);
        }

        public async Task<GroupBuyDto> GetAsync(Guid id)
        {
           return ObjectMapper.Map<GroupBuy,GroupBuyDto>(await _groupBuyRepository.GetAsync(id));
        }

        public async Task<PagedResultDto<GroupBuyDto>> GetListAsync(GetGroupBuyInput input)
        {
            var count = await _groupBuyRepository.GetGroupBuyCountAsync(input.FilterText,input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
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
            return new PagedResultDto<GroupBuyDto> {

                TotalCount = count,
                Items = ObjectMapper.Map<List<GroupBuy>, List<GroupBuyDto>>(result)

            };
        }

        public async Task<GroupBuyDto> UpdateAsync(Guid id, GroupBuyUpdateDto input)
        {
            var result = await _groupBuyManager.UpdateAsync(id,input.GroupBuyNo, input.Status, input.GroupBuyName, input.EntryURL, input.EntryURL2, input.SubjectLine
                                                       , input.ShortName, input.LogoURL, input.BannerURL, input.StartTime, input.EndTime, input.FreeShipping, input.allowShipToOuterTaiwan
                                                       , input.allowShipOversea, input.ExpectShippingDateFrom, input.ExpectShippingDateTo, input.MoneyTransferValidDayBy, input.MoneyTransferValidDays,
                                                       input.issueInvoice, input.AutoIssueTriplicateInvoice, input.InvoiceNote, input.ProtectPrivacyData, input.InviteCode, input.ProfitShare,
                                                       input.MetaPixelNo, input.FBID, input.IGID, input.LineID, input.GAID, input.GTM, input.WarningMessage, input.OrderContactInfo, input.ExchangePolicy, input.NotifyMessage);
            return ObjectMapper.Map<GroupBuy, GroupBuyDto>(result);
        }
    }
}
