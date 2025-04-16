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
    private long TotalMembers { get; set; }

    [Parameter]
    public EventCallback OnTagAdded { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                MemberTagOptions = await MemberTagAppService.GetMemberTagNamesAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    public void Create()
    {
        Input = new();
        OpenTagModal();
    }

    public void Edit(Guid id, string name)
    {
        Input = new() { EditingId = id, Name = name };
        OpenTagModal();
    }

    public void Hide()
    {
        CloseTagModal();
    }

    void OpenTagModal()
    {
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

    async Task OnRangeChange(AntDesign.DateRangeChangedEventArgs<DateTime?[]> args)
    {
        Input.RegistrationDateRange = args.Dates;
        await CountMembersAsync();
    }

    async Task CountMembersAsync()
    {
        try
        {
            TotalMembers = await MemberTagAppService.CountMembersAsync(Input);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
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

            if (TotalMembers == 0)
            {
                await Message.Error(L["PleaseAdjustFiltersToApplyTag"]);
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