using Kooco.Pikachu.TenantPaymentFees;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class TenantPayout
{
    private TCatFileImportModal TCatImportModal { get; set; }
    private Guid? SelectedTenantId { get; set; }
    private PaymentFeeType? SelectedFeeType { get; set; }
    private int? SelectedYear { get; set; }
    List<BreadcrumbItem> BreadcrumbItems { get; set; } = [new(1, "Dashboard")];
    private List<int> Years { get; } = [2025, 2024, 2023];
    async Task OnImportCompletedAsync()
    {
        await Task.CompletedTask;
    }

    static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var parts = name
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return string.Empty;

        var initials = parts[0][..1];

        if (parts.Length > 1)
            initials += parts[1][..1];

        return initials.ToUpper();
    }

    void SelectTenant(Guid tenantId)
    {
        SelectedTenantId = tenantId;
        BreadcrumbItems.Add(new(2, "Acme Corporation")); //Tenant name
    }

    void SelectFeeType(PaymentFeeType feeType)
    {
        SelectedFeeType = feeType;
        BreadcrumbItems.Add(new(3, "ECPay")); //Provider name
    }

    void SelectYear(int year)
    {
        SelectedYear = year;
        BreadcrumbItems.Add(new(4, year.ToString()));
        BreadcrumbItems.Add(new(5, "PayoutDetails"));
    }

    void BackToTenants()
    {
        SelectedTenantId = null;
        BreadcrumbItems.RemoveAll(bi => bi.Step > 1);
    }

    void BackToProviders()
    {
        SelectedFeeType = null;
        BreadcrumbItems.RemoveAll(bi => bi.Step > 2);
    }

    void BackToYears()
    {
        SelectedYear = null;
        BreadcrumbItems.RemoveAll(bi => bi.Step > 3);
    }

    private record BreadcrumbItem(int Step, string Name);
}