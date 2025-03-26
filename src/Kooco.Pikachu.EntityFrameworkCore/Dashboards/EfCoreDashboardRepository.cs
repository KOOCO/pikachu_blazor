using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Dashboards;

public class EfCoreDashboardRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : IDashboardRepository
{
    public bool? IsChangeTrackingEnabled { get; set; } = false;

    private async Task<PikachuDbContext> GetDbContextAsync()
    {
        return await dbContextProvider.GetDbContextAsync();
    }

    public async Task<DashboardStatsModel> GetDashboardStatsAsync(IEnumerable<Guid> selectedGroupBuyIds, DateTime? startDate, DateTime? endDate, DateTime? previousDate)
    {
        var dbContext = await GetDbContextAsync();

        var query = await dbContext.Orders
            .Where(o => o.ShippingStatus == ShippingStatus.Completed)
            .Where(o => startDate.HasValue && endDate.HasValue && previousDate.HasValue && o.CompletionTime.HasValue)
            .Where(o => o.CompletionTime.Value.Date >= previousDate && o.CompletionTime.Value.Date <= endDate)
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Select(o => new
            {
                CompletionTime = o.CompletionTime.Value.Date,
                o.TotalAmount,
                o.TotalQuantity
            }).ToListAsync();

        var newMembers = await dbContext.Users
            .Where(u => u.CreationTime >= previousDate && u.CreationTime <= endDate)
            .GroupJoin(dbContext.Orders,
            user => user.Id,
            orders => orders.UserId,
            (user, orders) => new { user, orders })
            .Where(x => !x.orders.Any())
            .Select(x => x.user.CreationTime)
            .ToListAsync();

        var timePeriodOrders = query.Where(o => o.CompletionTime >= startDate).ToList();
        var previousOrders = query.Where(o => o.CompletionTime < startDate).ToList();

        var model = new DashboardStatsModel
        {
            TotalOrders = timePeriodOrders.Count,
            TotalPreviousOrders = previousOrders.Count,

            TotalSales = query.Sum(o => o.TotalAmount),
            TotalPreviousSales = previousOrders.Sum(o => o.TotalAmount),

            TotalQuantity = query.Sum(o => o.TotalQuantity),
            TotalPreviousQuantity = previousOrders.Sum(o => o.TotalQuantity),

            NewMembers = newMembers.Where(o => o >= startDate).Count(),
            PreviousNewMembers = newMembers.Where(o => o < startDate).Count()
        };

        return model;
    }

    public async Task<DashboardChartsModel> GetDashboardChartsAsync(ReportCalculationUnits? periodOption, IEnumerable<Guid> selectedGroupBuyIds, DateTime? startDate, DateTime? endDate)
    {
        startDate ??= DateTime.Today;
        endDate ??= DateTime.Today.AddDays(1).AddMicroseconds(-1);
        var dbContext = await GetDbContextAsync();

        var orders = await dbContext.Orders
            .Where(o => o.ShippingStatus == ShippingStatus.Completed)
            .Where(o => startDate.HasValue && endDate.HasValue && o.CompletionTime.HasValue)
            .Where(o => o.CompletionTime.Value.Date >= startDate && o.CompletionTime.Value.Date <= endDate)
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Select(o => new
            {
                CompletionTime = o.CompletionTime.Value.Date,
                o.TotalAmount,
                o.TotalQuantity,
                o.GroupBuyId
            }).ToListAsync();

        var groupBuyNames = await dbContext.GroupBuys
            .Where(gb => orders.Select(o => o.GroupBuyId).Contains(gb.Id))
            .Select(gb => new { gb.GroupBuyName, gb.Id })
            .ToListAsync();

        var donutData = orders
            .GroupBy(o => o.GroupBuyId)
            .OrderByDescending(g => g.Sum(o => o.TotalAmount))
            .Take(10)
            .Select(g => new
            {
                TotalAmount = g.Sum(o => (int)o.TotalAmount),
                Label = groupBuyNames.Where(gbn => gbn.Id == g.Key).FirstOrDefault()?.GroupBuyName
            });

        var donut = new DashboardDonutChartModel
        {
            Labels = [.. donutData.Select(d => d.Label)],
            Series = [.. donutData.Select(d => d.TotalAmount)]
        };

        var barChart = new DashboardBarChartModel
        {
            Categories = new List<string>(),
            TotalSales = new List<int>(),
            TotalOrders = new List<int>()
        };

        if (periodOption == ReportCalculationUnits.Daily)
        {
            double totalDays = (endDate - startDate)?.TotalDays ?? 0;

            for (int i = 0; i <= totalDays; i++)
            {
                var currentDate = startDate.Value.AddDays(i).Date;
                barChart.Categories.Add(currentDate.ToString("MMM dd, yyyy"));
                barChart.TotalSales.Add((int)(orders
                    .Where(o => o.CompletionTime.Date == currentDate)
                    .Sum(o => o.TotalAmount)));

                barChart.TotalOrders.Add(orders
                    .Count(o => o.CompletionTime.Date == currentDate));
            }
        }
        else if (periodOption == ReportCalculationUnits.Weekly)
        {
            double totalDays = (endDate - startDate)?.TotalDays ?? 0;
            int totalWeeks = (int)Math.Ceiling(totalDays / 7);

            for (int i = 0; i < totalWeeks; i++)
            {
                var weekStart = startDate.Value.AddDays(i * 7).Date;
                var weekEnd = weekStart.AddDays(6).Date;
                barChart.Categories.Add($"{weekStart:MMM dd} - {weekEnd:MMM dd}");

                barChart.TotalSales.Add((int)(orders
                    .Where(o => o.CompletionTime.Date >= weekStart && o.CompletionTime.Date <= weekEnd)
                    .Sum(o => o.TotalAmount)));

                barChart.TotalOrders.Add(orders
                    .Count(o => o.CompletionTime.Date >= weekStart && o.CompletionTime.Date <= weekEnd));
            }
        }
        else if (periodOption == ReportCalculationUnits.Monthly)
        {
            int totalMonths = ((endDate.Value.Year - startDate.Value.Year) * 12) + (endDate.Value.Month - startDate.Value.Month) + 1;

            for (int i = 0; i < totalMonths; i++)
            {
                var monthStart = new DateTime(startDate.Value.Year, startDate.Value.Month, 1).AddMonths(i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                barChart.Categories.Add(monthStart.ToString("MMM yyyy"));

                barChart.TotalSales.Add((int)(orders
                    .Where(o => o.CompletionTime.Date >= monthStart && o.CompletionTime.Date <= monthEnd)
                    .Sum(o => o.TotalAmount)));

                barChart.TotalOrders.Add(orders
                    .Count(o => o.CompletionTime.Date >= monthStart && o.CompletionTime.Date <= monthEnd));
            }
        }


        var model = new DashboardChartsModel
        {
            Donut = donut,
            Bar = barChart
        };

        return model;
    }

    public async Task<DashboardOrdersWithCountModel> GetRecentOrdersAsync(int skipCount, int maxResultCount, IEnumerable<Guid> selectedGroupBuyIds)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Orders
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreationTime)
            .Select(o => new
            {
                o.Id,
                o.OrderNo,
                o.CustomerName,
                o.TotalAmount,
                o.ShippingStatus,
                OrderItem = o.OrderItems
                    .Where(oi => oi.ItemId.HasValue || oi.SetItemId.HasValue)
                    .FirstOrDefault()
            });

