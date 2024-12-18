using Blazorise;
using Kooco.Pikachu.WebsiteManagement;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Kooco.Pikachu.Extensions;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class WebsiteSettings
{
    private CreateWebsiteSettingsDto NewEntity { get; set; }
    private Validations ValidationsRef;

    private bool IsLoading { get; set; }

    private string LogoBase64 { get; set; }

    public WebsiteSettings()
    {
        NewEntity = new();
    }

    void NavigateToWebsiteSettings()
    {
        NavigationManager.NavigateTo("/Website-Settings");
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        var file = e.Files.FirstOrDefault();

        if (file != null)
        {
            string extension = Path.GetExtension(file.Name);

            if (file.Size > Constant.MaxImageSizeInBytes)
            {
                await UiNotificationService.Error(L["Pikachu:ImageSizeExceeds", Constant.MaxImageSizeInBytes.FromBytesToMB()]);
                return;
            }

            if (!Constant.ValidImageExtensions.Contains(extension))
            {
                await UiNotificationService.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", Constant.ValidImageExtensions)]);
                return;
            }

            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            LogoBase64 = Convert.ToBase64String(fileBytes);
            //NewEntity.LogoName = file.Name;

            await InvokeAsync(StateHasChanged);
        }
    }

    async Task CreateAsync()
    {
        var validate = await ValidationsRef.ValidateAll();
        if (!validate) return;
        return;
        try
        {
            IsLoading = true;

            var bytes = Convert.FromBase64String(LogoBase64);
            //NewEntity.LogoUrl = await ImageAppService.UploadImageAsync(NewEntity.LogoName, bytes);

            await WebsiteSettingsAppService.CreateAsync(NewEntity);
            NavigateToWebsiteSettings();
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }
}