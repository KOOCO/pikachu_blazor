using AutoMapper;
using Blazored.TextEditor;
using Blazorise;
using Blazorise.Components;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
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
        private const int MaxAllowedFilesPerUpload = 10;
        private const int TotalMaxAllowedFiles = 50;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private readonly List<string> ValidFileExtensions = new() { ".jpg", ".png", ".svg" };
        private BlazoredTextEditor QuillHtml;
        private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
        private string? SelectedAutoCompleteText { get; set; }
        private List<KeyValueDto> ItemsList { get; set; } = new();
        private bool IsAllSelected { get; set; } = false;
        private FilePicker FilePicker { get; set; }
        private CreateUpdateSetItemDto CreateUpdateSetItemDto { get; set; } = new();
        private List<ItemDetailsModel> ItemDetails { get; set; } = new();

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

        protected override async Task OnInitializedAsync()
        {
            try
            {
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
                    CreateUpdateSetItemDto.Images = CreateUpdateSetItemDto.Images.OrderBy(x => x.SortNo).ToList();

                    var itemDetails = ExistingItem.SetItemDetails.ToList();

                    itemDetails.ForEach(item =>
                    {
                        ItemDetails.Add(
                            new ItemDetailsModel(
                                item.ItemId, 
                                item.Item?.ItemName, 
                                item.Item?.ItemDescription, 
                                item.Item?.ItemDescriptionTitle, 
                                item.Quantity, 
                                false,
                                item.Item?.Images?.FirstOrDefault()?.ImageUrl
                                )
                            );
                    });

                    await QuillHtml.LoadHTMLContent(ExistingItem.Description);
                    ItemsList = await _itemAppService.GetItemsLookupAsync();

                }
                else
                {
                    NavigationManager.NavigateTo("/SetItem");
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
            }
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
                    if (file.Size > MaxAllowedFileSize)
                    {
                        count++;
                        await FilePicker.RemoveFile(file);
                        return;
                    }
                    string newFileName = Path.ChangeExtension(
                          Guid.NewGuid().ToString().Replace("-", ""),
                          Path.GetExtension(file.Name));
                    var stream = file.OpenReadStream(long.MaxValue);
                    try
                    {
                        var memoryStream = new MemoryStream();

                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

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
                    finally
                    {
                        stream.Close();
                    }
                }
                if (count > 0)
                {
                    await _uiMessageService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
            }
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

                    var item = await _itemAppService.GetAsync(id.Value);
                    ItemDetails.Add(new ItemDetailsModel
                        (
                            item.Id,
                            item.ItemName,
                            item.ItemDescription,
                            item.ItemDescriptionTitle
                        ));
                    ItemsList = ItemsList.Where(x => x.Id != id).ToList();
                    IsAllSelected = false;
                    ItemDetails.Where(x => x.ItemId == id).FirstOrDefault().ImageUrl = await _itemAppService.GetFirstImageUrlAsync(id.Value);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
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
                            Quantity = item.Quantity
                        });
                });

                CreateUpdateSetItemDto.Description = await QuillHtml.GetHTML();

                await _setItemAppService.UpdateAsync(EditingId, CreateUpdateSetItemDto);
                NavigationManager.NavigateTo("/SetItem");
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(ex.Code?.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        private void ValidateForm()
        {
            if (CreateUpdateSetItemDto.SetItemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
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
                ItemsList.Add(new KeyValueDto(item.ItemId, item.ItemName));
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
    }
}
