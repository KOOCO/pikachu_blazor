using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets;
public partial class WalletDetailsHeader
{
    [Parameter] public TenantWalletResultDto TenantWallet { get; set; }
    [Parameter] public bool CanEditTenantWallet { get; set; }
}