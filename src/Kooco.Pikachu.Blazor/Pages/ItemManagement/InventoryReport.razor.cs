﻿using Blazorise;
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
        string? FilterText = null;
        string Sorting = nameof(ItemDetailsDto.ItemName);
        bool Loading { get; set; } = true;
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
                Loading = true;
                int skipCount = PageIndex * PageSize;
                var result = await _itemDetailAppService.GetInventroyReport(new GetInventroyInputDto
                {
                    FilterText=FilterText,
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount
                });
                InventroyList = result.Items.ToList();
                Total = (int)result.TotalCount;
                Loading = false;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
                Loading = false;
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
                SkipCount = skipCount,
                FilterText= FilterText,
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
        async Task OnSearch()
        {
            PageIndex = 0;
            await UpdateItemList();
        }

    }
}
