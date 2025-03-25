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
}
