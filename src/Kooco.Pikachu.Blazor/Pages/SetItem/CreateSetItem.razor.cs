using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp;
using System.Collections.Generic;
using Blazored.TextEditor;
using Microsoft.AspNetCore.Components;
using Kooco.Pikachu.Items;
using Blazorise.Components;

namespace Kooco.Pikachu.Blazor.Pages.SetItem
{
    public partial class CreateSetItem
    {
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
        private bool _isInitialized = false;

        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly NavigationManager _navigationManager;
        private readonly IItemAppService _itemAppService;
        private readonly ISetItemAppService _setItemAppService;

        public CreateSetItem(
            IUiMessageService uiMessageService,
            ImageContainerManager imageConainerManager,
            NavigationManager navigationManager,
            IItemAppService itemAppService,
            ISetItemAppService setItemAppService
            )
        {
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageConainerManager;
            _navigationManager = navigationManager;
            _itemAppService = itemAppService;
            _setItemAppService = setItemAppService;
        }

        protected override async Task OnInitializedAsync()
        {
            if (!_isInitialized)
            {
                ItemsList = await _itemAppService.GetItemsLookupAsync();
                _isInitialized = true;
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
                    confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                    if (confirmed)
                    {
                        CreateUpdateSetItemDto.Images = CreateUpdateSetItemDto.Images.Where(x => x.BlobImageName != blobImageName).ToList();
                        StateHasChanged();
                    }
                    else
                    {
                        throw new BusinessException(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
                    }
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

        protected virtual async Task CreateSetItemAsync()
        {
            try
            {
                //ValidateForm();
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

                await _setItemAppService.CreateAsync(CreateUpdateSetItemDto);
                _navigationManager.NavigateTo("/SetItem");
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(ex.Code.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        private void CancelToSetItem()
        {
            _navigationManager.NavigateTo("/SetItem");
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

    public class ItemDetailsModel
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemDescriptionTitle { get; set; }
        public bool IsSelected { get; set; } = false;
        public string ImageUrl { get; set; }
        public ItemDetailsModel()
        {

        }

        public ItemDetailsModel(
            Guid id,
            string itemName,
            string? itemDescription = null,
            string? itemDescriptionTitle = null,
            int quantity = 1,
            bool isSelected = false
            )
        {
            ItemId = id;
            ItemName = itemName;
            ItemDescription = itemDescription;
            ItemDescriptionTitle = itemDescriptionTitle;
            Quantity = quantity;
            IsSelected = isSelected;
        }
    }
}
