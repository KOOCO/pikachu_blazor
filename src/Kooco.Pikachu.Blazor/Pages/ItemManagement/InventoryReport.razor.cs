using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using System.Linq;
using Volo.Abp.AspNetCore.Components.Messages;
using Microsoft.JSInterop;
using System.IO;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class InventoryReport
    {
        public List<ItemDetailsDto> InventroyList;
        int PageIndex = 1;
        int PageSize = 10;
        int Total = 0;
        string Sorting = nameof(ItemDetailsDto.ItemName);

        private readonly IUiMessageService _uiMessageService;
        private readonly IItemDetailsAppService _itemDetailAppService;

        public InventoryReport(
            IItemDetailsAppService itemDetailAppService,
            IUiMessageService messageService
            )
        {
            _itemDetailAppService = itemDetailAppService;
            _uiMessageService = messageService;

            InventroyList = new List<ItemDetailsDto>();
        }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ItemDetailsDto> e)
        {
            PageIndex = e.Page - 1;
            await UpdateItemList();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateItemList()
        {
            try
            {
                int skipCount = PageIndex * PageSize;
                var result = await _itemDetailAppService.GetListAsync(new PagedAndSortedResultRequestDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount
                });
                InventroyList = result.Items.ToList();
                Total = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
            }
        }
        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }

        async Task DownloadExcel() {
            int skipCount = PageIndex * PageSize;
          var input=  new InventroyExcelDownloadDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount
            };
            var remoteStreamContent = await _itemDetailAppService.GetListAsExcelFileAsync(input);
            using (var responseStream = remoteStreamContent.GetStream())
            {
                // Create Excel file from the stream
                using (var memoryStream = new MemoryStream())
                {
                    await responseStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Convert MemoryStream to byte array
                    var excelData = memoryStream.ToArray();

                    // Trigger the download using JavaScript interop
                    await JSRuntime.InvokeVoidAsync("downloadFile", new
                    {
                        ByteArray = excelData,
                        FileName = "InventoryData.xlsx",
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    });
                }
            }


        }

    }
}
