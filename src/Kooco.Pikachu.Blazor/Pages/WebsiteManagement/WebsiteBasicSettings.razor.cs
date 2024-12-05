using Blazorise;
using Kooco.Pikachu.EnumValues;
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

    private void OnColorSchemeChange(ColorScheme? colorScheme)
    {
        NewEntity.ColorScheme = colorScheme;
        switch (colorScheme)
        {
            case ColorScheme.ForestDawn:
                NewEntity.PrimaryColor = "#23856D";
                NewEntity.SecondaryColor = "#FFD057";
                NewEntity.BackgroundColor = "#FFFFFF";
                NewEntity.SecondaryBackgroundColor = "#C9D6BD";
                NewEntity.AlertColor = "#FF902A";
                break;

            case ColorScheme.TropicalSunset:
                NewEntity.PrimaryColor = "#FF902A";
                NewEntity.SecondaryColor = "#BDDA8D";
                NewEntity.BackgroundColor = "#FFFFFF";
                NewEntity.SecondaryBackgroundColor = "#E5D19A";
                NewEntity.AlertColor = "#FF902A";
                break;

            case ColorScheme.DeepSeaNight:
                NewEntity.PrimaryColor = "#133854";
                NewEntity.SecondaryColor = "#CAE28D";
                NewEntity.BackgroundColor = "#FFFFFF";
                NewEntity.SecondaryBackgroundColor = "#DCD6D0";
                NewEntity.AlertColor = "#A1E82D";
                break;

            case ColorScheme.SweetApricotCream:
                NewEntity.PrimaryColor = "#FFA085";
                NewEntity.SecondaryColor = "#BDDA8D";
                NewEntity.BackgroundColor = "#FFFFFF";
                NewEntity.SecondaryBackgroundColor = "#DCBFC3";
                NewEntity.AlertColor = "#FFC123";
                break;

            case ColorScheme.DesertDawn:
                NewEntity.PrimaryColor = "#C08C5D";
                NewEntity.SecondaryColor = "#E7AD99";
                NewEntity.BackgroundColor = "#FFFFFF";
                NewEntity.SecondaryBackgroundColor = "#EBC7AD";
                NewEntity.AlertColor = "#FF902A";
                break;


            default:
                NewEntity.PrimaryColor = string.Empty;
                NewEntity.SecondaryColor = string.Empty;
                NewEntity.BackgroundColor = string.Empty;
                NewEntity.SecondaryBackgroundColor = string.Empty;
                NewEntity.AlertColor = string.Empty;
                break;
        }
    }
}