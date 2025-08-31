using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class TenantPayout
{
    private TCatFileImportModal TCatImportModal { get; set; }

    async Task OnImportCompletedAsync()
    {
        await Task.CompletedTask;
    }
}