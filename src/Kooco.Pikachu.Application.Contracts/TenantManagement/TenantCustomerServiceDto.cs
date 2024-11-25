using System;

namespace Kooco.Pikachu.TenantManagement;

public class TenantCustomerServiceDto
{
    public string? ShortCode { get; set; }
    public string? CompanyName { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? CustomerServiceEmail { get; set; }
    public DateTime? ServiceHoursFrom { get; set; }
    public DateTime? ServiceHoursTo { get; set; }
}