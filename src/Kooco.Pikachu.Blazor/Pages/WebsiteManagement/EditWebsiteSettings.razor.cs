using Blazorise;
using Kooco.Pikachu.WebsiteManagement;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Kooco.Pikachu.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class EditWebsiteSettings
{
    [Parameter]
    public Guid Id { get; set; }

    private UpdateWebsiteSettingsDto EditingEntity { get; set; }

    private Validations ValidationsRef;

    private bool IsLoading { get; set; }

    private string LogoBase64 { get; set; }

    void NavigateToWebsiteSettings()
    {
        NavigationManager.NavigateTo("/Website-Settings");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                var websiteSettings = await WebsiteSettingsAppService.GetAsync(Id);
                EditingEntity = ObjectMapper.Map<WebsiteSettingsDto, UpdateWebsiteSettingsDto>(websiteSettings);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
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

            LogoBase64 = Convert.ToBase64String(fileBytes);
            EditingEntity.LogoName = file.Name;

            await InvokeAsync(StateHasChanged);
        }
    }

    async Task UpdateAsync()
    {
        var validate = await ValidationsRef.ValidateAll();
        if (!validate) return;
        try
        {
            IsLoading = true;

            string oldBlobName = "";
            if (!LogoBase64.IsNullOrWhiteSpace())
            {
                var bytes = Convert.FromBase64String(LogoBase64);
                oldBlobName = EditingEntity.LogoUrl.ExtractFileName();
                EditingEntity.LogoUrl = await ImageAppService.UploadImageAsync(EditingEntity.LogoName, bytes);
            }

            await WebsiteSettingsAppService.UpdateAsync(Id, EditingEntity);

            if (!oldBlobName.IsNullOrWhiteSpace())
            {
                await DeleteOldImage(oldBlobName);
            }

            NavigateToWebsiteSettings();
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task DeleteOldImage(string oldBlobName)
    {
        try
        {
            await ImageAppService.DeleteImageAsync(oldBlobName, false);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }
}