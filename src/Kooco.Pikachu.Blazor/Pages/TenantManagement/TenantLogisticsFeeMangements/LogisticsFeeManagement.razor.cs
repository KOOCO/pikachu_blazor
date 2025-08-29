using Blazorise.DataGrid;
using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsFeeManagements;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantLogisticsFeeMangements
{
    public partial class LogisticsFeeManagement
    {

        // Properties
        private IReadOnlyList<TenantLogisticsFeeDto> TenantList = new List<TenantLogisticsFeeDto>();
        private int TotalCount = 0;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private string? Filter = "";
        private string? StatusFilter = "";
        private bool loading = true;
        // Modal properties
        private Modal ImportModal;
        private Validations ImportValidations;
        private FileEdit FileEditRef;
        private LogisticsFileType SelectedFileType = LogisticsFileType.ECPay;
        private IFileEntry SelectedFile;
        private string SelectedFileName = "";
        private int UploadProgress = 0;
        private bool IsProcessing = false;
        private bool AutoDeduct = true;
        private bool SendNotifications = true;

        // Breadcrumb
        protected List<BreadcrumbItem> BreadcrumbItems = new();

        protected override async Task OnInitializedAsync()
        {

            await LoadData();
        }

        private async Task SetBreadcrumbItemsAsync()
        {

            //BreadcrumbItems.Add(new BreadcrumbItem(L["LogisticsManagement"]));
            //BreadcrumbItems.Add(new BreadcrumbItem(L["FeeManagement"], "/logistics-management", true));
        }

        private async Task LoadData()
        {
            try
            {
                loading = true;
                var result = await LogisticsFeeAppService.GetTenantSummariesAsync(
                    skipCount: (CurrentPage - 1) * PageSize,
                    maxResultCount: PageSize);

                TenantList = result.Items.Select(MapToTenantDto).ToList();
                TotalCount = (int)result.TotalCount;
                loading = false;
            }
            catch (Exception ex)
            {
                loading = false;
                await HandleErrorAsync(ex);
            }
        }

        private async Task OnDataGridReadData(DataGridReadDataEventArgs<TenantLogisticsFeeDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await LoadData();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnSearchKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                CurrentPage = 1;
                await LoadData();
            }
        }

        private void ViewTenantDetails(Guid tenantId)
        {
            NavigationManager.NavigateTo($"/logistics-management/tenant/{tenantId}");
        }

        private async Task OpenImportModal()
        {
            await ImportModal.Show();
        }

        private async Task CloseImportModal()
        {
            await ImportModal.Hide();
            ResetImportForm();
        }

        private void ResetImportForm()
        {
            SelectedFile = null;
            SelectedFileName = "";
            UploadProgress = 0;
            IsProcessing = false;
            AutoDeduct = true;
            SendNotifications = true;
            ImportValidations?.ClearAll();
        }

        private async Task OnFileChanged(FileChangedEventArgs e)
        {
            try
            {
                SelectedFile = e.Files.FirstOrDefault();
                if (SelectedFile != null)
                {
                    SelectedFileName = SelectedFile.Name;
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                await MessageService.Error(L["FileSelectionError"]);
            }
        }

        private Task OnFileWritten(FileWrittenEventArgs e)
        {
            UploadProgress = 100;
            return InvokeAsync(StateHasChanged);
        }

        private Task OnFileProgressed(FileProgressedEventArgs e)
        {
            UploadProgress = (int)(e.Percentage);
            return InvokeAsync(StateHasChanged);
        }

        private void ValidateFile(ValidatorEventArgs e)
        {
            var file = SelectedFile;
            if (file == null)
            {
                e.Status = ValidationStatus.Error;
                e.ErrorText = L["FileIsRequired"];
                return;
            }

            var allowedExtensions = new[] { ".csv", ".xlsx" };
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                e.Status = ValidationStatus.Error;
                e.ErrorText = L["OnlyCSVAndXLSXSupported"];
                return;
            }

            e.Status = ValidationStatus.Success;
        }

        private async Task ProcessUpload()
        {
            if (await ImportValidations.ValidateAll())
            {
                loading = true;
                IsProcessing = true;
                try
                {
                    await using var stream = SelectedFile.OpenReadStream(10 * 1024 * 1024);
                    var ct = string.IsNullOrWhiteSpace(SelectedFile.Type) ? "application/octet-stream" : SelectedFile.Type;

                    var result = await LogisticsFeeAppService.UploadFileAsync(
                           new RemoteStreamContent(stream, SelectedFile.Name, ct, readOnlyLength: SelectedFile.Size),
                           SelectedFileType, isMailSend: SendNotifications);
                    await MessageService.Success(L["FileUploadedSuccessfully"]);
                    await CloseImportModal();
                    var arg = new LogisticsFeeProcessingJobArgs
                    {
                        BatchId = result.BatchId,
                        IsMailSend = SendNotifications
                    };
                 

                    await LogisticsFeeProcessingJob.ExecuteAsync(arg);
                    await LoadData();
                }
                catch (Exception ex)
                {
                    loading = false;
                    await HandleErrorAsync(ex);
                }
                finally
                {

                    IsProcessing = false;
                    loading = false;
                }
            }
        }

        // Helper methods
        private string GetBalanceColorClass(decimal balance)
        {
            return balance < 100 ? "text-danger" : balance < 500 ? "text-warning" : "text-success";
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "Failed" => Color.Danger,
                "Success" => Color.Success,
                "Processing" => Color.Warning,
                _ => Color.Secondary
            };
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "Failed" => "FailedAutoDeduction",
                "Success" => "AutoDeductionSuccess",
                "Processing" => "Processing",
                _ => "Unknown"
            };
        }

        private TenantLogisticsFeeDto MapToTenantDto(TenantLogisticsFeeFileProcessingSummaryDto summary)
        {
            return new TenantLogisticsFeeDto
            {
                TenantId = summary.TenantId,
                TenantName = summary.TenantName,
                WalletId = summary.WalletId,
                WalletBalance = summary.WalletBalance,
                RecentStatus = summary.TenantFailedRecords == 0 ? "Success" : "Failed",
                LastUpdated = summary.ProcessedAt,
                HasFailedRecords = summary.TenantFailedRecords > 0
            };
        }

        private IFormFile CreateFormFileFromStream(Stream stream, string fileName, string contentType)
        {
            // Implementation to create IFormFile from stream
            // This would need to be implemented based on your specific needs
            return null;
        }

        // DTOs
        public class TenantLogisticsFeeDto
        {
            public Guid TenantId { get; set; }
            public string TenantName { get; set; }
            public Guid WalletId { get; set; }
            public decimal WalletBalance { get; set; }
            public string RecentStatus { get; set; }
            public DateTime LastUpdated { get; set; }
            public bool HasFailedRecords { get; set; }
        }
    }
}

