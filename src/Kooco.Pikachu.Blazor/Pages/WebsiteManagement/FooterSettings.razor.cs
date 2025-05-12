using Blazorise;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.WebsiteManagement.FooterSettings;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class FooterSettings
{
    private UpdateFooterSettingDto Entity { get; set; }
    private bool IsLoading { get; set; }
    private bool IsCancelling { get; set; }
    private int DraggedLinkIndex { get; set; }

    private Validations ValidationsRef;

    public FooterSettings()
    {
        Entity = new();
        Enum.GetValues<FooterSettingsPosition>().ToDynamicList().ForEach(enumValue =>
        {
            Entity.Sections.Add(new(enumValue));
        });
    }

    protected override async Task OnInitializedAsync()
    {
        await ResetAsync();
    }

    async Task UpdateAsync()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                if (Entity.Sections.Any(s => s.FooterSettingsType == FooterSettingsType.Link &&
                    (s.Links == null || s.Links.Count == 0 || s.Links.Count >= FooterSettingsConsts.MaxAllowedLinks)))
                {
                    throw new InvalidNumberOfLinksException(FooterSettingsConsts.MaxAllowedLinks);
                }

                IsLoading = true;

                foreach (var section in Entity.Sections)
                {
                    if (section.FooterSettingsType == FooterSettingsType.Image && section.ImageBase64 != null)
                    {
                        var bytes = Convert.FromBase64String(section.ImageBase64);
                        section.ImageUrl = await ImageAppService.UploadImageAsync(section.ImageName, bytes);
                    }
                }

                await FooterSettingAppService.UpdateAsync(Entity);
                await Message.Success(L["FooterSettingsUpdated"]);
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

    async Task ResetAsync()
    {
        try
        {
            IsCancelling = true;
            var footerSetting = await FooterSettingAppService.FirstOrDefaultAsync();
            if (footerSetting is not null)
            {
                Entity = ObjectMapper.Map<FooterSettingDto, UpdateFooterSettingDto>(footerSetting);
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("updateDropText");
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsCancelling = false;
            StateHasChanged();
        }
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e, UpdateFooterSettingSectionDto section)
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

                section.ImageBase64 = compressed.CompressedImage;
                section.ImageName = file.Name;

                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    Task AddLink(UpdateFooterSettingSectionDto section)
    {
        RefreshLinksIndex();
        section.Links.Add(new UpdateFooterSettingLinkDto(section.Links.Count));
        return Task.CompletedTask;
    }

    Task RemoveLink(UpdateFooterSettingSectionDto section, UpdateFooterSettingLinkDto link)
    {
        section.Links.Remove(link);
        RefreshLinksIndex();
        return Task.CompletedTask;
    }

    Task RefreshLinksIndex()
    {
        Entity.Sections.ForEach(section =>
        {
            section.Links.ForEach(link =>
            {
                link.Index = section.Links.IndexOf(link);
            });
        });

        return Task.CompletedTask;
    }

    Task StartLinkDrag(UpdateFooterSettingSectionDto section, UpdateFooterSettingLinkDto link)
    {
        DraggedLinkIndex = section.Links.IndexOf(link);
        return Task.CompletedTask;
    }

    Task LinkDrop(UpdateFooterSettingSectionDto section, UpdateFooterSettingLinkDto link)
    {
        if (section != null && link != null)
        {
            var index = section.Links.IndexOf(link);

            var current = section.Links[DraggedLinkIndex];

            section.Links.RemoveAt(DraggedLinkIndex);
            section.Links.Insert(index, current);

            RefreshLinksIndex();
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    async Task OnTypeChanged(UpdateFooterSettingSectionDto section, FooterSettingsType? footerSettingsType)
    {
        section.FooterSettingsType = footerSettingsType;
        if (footerSettingsType == FooterSettingsType.Image)
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
}