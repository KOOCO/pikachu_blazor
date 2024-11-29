using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.SalesReports;

public interface ISalesReportAppService : IApplicationService
{
    Task<List<SalesReportDto>> GetSalesReportAsync(GetSalesReportDto input);
}
