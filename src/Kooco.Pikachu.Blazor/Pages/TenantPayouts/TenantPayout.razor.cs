using Kooco.Pikachu.TenantPaymentFees;
using Kooco.Pikachu.TenantPayouts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Kooco.Pikachu.Blazor.Pages.TenantPayouts.TenantPayoutUiHelper;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class TenantPayout
{
    public TCatFileImportModal TCatImportModal { get; set; }
    public TenantPayoutSummaryDto? SelectedTenant { get; set; }
    public PaymentFeeType? SelectedFeeType { get; set; }
    public int? SelectedYear { get; set; }
    public List<TenantPayoutStep> Steps { get; set; } = [];
    public TenantPayoutStep SelectedStep { get; set; } = new();
    private IEnumerable<TenantPayoutBreadcrumb> BreadcrumbItems =>
        GetBreadcrumbs(
            SelectedTenant?.Name,
            SelectedFeeType,
            SelectedYear,
            SelectedStep?.Step ?? 1);

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
        SelectedFeeType = feeType;
        GoToStep(this, feeType.HasValue ? 3 : 2);
        await InvokeAsync(StateHasChanged);
    }

    async Task SelectYear(int? year)
    {
        SelectedYear = year;
        GoToStep(this, year.HasValue ? 4 : 3);
        await InvokeAsync(StateHasChanged);
    }

    void ConfigureStepList()
    {
        Steps = ConfigureSteps(this, L);
        SelectedStep = Steps.First();
    }
}