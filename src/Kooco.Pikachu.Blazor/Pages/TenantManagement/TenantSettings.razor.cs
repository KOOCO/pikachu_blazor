using Blazored.TextEditor;
using Blazorise;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement;

public partial class TenantSettings
{
    private TenantSettingsDto TenantSettingsDto { get; set; }
    private UpdateTenantSettingsDto Entity { get; set; }
    private bool CanEditTenantSettings { get; set; }
    private Validations ValidationsRef { get; set; }
    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;
    private bool ViewMode { get; set; } = true;

    private readonly List<string> TenantContactTitles = ["Mr.", "Ms."];
    private BlazoredTextEditor PrivacyPolicyHtml { get; set; }

    private string SelectedTab { get; set; } = "TenantInformation";

    public TenantSettings()
    {
        Entity = new();
    }

    protected override async Task OnInitializedAsync()
    {
        CanEditTenantSettings = await AuthorizationService.IsGrantedAsync(PikachuPermissions.TenantSettings.Edit);
        await ResetAsync();
    }

    private Task OnSelectedTabChanged(string name)
    {
        SelectedTab = name;

        return Task.CompletedTask;
    }

    private async Task UpdateAsync()
    {
        try
        {
            string oldLogo = "";
            string oldFavicon = "";
            string oldBanner = "";

            if (!CanEditTenantSettings) return;

            Entity.PrivacyPolicy = await PrivacyPolicyHtml.GetHTML();

            var validate = await ValidationsRef.ValidateAll();

            if (Entity.ServiceHoursFrom?.TimeOfDay >= Entity.ServiceHoursTo?.TimeOfDay)
            {
                throw new InvalidServiceHoursException();
            }

            if (!validate || (Entity.GtmEnabled && Entity.GtmContainerId.IsNullOrWhiteSpace()))
            {
                await UiNotificationService.Error(L["ValidationErrors"]);
                return;
            }
            IsLoading = true;
            if (Entity.FaviconBase64 != null)
            {
                oldFavicon = ExtractFileName(Entity.FaviconUrl);
                Entity.FaviconUrl = await TenantSettingsAppService.UploadImageAsync(new UploadImageDto(Entity.FaviconBase64, Entity.FaviconName));
            }
            if (Entity.LogoBase64 != null)
            {
                oldLogo = ExtractFileName(Entity.LogoUrl);
                Entity.LogoUrl = await TenantSettingsAppService.UploadImageAsync(new UploadImageDto(Entity.LogoBase64, Entity.LogoName));
            }
            if (Entity.BannerBase64 != null)
            {
                oldBanner = ExtractFileName(Entity.BannerUrl);
                Entity.BannerUrl = await TenantSettingsAppService.UploadImageAsync(new UploadImageDto(Entity.BannerBase64, Entity.BannerName));
            }

            await TenantSettingsAppService.UpdateAsync(Entity);

            await Message.Success(L["TenantSettingsUpdated"]);
            await ResetAsync();

            IsLoading = false;

            await DeleteOldImageAsync(oldFavicon);
            await DeleteOldImageAsync(oldLogo);
            await DeleteOldImageAsync(oldBanner);
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
            await TenantSettingsAppService.DeleteImageAsync(name);
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

    async Task SetDefaultPrivacyPolicy()
    {
        await PrivacyPolicyHtml.LoadHTMLContent(TenantSettingsConsts.DefaultPrivacyPolicy);
    }

    async Task EditAsync()
    {
        ViewMode = false;
        await PrivacyPolicyHtml.LoadHTMLContent(Entity.PrivacyPolicy);
        await PrivacyPolicyHtml.EnableEditor(true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task ResetAsync()
    {
        try
        {
            ViewMode = true;
            TenantSettingsDto = await TenantSettingsAppService.FirstOrDefaultAsync();
            Entity = ObjectMapper.Map<TenantSettingsDto, UpdateTenantSettingsDto>(TenantSettingsDto) ?? new();
            ValidationsRef?.ClearAll();
            await PrivacyPolicyHtml.LoadHTMLContent(Entity.PrivacyPolicy);
            await PrivacyPolicyHtml.EnableEditor(false);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
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
}