using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Dashboards;

[Authorize]
[RemoteService(IsEnabled = false)]
public class DashboardAppService(IDashboardRepository dashboardRepository) : PikachuAppService, IDashboardAppService
{
    private readonly IDashboardRepository _dashboardRepository = dashboardRepository;

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(DashboardFiltersDto input)
    {
        var stats = await _dashboardRepository.GetDashboardStatsAsync(
            input.SelectedGroupBuyIds,
            input.StartDate,
            input.EndDate,
            input.PreviousDate
        );

        return ObjectMapper.Map<DashboardStatsModel, DashboardStatsDto>(stats);
    }

    public async Task<DashboardChartsDto> GetDashboardChartsAsync(DashboardFiltersDto input)
    {
        var charts = await _dashboardRepository.GetDashboardChartsAsync(
            input.SelectedPeriodOption,
            input.SelectedGroupBuyIds,
            input.StartDate,
            input.EndDate
        );

        return ObjectMapper.Map<DashboardChartsModel, DashboardChartsDto>(charts);
    }
}
