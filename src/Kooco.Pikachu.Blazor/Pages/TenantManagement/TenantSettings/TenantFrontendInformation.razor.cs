using Blazorise;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using Kooco.Pikachu.Extensions;
using Microsoft.Extensions.Logging;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantFrontendInformation
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }

    private TenantFrontendInformationDto TenantInformationDto { get; set; }
    private UpdateTenantFrontendInformationDto Entity { get; set; }
    private Validations ValidationsRef { get; set; }

    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;

    public TenantFrontendInformation()
    {
        Entity = new();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ResetAsync();
        }
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e, string type)
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

            var validExtensions = type == "favicon" ? Constant.ValidFaviconExtensions : Constant.ValidImageExtensions;

            if (!validExtensions.Contains(extension))
            {
                await UiNotificationService.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", validExtensions)]);
                return;
            }

            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            if (type == "logo")
            {
                Entity.LogoBase64 = Convert.ToBase64String(fileBytes);
                Entity.LogoName = file.Name;
            }

            if (type == "banner")
            {
                Entity.BannerBase64 = Convert.ToBase64String(fileBytes);
                Entity.BannerName = file.Name;
            }

            if (type == "favicon")
            {
                Entity.FaviconBase64 = Convert.ToBase64String(fileBytes);
                Entity.FaviconName = file.Name;
            }

            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task UpdateAsync()
    {
        try
        {
            string oldLogo = "";
            string oldFavicon = "";
            string oldBanner = "";

            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;

                if (Entity.FaviconBase64 != null)
                {
                    oldFavicon = ExtractFileName(Entity.FaviconUrl);
                    Entity.FaviconUrl = await AppService.UploadImageAsync(new UploadImageDto(Entity.FaviconBase64, Entity.FaviconName));
                }
                if (Entity.LogoBase64 != null)
                {
                    oldLogo = ExtractFileName(Entity.LogoUrl);
                    Entity.LogoUrl = await AppService.UploadImageAsync(new UploadImageDto(Entity.LogoBase64, Entity.LogoName));
                }
                if (Entity.BannerBase64 != null)
                {
                    oldBanner = ExtractFileName(Entity.BannerUrl);
                    Entity.BannerUrl = await AppService.UploadImageAsync(new UploadImageDto(Entity.BannerBase64, Entity.BannerName));
                }

                await AppService.UpdateTenantFrontendInformationAsync(Entity);

                await Message.Success(L["FrontendInformationUpdated"]);

                await ResetAsync();

                IsLoading = false;

                await DeleteOldImageAsync(oldFavicon);
                await DeleteOldImageAsync(oldLogo);
                await DeleteOldImageAsync(oldBanner);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    static string ExtractFileName(string? url)
    {
        if (url.IsNullOrWhiteSpace())
        {
            return "";
        }

        return url[(url.LastIndexOf('/') + 1)..];
    }

    async Task DeleteOldImageAsync(string? name)
    {
        try
        {
            if (name.IsNullOrWhiteSpace()) return;
            await AppService.DeleteImageAsync(name);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    async Task CancelAsync()
    {
        var confirmation = await Message.Confirm(L["AreYouSureToResetTheForm"]);
        if (confirmation)
        {
            IsCancelling = true;
            await ResetAsync();
            IsCancelling = false;
        }
    }

    private async Task ResetAsync()
    {
        try
        {
            TenantInformationDto = await AppService.GetTenantFrontendInformationAsync();
            Entity = ObjectMapper.Map<TenantFrontendInformationDto, UpdateTenantFrontendInformationDto>(TenantInformationDto);
            ValidationsRef?.ClearAll();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}