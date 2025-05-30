﻿using AutoMapper;
using Blazored.TextEditor;
using Blazorise;
using Blazorise.Components;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.SetItem
{
    public partial class EditSetItem
    {
        [Parameter]
        public string Id { get; set; }

        public Guid EditingId { get; set; }
        public SetItemDto ExistingItem { get; set; }
        private const int MaxTextCount = 60;
        private const int MaxBadgeTextCount = 4; //input max length
        private const int MaxAllowedFilesPerUpload = 10;
        private const int TotalMaxAllowedFiles = 50;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg", ".jpeg", ".webp" };
        private BlazoredTextEditor QuillHtml;
        private Autocomplete<ItemWithItemTypeDto, Guid?> AutocompleteField { get; set; }
        private string? SelectedAutoCompleteText { get; set; }
        private List<ItemWithItemTypeDto> ItemsList { get; set; } = new();
        private bool IsAllSelected { get; set; } = false;
        private FilePicker FilePicker { get; set; }
        private CreateUpdateSetItemDto CreateUpdateSetItemDto { get; set; } = new();
        private List<ItemDetailsModel> ItemDetails { get; set; } = new();
        private List<ItemBadgeDto> ItemBadgeList { get; set; } = [];
        private ItemBadgeDto NewItemBadge { get; set; } = new();
        private bool SelectOpen { get; set; } = false;

        private readonly ISetItemAppService _setItemAppService;
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IItemAppService _itemAppService;

        public EditSetItem(
            ISetItemAppService setItemAppService,
            IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IItemAppService itemAppService
            )
        {
            _setItemAppService = setItemAppService;
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            _itemAppService = itemAppService;
        }
        private void NavigateToItemList()
        {
            NavigationManager.NavigateTo("/SetItem");
        }
        protected override async Task OnAfterRenderAsync(bool isFirstRender)
        {
            if (isFirstRender)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("updateDropText");
                    var result = Guid.TryParse(Id, out Guid editingId);
                    if (result)
                    {
                        EditingId = editingId;
                        ExistingItem = await _setItemAppService.GetAsync(editingId, true);

                        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
                        // Create a new mapper
                        var mapper = config.CreateMapper();

                        // Map ItemDto to UpdateItemDto
                        CreateUpdateSetItemDto = mapper.Map<CreateUpdateSetItemDto>(ExistingItem);
                        CreateUpdateSetItemDto.ItemBadgeDto = new ItemBadgeDto { ItemBadge = ExistingItem?.SetItemBadge, ItemBadgeColor = ExistingItem?.SetItemBadgeColor };
                        CreateUpdateSetItemDto.Images = CreateUpdateSetItemDto.Images.OrderBy(x => x.SortNo).ToList();

                        var itemDetails = ExistingItem.SetItemDetails.ToList();

                        itemDetails.ForEach(item =>
                        {
                            var itemDetail =
                                new ItemDetailsModel(
                                    item.ItemId,
                                    item.Item?.ItemName,
                                    item.Item?.ItemDescription,
                                    item.Item?.ItemDescriptionTitle,
                                    item.Quantity,
                                    false,
                                    item.Item?.Images?.FirstOrDefault()?.ImageUrl
                                    );

                            itemDetail.Attribute1Value = item.Attribute1Value;
                            itemDetail.Attribute2Value = item.Attribute2Value;
                            itemDetail.Attribute3Value = item.Attribute3Value;

                            ItemDetails.Add(itemDetail);
                        });
                        await GetItemBadgeListAsync();
                        ItemsList = await _itemAppService.GetItemsLookupAsync();
                        await GetAttributesForSelectedItems();
                        await LoadHtmlContent();
                        StateHasChanged();
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/SetItem");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await _uiMessageService.Error(ex.GetType()?.ToString());
                }
            }
        }

        private async Task GetAttributesForSelectedItems()
        {
            var ids = ItemDetails.Select(i => i.ItemId).ToList();
            var items = await ItemAppService.GetItemsWithAttributesAsync(ids);
            ItemDetails.ForEach(itemDetail =>
            {
                var item = items.FirstOrDefault(x => x.Id == itemDetail.ItemId);
                if (item != null)
                {
                    itemDetail.Attribute1Values = item.ItemDetails.Where(x => x.Attribute1Value != null).Select(x => x.Attribute1Value).Distinct().ToList();
                    itemDetail.Attribute2Values = item.ItemDetails.Where(x => x.Attribute2Value != null).Select(x => x.Attribute2Value).Distinct().ToList();
                    itemDetail.Attribute3Values = item.ItemDetails.Where(x => x.Attribute3Value != null).Select(x => x.Attribute3Value).Distinct().ToList();
                }
            });
        }

        private async Task LoadHtmlContent()
        {
            await Task.Delay(2);
            await QuillHtml.LoadHTMLContent(ExistingItem.Description);
        }

        async Task OnFileUploadAsync(FileChangedEventArgs e)
        {
            if (e.Files.Length > MaxAllowedFilesPerUpload)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
                await FilePicker.Clear();
                return;
            }
            if (CreateUpdateSetItemDto.Images.Count > TotalMaxAllowedFiles)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.AlreadyUploadMaxAllowedFiles]);
                await FilePicker.Clear();
                return;
            }
            var count = 0;
            try
            {
                foreach (var file in e.Files)
                {
                    if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name).ToLower()))
                    {
                        await FilePicker.RemoveFile(file);
                        return;
                    }

                    string newFileName = Path.ChangeExtension(
                          Guid.NewGuid().ToString().Replace("-", ""),
                          Path.GetExtension(file.Name));

                    var bytes = await file.GetBytes();

                    var compressed = await ImageCompressorService.CompressAsync(bytes);

                    if (compressed.CompressedSize > MaxAllowedFileSize)
                    {
                        count++;
                        await FilePicker.RemoveFile(file);
                        return;
                    }

                    var url = await _imageContainerManager.SaveAsync(newFileName, compressed.CompressedBytes);

                    int sortNo = 0;
                    if (CreateUpdateSetItemDto.Images.Any())
                    {
                        sortNo = CreateUpdateSetItemDto.Images.Max(x => x.SortNo);
                    }

                    CreateUpdateSetItemDto.Images.Add(new CreateImageDto
                    {
                        Name = file.Name,
                        BlobImageName = newFileName,
                        ImageUrl = url,
                        ImageType = ImageType.Item,
                        SortNo = sortNo + 1
                    });

                    await FilePicker.Clear();
                }
                if (count > 0)
                {
                    await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                }
            }
            catch (Exception exc)
            {
                await HandleErrorAsync(exc);
            }
        }
        private string LocalizeFilePicker(string key, object[] args)
        {
            return L[key];
        }
        async Task DeleteImageAsync(string blobImageName)
        {
            try
            {
                var confirmed = await _uiMessageService.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
                if (confirmed)
                {
                    await _imageContainerManager.DeleteAsync(blobImageName);
                    await _setItemAppService.DeleteSingleImageAsync(EditingId, blobImageName);
                    CreateUpdateSetItemDto.Images = CreateUpdateSetItemDto.Images.Where(x => x.BlobImageName != blobImageName).ToList();
                    StateHasChanged();
                }
                else
                {
                    throw new BusinessException(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
            }
        }

        private async void OnSelectedValueChanged(Guid? id)
        {
            try
            {
                if (id != null)
                {
                    await AutocompleteField.Clear();

                    var item = await _itemAppService.GetAsync(id.Value, true);

                    var itemDetail = new ItemDetailsModel(
                            item.Id,
                            item.ItemName,
                            item.ItemDescription,
                            item.ItemDescriptionTitle,
                            itemDetails: item.ItemDetails.ToList()
                        );

                    itemDetail.Attribute1Values = item.ItemDetails.Where(x => x.Attribute1Value != null).Select(x => x.Attribute1Value).Distinct().ToList();
                    itemDetail.Attribute2Values = item.ItemDetails.Where(x => x.Attribute2Value != null).Select(x => x.Attribute2Value).Distinct().ToList();
                    itemDetail.Attribute3Values = item.ItemDetails.Where(x => x.Attribute3Value != null).Select(x => x.Attribute3Value).Distinct().ToList();

                    itemDetail.Attribute1Value = itemDetail.Attribute1Values.FirstOrDefault();
                    itemDetail.Attribute2Value = itemDetail.Attribute2Values.FirstOrDefault();
                    itemDetail.Attribute3Value = itemDetail.Attribute3Values.FirstOrDefault();

                    ItemDetails.Add(itemDetail);

                    ItemsList = ItemsList.Where(x => x.Id != id).ToList();
                    IsAllSelected = false;
                    ItemDetails.Where(x => x.ItemId == id).FirstOrDefault().ImageUrl = await _itemAppService.GetFirstImageUrlAsync(id.Value);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        protected virtual async Task UpdateSetItemAsync()
        {
            try
            {
                ValidateForm();
                CreateUpdateSetItemDto.SetItemDetails = new();
                ItemDetails.ForEach(item =>
                {
                    CreateUpdateSetItemDto.SetItemDetails.Add(
                        new CreateUpdateSetItemDetailsDto
                        {
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            Attribute1Value = item.Attribute1Value,
                            Attribute2Value = item.Attribute2Value,
                            Attribute3Value = item.Attribute3Value
                        });
                });

                CreateUpdateSetItemDto.Description = await QuillHtml.GetHTML();

                CreateUpdateSetItemDto.SetItemMainImageURL = CreateUpdateSetItemDto.Images.FirstOrDefault()?.ImageUrl;

                await _setItemAppService.UpdateAsync(EditingId, CreateUpdateSetItemDto);
                NavigationManager.NavigateTo("/SetItem");
            }
            catch (BusinessException ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.Code?.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        private void ValidateForm()
        {
            if (CreateUpdateSetItemDto.SetItemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
            }

            if (!CreateUpdateSetItemDto.ItemStorageTemperature.HasValue)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemStorageTemperatureCannotBeNull]);
            }
        }

        private void CancelToSetItem()
        {
            NavigationManager.NavigateTo("/SetItem");
        }

        private void RemoveSelectedItems()
        {
            var selected = ItemDetails.Where(x => x.IsSelected).ToList();
            selected.ForEach(item =>
            {
                ItemDetails.Remove(item);
                ItemsList.Add(new ItemWithItemTypeDto(item.ItemId, item.ItemName, ItemType.Item));
            });
            IsAllSelected = false;
        }

        private void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool)e.Value;
            ItemDetails.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }

        private void AddItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItemBadge?.ItemBadge))
            {
                var newItemBadge = new ItemBadgeDto { ItemBadge = NewItemBadge.ItemBadge, ItemBadgeColor = NewItemBadge.ItemBadgeColor };
                ItemBadgeList.Add(newItemBadge);
                CreateUpdateSetItemDto.ItemBadgeDto = newItemBadge;
                NewItemBadge = new();
            }
        }

        async Task DeleteItemBadge(ItemBadgeDto itemBadge)
        {
            try
            {
                var confirmation = await Message.Confirm(L["AreYouSureToDeleteThisBadge"]);
                if (confirmation)
                {
                    await _itemAppService.DeleteItemBadgeAsync(itemBadge);
                    await GetItemBadgeListAsync();
                }

                SelectOpen = true;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        async Task GetItemBadgeListAsync()
        {
            ItemBadgeList = await _itemAppService.GetItemBadgesAsync();
        }
    }
}
