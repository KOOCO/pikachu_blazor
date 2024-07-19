using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement;

public partial class TenantContactTitle
{
    [Parameter]
    public object Data { get; set; }
    public string TitleName { get; set; }

    public TenantContactTitle() { }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        TenantDto tenant = Data.As<TenantDto>();



    }
}
