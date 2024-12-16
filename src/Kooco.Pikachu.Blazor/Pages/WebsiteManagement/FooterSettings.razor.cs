using Blazorise;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.WebsiteManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class FooterSettings
{
    private List<Model> Models = [new("Left"), new("Center"), new("Right")];
    private bool IsLoading { get; set; }
    private int DraggedLinkIndex { get; set; }

    async Task UpdateAsync()
    {
        IsLoading = true;
        StateHasChanged();
        await Task.Delay(TimeSpan.FromSeconds(2));
        IsLoading = false;
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e, Model model)
    {
        var file = e.Files.FirstOrDefault();

        if (file != null)
        {
            string extension = Path.GetExtension(file.Name);

            if (file.Size > Constant.MaxImageSizeInBytes)
            {
                await Message.Error(L["Pikachu:ImageSizeExceeds", Constant.MaxImageSizeInBytes.FromBytesToMB()]);
                return;
            }

            if (!Constant.ValidImageExtensions.Contains(extension))
            {
                await Message.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", Constant.ValidImageExtensions)]);
                return;
            }

            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            model.ImageBase64 = Convert.ToBase64String(fileBytes);
            model.ImageName = file.Name;

            await InvokeAsync(StateHasChanged);
        }
    }

    Task AddLink(Model model)
    {
        RefreshLinksIndex();
        model.Links.Add(new LinkModel(model.Links.Count));
        return Task.CompletedTask;
    }

    Task RemoveLink(Model model, LinkModel link)
    {
        model.Links.Remove(link);
        RefreshLinksIndex();
        return Task.CompletedTask;
    }

    Task RefreshLinksIndex()
    {
        Models.ForEach(model =>
        {
            model.Links.ForEach(link =>
            {
                link.Index = model.Links.IndexOf(link);
            });
        });

        return Task.CompletedTask;
    }

    Task StartLinkDrag(Model model, LinkModel link)
    {
        DraggedLinkIndex = model.Links.IndexOf(link);
        return Task.CompletedTask;
    }

    Task LinkDrop(Model model, LinkModel link)
    {
        if (model != null && link != null)
        {
            var index = model.Links.IndexOf(link);

            var current = model.Links[DraggedLinkIndex];

            model.Links.RemoveAt(DraggedLinkIndex);
            model.Links.Insert(index, current);

            RefreshLinksIndex();
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    private class Model
    {
        public string Heading { get; set; }
        public FooterSettingsType? FooterSettingsType { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageName { get; set; }
        public List<LinkModel> Links { get; set; }

        public Model(string heading)
        {
            Heading = heading;
            Links = [];
        }
    }

    private class LinkModel
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public LinkModel(int index)
        {
            Index = index;
        }
    }
}