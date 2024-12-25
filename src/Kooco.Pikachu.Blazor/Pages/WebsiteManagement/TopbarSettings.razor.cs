using Kooco.Pikachu.WebsiteManagement.TopbarSettings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class TopbarSettings
{
    private TopbarLinkSettings? SelectedLinkSettings { get; set; }
    private List<Model> Models { get; set; } = [];
    private int DraggedModelIndex { get; set; }
    private int DraggedChildIndex { get; set; }
    private bool IsChildDragged { get; set; }
    private bool IsLoading { get; set; }

    async Task UpdateAsync()
    {
        IsLoading = true;
        StateHasChanged();
        await Task.Delay(TimeSpan.FromSeconds(2));
        IsLoading = false;
    }

    Task OnLinkSettingsSelected(TopbarLinkSettings? topbarLinkSettings)
    {
        RefreshModelsIndex();
        if (topbarLinkSettings.HasValue)
        {
            Models.Add(new Model(Models.Count, topbarLinkSettings.Value));
        }
        SelectedLinkSettings = null;
        return Task.CompletedTask;
    }

    static Task OnTopbarCategoryLinkOptionSelected(Model model, TopbarCategoryLinkOption topbarCategoryLinkOption)
    {
        RefreshChildrenIndex(model);
        model.Children.Add(new ChildModel(model.Children.Count, topbarCategoryLinkOption));
        return Task.CompletedTask;
    }

    Task RefreshModelsIndex()
    {
        Models.ForEach(model =>
        {
            model.Index = Models.IndexOf(model);
        });
        return Task.CompletedTask;
    }

    static Task RefreshChildrenIndex(Model model)
    {
        model.Children.ForEach(child =>
        {
            child.Index = model.Children.IndexOf(child);
        });
        return Task.CompletedTask;
    }

    Task RemoveModel(Model model)
    {
        Models.Remove(model);
        return Task.CompletedTask;
    }

    static Task RemoveChild(Model model, ChildModel child)
    {
        model.Children.Remove(child);
        return Task.CompletedTask;
    }

    Task StartModelDrag(Model model)
    {
        IsChildDragged = false;
        DraggedModelIndex = Models.IndexOf(model);
        return Task.CompletedTask;
    }

    Task ModelDrop(Model model)
    {
        if (!IsChildDragged && model != null)
        {
            var index = Models.IndexOf(model);

            var current = Models[DraggedModelIndex];

            Models.RemoveAt(DraggedModelIndex);
            Models.Insert(index, current);

            RefreshModelsIndex();
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    Task StartChildDrag(Model model, ChildModel child)
    {
        IsChildDragged = true;
        DraggedChildIndex = model.Children.IndexOf(child);
        return Task.CompletedTask;
    }

    Task ChildDrop(Model model, ChildModel child)
    {
        if (IsChildDragged && model != null && child != null)
        {
            var index = model.Children.IndexOf(child);

            var current = model.Children[DraggedChildIndex];

            model.Children.RemoveAt(DraggedChildIndex);
            model.Children.Insert(index, current);

            RefreshChildrenIndex(model);
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    private class Model
    {
        public int Index { get; set; }
        public TopbarLinkSettings TopbarLinkSettings { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public List<ChildModel> Children { get; set; } = [];

        public Model(int index, TopbarLinkSettings topbarLinkSettings)
        {
            Index = index;
            TopbarLinkSettings = topbarLinkSettings;
        }
    }

    private class ChildModel
    {
        public int Index { get; set; }
        public TopbarCategoryLinkOption TopbarCategoryLinkOption { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }

        public ChildModel(int index, TopbarCategoryLinkOption topbarCategoryLinkOption)
        {
            Index = index;
            TopbarCategoryLinkOption = topbarCategoryLinkOption;
        }
    }
}