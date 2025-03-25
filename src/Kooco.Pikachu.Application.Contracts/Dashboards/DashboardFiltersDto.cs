using Kooco.Pikachu.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kooco.Pikachu.Dashboards;

public class DashboardFiltersDto
{
    [Required]
    public ReportCalculationUnits? SelectedPeriodOption { get; set; } = ReportCalculationUnits.Daily;
    public IEnumerable<Guid> SelectedGroupBuyIds { get; set; } = [];
    public DateTime?[] SelectedDateRange { get; set; } = [DateTime.Today, DateTime.Today.AddDays(1).AddMilliseconds(-1)];
    public DateTime? StartDate { get { return SelectedDateRange.FirstOrDefault()?.Date; } }
    public DateTime? EndDate { get { return SelectedDateRange.Length > 1 ? SelectedDateRange.LastOrDefault()?.Date : null; } }

    public DateTime? PreviousDate
    {
        get
        {
            return SelectedPeriodOption switch
            {
                ReportCalculationUnits.Daily => StartDate?.AddDays(-1),
                ReportCalculationUnits.Weekly => StartDate?.AddDays(-7),
                ReportCalculationUnits.Monthly => StartDate?.AddMonths(-1),
                _ => null
            };
        }
    }
}
