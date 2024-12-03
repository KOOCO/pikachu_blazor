using Kooco.Pikachu.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.SalesReports;

public class GetSalesReportDto
{
    [Required]
    public DateTime? StartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime? EndDate { get; set; } = DateTime.Today.AddDays(1).AddMilliseconds(-1);

    public Guid? GroupBuyId { get; set; }

    public ReportCalculationUnits ReportCalculationUnit { get; set; } = ReportCalculationUnits.Daily;
}