using Kooco.Pikachu.Blazor.Pages.Components;
using Kooco.Pikachu.CodTradeInfos;
using Kooco.Pikachu.Extensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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
    private bool _processingFile;

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

    async Task FileInfoChanged(PikachuFileInfo fileInfo)
    {
        try
        {
            _records = [];
            if (fileInfo != null)
            {
                _processingFile = true;
                _records = await TCatCodTradeInfoAppService.ProcessFile(fileInfo.Name, fileInfo.FileBytes);
                _recordsVisible = true;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _processingFile = false;
        }
    }
}