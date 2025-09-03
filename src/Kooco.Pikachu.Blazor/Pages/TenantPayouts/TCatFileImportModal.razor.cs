using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.CodTradeInfos;
using Kooco.Pikachu.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class TCatFileImportModal
{
    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public EventCallback OnImportCompleted { get; set; }

    private List<TCatCodTradeInfoRecordDto> _records;
    private bool _recordsVisible;
    private bool _importing;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_recordsVisible)
        {
            await JSRuntime.InvokeVoidAsync("initUploadAreas");
        }
    }

    async Task Import()
    {
        if (_records.OrEmptyListIfNull().Count > 0)
        {
            try
            {
                _importing = true;
                await TCatCodTradeInfoAppService.ImportAsync(_records);
                CloseModal();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
            finally
            {
                _importing = false;
                StateHasChanged();
            }
        }
    }

    public void ShowModal()
    {
        VisibleChangedHandler(true);
        _recordsVisible = false;
        _records = [];
        StateHasChanged();
    }

    public void CloseModal()
    {
        _records = [];
        VisibleChangedHandler(false);
        StateHasChanged();
    }

    void VisibleChangedHandler(bool value)
    {
        Visible = value;
        VisibleChanged.InvokeAsync(value);
    }

    void ChooseAnother()
    {
        _recordsVisible = false;
        _records = [];
    }

    public async Task OnFileChange(InputFileChangeEventArgs args)
    {
        try
        {
            var file = args.File;

            if (file != null)
            {
                string ext = Path.GetExtension(file.Name);

                if (ext != ".xlsx" && ext != ".csv")
                {
                    await Message.Error("Only .xlsx and .csv are allowed.", "Invalid Extension");
                    return;
                }

                var bytes = await file.GetBytes();

                _records = await TCatCodTradeInfoAppService.ProcessFile(file.Name, bytes);
                _recordsVisible = true;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}