using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders;
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
        var startTime = input.StartDate ?? DateTime.Today;
        var endTime = input.EndDate ?? startTime.AddDays(1).AddMicroseconds(-1);

        var queryable = await orderRepository.GetQueryableAsync();

        var timePeriodOrders = queryable
            .Where(x => x.CreationTime.Date >= startTime.Date)
            .Where(x => x.CreationTime.Date <= endTime.Date)
            .WhereIf(input.GroupBuyId.HasValue, x => x.GroupBuyId == input.GroupBuyId)
            .OrderByDescending(x => x.CreationTime)
            .ToList();

        var groupedOrders = timePeriodOrders.GroupBy(x => x.CreationTime.Date).ToList();

        var totalDays = (endTime - startTime).TotalDays;

        List<SalesReportDto> results = [];

        for (int i = 0; i <= totalDays; i++)
        {
            var date = startTime.AddDays(i);

            var orders = groupedOrders
                .Where(x => x.Key.Date == date.Date)
                .FirstOrDefault();

            results.Add(new SalesReportDto
            {
                Date = date,
                GrossSales = orders?.Sum(y => y.TotalAmount) ?? 0,
                NetSales = 0,
                Discount = 0,
                NumberOfOrders = orders?.Count() ?? 0,
                CostOfGoodsSold = 0,
                ShippingCost = orders?.Sum(y => (y.DeliveryCost ?? 0) + (y.DeliveryCostForNormal ?? 0) + (y.DeliveryCostForFreeze ?? 0) + (y.DeliveryCostForFrozen ?? 0)) ?? 0,
                GrossProfit = 0,
                GrossProfitMargin = 0
            });
        }

        return results;
    }

    public async Task<List<SalesReportDto>> GetGroupBuySalesReportAsync(DateTime date)
    {
        var queryable = await orderRepository.GetQueryableAsync();

        var timePeriodOrders = queryable
            .Where(x => x.CreationTime.Date == date.Date)
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
                Date = date,
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
}
