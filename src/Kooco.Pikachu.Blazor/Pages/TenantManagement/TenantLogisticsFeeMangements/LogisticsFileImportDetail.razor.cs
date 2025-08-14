using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsFeeManagements;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components.Web;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantLogisticsFeeMangements
{
    public partial class LogisticsFileImportDetail
    {


        [Parameter] public Guid TenantId { get; set; }
        private FilePicker FilePickerRef;
        // Properties
        private IReadOnlyList<LogisticsFeeFileImportDto> FileImports = new List<LogisticsFeeFileImportDto>();
        private int TotalCount = 0;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private string Filter = "";
        private string StatusFilter = "";
        private string TenantName = "";

        // Summary properties
        private decimal CurrentBalance = 0;
        private int TotalFilesProcessed = 0;
        private int FailedBatches = 0;

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

        protected List<BreadcrumbItem> BreadcrumbItems = new();

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await LoadTenantInfo();
            await LoadData();
        }

        private async Task SetBreadcrumbItemsAsync()
        {
            //BreadcrumbItems.Add(new BreadcrumbItem(L["LogisticsManagement"], "/logistics-management"));
            //BreadcrumbItems.Add(new BreadcrumbItem(L["FeeManagement"], "/logistics-management"));
            //BreadcrumbItems.Add(new BreadcrumbItem(TenantName, $"/logistics-management/tenant/{TenantId}", true));
        }

        private async Task LoadTenantInfo()
        {
            try
            {
                var summaries = (await LogisticsFeeAppService.GetTenantSummariesAsync(TenantId)).Items?.FirstOrDefault();
                if (summaries != null)
                {
                    var summary = summaries;
                    TenantName = summary.TenantName;
                    CurrentBalance = summary.WalletBalance;
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task LoadData()
        {
            try
            {
                var input = new GetLogisticsFeeFileImportsInput
                {
                    SkipCount = (CurrentPage - 1) * PageSize,
                    MaxResultCount = PageSize,
                    Filter = Filter,
                    Status = GetStatusEnum(StatusFilter)
                };

                var result = await LogisticsFeeAppService.GetFileImportsAsync(input);

                FileImports = result.Items;
                TotalCount = (int)result.TotalCount;

                // Update summary stats
                TotalFilesProcessed = (int)result.TotalCount;
                FailedBatches = result.Items.Sum(f => f.FailedRecords);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task OnDataGridReadData(DataGridReadDataEventArgs<LogisticsFeeFileImportDto> e)
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

        private void ViewFileDetails(Guid fileId)
        {
            NavigationManager.NavigateTo($"/logistics-management/tenant/{TenantId}/file/{fileId}");
        }
        private void NavigateToLogisticsFeeManagement()
        {
            NavigationManager.NavigateTo("/logistics-management");


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
                IsProcessing = true;
                try
                {
                    await using var stream = SelectedFile.OpenReadStream(10 * 1024 * 1024);
                    var ct = string.IsNullOrWhiteSpace(SelectedFile.Type) ? "application/octet-stream" : SelectedFile.Type;

                    await LogisticsFeeAppService.UploadFileAsync(
                        new RemoteStreamContent(stream, SelectedFile.Name, ct, readOnlyLength: SelectedFile.Size),
                        SelectedFileType, SendNotifications);

                    await MessageService.Success(L["FileUploadedSuccessfully"]);
                    await CloseImportModal();
                    await LoadData();
                }
                catch (Exception ex)
                {
                    await HandleErrorAsync(ex);
                }
                finally
                {
                    IsProcessing = false;
                    for (int i = 0; i < 3; i++)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        await LoadData();
                    }
                }
            }
        }

        // Helper methods
        private TextColor GetBalanceTextColor(decimal balance)
        {
            return balance < 100 ? TextColor.Danger : balance < 500 ? TextColor.Warning : TextColor.Success;
        }

        private IconName GetFileIcon(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".csv" => IconName.File,
                ".xlsx" => IconName.File,
                _ => IconName.File
            };
        }

        private Color GetStatusColor(LogisticsFeeFileImportDto item)
        {
            if (item.SuccessfulRecords == item.TotalRecords)
            {
                return Color.Success;
            }
            else if (item.FailedRecords == item.TotalRecords)
            {
                return Color.Danger;
            }
            else if (item.SuccessfulRecords < item.TotalRecords)
            {

                return Color.Info;
            }
            else
            {
                return Color.Warning;
            }

        }

        private string GetStatusText(LogisticsFeeFileImportDto item)
        {
            if (item.SuccessfulRecords == item.TotalRecords)
            {
                return "BatchSuccess";
            }
            else if (item.FailedRecords == item.TotalRecords)
            {
                return "BatchFailed";
            }
            else if (item.SuccessfulRecords < item.TotalRecords)
            {

                return "PartialSuccess";
            }
            else
            {
                return "Processing";
            }

        }



        private Color GetProgressColor(decimal successRate)
        {
            return successRate switch
            {
                >= 80 => Color.Success,
                >= 60 => Color.Warning,
                _ => Color.Danger
            };
        }

        private FileProcessingStatus? GetStatusEnum(string statusFilter)
        {
            return statusFilter switch
            {
                "success" => FileProcessingStatus.BatchSuccess,
                "failed" => FileProcessingStatus.BatchFailed,
                "processing" => FileProcessingStatus.Processing,
                _ => null
            };
        }

        private IFormFile CreateFormFileFromStream(Stream stream, string fileName, string contentType)
        {
            // Implementation needed based on your specific IFormFile implementation
            return null;
        }

    }
}
