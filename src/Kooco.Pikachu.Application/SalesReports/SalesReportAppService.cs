using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.SalesReports;

[Authorize]
public class SalesReportAppService(ISalesReportRepository salesReportRepository) : PikachuAppService, ISalesReportAppService
{
    public async Task<List<SalesReportDto>> GetSalesReportAsync(GetSalesReportDto input)
    {
        var startDate = input.StartDate ?? DateTime.Today;
        var endDate = input.EndDate ?? startDate.AddDays(1).AddMicroseconds(-1);

        var result = await salesReportRepository.GetSalesReportAsync(startDate, endDate, input.GroupBuyId, input.ReportCalculationUnit);
        return ObjectMapper.Map<List<SalesReportModel>, List<SalesReportDto>>(result);
    }

    public async Task<List<SalesReportDto>> GetGroupBuySalesReportAsync(DateTime startDate, DateTime endDate, Guid? groupBuyId)
    {
        var result = await salesReportRepository.GetGroupBuySalesReportAsync(startDate, endDate, groupBuyId);
        return ObjectMapper.Map<List<SalesReportModel>, List<SalesReportDto>>(result);
    }
}
