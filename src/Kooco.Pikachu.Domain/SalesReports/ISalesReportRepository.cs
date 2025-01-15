using Kooco.Pikachu.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.SalesReports;

public interface ISalesReportRepository : IRepository
{
    Task<List<SalesReportModel>> GetSalesReportAsync(DateTime startDate, DateTime endDate, Guid? groupBuyId, ReportCalculationUnits unit);
    Task<List<SalesReportModel>> GetGroupBuySalesReportAsync(DateTime startDate, DateTime endDate, Guid? groupBuyId);
}
