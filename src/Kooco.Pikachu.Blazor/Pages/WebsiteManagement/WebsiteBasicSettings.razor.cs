using Blazorise;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.WebsiteManagement;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class WebsiteBasicSettings
{
    private CreateWebsiteSettingsDto NewEntity { get; set; }
    private Validations ValidationsRef;

    private bool IsLoading { get; set; }

    private string LogoBase64 { get; set; }

    public WebsiteBasicSettings()
    {
        NewEntity = new();
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
            NewEntity.LogoName = file.Name;

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
            NewEntity.LogoUrl = await ImageAppService.UploadImageAsync(NewEntity.LogoName, bytes);

            await WebsiteSettingsAppService.CreateAsync(NewEntity);
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }
}