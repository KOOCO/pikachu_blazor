using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp;
using System.Linq;
using Microsoft.AspNetCore.Components;
using AutoMapper;
using Kooco.Pikachu.FreeBies.Dtos;
using Blazored.TextEditor;

namespace Kooco.Pikachu.Blazor.Pages.Freebies
{
    public partial class EditFreebie
    {
        [Parameter]
        public string Id { get; set; }
        private FreebieDto ExistingItem { get; set; }
        private const int MaxTextCount = 60;
        private const int TotalMaxAllowedFiles = 10;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private BlazoredTextEditor ItemDescription;
        private List<KeyValueDto> GroupBuyList { get; set; } = new();
        private FilePicker FilePicker { get; set; }
        private List<string> SelectedTexts { get; set; } = new();
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieAppService _freebieAppService;
        private UpdateFreebieDto UpdateFreebieDto = new();
        public Guid EditingId { get; private set; }

        public EditFreebie(
            IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IFreebieAppService freebieAppService
            )
        {
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            _freebieAppService = freebieAppService;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                EditingId = Guid.Parse(Id);

                ExistingItem = await _freebieAppService.GetAsync(EditingId, true);

                var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
                // Create a new mapper
                var mapper = config.CreateMapper();

                // Map ItemDto to UpdateItemDto
                UpdateFreebieDto = mapper.Map<UpdateFreebieDto>(ExistingItem);
                UpdateFreebieDto.Images = UpdateFreebieDto.Images.OrderBy(x => x.SortNo).ToList();

                await LoadHtmlContent();
                GroupBuyList = await _freebieAppService.GetGroupBuyLookupAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        private async Task LoadHtmlContent()
        {
            await Task.Delay(1);
            await ItemDescription.LoadHTMLContent(ExistingItem.ItemDescription);
        }

        async Task OnImageUploadAsync(FileChangedEventArgs e)
        {
            if (e.Files.Length > TotalMaxAllowedFiles)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
                await FilePicker.Clear();
                return;
            }
            if (UpdateFreebieDto.Images?.Count > TotalMaxAllowedFiles)
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
                    if (file.Size > MaxAllowedFileSize)
                    {
                        count++;
                        await FilePicker.RemoveFile(file);
                        return;
                    }
                    string newFileName = Path.ChangeExtension(
                          Path.GetRandomFileName(),
                          Path.GetExtension(file.Name));
                    var stream = file.OpenReadStream();
                    try
                    {
                        var memoryStream = new MemoryStream();

                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                        int sortNo = UpdateFreebieDto.Images.LastOrDefault()?.SortNo ?? 0;

                        UpdateFreebieDto.Images.Add(new CreateImageDto
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
                    await _freebieAppService.DeleteSingleImageAsync(EditingId, blobImageName);
                    UpdateFreebieDto.Images = UpdateFreebieDto.Images.Where(x => x.BlobImageName != blobImageName).ToList();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
            }
        }

        protected virtual async Task UpdateFreebieAsync()
        {
            try
            {
                UpdateFreebieDto.ItemDescription = await ItemDescription.GetHTML();

                ValidateForm();

                await _freebieAppService.UpdateAsync(EditingId, UpdateFreebieDto);

                CancelToFreebieList();
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
            if (UpdateFreebieDto.ItemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
            }
            if (UpdateFreebieDto.ItemName.Length > MaxTextCount)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.InvalidItemName]);
            }
            if (!UpdateFreebieDto.Images.Any())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SelectAtLeastOneImage]);
            }
            if (UpdateFreebieDto.ItemDescription.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemDetailsCannotBeEmpty]);
            }
            if (UpdateFreebieDto.ApplyToAllGroupBuy == false && !UpdateFreebieDto.FreebieGroupBuys.Any())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SelectAtLeastOneGroupBuy]);
            }
        }

        private void CancelToFreebieList()
        {
            NavigationManager.NavigateTo("Freebie/FreebieList");
        }

        Task OnGroupbuyCheckedValueChanged(bool value)
        {
            UpdateFreebieDto.ApplyToAllGroupBuy = value;

            return Task.CompletedTask;
        }

        Task OnUnconditionCheckedValueChanged(bool value)
        {
            UpdateFreebieDto.UnCondition = value;

            return Task.CompletedTask;
        }
    }
}
