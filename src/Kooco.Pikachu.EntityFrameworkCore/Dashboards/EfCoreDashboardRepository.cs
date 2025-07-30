using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
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
        var today = DateTime.Today;
        startDate = startDate?.Date ?? today;
        endDate = (endDate?.Date ?? today).AddDays(1).AddTicks(-1);
        previousDate = previousDate?.Date ?? startDate.Value.AddDays(-1);

        var dbContext = await GetDbContextAsync();

        var query = await dbContext.Orders
            .Where(o => o.ShippingStatus == ShippingStatus.Completed)
            .Where(o => o.CreationTime.Date >= previousDate && o.CreationTime.Date <= endDate)
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Select(o => new
            {
                o.CreationTime,
                o.TotalAmount,
                o.TotalQuantity
            }).ToListAsync();

        var newMembers = await dbContext.Users
            .Where(u => u.CreationTime.Date >= previousDate && u.CreationTime.Date <= endDate)
            .GroupJoin(dbContext.Orders,
                user => user.Id,
                orders => orders.UserId,
                (user, orders) => new { user.CreationTime, HasOrders = orders.Any() })
            .Where(x => !x.HasOrders)
            .Select(x => x.CreationTime)
            .ToListAsync();

        var timePeriodOrders = query.Where(o => o.CreationTime.Date >= startDate).ToList();
        var previousOrders = query.Where(o => o.CreationTime.Date < startDate).ToList();

        var model = new DashboardStatsModel
        {
            TotalOrders = timePeriodOrders.Count,
            TotalPreviousOrders = previousOrders.Count,

            TotalSales = timePeriodOrders.Sum(o => o.TotalAmount),
            TotalPreviousSales = previousOrders.Sum(o => o.TotalAmount),

            TotalQuantity = timePeriodOrders.Sum(o => o.TotalQuantity),
            TotalPreviousQuantity = previousOrders.Sum(o => o.TotalQuantity),

            NewMembers = newMembers.Count(o => o >= startDate),
            PreviousNewMembers = newMembers.Count(o => o < startDate)
        };

        return model;
    }

    public async Task<DashboardChartsModel> GetDashboardChartsAsync(ReportCalculationUnits? periodOption, IEnumerable<Guid> selectedGroupBuyIds, DateTime? startDate, DateTime? endDate)
    {
        var today = DateTime.Today;
        startDate = startDate?.Date ?? today;
        endDate = (endDate?.Date ?? today).AddDays(1).AddTicks(-1);

        var dbContext = await GetDbContextAsync();

        var orders = await dbContext.Orders
            .Where(o => o.ShippingStatus == ShippingStatus.Completed)
            .Where(o => o.CreationTime.Date >= startDate && o.CreationTime.Date <= endDate)
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Select(o => new
            {
                CreationTime = o.CreationTime.Date,
                o.TotalAmount,
                o.TotalQuantity,
                o.GroupBuyId
            }).ToListAsync();

        var groupBuyNames = await dbContext.GroupBuys
            .Where(gb => orders.Select(o => o.GroupBuyId).Contains(gb.Id))
            .Select(gb => new { gb.Id, gb.GroupBuyName })
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
            Categories = [],
            TotalSales = [],
            TotalOrders = []
        };

        if (periodOption == ReportCalculationUnits.Daily)
        {
            double totalDays = (endDate - startDate)?.TotalDays ?? 0;

            for (int i = 0; i <= totalDays; i++)
            {
                var currentDate = startDate.Value.AddDays(i).Date;
                barChart.Categories.Add(currentDate.ToString("MMM dd, yyyy"));
                barChart.TotalSales.Add((int)(orders
                    .Where(o => o.CreationTime.Date == currentDate)
                    .Sum(o => o.TotalAmount)));

                barChart.TotalOrders.Add(orders
                    .Count(o => o.CreationTime.Date == currentDate));
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
                    .Where(o => o.CreationTime.Date >= weekStart && o.CreationTime.Date <= weekEnd)
                    .Sum(o => o.TotalAmount)));

                barChart.TotalOrders.Add(orders
                    .Count(o => o.CreationTime.Date >= weekStart && o.CreationTime.Date <= weekEnd));
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
                    .Where(o => o.CreationTime.Date >= monthStart && o.CreationTime.Date <= monthEnd)
                    .Sum(o => o.TotalAmount)));

                barChart.TotalOrders.Add(orders
                    .Count(o => o.CreationTime.Date >= monthStart && o.CreationTime.Date <= monthEnd));
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
            .OrderByDescending(o => o.CreationTime)
            .Select(o => new
            {
                o.Id,
                o.OrderNo,
                o.CustomerName,
                o.TotalAmount,
                o.ShippingStatus,
                o.GroupBuyId
            });

        var totalCount = await query.LongCountAsync();
        var orders = await query.PageBy(skipCount, maxResultCount).ToListAsync();

        var groupBuyNames = await dbContext.GroupBuys
            .Where(g => orders.Select(o => o.GroupBuyId).Contains(g.Id))
            .Select(g => new { g.Id, g.GroupBuyName })
            .ToListAsync();

        var model = orders.Select(o => new DashboardOrdersModel
        {
            Id = o.Id,
            OrderNo = o.OrderNo,
            CustomerName = o.CustomerName,
            TotalAmount = o.TotalAmount,
            ShippingStatus = o.ShippingStatus,
            GroupBuyName = groupBuyNames.Where(g => g.Id == o.GroupBuyId).FirstOrDefault()?.GroupBuyName
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

        var orderItems = dbContext.Orders
            .AsNoTracking()
            .Where(o => o.ShippingStatus == ShippingStatus.Completed)
            .WhereIf(selectedGroupBuyIds.Any(), o => selectedGroupBuyIds.Contains(o.GroupBuyId))
            .Include(o => o.OrderItems)
            .SelectMany(o => o.OrderItems)
            .Select(oi => new
            {
                oi.ItemType,
                oi.ItemId,
                oi.Quantity,
                oi.TotalAmount,
                oi.SetItemId
            });

        var itemGroup = await orderItems
            .Where(oi => oi.ItemId.HasValue)
            .GroupBy(oi => oi.ItemId!.Value)
            .Select(g => new
            {
                Id = g.Key,
                Quantity = g.Sum(x => x.Quantity),
                Amount = g.Sum(x => x.TotalAmount),
                Type = ItemType.Item
            })
            .ToListAsync();

        var setItemGroup = await orderItems
            .Where(oi => oi.SetItemId.HasValue)
            .GroupBy(oi => oi.SetItemId!.Value)
            .Select(g => new
            {
                Id = g.Key,
                Quantity = g.Sum(x => x.Quantity),
                Amount = g.Sum(x => x.TotalAmount),
                Type = ItemType.SetItem
            })
            .ToListAsync();

        var topItems = itemGroup
            .Concat(setItemGroup)
            .OrderByDescending(x => x.Quantity)
            .Take(3)
            .ToList();

        var itemIds = topItems.Where(oi => oi.Type == ItemType.Item).Select(oi => oi.Id).ToList();
        var setItemIds = topItems.Where(oi => oi.Type == ItemType.SetItem).Select(oi => oi.Id).ToList();

        var items = await dbContext.Items
            .Where(i => itemIds.Contains(i.Id))
            .Include(i => i.Images)
            .Select(i => new { i.Id, i.ItemName, Image = i.Images.Where(image => image.ImageUrl != null).FirstOrDefault() })
            .ToListAsync();

        var setItems = await dbContext.SetItems
            .Where(i => setItemIds.Contains(i.Id))
            .Include(i => i.Images)
            .Select(i => new { i.Id, i.SetItemName, Image = i.Images.Where(image => image.ImageUrl != null).FirstOrDefault() })
            .ToListAsync();

        var totalQuantity = topItems.Sum(oi => oi.Quantity);

        var model = topItems.Select(oi => new DashboardBestSellerModel
        {
            ItemId = oi.Id,
            ItemName = oi.Type == ItemType.Item
                ? items.Where(i => i.Id == oi.Id).FirstOrDefault()?.ItemName
                : setItems.Where(i => i.Id == oi.Id).FirstOrDefault()?.SetItemName,
            ImageUrl = oi.Type == ItemType.Item
                ? items.Where(i => i.Id == oi.Id).FirstOrDefault()?.Image?.ImageUrl
                : setItems.Where(i => i.Id == oi.Id).FirstOrDefault()?.Image?.ImageUrl,
            Quantity = oi.Quantity,
            Amount = oi.Amount
        })
            .OrderByDescending(oi => oi.Quantity)
            .ToList();

        for (int i = 0; i < model.Count; i++)
        {
            double quantity = model[i].Quantity;
            model[i].Percentage = totalQuantity > 0 ? (quantity / totalQuantity) * 100 : 0d;
        }

        return model;
    }

    public async Task<SummaryReportModel> GetSummaryReportAsync(DateTime startDate, DateTime endDate, IEnumerable<Guid> selectedGroupBuyIds)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Orders
            .AsNoTracking()
            .Where(order => order.CreationTime.Date >= startDate.Date && order.CreationTime.Date <= endDate.Date)
            .WhereIf(selectedGroupBuyIds.Any(), order => selectedGroupBuyIds.Contains(order.GroupBuyId))
            .Include(order => order.OrderItems)
            .Select(order => new
            {
                order.ShippingStatus,
                order.TotalQuantity,
                order.TotalAmount,
                order.GroupBuyId,
                order.OrderItems
            });

        var totalOrders = await query.CountAsync();
        var orders = query.Where(order => order.ShippingStatus == ShippingStatus.Completed).ToList();

        var result = new SummaryReportModel
        {
            TotalOrdersCompleted = orders.Count,
            TotalItemsSold = orders.Sum(order => order.TotalQuantity),
            TotalSales = orders.Sum(order => order.TotalAmount),
            ActiveGroupBuys = query.Select(order => order.GroupBuyId).Distinct().Count(),
            TotalOrders = totalOrders,
        };

        var memberRole = await dbContext.Roles.FirstOrDefaultAsync(role => role.NormalizedName == MemberConsts.Role);
        result.NewMembers = memberRole != null
            ? await dbContext.Users
            .Where(user => user.Roles.Any(role => role.RoleId == memberRole.Id))
            .Where(user => user.CreationTime.Date >= startDate.Date && user.CreationTime.Date <= endDate.Date)
            .CountAsync()
            : 0;

        var topPerformingProduct = orders
            .SelectMany(q => q.OrderItems)
            .Where(q => q.ItemId != null)
            .GroupBy(q => q.ItemId!.Value)
            .Select(q => new { q.Key, TotalQuantity = q.Sum(oi => oi.Quantity), Type = ItemType.Item })
            .Concat(orders
                .SelectMany(q => q.OrderItems)
                .Where(q => q.SetItemId != null)
                .GroupBy(q => q.SetItemId!.Value)
                .Select(q => new { q.Key, TotalQuantity = q.Sum(oi => oi.Quantity), Type = ItemType.SetItem }))
            .OrderByDescending(q => q.TotalQuantity)
            .FirstOrDefault();

        var productName = topPerformingProduct?.Type == ItemType.Item
                ? await dbContext.Items
                .Where(item => topPerformingProduct != null && item.Id == topPerformingProduct.Key)
                .Select(item => item.ItemName)
                .FirstOrDefaultAsync()
                : await dbContext.SetItems
                .Where(setItem => topPerformingProduct != null && setItem.Id == topPerformingProduct.Key)
                .Select(setItem => setItem.SetItemName)
                .FirstOrDefaultAsync();

        result.TopPerformingProduct = productName;
        result.TopPerformingProductQuantity = topPerformingProduct?.TotalQuantity;

        var groupBuyIds = orders.Select(order => order.GroupBuyId);
        var groupBuys = await dbContext.GroupBuys
            .Where(gb => groupBuyIds.Contains(gb.Id))
            .Select(gb => new
            {
                Id = gb.Id,
                Name = gb.GroupBuyName
            }).ToListAsync();

        var groupBuySummaries = groupBuys
            .GroupJoin(orders,
                gb => gb.Id,
                o => o.GroupBuyId,
                (gb, orders) => new SummaryReportGroupBuyModel
                {
                    Name = gb.Name,
                    Revenue = orders.Sum(o => o.TotalAmount),
                    CompletedOrders = orders.Count()
                })
            .ToList();

        var totalRevenue = groupBuySummaries.Sum(x => x.Revenue);

        foreach (var summary in groupBuySummaries)
        {
            summary.PercentageOfTotal = totalRevenue == 0 ? 0 : (summary.Revenue / totalRevenue) * 100;
        }

        result.GroupBuys = groupBuySummaries;

        return result;
    }

    public async Task<List<ProductSummaryModel>> GetProductSummaryAsync(DateTime startDate, DateTime endDate, IEnumerable<Guid> selectedGroupBuyIds)
    {
        var dbContext = await GetDbContextAsync();

        var data = await dbContext.Orders
            .Where(order => order.CreationTime.Date >= startDate.Date && order.CreationTime.Date <= endDate.Date
            && order.ShippingStatus == ShippingStatus.Completed)
            .WhereIf(selectedGroupBuyIds.Any(), order => selectedGroupBuyIds.Contains(order.GroupBuyId))
            .Include(order => order.OrderItems)
            .Select(order => new
            {
                items = order.OrderItems
                    .Select(oi => new
                    {
                        OrderId = order.Id,
                        order.GroupBuyId,
                        oi.ItemId,
                        oi.SKU,
                        oi.SetItemId,
                        oi.Quantity,
                        oi.ItemPrice,
                        oi.TotalAmount
                    }).Where(oi => oi.ItemId.HasValue || oi.SetItemId.HasValue)
            }).SelectMany(x => x.items)
            .ToListAsync();

        var items = data
            .Where(x => x.ItemId.HasValue)
            .GroupBy(x => new { ItemId = x.ItemId!.Value, x.SKU, x.GroupBuyId });

        var itemIds = items.Select(i => i.Key.ItemId);

        var itemNames = await dbContext.Items
            .Where(x => itemIds.Contains(x.Id))
            .Select(x => new { x.Id, x.ItemName, Category = x.CategoryProducts.FirstOrDefault() })
            .ToListAsync();

        var categoryIds = itemNames
            .Select(x => x.Category?.ProductCategoryId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var categories = await dbContext.ProductCategories
            .Where(pc => categoryIds.Contains(pc.Id))
            .Select(pc => new { pc.Id, pc.Name, pc.ZHName })
            .ToListAsync();

        var itemCategories = itemNames
            .Where(x => x.Category != null)
            .Join(categories,
                item => item.Category!.ProductCategoryId,
                category => category.Id,
                (item, category) => new
                {
                    ItemId = item.Id,
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    CategoryZHName = category.ZHName
                })
            .ToList();

        var setItems = data
            .Where(x => x.SetItemId.HasValue)
            .GroupBy(x => new { SetItemId = x.SetItemId!.Value, x.GroupBuyId });

        var setItemIds = setItems.Select(x => x.Key.SetItemId);

        var setItemNames = await dbContext.SetItems
            .Where(x => setItemIds.Contains(x.Id))
            .Select(x => new { x.Id, x.SetItemName })
            .ToListAsync();

        var groupBuyIds = data.Select(x => x.GroupBuyId);

        var groupBuyNames = await dbContext.GroupBuys
            .Where(gb => groupBuyIds.Contains(gb.Id))
            .Select(gb => new { gb.Id, gb.GroupBuyName })
            .ToListAsync();

        var result = items.Select(x => new ProductSummaryModel
        {
            Name = itemNames.Where(i => i.Id == x.Key.ItemId).FirstOrDefault()?.ItemName,
            SKU = x.Key.SKU,
            TotalQuantity = x.Sum(y => y.Quantity),
            UnitPrice = x.FirstOrDefault() != null ? x.First().ItemPrice : 0,
            TotalRevenue = x.Sum(y => y.TotalAmount),
            GroupBuy = groupBuyNames.Where(gb => gb.Id == x.Key.GroupBuyId).FirstOrDefault()?.GroupBuyName,
            Category = itemCategories.Where(c => c.ItemId == x.Key.ItemId).FirstOrDefault()?.CategoryName,
            CategoryZH = itemCategories.Where(c => c.ItemId == x.Key.ItemId).FirstOrDefault()?.CategoryZHName
        }).ToList();

        result.AddRange([.. setItems.Select(x => new ProductSummaryModel
        {
            Name = setItemNames.Where(i => i.Id == x.Key.SetItemId).FirstOrDefault()?.SetItemName,
            TotalQuantity = x.Sum(y => y.Quantity),
            UnitPrice = x.FirstOrDefault() != null ? x.First().ItemPrice : 0,
            TotalRevenue = x.Sum(y => y.TotalAmount),
            GroupBuy = groupBuyNames.Where(gb => gb.Id == x.Key.GroupBuyId).FirstOrDefault()?.GroupBuyName
        })]);

        return result;
    }

    public async Task<List<OrderItemDetailsModel>> GetOrderItemDetailsAsync(DateTime startDate, DateTime endDate, IEnumerable<Guid> selectedGroupBuyIds)
    {
        var dbContext = await GetDbContextAsync();

        var data = await dbContext.Orders
            .Where(order => order.CreationTime.Date >= startDate.Date && order.CreationTime.Date <= endDate.Date)
            .WhereIf(selectedGroupBuyIds.Any(), order => selectedGroupBuyIds.Contains(order.GroupBuyId))
            .Include(order => order.OrderItems)
            .Select(order => new
            {
                Items = order.OrderItems
                    .Select(oi => new
                    {
                        order.OrderNo,
                        order.CreationTime,
                        order.GroupBuyId,
                        oi.ItemId,
                        oi.SetItemId,
                        oi.SKU,
                        oi.Spec,
                        oi.Quantity,
                        oi.ItemPrice,
                        oi.TotalAmount,
                        order.ShippingStatus
                    })
            }).SelectMany(x => x.Items)
            .ToListAsync();

        var setItemIds = data.Where(x => x.SetItemId.HasValue).Select(x => x.SetItemId).Distinct();
        var setItems = await dbContext.SetItems
            .Where(si => setItemIds.Contains(si.Id))
            .Select(si => new { si.Id, si.SetItemName })
            .ToListAsync();

        var itemIds = data.Where(x => x.ItemId.HasValue).Select(x => x.ItemId!.Value).ToList();
        itemIds = itemIds.Distinct().ToList();

        var items = await dbContext.Items
            .Where(item => itemIds.Contains(item.Id))
            .Select(item => new { item.Id, item.ItemName })
            .ToListAsync();

        var groupBuyIds = data.Select(x => x.GroupBuyId);
        var groupBuys = await dbContext.GroupBuys
            .Where(gb => groupBuyIds.Contains(gb.Id))
            .Select(gb => new { gb.Id, gb.GroupBuyName })
            .ToListAsync();

        var result = data.Select(x => new OrderItemDetailsModel
        {
            OrderNo = x.OrderNo,
            CreationTime = x.CreationTime,
            GroupBuy = groupBuys.Where(gb => gb.Id == x.GroupBuyId).FirstOrDefault()?.GroupBuyName,
            ProductName = x.ItemId.HasValue
                ? items.Where(item => item.Id == x.ItemId).FirstOrDefault()?.ItemName
                : setItems.Where(setItem => setItem.Id == x.SetItemId).FirstOrDefault()?.SetItemName,
            Attributes = x.Spec,
            SKU = x.SKU,
            Quantity = x.Quantity,
            UnitPrice = x.ItemPrice,
            Total = x.TotalAmount,
            ShippingStatus = x.ShippingStatus
        })
            .OrderByDescending(x => x.CreationTime)
            .ToList();

        return result;
    }
}