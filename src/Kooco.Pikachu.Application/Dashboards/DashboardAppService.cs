using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

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

    public async Task<PagedResultDto<DashboardOrdersDto>> GetRecentOrdersAsync(DashboardFiltersDto input)
    {
        var data = await _dashboardRepository.GetRecentOrdersAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.SelectedGroupBuyIds
            );

        return new PagedResultDto<DashboardOrdersDto>
        {
            TotalCount = data.TotalCount,
            Items = ObjectMapper.Map<List<DashboardOrdersModel>, List<DashboardOrdersDto>>(data.Items)
        };
    }

    public async Task<List<DashboardBestSellerDto>> GetBestSellerItemsAsync(DashboardFiltersDto input)
    {
        var data = await _dashboardRepository.GetBestSellerItemsAsync(input.SelectedGroupBuyIds);
        return ObjectMapper.Map<List<DashboardBestSellerModel>, List<DashboardBestSellerDto>>(data);
    }
}