        var totalCount = await query.LongCountAsync();
        var orders = await query.PageBy(skipCount, maxResultCount).ToListAsync();

        var itemIds = orders.Where(o => o.OrderItem?.ItemId != null).Select(o => o.OrderItem!.ItemId!.Value).ToList();
        var setItemIds = orders.Where(o => o.OrderItem?.SetItemId != null).Select(o => o.OrderItem!.SetItemId!.Value).ToList();

        var items = await dbContext.Items.Where(i => itemIds.Contains(i.Id)).Select(i => new { i.Id, i.ItemName }).ToListAsync();
        var setItems = await dbContext.SetItems.Where(s => setItemIds.Contains(s.Id)).Select(s => new { s.Id, s.SetItemName }).ToListAsync();

        var model = orders.Select(o => new DashboardOrdersModel
        {
            Id = o.Id,
            OrderNo = o.OrderNo,
            CustomerName = o.CustomerName,
            TotalAmount = o.TotalAmount,
            ShippingStatus = o.ShippingStatus,
            ItemName = o.OrderItem?.ItemType == ItemType.Item
                ? items.Where(i => i.Id == o.OrderItem.ItemId).FirstOrDefault()?.ItemName
                : setItems.Where(s => o.OrderItem != null && s.Id == o.OrderItem.SetItemId).FirstOrDefault()?.SetItemName
        }).ToList();

        return new DashboardOrdersWithCountModel
        {
            TotalCount = totalCount,
            Items = model
        };
    }

    public async Task<List<DashboardBestSellerModel>> GetBestSellerItemsAsync(IEnumerable<Guid> selectedGroupBuyIds)
    {
        var dbContext = await GetDbContextAsync();

        var orderItems = await dbContext.Orders
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Include(o => o.OrderItems)
            .SelectMany(o => o.OrderItems)
            .Select(oi => new
            {
                oi.ItemType,
                oi.ItemId,
                oi.Quantity,
                oi.TotalAmount
            })
            .Where(oi => oi.ItemId.HasValue)
            .GroupBy(oi => oi.ItemId!.Value)
            .Select(g => new
            {
                ItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity),
                Amount = g.Sum(x => x.TotalAmount),
            })
            .ToListAsync();

        var itemIds = orderItems.Select(oi => oi.ItemId).ToList();
        var items = await dbContext.Items
            .Where(i => itemIds.Contains(i.Id))
            .Include(i => i.Images)
            .Select(i => new { i.Id, i.ItemName, Image = i.Images.Where(image => image.ImageUrl != null).FirstOrDefault() })
            .ToListAsync();

        var totalAmount = orderItems.Sum(oi => oi.Amount);

        var model = orderItems.Select(oi => new DashboardBestSellerModel
        {
            ItemId = oi.ItemId,
            ItemName = items.Where(i => i.Id == oi.ItemId).FirstOrDefault()?.ItemName,
            ImageUrl = items.Where(i => i.Id == oi.ItemId).FirstOrDefault()?.Image?.ImageUrl,
            Quantity = oi.Quantity,
            Amount = oi.Amount
        })
            .Where(oi => !string.IsNullOrEmpty(oi.ItemName))
            .OrderByDescending(oi => oi.Amount)
            .Take(3)
            .ToList();

        model.ForEach(m => m.Percentage = (int)Math.Round(totalAmount > 0 ? (m.Amount / totalAmount) * 100 : 0));

        return model;
    }
}
