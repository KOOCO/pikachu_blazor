using Blazorise;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class WebsiteBasicSettings
{
    private UpdateWebsiteBasicSettingDto Entity { get; set; }
    private bool IsLoading { get; set; }
    private bool IsCancelling { get; set; }
    private string LogoBase64 { get; set; }

    private Validations ValidationsRef;

    public WebsiteBasicSettings()
    {
        Entity = new();
    }

    protected override Task OnInitializedAsync()
    {
        return ResetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("updateDropText");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    async Task UpdateAsync()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;

                if (LogoBase64 != null)
                {
                    var bytes = Convert.FromBase64String(LogoBase64);
                    Entity.LogoUrl = await ImageAppService.UploadImageAsync(Entity.LogoName, bytes);
                }

                await WebsiteBasicSettingAppService.UpdateAsync(Entity);

                IsLoading = false;

                await Message.Success(L["WebsiteBasicSettingsUpdated"]);
            }
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task ResetAsync()
    {
        try
        {
            IsCancelling = true;

            var entity = await WebsiteBasicSettingAppService.FirstOrDefaultAsync();

            Entity = entity == null ? new() : ObjectMapper.Map<WebsiteBasicSettingDto, UpdateWebsiteBasicSettingDto>(entity);

            IsCancelling = false;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            IsCancelling = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        try
        {
            var file = e.Files.FirstOrDefault();

            if (file != null)
            {
                string extension = Path.GetExtension(file.Name);

                if (!Constant.ValidImageExtensions.Contains(extension))
                {
                    await Message.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", Constant.ValidImageExtensions)]);
                    return;
                }

                var bytes = await file.GetBytes();

                var compressed = await ImageCompressorService.CompressAsync(bytes);

                if (compressed.CompressedSize > Constant.MaxImageSizeInBytes)
                {
                    await Message.Error(L["Pikachu:ImageSizeExceeds", Constant.MaxImageSizeInBytes.FromBytesToMB()]);
                    return;
                }

                LogoBase64 = compressed.CompressedImage;
                Entity.LogoName = file.Name;

                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnColorSchemeChange(ColorScheme? colorScheme)
    {
        Entity.ColorScheme = colorScheme;
        switch (colorScheme)
        {
            case ColorScheme.ForestDawn:
                Entity.PrimaryColor = "#23856D";
                Entity.SecondaryColor = "#FFD057";
                Entity.BackgroundColor = "#FFFFFF";
                Entity.SecondaryBackgroundColor = "#C9D6BD";
                Entity.AlertColor = "#FF902A";
                Entity.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.TropicalSunset:
                Entity.PrimaryColor = "#FF902A";
                Entity.SecondaryColor = "#BDDA8D";
                Entity.BackgroundColor = "#FFFFFF";
                Entity.SecondaryBackgroundColor = "#E5D19A";
                Entity.AlertColor = "#FF902A";
                Entity.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.DeepSeaNight:
                Entity.PrimaryColor = "#133854";
                Entity.SecondaryColor = "#CAE28D";
                Entity.BackgroundColor = "#FFFFFF";
                Entity.SecondaryBackgroundColor = "#DCD6D0";
                Entity.AlertColor = "#A1E82D";
                Entity.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.SweetApricotCream:
                Entity.PrimaryColor = "#FFA085";
                Entity.SecondaryColor = "#BDDA8D";
                Entity.BackgroundColor = "#FFFFFF";
                Entity.SecondaryBackgroundColor = "#DCBFC3";
                Entity.AlertColor = "#FFC123";
                Entity.BlockColor = "#EFF4EB";
                break;

            case ColorScheme.DesertDawn:
                Entity.PrimaryColor = "#C08C5D";
                Entity.SecondaryColor = "#E7AD99";
                Entity.BackgroundColor = "#FFFFFF";
                Entity.SecondaryBackgroundColor = "#EBC7AD";
                Entity.AlertColor = "#FF902A";
                Entity.BlockColor = "#EFF4EB";
                break;


            default:
                Entity.PrimaryColor = string.Empty;
                Entity.SecondaryColor = string.Empty;
                Entity.BackgroundColor = string.Empty;
                Entity.SecondaryBackgroundColor = string.Empty;
                Entity.AlertColor = string.Empty;
                Entity.BlockColor = string.Empty;
                break;
        }
    }
}