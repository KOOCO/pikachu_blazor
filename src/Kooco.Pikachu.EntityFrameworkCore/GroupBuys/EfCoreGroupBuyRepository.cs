using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.GroupBuys;

public class EfCoreGroupBuyRepository : EfCoreRepository<PikachuDbContext, GroupBuy, Guid>, IGroupBuyRepository
{
    public EfCoreGroupBuyRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider) { }


    public async Task<long> GetGroupBuyCountAsync(string? filterText = null, int? groupBuyNo = null, string? status = null, string? groupBuyName = null, string? entryURL = null, string? entryURL2 = null, string? subjectLine = null, string? shortName = null, string? logoURL = null, string? bannerURL = null, DateTime? startTime = null, DateTime? endTime = null, bool? freeShipping = false, bool? allowShipToOuterTaiwan = false, bool? allowShipOversea = false, DateTime? expectShippingDateFrom = null, DateTime? expectShippingDateTo = null, int? moneyTransferValidDayBy = null, int? moneyTransferValidDays = null, bool? issueInvoice = false, bool? autoIssueTriplicateInvoice = false, string? invoiceNote = null, bool? protectPrivacyData = false, string? inviteCode = null, int? profitShare = null, int? metaPixelNo = null, string? fBID = null, string? iGID = null, string? lineID = null, string? gAID = null, string? gTM = null, string? warningMessage = null, string? orderContactInfo = null, string? exchangePolicy = null, string? notifyMessage = null, string? ExcludeShippingMethod = null, string? PaymentMethod = null, bool? IsInviteCode = null, bool? IsEnterprise = null, bool? isGroupbuyAvailable = null, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, groupBuyNo, status, groupBuyName, entryURL, entryURL2, subjectLine, shortName, logoURL, bannerURL, startTime, endTime, freeShipping,
            allowShipToOuterTaiwan, allowShipOversea, expectShippingDateFrom, expectShippingDateTo, moneyTransferValidDayBy, moneyTransferValidDays,
            issueInvoice, autoIssueTriplicateInvoice, invoiceNote, protectPrivacyData, inviteCode, profitShare, metaPixelNo, fBID, iGID, lineID, gAID, gTM,
            warningMessage, orderContactInfo, exchangePolicy, notifyMessage, ExcludeShippingMethod, PaymentMethod, IsInviteCode, IsEnterprise, isGroupbuyAvailable);
        if (!string.IsNullOrWhiteSpace(ExcludeShippingMethod)&& ExcludeShippingMethod!="[]")
        {
            var shippingMethod = JsonConvert.DeserializeObject<List<string>>(ExcludeShippingMethod);
            var newquery = query.AsEnumerable().Where(e => JsonConvert.DeserializeObject<List<string>>(e.ExcludeShippingMethod)
                .Any(method => shippingMethod.Contains(method)));
            return newquery.LongCount();
        }
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<GroupBuyList>> GetGroupBuyListAsync(string? filterText = null, int? groupBuyNo = null, string? status = null, string? groupBuyName = null, string? entryURL = null, string? entryURL2 = null, string? subjectLine = null, string? shortName = null, string? logoURL = null, string? bannerURL = null, DateTime? startTime = null, DateTime? endTime = null, bool? freeShipping = false, bool? allowShipToOuterTaiwan = false, bool? allowShipOversea = false, DateTime? expectShippingDateFrom = null, DateTime? expectShippingDateTo = null, int? moneyTransferValidDayBy = null, int? moneyTransferValidDays = null, bool? issueInvoice = false, bool? autoIssueTriplicateInvoice = false, string? invoiceNote = null, bool? protectPrivacyData = false, string? inviteCode = null, int? profitShare = null, int? metaPixelNo = null, string? fBID = null, string? iGID = null, string? lineID = null, string? gAID = null, string? gTM = null, string? warningMessage = null, string? orderContactInfo = null, string? exchangePolicy = null, string? notifyMessage = null, string? ExcludeShippingMethod = null, string? PaymentMethod = null, bool? IsInviteCode = null, bool? IsEnterprise = null, bool? isGroupbuyAvailable = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, groupBuyNo, status, groupBuyName, entryURL, entryURL2, subjectLine, shortName, logoURL, bannerURL, startTime, endTime, freeShipping,
            allowShipToOuterTaiwan, allowShipOversea, expectShippingDateFrom, expectShippingDateTo, moneyTransferValidDayBy, moneyTransferValidDays,
            issueInvoice, autoIssueTriplicateInvoice, invoiceNote, protectPrivacyData, inviteCode, profitShare, metaPixelNo, fBID, iGID, lineID, gAID, gTM,
            warningMessage, orderContactInfo, exchangePolicy, notifyMessage, ExcludeShippingMethod, PaymentMethod, IsInviteCode, IsEnterprise, isGroupbuyAvailable);
        if (!string.IsNullOrWhiteSpace(ExcludeShippingMethod) && ExcludeShippingMethod != "[]")
        {
            var shippingMethod = JsonConvert.DeserializeObject<List<string>>(ExcludeShippingMethod);
            var newquery = query.AsEnumerable().Where(e => JsonConvert.DeserializeObject<List<string>>(e.ExcludeShippingMethod)
                  .Any(method => shippingMethod.Contains(method)));
            newquery = newquery.AsQueryable().OrderBy(string.IsNullOrWhiteSpace(sorting) ? GroupBuyConsts.GetDefaultSorting(false) : sorting).PageBy(skipCount, maxResultCount);
            var newresult = newquery.AsQueryable().Select(x => new GroupBuyList
            {
                Id = x.Id,
                GroupBuyName = x.GroupBuyName,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                CreationTime = x.CreationTime,
                IsGroupBuyAvaliable = x.IsGroupBuyAvaliable
            }).ToList();
            return newresult;
        }
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? GroupBuyConsts.GetDefaultSorting(false) : sorting).PageBy(skipCount, maxResultCount);
        var result = await query.Select(x => new GroupBuyList
        {
            Id = x.Id,
            GroupBuyName = x.GroupBuyName,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            CreationTime = x.CreationTime,
            IsGroupBuyAvaliable = x.IsGroupBuyAvaliable
        }).ToListAsync();
        return result;
    }

    public async Task<GroupBuy> GetWithDetailsAsync(Guid id)
    {
        //var dbContext = await GetDbContextAsync();

        IQueryable<GroupBuy> query = await GetQueryableAsync();

        return await query.AsNoTracking()
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
                 .Include(x => x.ItemGroups.OrderBy(i => i.SortOrder))
              .ThenInclude(ig => ig.ImageModules)
                .ThenInclude(im => im.Images)
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

    public async Task<List<GroupBuyItemGroup>> GetGroupBuyItemGroupBuyGroupBuyIdAsync(Guid groupBuyId)
    {
        PikachuDbContext dbContext = await GetDbContextAsync();

        var itemGroups = dbContext.GroupBuyItemGroups
            .Where(x => x.GroupBuyId == groupBuyId)
            .OrderBy(x => x.SortOrder)
            .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                .ThenInclude(igd => igd.SetItem)
                .ThenInclude(s => s.Images)
            .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                .ThenInclude(igd => igd.SetItem)
                .ThenInclude(s => s.SetItemDetails)
                .ThenInclude(sid => sid.Item)
                .ThenInclude(i => i.ItemDetails)
            .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                .ThenInclude(igd => igd.Item)
                .ThenInclude(i => i.Images)
            .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
                .ThenInclude(igd => igd.Item)
                .ThenInclude(i => i.ItemDetails)
            .Include(ig => ig.ItemGroupDetails.OrderBy(i => i.SortOrder))
            .Include(ig => ig.ImageModules)
                .ThenInclude(im => im.Images.OrderBy(i => i.SortNo));
        return [.. itemGroups];
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
            .Include(ig => ig.ImageModules)
                .ThenInclude(im => im.Images)
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

        // Filter the items to include only those that are available
        if (itemGroup != null)
        {
            itemGroup.ItemGroupDetails = itemGroup.ItemGroupDetails
                .Select(igd =>
                {
                    igd.Item = igd.Item != null && igd.Item.IsItemAvaliable ? igd.Item : null;
                    if (igd.SetItem != null)
                    {
                        igd.SetItem.SetItemDetails = igd.SetItem.SetItemDetails
                            .Where(sid => sid.Item != null && sid.Item.IsItemAvaliable)
                            .ToList();
                    }
                    return igd;
                }).ToList();
        }

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
                        TotalOrder = groupedOrders.Count(),
                        CompleteOrders = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Count(),
                        SalesAmount = groupedOrders.Sum(order => order.TotalAmount),
                        AmountReceived = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Sum(order => order.TotalAmount),
                        ProfitShare = groupedOrders.First().GroupBuy.ProfitShare,
                        SalesAmountDevlieryCost = groupedOrders.Sum(order => order.DeliveryCost),
                        AmountReceivedDeliveryCost = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Sum(order => order.DeliveryCost)
                    };

        var data = await query
                .OrderBy(sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync();

        data?.ForEach(item =>
            {
                item.SalesAmount -= (item.SalesAmountDevlieryCost ?? 0);
                item.AmountReceived -= (item.AmountReceivedDeliveryCost ?? 0);
            });

        return data;
    }
    public async Task<List<GroupBuyReport>> GetGroupBuyTenantReportListAsync(int skipCount, int maxResultCount, string? sorting)
    {
        var dbContext = await GetDbContextAsync();
        var query = from order in dbContext.Orders
                    join groupbuy in dbContext.GroupBuys on order.GroupBuyId equals groupbuy.Id
                    join tenant in dbContext.Tenants on groupbuy.TenantId equals tenant.Id // Assuming there's a Tenant navigation property in GroupBuy entity
                    where groupbuy.TenantId != null
                    group order by order.GroupBuyId into groupedOrders

                    select new GroupBuyReport
                    {
                        GroupBuyId = groupedOrders.Key,
                        GroupBuyName = groupedOrders.First().GroupBuy.GroupBuyName,
                        TenantName = dbContext.Tenants.Where(x => x.Id == groupedOrders.First().GroupBuy.TenantId).Select(x => x.Name).FirstOrDefault().ToString(), // Assuming TenantName is a property in the Tenant entity
                        TotalQuantity = groupedOrders.Sum(order => order.TotalQuantity),
                        TotalAmount = groupedOrders.Where(x => x.OrderStatus == OrderStatus.Open && x.ShippingStatus == ShippingStatus.WaitingForPayment).Sum(order => order.TotalAmount),
                        PaidAmount = groupedOrders.Where(x => x.OrderStatus == OrderStatus.Open && x.ShippingStatus == ShippingStatus.PrepareShipment).Sum(order => order.TotalAmount),
                        TotalOrder = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Count(),
                        SalesAmount = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Sum(order => order.TotalAmount),
                        WatingForShipment = groupedOrders.Where(x => x.OrderStatus == OrderStatus.Open && x.ShippingStatus == ShippingStatus.PrepareShipment).Count(),
                        AmountReceived = groupedOrders.Where(x => x.OrderStatus == OrderStatus.Open && x.ShippingStatus == ShippingStatus.PrepareShipment).Sum(order => order.TotalAmount)
                    };

        return await query
                .OrderBy(sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync();
    }

    protected virtual IQueryable<GroupBuy> ApplyFilter(
        IQueryable<GroupBuy> query,
        string filterText,
        int? groupBuyNo = null, string? status = null, string? groupBuyName = null, string? entryURL = null, string? entryURL2 = null, string? subjectLine = null, string? shortName = null, string? logoURL = null, string? bannerURL = null, DateTime? startTime = null, DateTime? endTime = null, bool? freeShipping = false, bool? allowShipToOuterTaiwan = false, bool? allowShipOversea = false, DateTime? expectShippingDateFrom = null, DateTime? expectShippingDateTo = null, int? moneyTransferValidDayBy = null, int? moneyTransferValidDays = null, bool? issueInvoice = false, bool? autoIssueTriplicateInvoice = false, string? invoiceNote = null, bool? protectPrivacyData = false, string? inviteCode = null, int? profitShare = null, int? metaPixelNo = null, string? fBID = null, string? iGID = null, string? lineID = null, string? gAID = null, string? gTM = null, string? warningMessage = null, string? orderContactInfo = null, string? exchangePolicy = null, string? notifyMessage = null, string? ExcludeShippingMethod = null, string? PaymentMethod = null, bool? IsInviteCode = null, bool? IsEnterprise = null, bool? isGroupbuyAvailable = null, CancellationToken cancellationToken = default)
    {
        string[] payment= [];
        if (!string.IsNullOrWhiteSpace(PaymentMethod))
        {
            payment = PaymentMethod.Split(",");
        }
        return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Status.Contains(filterText) || e.GroupBuyName.Contains(filterText) ||
                e.Id.ToString().Contains(filterText) || e.EntryURL.Contains(filterText) || e.EntryURL2.Contains(filterText) || e.SubjectLine.Contains(filterText) || e.ShortName.Contains(filterText)
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
                
                //.WhereIf(!string.IsNullOrWhiteSpace(ExcludeShippingMethod), e =>
                //    JsonConvert.DeserializeObject<List<string>>(e.ExcludeShippingMethod)
                //    .Intersect(JsonConvert.DeserializeObject<List<string>>(ExcludeShippingMethod))
                //    .Any())
                .WhereIf(payment.Any(), e => payment.Any(pm => EF.Functions.Like(e.PaymentMethod, "%" + pm + "%")))
                .WhereIf(issueInvoice.HasValue, e => e.IssueInvoice == issueInvoice)
                .WhereIf(startTime.HasValue || endTime.HasValue, e => (!startTime.HasValue || e.EndTime.Value.Date >= startTime.Value.Date) &&
                         (!endTime.HasValue || e.StartTime.Value.Date <= endTime.Value.Date))
                .WhereIf(isGroupbuyAvailable.HasValue, e => e.IsGroupBuyAvaliable == isGroupbuyAvailable)
                .WhereIf(IsEnterprise.HasValue, e => e.IsEnterprise == IsEnterprise)
                .WhereIf(freeShipping.HasValue, e => e.FreeShipping == freeShipping)
                .WhereIf(allowShipToOuterTaiwan.HasValue, e => e.AllowShipToOuterTaiwan == allowShipToOuterTaiwan)

                .WhereIf(IsInviteCode.HasValue, e => !string.IsNullOrWhiteSpace(e.InviteCode))



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
    public async Task<long> GetGroupBuyTenantReportCountAsync()
    {
        var dbContext = await GetDbContextAsync();

        return await (from groupbuy in dbContext.GroupBuys
                      join orders in dbContext.Orders
                      on groupbuy.Id equals orders.GroupBuyId
                      where orders.PaymentDate.HasValue && groupbuy.TenantId != null
                      select groupbuy.Id)
                      .Distinct()
                      .CountAsync();
    }

    //public async Task<GroupBuyReportDetails> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null)
    //{
    //    var dbContext = await GetDbContextAsync();
    //    var query = await (from order in dbContext.Orders
    //                       .Include(x => x.OrderItems).ThenInclude(x => x.Item)
    //                .WhereIf(startDate.HasValue, x => x.CreationTime >= startDate)
    //                .WhereIf(endDate.HasValue, x => x.CreationTime < endDate)
    //                .WhereIf(orderStatus.HasValue, x => x.OrderStatus == orderStatus)
    //                       join groupbuy in dbContext.GroupBuys.Where(g => g.Id == id) on order.GroupBuyId equals groupbuy.Id
    //                       group order by order.GroupBuyId into groupedOrders
    //                       select new GroupBuyReportDetails
    //                       {
    //                           GroupBuyName = groupedOrders.First().GroupBuy.GroupBuyName,
    //                           StartDate = startDate ?? groupedOrders.First().GroupBuy.CreationTime,
    //                           EndDate = endDate ?? DateTime.Now,
    //                           OrderItems = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Distinct().SelectMany(o => o.OrderItems).ToList(),
    //                           OrderItemsPaid = groupedOrders.Where(order => (order.PaymentMethod == PaymentMethods.CashOnDelivery && order.ShippingStatus != ShippingStatus.PrepareShipment && order.ShippingStatus != ShippingStatus.ToBeShipped) || (order.PaymentMethod != PaymentMethods.CashOnDelivery && order.OrderStatus == OrderStatus.Open && order.ShippingStatus != ShippingStatus.WaitingForPayment)).Distinct().SelectMany(o => o.OrderItems).ToList(),
    //                           OrderQuantityPaid = groupedOrders.Where(order => (order.PaymentMethod == PaymentMethods.CashOnDelivery && order.ShippingStatus != ShippingStatus.PrepareShipment && order.ShippingStatus != ShippingStatus.ToBeShipped) || (order.PaymentMethod != PaymentMethods.CashOnDelivery && order.OrderStatus == OrderStatus.Open && order.ShippingStatus != ShippingStatus.WaitingForPayment)).Count() - (groupedOrders.Where(order => order.IsRefunded).Count()),
    //                           TotalOrderQuantity = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Count(),
    //                           SalesAmount = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Distinct().Sum(order => order.TotalAmount),
    //                           //SalesAmountExclShipping = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open && order.ShippingStatus == ShippingStatus.PrepareShipment).Sum(order => order.TotalAmount) ,
    //                           AmountReceived = groupedOrders.Where(x => (x.PaymentMethod == PaymentMethods.CashOnDelivery && x.ShippingStatus != ShippingStatus.PrepareShipment && x.ShippingStatus != ShippingStatus.ToBeShipped) || (x.PaymentMethod != PaymentMethods.CashOnDelivery && x.OrderStatus == OrderStatus.Open && x.ShippingStatus != ShippingStatus.WaitingForPayment)).Distinct().Sum(order => order.TotalAmount),
    //                           GroupBuy = groupedOrders.First().GroupBuy,
    //                           // SalesAmountMinusShipping = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Sum(order => order.TotalAmount) - groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open).Sum(x => x.TotalAmount - x.OrderItems.Sum(y => y.ItemPrice)),
    //                           //BloggersProfit = (groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open && order.ShippingStatus==ShippingStatus.PrepareShipment).Sum(order => order.TotalAmount) - groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open && order.ShippingStatus == ShippingStatus.PrepareShipment).Count() * 200) * (groupedOrders.First().GroupBuy.ProfitShare / 100.0M)
    //                       }).FirstOrDefaultAsync();
    //    if (query != null)
    //    {
    //        query.SalesAmountExclShipping = query.OrderItems.Sum(order => order.TotalAmount) - query.OrderItems.Sum(x => x.TotalAmount - (x.ItemPrice * x.Quantity));
    //        query.SalesAmountMinusShipping = query.SalesAmountExclShipping;
    //        query.AmountReceivedExclShipping = query.OrderItemsPaid.Sum(x => x.TotalAmount);
    //        var groupbuyProfit = query.AmountReceivedExclShipping * ((query.GroupBuy.ProfitShare) / 100.0M);
    //        var itemWithShareProfit = query.OrderItemsPaid.Where(x => x.Item != null && x.Item.ShareProfit > 0).ToList();
    //        decimal profit = 0;
    //        foreach (var item in itemWithShareProfit)
    //        {
    //            profit += ((decimal)(query.GroupBuy.ProfitShare - item.Item.ShareProfit) / 100.0M) * item.TotalAmount;

    //        }
    //        if (groupbuyProfit < profit)
    //        {
    //            query.BloggersProfit = groupbuyProfit - profit;
    //            query.BloggersProfit = -1 * query.BloggersProfit;
    //        }
    //        else
    //        {
    //            query.BloggersProfit = (decimal)(groupbuyProfit - profit);
    //        }
    //    }
    //    //query.BloggersProfit = (query.OrderItems
    //    //      .Where(item => item.Item.ShareProfit > 0)
    //    //      .Sum(item => item.ItemPrice)
    //    //      * (query.GroupBuy.ProfitShare / 100.0M))
    //    //   +
    //    //   (query.OrderItems
    //    //      .Where(item => item.Item.ShareProfit > 0)
    //    //      .Select(item => item.Quantity)
    //    //      .Distinct()
    //    //      .Count()
    //    //      * (query.GroupBuy.ProfitShare / 100.0M));
    //    if (query == null)
    //    {
    //        var groupBuy = await dbContext.GroupBuys.FirstOrDefaultAsync(x => x.Id == id);
    //        query = new GroupBuyReportDetails
    //        {
    //            GroupBuyName = groupBuy?.GroupBuyName,
    //            StartDate = startDate,
    //            EndDate = endDate
    //        };
    //    }

    //    return query;
    //}
    public async Task<GroupBuyReportDetails> GetGroupBuyReportDetailsAsync(Guid id, DateTime? startDate = null, DateTime? endDate = null, OrderStatus? orderStatus = null)
    {
        var dbContext = await GetDbContextAsync();

        var query = await (from order in dbContext.Orders
                            .Include(x => x.OrderItems).ThenInclude(x => x.Item)
                            .WhereIf(startDate.HasValue, x => x.CreationTime >= startDate)
                            .WhereIf(endDate.HasValue, x => x.CreationTime < endDate)
                            .WhereIf(orderStatus.HasValue && orderStatus != OrderStatus.Open, x => x.OrderStatus == orderStatus) // Apply filter only when status is not Open
                           join groupbuy in dbContext.GroupBuys.Where(g => g.Id == id) on order.GroupBuyId equals groupbuy.Id
                           group order by order.GroupBuyId into groupedOrders
                           select new GroupBuyReportDetails
                           {
                               GroupBuyName = groupedOrders.First().GroupBuy.GroupBuyName,
                               StartDate = startDate ?? groupedOrders.First().GroupBuy.CreationTime,
                               EndDate = endDate ?? DateTime.Now,
                               OrderItems = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open || (orderStatus.HasValue && order.OrderStatus == orderStatus)).Distinct().SelectMany(o => o.OrderItems).ToList(),
                               OrderItemsPaid = groupedOrders.Where(order => (order.PaymentMethod == PaymentMethods.CashOnDelivery && order.ShippingStatus != ShippingStatus.PrepareShipment && order.ShippingStatus != ShippingStatus.ToBeShipped) || (order.PaymentMethod != PaymentMethods.CashOnDelivery && order.OrderStatus == OrderStatus.Open && order.ShippingStatus != ShippingStatus.WaitingForPayment)).Distinct().SelectMany(o => o.OrderItems).ToList(),
                               OrderItemsPaidAndDeliverd = groupedOrders.Where(order => (order.PaymentMethod == PaymentMethods.CashOnDelivery && order.ShippingStatus == ShippingStatus.Delivered) || (order.PaymentMethod != PaymentMethods.CashOnDelivery && order.OrderStatus == OrderStatus.Open && order.ShippingStatus == ShippingStatus.Delivered)).Distinct().SelectMany(o => o.OrderItems).ToList(),
                               OrderQuantityPaid = groupedOrders.Where(order => (order.PaymentMethod == PaymentMethods.CashOnDelivery && order.ShippingStatus != ShippingStatus.PrepareShipment && order.ShippingStatus != ShippingStatus.ToBeShipped) || (order.PaymentMethod != PaymentMethods.CashOnDelivery && order.OrderStatus == OrderStatus.Open && order.ShippingStatus != ShippingStatus.WaitingForPayment)).Count() - (groupedOrders.Where(order => order.IsRefunded).Count()),
                               TotalOrderQuantity = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open || (orderStatus.HasValue && order.OrderStatus == orderStatus)).Count(),
                               SalesAmount = groupedOrders.Where(order => order.OrderStatus == OrderStatus.Open || (orderStatus.HasValue && order.OrderStatus == orderStatus)).Distinct().Sum(order => order.TotalAmount),
                               AmountReceived = groupedOrders.Where(x => (x.PaymentMethod == PaymentMethods.CashOnDelivery && x.ShippingStatus != ShippingStatus.PrepareShipment && x.ShippingStatus != ShippingStatus.ToBeShipped) || (x.PaymentMethod != PaymentMethods.CashOnDelivery && x.OrderStatus == OrderStatus.Open && x.ShippingStatus != ShippingStatus.WaitingForPayment)).Distinct().Sum(order => order.TotalAmount),
                               GroupBuy = groupedOrders.First().GroupBuy,
                           }).FirstOrDefaultAsync();

        if (query != null)
        {
            query.SalesAmountExclShipping = query.OrderItems.Sum(order => order.TotalAmount) - query.OrderItems.Sum(x => x.TotalAmount - (x.ItemPrice * x.Quantity));
            query.SalesAmountMinusShipping = query.SalesAmountExclShipping;
            query.AmountReceivedExclShipping = query.OrderItemsPaid.Sum(x => x.TotalAmount);
            var AmountReceivedExclShippingForProfit = query.OrderItemsPaidAndDeliverd.Sum(x => x.TotalAmount);
            var groupbuyProfit = query.AmountReceivedExclShipping * ((query.GroupBuy.ProfitShare) / 100.0M);
            var itemWithShareProfit = query.OrderItemsPaidAndDeliverd.Where(x => x.Item != null && x.Item.ShareProfit > 0).ToList();
            decimal profit = 0;

            foreach (var item in itemWithShareProfit)
            {
                profit += ((decimal)(query.GroupBuy.ProfitShare - item.Item.ShareProfit) / 100.0M) * item.TotalAmount;
            }

            query.BloggersProfit = groupbuyProfit < profit ? -1 * (groupbuyProfit - profit) : (decimal)(groupbuyProfit - profit);
        }
        if (query?.OrderQuantityPaid < 0)
        {
            query.OrderQuantityPaid = 0;
        }

        if (query == null)
        {
            var groupBuy = await dbContext.GroupBuys.FirstOrDefaultAsync(x => x.Id == id);
            query = new GroupBuyReportDetails
            {
                GroupBuyName = groupBuy?.GroupBuyName,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        return query;
    }

    public async Task<GroupBuyReportDetails> GetNewGroupBuyReportDetailsAsync(
        Guid id, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        OrderStatus? orderStatus = null,
        DateTime? completionTimeFrom = null,
        DateTime? completionTimeTo = null,
        ShippingStatus? shippingStatus = null
        )
    {
        var dbContext = await GetDbContextAsync();

        var query = await (from order in dbContext.Orders
                            .Include(x => x.OrderItems).ThenInclude(x => x.Item)
                            .WhereIf(startDate.HasValue, x => x.CreationTime >= startDate)
                            .WhereIf(endDate.HasValue, x => x.CreationTime < endDate)
                            .WhereIf(orderStatus.HasValue && orderStatus != OrderStatus.Open, x => x.OrderStatus == orderStatus) // Apply filter only when status is not Open
                            .WhereIf(completionTimeFrom.HasValue, x => x.CompletionTime.HasValue && x.CompletionTime.Value.Date >= completionTimeFrom!.Value.Date)
                            .WhereIf(completionTimeTo.HasValue, x => x.CompletionTime.HasValue && x.CompletionTime.Value.Date <= completionTimeTo!.Value.Date)
                            .WhereIf(shippingStatus.HasValue, x => x.ShippingStatus == shippingStatus)
                            join groupbuy in dbContext.GroupBuys.Where(g => g.Id == id) on order.GroupBuyId equals groupbuy.Id
                            group order by order.GroupBuyId into groupedOrders
                            select new GroupBuyReportDetails
                            {
                                GroupBuyName = groupedOrders.First().GroupBuy.GroupBuyName,
                                StartDate = startDate ?? groupedOrders.First().GroupBuy.CreationTime,
                                EndDate = endDate ?? DateTime.Now,
                                TotalOrderQuantity = groupedOrders.Count(),
                                OrderQuantityPaid = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Count(),
                                SalesAmountExclShipping = groupedOrders.Sum(order => order.TotalAmount),
                                SalesAmount = groupedOrders.Sum(order => order.TotalAmount),
                                AmountReceivedExclShipping = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Sum(order => order.TotalAmount),
                                AmountReceived = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Sum(order => order.TotalAmount),
                                ProfitShare = groupedOrders.First().GroupBuy.ProfitShare,
                                SalesAmountDiscount = groupedOrders.Sum(order => order.DeliveryCost),
                                AmountReceivedDiscount = groupedOrders.Where(order => OrderConsts.CompletedShippingStatus.Contains(order.ShippingStatus)).Sum(order => order.DeliveryCost)
                            }).FirstOrDefaultAsync();

        if (query != null)
        {
            query.SalesAmountExclShipping -= (query.SalesAmountDiscount ?? 0);
            query.AmountReceivedExclShipping -= (query.AmountReceivedDiscount ?? 0);
        }

        return query;
    }
}
