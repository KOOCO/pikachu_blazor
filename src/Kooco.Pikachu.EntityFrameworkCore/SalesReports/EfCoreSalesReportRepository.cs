using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.SalesReports;

public class EfCoreSalesReportRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : ISalesReportRepository
{
    public bool? IsChangeTrackingEnabled { get; set; } = false;

    protected virtual Task<PikachuDbContext> GetDbContextAsync()
    {
        return dbContextProvider.GetDbContextAsync();
    }

    public async Task<List<SalesReportModel>> GetSalesReportAsync(DateTime startDate, DateTime endDate, Guid? groupBuyId, ReportCalculationUnits unit)
    {
        var dbContext = await GetDbContextAsync();

        var timePeriodOrders = await dbContext.Orders
            .Where(x => x.CreationTime.Date >= startDate.Date)
            .Where(x => x.CreationTime.Date <= endDate.Date)
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
             .OrderByDescending(x => x.CreationTime)
            .Include(o => o.OrderItems)
           
            .GroupBy(x => x.CreationTime)
            .ToListAsync();

        var allProductIds = timePeriodOrders.SelectMany(g => g.SelectMany(o => o.OrderItems)).Select(oi => new { oi.ItemId, oi.Spec, oi.SKU, oi.SetItemId });
        var itemIds = allProductIds.Where(p => p.ItemId.HasValue).Select(p => new { p.ItemId, p.Spec, p.SKU }).Distinct().ToList();
        var setItemIds = allProductIds.Where(p => p.SetItemId.HasValue).Select(p => new { p.SetItemId, p.Spec, p.SKU }).Distinct().ToList();

        var items = await dbContext.Items
            .Where(i => itemIds.Select(i => i.ItemId).Contains(i.Id))
            .Include(i => i.ItemDetails)
            .SelectMany(i => i.ItemDetails)
            .Where(id => itemIds.Select(i => i.Spec).Contains(id.SKU))
            .Select(x => new DataModel
            {
                ItemId = x.ItemId,
                Cost = x.Cost,
                SKU = x.SKU,
            }).ToListAsync();

        var setItems = await dbContext.SetItems
            .Where(si => setItemIds.Select(sid => sid.SetItemId).Contains(si.Id))
            .Include(si => si.SetItemDetails)
            .ThenInclude(sid => sid.SetItem)
            .SelectMany(si => si.SetItemDetails)
            .Select(x => new DataModel
            {
                SetItemId = x.SetItemId,
                ItemId = x.ItemId,
                Details = dbContext.ItemDetails
                    .Where(i => x.ItemId == i.ItemId && x.Attribute1Value == i.Attribute1Value && x.Attribute2Value == i.Attribute2Value && x.Attribute3Value == i.Attribute3Value)
                    .Select(i => new DetailsModel
                    {
                        Cost = i.Cost,
                        ItemId = i.ItemId
                    }).ToList()
            }).ToListAsync();

        List<SalesReportModel> results = [];

        if (unit == ReportCalculationUnits.Daily)
        {
            var totalDays = (endDate - startDate).TotalDays;

            for (int i = 0; i <= totalDays; i++)
            {
                var date = startDate.AddDays(i);

                var orderGroup = timePeriodOrders
                    .Where(x => x.Key.Date == date.Date)
                    .SelectMany(x => x)
                    .ToList();

                var orderItems = orderGroup?.SelectMany(og => og.OrderItems).ToList() ?? [];
                var cost = Cost(items, setItems, orderItems);

                results.Add(CreateSalesReport(date, date, orderGroup, ReportCalculationUnits.Daily, cost));
            }
        }

        if (unit == ReportCalculationUnits.Weekly)
        {
            var currentStart = startDate.Date;

            while (currentStart <= endDate)
            {
                // Calculate the end of the week (Sunday)
                var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)currentStart.DayOfWeek + 7) % 7;
                var currentEnd = currentStart.AddDays(daysUntilSunday);
                if (currentEnd > endDate) currentEnd = endDate;

                var weeklyOrders = timePeriodOrders
                    .Where(x => x.Key.Date >= currentStart && x.Key.Date <= currentEnd)
                    .SelectMany(x => x)
                    .ToList();

                var orderItems = weeklyOrders?.SelectMany(w => w.OrderItems).ToList();
                var cost = Cost(items, setItems, orderItems);

                results.Add(CreateSalesReport(currentStart, currentEnd, weeklyOrders, ReportCalculationUnits.Weekly, cost));

                // Move to the start of the next week (Monday)
                currentStart = currentEnd.AddDays(1);
            }
        }

        if (unit == ReportCalculationUnits.Monthly)
        {
            var currentStart = startDate.Date;

            while (currentStart <= endDate)
            {
                DateTime currentEnd;

                if (currentStart.Month == endDate.Month && currentStart.Year == endDate.Year)
                {
                    // For the last month, end at the specified endDate
                    currentEnd = endDate;
                }
                else if (currentStart == startDate)
                {
                    // For the first month, start from the given startDate and end at the month's end
                    currentEnd = new DateTime(currentStart.Year, currentStart.Month, DateTime.DaysInMonth(currentStart.Year, currentStart.Month));
                }
                else
                {
                    // For intermediate months, calculate the entire month range
                    currentEnd = new DateTime(currentStart.Year, currentStart.Month, DateTime.DaysInMonth(currentStart.Year, currentStart.Month));
                }

                if (currentEnd > endDate)
                {
                    currentEnd = endDate;
                }

                var monthlyOrders = timePeriodOrders
                    .Where(x => x.Key.Date >= currentStart && x.Key.Date <= currentEnd)
                    .SelectMany(x => x);

                var orderItems = monthlyOrders?.SelectMany(mo => mo.OrderItems).ToList();
                var cost = Cost(items, setItems, orderItems);
                
                results.Add(CreateSalesReport(currentStart, currentEnd, monthlyOrders, ReportCalculationUnits.Monthly, cost));

                // Move to the next month's 1st day
                currentStart = new DateTime(currentStart.Year, currentStart.Month, 1).AddMonths(1);
            }
        }

        return results.OrderByDescending(x=>x.Date).ToList();
    }

    private static SalesReportModel CreateSalesReport(
        DateTime startDate,
        DateTime endDate,
        IEnumerable<Order> orders,
        ReportCalculationUnits calculationUnit,
        decimal costOfGoodsSold
        )
    {
        orders ??= [];

        var grossSales = orders.Sum(y => y.TotalAmount);
        var discountAmount = orders.Sum(o => o.DiscountAmount) ?? 0;
        var netSales = grossSales - orders.Sum(o => o.OrderStatus == OrderStatus.Returned ? o.TotalAmount : (o.DiscountAmount ?? 0));
        var grossProfit = netSales - costOfGoodsSold;
        var grossProfitMargin = grossSales > 0 ? (grossProfit / netSales) * 100 : 0; //Multiply by 100 to caclulate in percentage.

        return new SalesReportModel
        {
            StartDate = startDate,
            EndDate = endDate,
            GrossSales = grossSales,
            NetSales = netSales,
            Discount = discountAmount,
            NumberOfOrders = orders.Count(),
            CostOfGoodsSold = costOfGoodsSold,
            ShippingCost = orders.Sum(y =>
                (y.DeliveryCost ?? 0) +
                (y.DeliveryCostForNormal ?? 0) +
                (y.DeliveryCostForFreeze ?? 0) +
                (y.DeliveryCostForFrozen ?? 0)),
            GrossProfit = grossProfit,
            GrossProfitMargin = grossProfitMargin,
            ReportCalculationUnit = calculationUnit
        };
    }

    private static decimal Cost(List<DataModel> items, List<DataModel> setItems, List<OrderItem> orderItems)
    {
        var itemsCost = items
                    .Where(x => orderItems
                           .Where(oi => oi.ItemId.HasValue)
                           .Select(oi => new { ItemId = oi.ItemId!.Value, P = oi.Spec }).ToList()
                           .Contains(new { x.ItemId, P = (string?)x.SKU }))
                    .Sum(i => i.Cost);

        var setItemsCost = setItems
            .Where(si => orderItems
                    .Where(oi => oi.SetItemId.HasValue)
                    .Select(oi => oi.SetItemId)
                    .Contains(si.SetItemId))
            .SelectMany(si => si.Details)
            .Sum(id => id.Cost);

        var cost = itemsCost + setItemsCost;
        return Convert.ToDecimal(cost);
    }

    public async Task<List<SalesReportModel>> GetGroupBuySalesReportAsync(DateTime startDate, DateTime endDate, Guid? groupBuyId)
    {
        var dbContext = await GetDbContextAsync();

        var timePeriodOrders = await dbContext.Orders
            .Where(x => x.CreationTime.Date >= startDate.Date && x.CreationTime.Date <= endDate.Date)
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .Include(x => x.OrderItems)
            .OrderByDescending(x => x.CreationTime)
            .GroupBy(x => x.GroupBuyId)
            .ToListAsync();

        var allProductIds = timePeriodOrders.SelectMany(g => g.SelectMany(o => o.OrderItems)).Select(oi => new { oi.ItemId, oi.Spec, oi.SKU, oi.SetItemId });
        var itemIds = allProductIds.Where(p => p.ItemId.HasValue).Select(p => new { p.ItemId, p.Spec, p.SKU }).Distinct().ToList();
        var setItemIds = allProductIds.Where(p => p.SetItemId.HasValue).Select(p => new { p.SetItemId, p.Spec, p.SKU }).Distinct().ToList();

        var items = await dbContext.Items
            .Where(i => itemIds.Select(i => i.ItemId).Contains(i.Id))
            .Include(i => i.ItemDetails)
            .SelectMany(i => i.ItemDetails)
            .Where(id => itemIds.Select(i => i.Spec).Contains(id.SKU))
            .Select(x => new DataModel
            {
                ItemId = x.ItemId,
                Cost = x.Cost,
                SKU = x.SKU,
            }).ToListAsync();

        var setItems = await dbContext.SetItems
            .Where(si => setItemIds.Select(sid => sid.SetItemId).Contains(si.Id))
            .Include(si => si.SetItemDetails)
            .ThenInclude(sid => sid.SetItem)
            .SelectMany(si => si.SetItemDetails)
            .Select(x => new DataModel
            {
                SetItemId = x.SetItemId,
                ItemId = x.ItemId,
                Details = dbContext.ItemDetails
                    .Where(i => x.ItemId == i.ItemId && x.Attribute1Value == i.Attribute1Value && x.Attribute2Value == i.Attribute2Value && x.Attribute3Value == i.Attribute3Value)
                    .Select(i => new DetailsModel
                    {
                        Cost = i.Cost,
                        ItemId = i.ItemId
                    }).ToList()
            }).ToListAsync();

        var groupBuyIds = timePeriodOrders.Select(g => g.Key).ToList();

        var groupBuys = dbContext.GroupBuys
            .Where(g => groupBuyIds.Contains(g.Id))
            .Select(g => new { g.Id, g.GroupBuyName })
            .ToList();

        List<SalesReportModel> results = [];

        foreach (var orderGroup in timePeriodOrders)
        {
            var orders = orderGroup.Select(x => x);
            
            var orderItems = orderGroup.SelectMany(og => og.OrderItems).ToList() ?? [];
            
            var cost = Cost(items, setItems, orderItems);
            
            var salesReport = CreateSalesReport(startDate, endDate, orders, ReportCalculationUnits.Daily, cost);
            salesReport.GroupBuyName = groupBuys.Where(g => g.Id == orderGroup.Key).FirstOrDefault()?.GroupBuyName;

            results.Add(salesReport);
        }

        return results.Where(x => x.GrossSales > 0).ToList();
    }

    private class DataModel
    {
        public Guid SetItemId { get; set; }
        public Guid ItemId { get; set; }
        public float Cost { get; set; }
        public string SKU { get; set; }
        public List<DetailsModel> Details { get; set; }
    }

    private class DetailsModel
    {
        public Guid ItemId { get; set; }
        public float Cost { get; set; }
    }
}
