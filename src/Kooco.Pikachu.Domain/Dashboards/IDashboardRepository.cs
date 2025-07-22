using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.Reports;

namespace Kooco.Pikachu.Dashboards;

public interface IDashboardRepository : IRepository
{
    Task<DashboardStatsModel> GetDashboardStatsAsync(IEnumerable<Guid> selectedGroupBuyIds, DateTime? startDate, DateTime? endDate, DateTime? previousDate);
    Task<DashboardChartsModel> GetDashboardChartsAsync(ReportCalculationUnits? periodOption, IEnumerable<Guid> selectedGroupBuyIds, DateTime? startDate, DateTime? endDate);
    Task<DashboardOrdersWithCountModel> GetRecentOrdersAsync(int skipCount, int maxResultCount, IEnumerable<Guid> selectedGroupBuyIds);
    Task<List<DashboardBestSellerModel>> GetBestSellerItemsAsync(IEnumerable<Guid> selectedGroupBuyIds);
    Task<SummaryReportModel> GetSummaryReportAsync(DateTime startDate, DateTime endDate, IEnumerable<Guid> selectedGroupBuyIds);
    Task<List<ProductSummaryModel>> GetProductSummaryAsync(DateTime startDate, DateTime endDate, IEnumerable<Guid> selectedGroupBuyIds);
    Task<List<OrderItemDetailsModel>> GetOrderItemDetailsAsync(DateTime startDate, DateTime endDate, IEnumerable<Guid> selectedGroupBuyIds);
}
