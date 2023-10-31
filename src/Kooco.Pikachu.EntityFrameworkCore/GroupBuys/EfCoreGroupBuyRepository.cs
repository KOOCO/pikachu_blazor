using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuys
{
    public class EfCoreGroupBuyRepository : EfCoreRepository<PikachuDbContext, GroupBuy, Guid>, IGroupBuyRepository
    {
        public EfCoreGroupBuyRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider) { }


        public async Task<long> GetGroupBuyCountAsync(string? filterText = null, int? groupBuyNo = null, string? status = null, string? groupBuyName = null, string? entryURL = null, string? entryURL2 = null, string? subjectLine = null, string? shortName = null, string? logoURL = null, string? bannerURL = null, DateTime? startTime = null, DateTime? endTime = null, bool? freeShipping = false, bool? allowShipToOuterTaiwan = false, bool? allowShipOversea = false, DateTime? expectShippingDateFrom = null, DateTime? expectShippingDateTo = null, int? moneyTransferValidDayBy = null, int? moneyTransferValidDays = null, bool? issueInvoice = false, bool? autoIssueTriplicateInvoice = false, string? invoiceNote = null, bool? protectPrivacyData = false, string? inviteCode = null, int? profitShare = null, int? metaPixelNo = null, string? fBID = null, string? iGID = null, string? lineID = null, string? gAID = null, string? gTM = null, string? warningMessage = null, string? orderContactInfo = null, string? exchangePolicy = null, string? notifyMessage = null, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, groupBuyNo, status, groupBuyName, entryURL, entryURL2, subjectLine, shortName, logoURL, bannerURL, startTime, endTime, freeShipping,
                allowShipToOuterTaiwan, allowShipOversea, expectShippingDateFrom, expectShippingDateTo, moneyTransferValidDayBy, moneyTransferValidDays,
                issueInvoice, autoIssueTriplicateInvoice, invoiceNote, protectPrivacyData, inviteCode, profitShare, metaPixelNo, fBID, iGID, lineID, gAID, gTM,
                warningMessage, orderContactInfo, exchangePolicy, notifyMessage);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<GroupBuy>> GetGroupBuyListAsync(string? filterText = null, int? groupBuyNo = null, string? status = null, string? groupBuyName = null, string? entryURL = null, string? entryURL2 = null, string? subjectLine = null, string? shortName = null, string? logoURL = null, string? bannerURL = null, DateTime? startTime = null, DateTime? endTime = null, bool? freeShipping = false, bool? allowShipToOuterTaiwan = false, bool? allowShipOversea = false, DateTime? expectShippingDateFrom = null, DateTime? expectShippingDateTo = null, int? moneyTransferValidDayBy = null, int? moneyTransferValidDays = null, bool? issueInvoice = false, bool? autoIssueTriplicateInvoice = false, string? invoiceNote = null, bool? protectPrivacyData = false, string? inviteCode = null, int? profitShare = null, int? metaPixelNo = null, string? fBID = null, string? iGID = null, string? lineID = null, string? gAID = null, string? gTM = null, string? warningMessage = null, string? orderContactInfo = null, string? exchangePolicy = null, string? notifyMessage = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, groupBuyNo, status, groupBuyName, entryURL, entryURL2, subjectLine, shortName, logoURL, bannerURL, startTime, endTime, freeShipping,
                allowShipToOuterTaiwan, allowShipOversea, expectShippingDateFrom, expectShippingDateTo, moneyTransferValidDayBy, moneyTransferValidDays,
                issueInvoice, autoIssueTriplicateInvoice, invoiceNote, protectPrivacyData, inviteCode, profitShare, metaPixelNo, fBID, iGID, lineID, gAID, gTM,
                warningMessage, orderContactInfo, exchangePolicy, notifyMessage);

            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? GroupBuyConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<GroupBuy> GetWithDetailsAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.GroupBuys
                .Where(x => x.Id == id)
                .Include(x => x.ItemGroups.OrderBy(i => i.SortOrder))
                    .ThenInclude(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.SetItem)
                    .ThenInclude(s => s.Images)
                .Include(x => x.ItemGroups.OrderBy(i => i.SortOrder))
                    .ThenInclude(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.SetItem)
                    .ThenInclude(s => s.SetItemDetails)
                    .ThenInclude(sid => sid.Item)
                .Include(x => x.ItemGroups.OrderBy(i => i.SortOrder))
                    .ThenInclude(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.Item)
                    .ThenInclude(i => i.Images)
                .Include(x => x.ItemGroups)
                    .ThenInclude(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.Item)
                    .ThenInclude(i => i.ItemDetails)
                .Include(x => x.ItemGroups.OrderBy(i => i.SortOrder))
                    .ThenInclude(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync();
        }
        public async Task<GroupBuy> GetWithItemGroupsAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.GroupBuys
                .Where(x => x.Id == id)
                .Include(x => x.ItemGroups.OrderBy(x => x.SortOrder))
                .FirstOrDefaultAsync();
        }

        public async Task<GroupBuyItemGroup> GetGroupBuyItemGroupAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.GroupBuyItemGroups
                .Where(x => x.Id == id)
                .Include(ig => ig.ItemGroupDetails)
                    .ThenInclude(igd => igd.SetItem)
                    .ThenInclude(s => s.Images)
                .Include(ig => ig.ItemGroupDetails)
                    .ThenInclude(igd => igd.Item)
                    .ThenInclude(s => s.Images)
                .Include(ig => ig.ItemGroupDetails)
                    .ThenInclude(igd => igd.Item)
                    .ThenInclude(i => i.ItemDetails)
                .FirstOrDefaultAsync();
        }

        public async Task<GroupBuyItemGroupWithCount> GetPagedItemGroupAsync(Guid id, int skipCount)
        {
            var dbContext = await GetDbContextAsync();

            var itemGroups = dbContext.GroupBuyItemGroups
                .Where(x => x.GroupBuyId == id)
                .OrderBy(x => x.SortOrder)
                .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.SetItem)
                    .ThenInclude(s => s.Images)
                .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.SetItem)
                    .ThenInclude(s => s.SetItemDetails)
                    .ThenInclude(sid => sid.Item)
                .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.Item)
                    .ThenInclude(i => i.Images)
                .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                    .ThenInclude(igd => igd.Item)
                    .ThenInclude(i => i.ItemDetails)
                .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder));

            var itemGroup = await itemGroups
                .PageBy(skipCount, 1)
                .FirstOrDefaultAsync();

            return new GroupBuyItemGroupWithCount
            {
                TotalCount = itemGroups.Count(),
                ItemGroup = itemGroup
            };
        }

        public async Task<List<GroupBuyReport>> GetGroupBuyReportListAsync(int skipCount, int maxResultCount, string? sorting)
        {
            var dbContext = await GetDbContextAsync();
            var query = from order in dbContext.Orders
                        join groupbuy in dbContext.GroupBuys on order.GroupBuyId equals groupbuy.Id
                        group order by order.GroupBuyId into groupedOrders
                        select new GroupBuyReport
                        {
                            GroupBuyId = groupedOrders.Key,
                            GroupBuyName = groupedOrders.First().GroupBuy.GroupBuyName,
                            TotalQuantity = groupedOrders.Sum(order => order.TotalQuantity),
                            TotalAmount = groupedOrders.Where(x => x.OrderStatus == OrderStatus.Open && x.ShippingStatus == ShippingStatus.WaitingForPayment).Sum(order => order.TotalAmount),
                            PaidAmount = groupedOrders.Where(x => x.OrderStatus == OrderStatus.Open && x.ShippingStatus == ShippingStatus.PrepareShipment).Sum(order => order.TotalAmount)
                        };

            return await query
                    .OrderBy(sorting)
                    .PageBy(skipCount, maxResultCount)
                    .ToListAsync();
        }

        protected virtual IQueryable<GroupBuy> ApplyFilter(
            IQueryable<GroupBuy> query,
            string filterText,
            int? groupBuyNo = null, string? status = null, string? groupBuyName = null, string? entryURL = null, string? entryURL2 = null, string? subjectLine = null, string? shortName = null, string? logoURL = null, string? bannerURL = null, DateTime? startTime = null, DateTime? endTime = null, bool? freeShipping = false, bool? allowShipToOuterTaiwan = false, bool? allowShipOversea = false, DateTime? expectShippingDateFrom = null, DateTime? expectShippingDateTo = null, int? moneyTransferValidDayBy = null, int? moneyTransferValidDays = null, bool? issueInvoice = false, bool? autoIssueTriplicateInvoice = false, string? invoiceNote = null, bool? protectPrivacyData = false, string? inviteCode = null, int? profitShare = null, int? metaPixelNo = null, string? fBID = null, string? iGID = null, string? lineID = null, string? gAID = null, string? gTM = null, string? warningMessage = null, string? orderContactInfo = null, string? exchangePolicy = null, string? notifyMessage = null, CancellationToken cancellationToken = default)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Status.Contains(filterText) || e.GroupBuyName.Contains(filterText) ||
                    e.EntryURL.Contains(filterText) || e.EntryURL2.Contains(filterText) || e.SubjectLine.Contains(filterText) || e.ShortName.Contains(filterText)
                    || e.InvoiceNote.Contains(filterText) || e.FBID.Contains(filterText) || e.IGID.Contains(filterText) || e.LineID.Contains(filterText) || e.GAID.Contains(filterText) ||
                    e.GTM.Contains(filterText) || e.WarningMessage.Contains(filterText) || e.OrderContactInfo.Contains(filterText) || e.ExchangePolicy.Contains(filterText)
                    || e.NotifyMessage.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(groupBuyName), e => e.GroupBuyName.Contains(groupBuyName))
                    .WhereIf(!string.IsNullOrWhiteSpace(entryURL), e => e.EntryURL.Contains(entryURL))
                    .WhereIf(!string.IsNullOrWhiteSpace(entryURL2), e => e.EntryURL2.Contains(entryURL2))
                    .WhereIf(!string.IsNullOrWhiteSpace(subjectLine), e => e.SubjectLine.Contains(subjectLine))
                    .WhereIf(!string.IsNullOrWhiteSpace(shortName), e => e.ShortName.Contains(shortName))
                    .WhereIf(!string.IsNullOrWhiteSpace(invoiceNote), e => e.InvoiceNote.Contains(invoiceNote))
                    .WhereIf(!string.IsNullOrWhiteSpace(fBID), e => e.FBID.Contains(fBID))
                    .WhereIf(!string.IsNullOrWhiteSpace(iGID), e => e.IGID.Contains(iGID))
                    .WhereIf(!string.IsNullOrWhiteSpace(lineID), e => e.LineID.Contains(lineID))
                    .WhereIf(!string.IsNullOrWhiteSpace(gAID), e => e.GAID.Contains(gAID))
                    .WhereIf(!string.IsNullOrWhiteSpace(gTM), e => e.GTM.Contains(gTM))
                    .WhereIf(!string.IsNullOrWhiteSpace(warningMessage), e => e.WarningMessage.Contains(warningMessage))
                    .WhereIf(!string.IsNullOrWhiteSpace(orderContactInfo), e => e.OrderContactInfo.Contains(orderContactInfo))
                    .WhereIf(!string.IsNullOrWhiteSpace(exchangePolicy), e => e.ExchangePolicy.Contains(exchangePolicy))
                    .WhereIf(!string.IsNullOrWhiteSpace(notifyMessage), e => e.NotifyMessage.Contains(notifyMessage))
                    .WhereIf(groupBuyNo.HasValue, e => e.GroupBuyNo == groupBuyNo);
        }

        public async Task<long> GetGroupBuyReportCountAsync()
        {
            var dbContext = await GetDbContextAsync();

            return await (from groupbuy in dbContext.GroupBuys
                          join orders in dbContext.Orders
                          on groupbuy.Id equals orders.GroupBuyId
                          where orders.PaymentDate.HasValue
                          select groupbuy.Id)
                          .Distinct()
                          .CountAsync();
        }
    }
}
