using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Groupbuys;

public interface IGroupBuyRepository : IRepository<GroupBuy, Guid>
{
    Task<List<GroupBuyList>> GetGroupBuyListAsync(
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
         string? ExcludeShippingMethod = null, string? PaymentMethod = null, bool? IsInviteCode = null, bool? IsEnterprise = null,
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
         string?ExcludeShippingMethod=null, string? PaymentMethod = null, bool? IsInviteCode=null, bool?IsEnterprise = null,
        CancellationToken cancellationToken = default);

    Task<GroupBuy> GetWithDetailsAsync(Guid id);
    Task<GroupBuyItemGroupWithCount> GetPagedItemGroupAsync(Guid id, int skipCount);
    Task<List<GroupBuyReport>> GetGroupBuyReportListAsync(int skipCount, int maxResultCount, string? sorting);
    Task<List<GroupBuyReport>> GetGroupBuyTenantReportListAsync(int skipCount, int maxResultCount, string? sorting);
    Task<long> GetGroupBuyTenantReportCountAsync();
    Task<long> GetGroupBuyReportCountAsync();
    Task<GroupBuy> GetWithItemGroupsAsync(Guid id);
    Task<List<GroupBuyItemGroup>> GetGroupBuyItemGroupBuyGroupBuyIdAsync(Guid groupBuyId);
    Task<GroupBuyItemGroup> GetGroupBuyItemGroupAsync(Guid id);
    Task<GroupBuyReportDetails> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null);
}
