using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.SalesReports;

public class GetSalesReportDto
{
    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }
    
    public Guid? GroupBuyId { get; set; }
}