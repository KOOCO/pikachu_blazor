using Blazorise;
using Kooco.Pikachu.WebsiteManagement.TopbarSettings;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class TopbarSettings
{
    private UpdateTopbarSettingDto Entity { get; set; }
    private TopbarLinkSettings? SelectedLinkSettings { get; set; }
    private int DraggedLinkIndex { get; set; }
    private int DraggedCategoryOptionIndex { get; set; }
    private bool IsCategoryOptionDragged { get; set; }
    private bool IsLoading { get; set; }
    private bool IsCancelling { get; set; }

    private Validations ValidationsRef;

    public TopbarSettings()
    {
        Entity = new();
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
                IsLoading = true;
                await TopbarSettingAppService.UpdateAsync(Entity);
                await ResetAsync();
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
            var entity = await TopbarSettingAppService.FirstOrDefaultAsync();
            if(entity is not null)
            {
                Entity = ObjectMapper.Map<TopbarSettingDto, UpdateTopbarSettingDto>(entity);
            }
            ValidationsRef?.ClearAll();
        }
        catch(Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsCancelling = false;
        }
    }

    Task OnLinkSettingsSelected(TopbarLinkSettings? topbarLinkSettings)
    {
        RefreshLinksIndex();
        if (topbarLinkSettings.HasValue)
        {
            Entity.Links.Add(new UpdateTopbarSettingLinkDto(Entity.Links.Count, topbarLinkSettings.Value));
        }
        SelectedLinkSettings = null;
        return Task.CompletedTask;
    }

    static Task OnTopbarCategoryLinkOptionSelected(UpdateTopbarSettingLinkDto link, TopbarCategoryLinkOption topbarCategoryLinkOption)
    {
        RefreshCategoryOptionsIndex(link);
        link.CategoryOptions.Add(new UpdateTopbarSettingCategoryOptionDto(link.CategoryOptions.Count, topbarCategoryLinkOption));
        return Task.CompletedTask;
    }

    Task RefreshLinksIndex()
    {
        Entity.Links.ForEach(link =>
        {
            link.Index = Entity.Links.IndexOf(link);
        });
        return Task.CompletedTask;
    }

    static Task RefreshCategoryOptionsIndex(UpdateTopbarSettingLinkDto link)
    {
        link.CategoryOptions.ForEach(categoryOption =>
        {
            categoryOption.Index = link.CategoryOptions.IndexOf(categoryOption);
        });
        return Task.CompletedTask;
    }

    Task RemoveLink(UpdateTopbarSettingLinkDto link)
    {
        Entity.Links.Remove(link);
        return Task.CompletedTask;
    }

    static Task RemoveCategoryOption(UpdateTopbarSettingLinkDto link, UpdateTopbarSettingCategoryOptionDto categoryOption)
    {
        link.CategoryOptions.Remove(categoryOption);
        return Task.CompletedTask;
    }

    Task StartLinkDrag(UpdateTopbarSettingLinkDto link)
    {
        IsCategoryOptionDragged = false;
        DraggedLinkIndex = Entity.Links.IndexOf(link);
        return Task.CompletedTask;
    }

    Task LinkDrop(UpdateTopbarSettingLinkDto link)
    {
        if (!IsCategoryOptionDragged && link != null)
        {
            var index = Entity.Links.IndexOf(link);

            var current = Entity.Links[DraggedLinkIndex];

            Entity.Links.RemoveAt(DraggedLinkIndex);
            Entity.Links.Insert(index, current);

            RefreshLinksIndex();
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    Task StartCategoryOptionDrag(UpdateTopbarSettingLinkDto link, UpdateTopbarSettingCategoryOptionDto categoryOption)
    {
        IsCategoryOptionDragged = true;
        DraggedCategoryOptionIndex = link.CategoryOptions.IndexOf(categoryOption);
        return Task.CompletedTask;
    }

    Task CategoryOptionDrop(UpdateTopbarSettingLinkDto link, UpdateTopbarSettingCategoryOptionDto categoryOption)
    {
        if (IsCategoryOptionDragged && link != null && categoryOption != null)
        {
            var index = link.CategoryOptions.IndexOf(categoryOption);

            var current = link.CategoryOptions[DraggedCategoryOptionIndex];

            link.CategoryOptions.RemoveAt(DraggedCategoryOptionIndex);
            link.CategoryOptions.Insert(index, current);

            RefreshCategoryOptionsIndex(link);
            StateHasChanged();
        }
        return Task.CompletedTask;
    }
}