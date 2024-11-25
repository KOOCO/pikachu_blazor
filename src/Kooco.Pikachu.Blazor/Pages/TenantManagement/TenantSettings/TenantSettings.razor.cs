using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantSettings
{
    private string SelectedTab { get; set; } = "TenantInformation";

    private Task OnSelectedTabChanged(string name)
    {
        SelectedTab = name;

        return Task.CompletedTask;
    }
}