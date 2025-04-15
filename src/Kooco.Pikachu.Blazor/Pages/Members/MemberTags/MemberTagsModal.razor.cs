using Blazorise;
using Kooco.Pikachu.Members.MemberTags;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberTags;

public partial class MemberTagsModal
{
    private Modal TagModal;
    private bool Loading { get; set; } = false;
    private AddTagForUsersDto Input { get; set; } = new();
    private IReadOnlyList<string> MemberTagOptions { get; set; } = [];
    
    [Parameter]
    public EventCallback OnTagAdded { get; set; }

    public void Show()
    {
        OpenTagModal();
    }

    public void Hide()
    {
        CloseTagModal();
    }

    void OpenTagModal()
    {
        Input = new();
        TagModal?.Show();
    }

    void CloseTagModal()
    {
        TagModal?.Hide();
    }

    static Task OnModalClosing(ModalClosingEventArgs e)
    {
        e.Cancel = e.CloseReason != CloseReason.UserClosing;
        return Task.CompletedTask;
    }

    async Task AddTagsToMembersAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Input?.Name))
            {
                await Message.Error(L["PleaseProvideTagName"]);
                return;
            }

            Loading = true;

            await MemberTagAppService.AddTagForUsersAsync(Input);

            await OnTagAdded.InvokeAsync();

            CloseTagModal();

            Loading = false;
        }
        catch (Exception ex)
        {
            Loading = false;
            await HandleErrorAsync(ex);
        }
    }
}