using Blazorise;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Tenants.Repositories;
using Kooco.Pikachu.Tenants.TenantWallet;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using Volo.Abp.AspNetCore.Components;

namespace Kooco.Pikachu.Blazor.Helpers;
public abstract class FormComponentBase : AbpComponentBase
{
    protected FormComponentBase()
    {
        LocalizationResource = typeof(PikachuResource);
    }

    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;
    [Parameter] public Guid Id { get; set; }
    [Parameter] public string PreviousPage { get; set; } = null!;
    public IFileEntry? FileEntry { get; set; }
    public LoadingIndicator Loading { get; set; } = new();
    public required NavigationManager NavigationManager { get; init; }
    public required IJSRuntime JSRuntime { get; init; }
    public required IStringLocalizer<PageLayoutResource> PL { get; set; }
    public required ITenantWalletRepository TenantWalletRepository { get; init; }
    public required ITenantWalletAppService TenantWalletAppService { get; init; }
}