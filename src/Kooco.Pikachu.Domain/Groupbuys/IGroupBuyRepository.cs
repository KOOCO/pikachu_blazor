using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Groupbuys
{
    public interface IGroupBuyRepository : IRepository<GroupBuy, Guid>
    {
        Task<List<GroupBuy>> GetGroupBuyListAsync(
            string? filterText = null,
            int? groupBuyNo = null,
            string? status = null,
            string? groupBuyName = null,
            string? entryURL = null,
            string? entryURL2 = null,
            string? subjectLine = null,
            string? shortName = null,
            string? logoURL = null,
            string? bannerURL = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool? freeShipping = false,
            bool? allowShipToOuterTaiwan = false,
            bool? allowShipOversea = false,
            DateTime? expectShippingDateFrom = null,
            DateTime? expectShippingDateTo = null,
            int? moneyTransferValidDayBy = null,
            int? moneyTransferValidDays = null,
            bool? issueInvoice = false,
            bool? autoIssueTriplicateInvoice = false,
            string? invoiceNote = null,
            bool? protectPrivacyData = false,
            string? inviteCode = null,
            int? profitShare = null,
            int? metaPixelNo = null,
            string? fBID = null,
            string? iGID = null,
            string? lineID = null,
            string? gAID = null,
            string? gTM = null,
            string? warningMessage = null,
            string? orderContactInfo = null,
            string? exchangePolicy = null,
            string? notifyMessage = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
            );
        
        Task<long> GetGroupBuyCountAsync(
            string? filterText = null,
            int? groupBuyNo = null,
            string? status = null,
            string? groupBuyName = null,
            string? entryURL = null,
            string? entryURL2 = null,
            string? subjectLine = null,
            string? shortName = null,
            string? logoURL = null,
            string? bannerURL = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool? freeShipping = false,
            bool? allowShipToOuterTaiwan = false,
            bool? allowShipOversea = false,
            DateTime? expectShippingDateFrom = null,
            DateTime? expectShippingDateTo = null,
            int? moneyTransferValidDayBy = null,
            int? moneyTransferValidDays = null,
            bool? issueInvoice = false,
            bool? autoIssueTriplicateInvoice = false,
            string? invoiceNote = null,
            bool? protectPrivacyData = false,
            string? inviteCode = null,
            int? profitShare = null,
            int? metaPixelNo = null,
            string? fBID = null,
            string? iGID = null,
            string? lineID = null,
            string? gAID = null,
            string? gTM = null,
            string? warningMessage = null,
            string? orderContactInfo = null,
            string? exchangePolicy = null,
            string? notifyMessage = null,
            CancellationToken cancellationToken = default);

        Task<GroupBuy> GetWithDetailsAsync(Guid id);
        Task<GroupBuyItemGroupWithCount> GetPagedItemGroupAsync(Guid id, int skipCount);
        Task<List<GroupBuyReport>> GetGroupBuyReportListAsync(int skipCount, int maxResultCount, string? sorting);
        Task<long> GetGroupBuyReportCountAsync();
        Task<GroupBuy> GetWithItemGroupsAsync(Guid id);
        Task<GroupBuyItemGroup> GetGroupBuyItemGroupAsync(Guid id);
    }
}
