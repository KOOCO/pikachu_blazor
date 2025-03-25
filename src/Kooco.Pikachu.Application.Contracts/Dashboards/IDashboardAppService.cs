using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Dashboards;

public interface IDashboardAppService : IApplicationService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(DashboardFiltersDto input);
    Task<DashboardChartsDto> GetDashboardChartsAsync(DashboardFiltersDto input);
    Task<PagedResultDto<DashboardOrdersDto>> GetRecentOrdersAsync(DashboardFiltersDto input);
}
