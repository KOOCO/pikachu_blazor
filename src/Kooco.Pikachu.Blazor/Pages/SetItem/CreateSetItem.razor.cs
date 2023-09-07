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
        private bool IsAllSelected { get; set; } = false;

        private FilePicker FilePicker { get; set; }
        private CreateUpdateSetItemDto CreateUpdateSetItemDto { get; set; } = new();

        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly NavigationManager _navigationManager;

        public CreateSetItem(
            IUiMessageService uiMessageService,
            ImageContainerManager imageConainerManager,
            NavigationManager navigationManager
            )
        {
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageConainerManager;
            _navigationManager = navigationManager;
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
                    var stream = file.OpenReadStream();
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

        protected virtual async Task CreateSetItemAsync()
        {
            //try
            //{
            //    ValidateForm();
            //    GenerateAttributesForItemDetails();

            //    CreateItemDto.ItemDetails = ItemDetailsList;
            //    CreateItemDto.ItemDescription = await QuillHtml.GetHTML();
            //    CreateItemDto.ItemTags = string.Join(",", ItemTags);
            //    await _itemAppService.CreateAsync(CreateItemDto);
            //    NavigationManager.NavigateTo("Items");
            //}
            //catch (BusinessException ex)
            //{
            //    await _uiMessageService.Error(ex.Code.ToString());
            //}
            //catch (Exception ex)
            //{
            //    await _uiMessageService.Error(ex.GetType().ToString());
            //}
        }

        private void CancelToSetItem()
        {
            _navigationManager.NavigateTo("/SetItem");
        }

        private void HandleSelectAllChange()
        {

        }
    }
}
