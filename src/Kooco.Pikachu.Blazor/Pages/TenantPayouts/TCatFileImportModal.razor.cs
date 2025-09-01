using Blazorise;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.CodTradeInfos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.Extensions;

namespace Kooco.Pikachu.Blazor.Pages.TenantPayouts;

public partial class TCatFileImportModal
{
    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public EventCallback OnImportCompleted { get; set; }

    private List<TCatCodTradeInfoRecordDto> _records;
    private bool _recordsVisible;
    private FilePicker? _pickerRef;
    private bool _importing;

    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        try
        {
            var file = e.Files.FirstOrDefault();

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
                _pickerRef?.Clear();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
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
        _pickerRef?.Clear();
        StateHasChanged();
    }

    public void CloseModal()
    {
        _records = [];
        _pickerRef?.Clear();
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
}