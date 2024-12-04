using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Reports;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.SalesReports;

[Authorize]
public class SalesReportAppService(IOrderRepository orderRepository, IGroupBuyRepository groupBuyRepository) : PikachuAppService, ISalesReportAppService
{
    public async Task<List<SalesReportDto>> GetSalesReportAsync(GetSalesReportDto input)
    {
        var startDate = input.StartDate ?? DateTime.Today;
        var endDate = input.EndDate ?? startDate.AddDays(1).AddMicroseconds(-1);

        var queryable = await orderRepository.GetQueryableAsync();

        var timePeriodOrders = queryable
            .Where(x => x.CreationTime.Date >= startDate.Date)
            .Where(x => x.CreationTime.Date <= endDate.Date)
            .WhereIf(input.GroupBuyId.HasValue, x => x.GroupBuyId == input.GroupBuyId)
            .OrderByDescending(x => x.CreationTime)
            .ToList();

        var groupedOrders = timePeriodOrders.GroupBy(x => x.CreationTime.Date).ToList();

        var results = input.ReportCalculationUnit switch
        {
            ReportCalculationUnits.Daily => CalculateDailySales(startDate, endDate, groupedOrders),
            ReportCalculationUnits.Weekly => CalculateWeeklySales(startDate, endDate, groupedOrders),
            ReportCalculationUnits.Monthly => CalculateMonthlySales(startDate, endDate, groupedOrders),
            _ => []
        };

        return results;
    }

    public async Task<List<SalesReportDto>> GetGroupBuySalesReportAsync(DateTime startDate, DateTime endDate, Guid? groupBuyId)
    {
        var queryable = await orderRepository.GetQueryableAsync();

        var timePeriodOrders = queryable
            .Where(x => x.CreationTime.Date >= startDate.Date && x.CreationTime.Date <= endDate.Date)
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .OrderByDescending(x => x.CreationTime)
            .ToList();

        var groupedOrders = timePeriodOrders.GroupBy(x => x.GroupBuyId).ToList();

        var groupBuyIds = groupedOrders.Select(g => g.Key).ToList();

        var groupBuys = (await groupBuyRepository.GetQueryableAsync())
            .Where(g => groupBuyIds.Contains(g.Id))
            .Select(g => new { g.Id, g.GroupBuyName })
            .ToList();

        List<SalesReportDto> results = [];

        foreach (var orderGroup in groupedOrders)
        {
            results.Add(new SalesReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                GroupBuyName = groupBuys.Where(g => g.Id == orderGroup.Key).FirstOrDefault()?.GroupBuyName,
                GrossSales = orderGroup?.Sum(y => y.TotalAmount) ?? 0,
                NetSales = 0,
                Discount = 0,
                NumberOfOrders = orderGroup?.Count() ?? 0,
                CostOfGoodsSold = 0,
                ShippingCost = orderGroup?.Sum(y => (y.DeliveryCost ?? 0) + (y.DeliveryCostForNormal ?? 0) + (y.DeliveryCostForFreeze ?? 0) + (y.DeliveryCostForFrozen ?? 0)) ?? 0,
                GrossProfit = 0,
                GrossProfitMargin = 0
            });
        }

        return results.Where(x => x.GrossSales > 0).ToList();
    }

    private static List<SalesReportDto> CalculateDailySales(DateTime startDate, DateTime endDate, List<IGrouping<DateTime, Order>> groupedOrders)
    {
        var totalDays = (endDate - startDate).TotalDays;

        List<SalesReportDto> results = [];

        for (int i = 0; i <= totalDays; i++)
        {
            var date = startDate.AddDays(i);

            var orderGroup = groupedOrders
                .Where(x => x.Key.Date == date.Date)
                .FirstOrDefault();

            results.Add(new SalesReportDto
            {
                StartDate = date,
                EndDate = date,
                GrossSales = orderGroup?.Sum(y => y.TotalAmount) ?? 0,
                NetSales = 0,
                Discount = 0,
                NumberOfOrders = orderGroup?.Count() ?? 0,
                CostOfGoodsSold = 0,
                ShippingCost = orderGroup?.Sum(y => (y.DeliveryCost ?? 0) + (y.DeliveryCostForNormal ?? 0) + (y.DeliveryCostForFreeze ?? 0) + (y.DeliveryCostForFrozen ?? 0)) ?? 0,
                GrossProfit = 0,
                GrossProfitMargin = 0,
                ReportCalculationUnit = ReportCalculationUnits.Daily
            });
        }

        return results;
    }

    private static List<SalesReportDto> CalculateWeeklySales(DateTime startDate, DateTime endDate, List<IGrouping<DateTime, Order>> groupedOrders)
    {
        var results = new List<SalesReportDto>();
        var currentStart = startDate.Date;

        while (currentStart <= endDate)
        {
            // Calculate the end of the week (Sunday)
            var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)currentStart.DayOfWeek + 7) % 7;
            var currentEnd = currentStart.AddDays(daysUntilSunday);
            if (currentEnd > endDate) currentEnd = endDate;

            var weeklyOrders = groupedOrders
                .Where(x => x.Key.Date >= currentStart && x.Key.Date <= currentEnd)
                .SelectMany(x => x);

            results.Add(new SalesReportDto
            {
                StartDate = currentStart,
                EndDate = currentEnd,
                GrossSales = weeklyOrders.Sum(y => y.TotalAmount),
                NetSales = 0,
                Discount = 0,
                NumberOfOrders = weeklyOrders.Count(),
                CostOfGoodsSold = 0,
                ShippingCost = weeklyOrders.Sum(y => (y.DeliveryCost ?? 0) + (y.DeliveryCostForNormal ?? 0) + (y.DeliveryCostForFreeze ?? 0) + (y.DeliveryCostForFrozen ?? 0)),
                GrossProfit = 0,
                GrossProfitMargin = 0,
                ReportCalculationUnit = ReportCalculationUnits.Weekly
            });

            // Move to the start of the next week (Monday)
            currentStart = currentEnd.AddDays(1);
        }

        return results;
    }

    private static List<SalesReportDto> CalculateMonthlySales(DateTime startDate, DateTime endDate, List<IGrouping<DateTime, Order>> groupedOrders)
    {
        var results = new List<SalesReportDto>();

        // Start with the given startDate
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

            var monthlyOrders = groupedOrders
                .Where(x => x.Key.Date >= currentStart && x.Key.Date <= currentEnd)
                .SelectMany(x => x);

            results.Add(new SalesReportDto
            {
                StartDate = currentStart,
                EndDate = currentEnd,
                GrossSales = monthlyOrders.Sum(y => y.TotalAmount),
                NetSales = 0,
                Discount = 0,
                NumberOfOrders = monthlyOrders.Count(),
                CostOfGoodsSold = 0,
                ShippingCost = monthlyOrders.Sum(y => (y.DeliveryCost ?? 0) + (y.DeliveryCostForNormal ?? 0) + (y.DeliveryCostForFreeze ?? 0) + (y.DeliveryCostForFrozen ?? 0)),
                GrossProfit = 0,
                GrossProfitMargin = 0,
                ReportCalculationUnit = ReportCalculationUnits.Monthly
            });

            // Move to the next month's 1st day
            currentStart = new DateTime(currentStart.Year, currentStart.Month, 1).AddMonths(1);
        }

        return results;
    }

}
