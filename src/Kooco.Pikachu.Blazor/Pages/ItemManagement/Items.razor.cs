﻿using Blazorise;
using Blazorise.Components;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement
{
    public partial class Items(
        IItemAppService itemAppService,
        IUiMessageService messageService
            )
    {
        public GetItemListDto Filters { get; set; } = new();
        public List<ItemListDto> ItemList { get; set; } = [];
        public List<KeyValueDto> ItemLookup { get; set; } = [];
        bool Loading { get; set; } = true;
        Autocomplete<KeyValueDto, Guid?> Autocomplete { get; set; }

        public bool IsAllSelected = false;
        int PageIndex = 1;
        int Total = 0;
        private readonly IUiMessageService _uiMessageService = messageService;
        private readonly IItemAppService _itemAppService = itemAppService;

        public bool IsCardView { get; set; } = false;

        // Dictionary to store the first image URL for each item
        public Dictionary<Guid, string> ItemImageUrls { get; set; } = new Dictionary<Guid, string>();

        // Image fit style - true for cover, false for contain
        public bool UseImageCover { get; } = true;
        private bool ShowImportModal = false;
        private bool UploadInProgress = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ItemLookup = await _itemAppService.GetAllItemsLookupAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ItemListDto> e)
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
                Filters.SkipCount = PageIndex * Filters.MaxResultCount;
                var result = await _itemAppService.GetItemsListAsync(Filters);
                ItemList = result.Items.ToList();
                Total = (int)result.TotalCount;

                // If card view is active, load images for the updated item list
                if (IsCardView)
                {
                    await LoadItemImagesAsync();
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                Loading = false;
            }
        }

        public async Task FilterAsync()
        {
            Filters.SkipCount = 0;
            await UpdateItemList();
        }

        public async Task ResetAsync()
        {
            Filters = new();
            await Autocomplete?.Clear();
            await UpdateItemList();
        }
        private async Task CopyAsync()
        {

            var id = ItemList.Where(x => x.IsSelected == true).Select(x => x.Id).FirstOrDefault();

            var copy = await _itemAppService.CopyAysnc(id);
            NavigationManager.NavigateTo("/Items/Edit/" + copy.Id);


        }
        public async Task OnPageSizeChanged(int value)
        {
            Filters.MaxResultCount = value;
            await UpdateItemList();
        }

        public async Task OnItemAvaliablityChange(Guid id)
        {
            try
            {
                Loading = true;
                var item = ItemList.Where(x => x.Id == id).First();
                item.IsItemAvaliable = !item.IsItemAvaliable;
                await _itemAppService.ChangeItemAvailability(id);
                await UpdateItemList();
                await InvokeAsync(StateHasChanged);
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(ex.Code.ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                Loading = false;
            }
        }
        private void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool?)e?.Value ?? false;
            ItemList.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }
        private async Task DeleteSelectedAsync()
        {
            try
            {
                var itemIds = ItemList.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                if (itemIds.Count > 0)
                {
                    var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                    if (confirmed)
                    {
                        Loading = true;
                        await _itemAppService.DeleteManyItemsAsync(itemIds);
                        await UpdateItemList();
                        IsAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                Loading = false;
            }
        }
        public void CreateNewItem()
        {
            NavigationManager.NavigateTo("Items/New");
        }
        public void OnEditItem(ItemListDto e)
        {
            var id = e.Id;
            NavigationManager.NavigateTo($"Items/Edit/{id}");
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Filters.Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }

        private async void ToggleViewMode(bool isCardView)
        {
            IsCardView = isCardView;

            // If switching to card view, load images for all items
            if (isCardView)
            {
                await LoadItemImagesAsync();
            }

            StateHasChanged();
        }
        async Task DownloadExcel()
        {
            try
            {
              
                var remoteStreamContent = await _itemAppService.ExportItemListToExcelAsync(ItemList.Where(x=>x.IsSelected).Select(x=>x.Id).ToList());
                using var responseStream = remoteStreamContent.GetStream();
                // Create Excel file from the stream
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Convert MemoryStream to byte array
                var excelData = memoryStream.ToArray();

                // Trigger the download using JavaScript interop
                await JSRuntime.InvokeVoidAsync("downloadFile", new
                {
                    ByteArray = excelData,
                    remoteStreamContent.FileName,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
            }
            
        }
        private async Task LoadItemImagesAsync()
        {
            foreach (var item in ItemList)
            {
                if (!ItemImageUrls.ContainsKey(item.Id))
                {
                    var imageUrl = await _itemAppService.GetFirstImageUrlAsync(item.Id);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        ItemImageUrls[item.Id] = imageUrl;
                    }
                }
            }
        }
        private async Task OnFileSelected(InputFileChangeEventArgs e)
        {
            try
            {
                UploadInProgress = true;

                var file = e.File;
                if (file == null)
                {
                    await _uiMessageService.Error("Please select a file.");
                    return;
                }

                var stream = file.OpenReadStream(long.MaxValue);
                var remoteFile = new RemoteStreamContent(stream, file.Name, file.ContentType);

                await _itemAppService.ImportItemsFromExcelAsync(remoteFile);

                await _uiMessageService.Success("Items imported successfully.");
                ShowImportModal = false;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error($"Error: {ex.Message}");
            }
            finally
            {
                UploadInProgress = false;
                await UpdateItemList();

            }
        }

        private async Task DownloadTemplate()
        {
            await JSRuntime.InvokeVoidAsync("open", "Templates/ItemTemplate.xlsx", "_blank");

        }
    }
}

