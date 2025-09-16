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
    public List<TenantPayoutStep> Steps { get; set; } = [];
    public TenantPayoutStep CurrentStep { get; set; } = new();
    public TenantPayoutContext Context { get; set; } = new();

    private IEnumerable<TenantPayoutBreadcrumb> BreadcrumbItems =>
        GetBreadcrumbs(Context.TenantName, Context.FeeType, Context.Year);

    protected override void OnInitialized()
    {
        Context = new() { Service = TenantPayoutAppService };
        ConfigureStepList();
        base.OnInitialized();
    }

    async Task OnImportCompletedAsync()
    {
        GoToStep(this, 1);
        await InvokeAsync(StateHasChanged);
    }

    async Task TenantChanged(TenantPayoutSummaryDto? tenant)
    {
        Context.TenantId = tenant?.TenantId;
        Context.TenantName = tenant?.Name;
        GoToStep(this, tenant != null ? 2 : 1);
        await InvokeAsync(StateHasChanged);
    }

    async Task FeeTypeChanged(PaymentFeeType? feeType)
    {
        Context.FeeType = feeType;
        GoToStep(this, feeType.HasValue ? 3 : 2);
        await InvokeAsync(StateHasChanged);
    }

    async Task YearChanged(int? year)
    {
        Context.Year = year;
        GoToStep(this, year.HasValue ? 4 : 3);
        await InvokeAsync(StateHasChanged);
    }

    void ConfigureStepList()
    {
        Steps = ConfigureSteps(this, L);
        CurrentStep = Steps.First();
    }
}