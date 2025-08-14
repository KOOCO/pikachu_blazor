using Blazorise.DataGrid;
using Kooco.Pikachu.LogisticsFeeManagements;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Blazorise;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantLogisticsFeeMangements
{
    public partial class LogisticsFeeOrderRecord
    {

        [Parameter] public Guid TenantId { get; set; }
        [Parameter] public Guid FileId { get; set; }

        // Properties
        private IReadOnlyList<TenantLogisticsFeeRecordDto> Records = new List<TenantLogisticsFeeRecordDto>();
        private List<TenantLogisticsFeeRecordDto> SelectedRows = new List<TenantLogisticsFeeRecordDto>();
        private List<Guid> SelectedFailedRecords = new();
        private List<Guid> RetryingRecords = new();

        private int TotalCount = 0;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private string FileName = "";
        private string TenantName = "";

        // Summary properties
        private int TotalRecords = 0;
        private int SuccessfulDeductions = 0;
        private int FailedDeductions = 0;

        // Retry properties
        private bool IsRetryingBatch = false;
        private Modal RetryResultsModal;
        private RetryBatchResult LastRetryResult;

        protected List<BreadcrumbItem> BreadcrumbItems = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadFileInfo();
            await SetBreadcrumbItemsAsync();
         
        }

        private async Task LoadFileInfo()
        {
            try
            {
                var fileImport = await LogisticsFeeAppService.GetFileImportAsync(FileId);
                FileName = fileImport.OriginalFileName;


                var tenant = await TenantAppService.GetAsync(TenantId);
                TenantName = tenant.Name;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task SetBreadcrumbItemsAsync()
        {
            //BreadcrumbItems.Add(new BreadcrumbItem(L["LogisticsManagement"], "/logistics-management"));
            //BreadcrumbItems.Add(new BreadcrumbItem(L["FeeManagement"], "/logistics-management"));
            //BreadcrumbItems.Add(new BreadcrumbItem(TenantName, $"/logistics-management/tenant/{TenantId}"));
            //BreadcrumbItems.Add(new BreadcrumbItem(FileName, $"/logistics-management/tenant/{TenantId}/file/{FileId}", true));
        }

        private async Task LoadData()
        {
            try
            {
                var input = new GetTenantLogisticsFeeRecordsInput
                {
                    SkipCount = (CurrentPage - 1) * PageSize,
                    MaxResultCount = PageSize,
                    TenantId = TenantId,
                    FileImportId = FileId
                };

                var result = await LogisticsFeeAppService.GetRecordsAsync(input);

                Records = result.Items;
                TotalCount = (int)result.TotalCount;

                // Update summary stats
                TotalRecords = (int)result.TotalCount;
                input.MaxResultCount = 1000;
                var recordsCount = await LogisticsFeeAppService.GetStatusRecordCount(input);
                SuccessfulDeductions = recordsCount.Item1;
                FailedDeductions = recordsCount.Item2;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private void RowSelectableHandler(TenantLogisticsFeeRecordDto rowSeleced)
        {
            if (rowSeleced.DeductionStatus == WalletDeductionStatus.Failed && !rowSeleced.IsSelected)
            {
                SelectedFailedRecords.Add(rowSeleced.Id);

            }
            else if (rowSeleced.DeductionStatus == WalletDeductionStatus.Failed && rowSeleced.IsSelected)
            {
                SelectedFailedRecords.Remove(rowSeleced.Id);

            }



        }

        private async Task OnDataGridReadData(DataGridReadDataEventArgs<TenantLogisticsFeeRecordDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await LoadData();
            await InvokeAsync(StateHasChanged);
        }

        private void SelectAllFailedRecords()
        {
            var failedRecords = Records.Where(r => r.DeductionStatus == WalletDeductionStatus.Failed).ToList();
            SelectedFailedRecords = failedRecords.Select(r => r.Id).ToList();
            StateHasChanged();
        }

        private async Task RetryIndividualRecord(Guid recordId)
        {
            RetryingRecords.Add(recordId);
            StateHasChanged();

            try
            {
                var result = await LogisticsFeeAppService.RetryRecordAsync(recordId);

                if (result.Success)
                {
                    await MessageService.Success(L["RecordRetriedSuccessfully"]);
                    await LoadData();
                }
                else
                {
                    await MessageService.Warning($"{L["RetryFailed"]}: {result.Reason}");
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
            finally
            {
                RetryingRecords.Remove(recordId);
                StateHasChanged();
            }
        }

        private async Task RetrySelectedFailedRecords()
        {
            if (!SelectedFailedRecords.Any())
            {
                await MessageService.Warning(L["NoRecordsSelected"]);
                return;
            }

            IsRetryingBatch = true;
            StateHasChanged();

            try
            {
                var input = new RetryBatchInput
                {
                    RecordIds = SelectedFailedRecords
                };

                LastRetryResult = await LogisticsFeeAppService.RetryBatchAsync(input);

                await RetryResultsModal.Show();
                await LoadData();

                SelectedFailedRecords.Clear();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
            finally
            {
                IsRetryingBatch = false;
                StateHasChanged();
            }
        }

        private async Task CloseRetryResultsModal()
        {
            await RetryResultsModal.Hide();
        }

        private bool IsRetryingRecord(Guid recordId)
        {
            return RetryingRecords.Contains(recordId);
        }

        // Helper methods
        private Color GetDeductionStatusColor(WalletDeductionStatus status)
        {
            return status switch
            {
                WalletDeductionStatus.Completed => Color.Success,
                WalletDeductionStatus.Failed => Color.Danger,
                WalletDeductionStatus.Pending => Color.Warning,
                _ => Color.Secondary
            };
        }

        private string GetDeductionStatusText(WalletDeductionStatus status)
        {
            return status switch
            {
                WalletDeductionStatus.Completed => "Successful",
                WalletDeductionStatus.Failed => "Failed",
                WalletDeductionStatus.Pending => "Processing",
                _ => "Unknown"
            };
        }

        private void NavigateToFileImportHistory()
        {
            NavigationManager.NavigateTo($"/logistics-management/tenant/{TenantId}");


        }
    }
}

