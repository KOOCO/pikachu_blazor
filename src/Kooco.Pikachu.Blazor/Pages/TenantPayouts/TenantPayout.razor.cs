using Kooco.Pikachu.TenantPaymentFees;
using Kooco.Pikachu.TenantPayouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Kooco.Pikachu.Blazor.Pages.TenantPayouts.TenantPayoutUiHelper;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class TenantPayout
{
    public TCatFileImportModal TCatImportModal { get; set; }
    public TenantPayoutSummaryDto? SelectedTenant { get; set; }
    private Guid? TenantId => SelectedTenant?.TenantId;
    public PaymentFeeType? FeeType { get; set; }
    public int? Year { get; set; }
    public List<TenantPayoutStep> Steps { get; set; } = [];
    public TenantPayoutStep CurrentStep { get; set; } = new();

    private IEnumerable<TenantPayoutBreadcrumb> BreadcrumbItems =>
        GetBreadcrumbs(SelectedTenant?.Name, FeeType, Year);

    protected override void OnInitialized()
    {
        ConfigureStepList();
        base.OnInitialized();
    }

    async Task OnImportCompletedAsync()
    {
        await Task.CompletedTask;
    }

    async Task TenantChanged(TenantPayoutSummaryDto? tenant)
    {
        SelectedTenant = tenant;
        GoToStep(this, tenant != null ? 2 : 1);
        await InvokeAsync(StateHasChanged);
    }

    async Task FeeTypeChanged(PaymentFeeType? feeType)
    {
        FeeType = feeType;
        GoToStep(this, feeType.HasValue ? 3 : 2);
        await InvokeAsync(StateHasChanged);
    }

    async Task YearChanged(int? year)
    {
        Year = year;
        GoToStep(this, year.HasValue ? 4 : 3);
        await InvokeAsync(StateHasChanged);
    }

    void ConfigureStepList()
    {
        Steps = ConfigureSteps(this, L);
        CurrentStep = Steps.First();
    }
}