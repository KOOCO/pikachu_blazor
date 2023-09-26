using Blazorise;
using Blazorise.Components;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.Freebies
{

    public partial class CreateFreebie
    {
        private const int MaxTextCount = 60;
        private const int TotalMaxAllowedFiles = 10;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private Blazored.TextEditor.BlazoredTextEditor ItemDescription;
        private List<KeyValueDto> GroupBuyList { get; set; } = new();
        public List<CreateImageDto> ImageList { get; set; }
        private FilePicker FilePicker { get; set; }
        private List<string> SelectedTexts { get; set; } = new();

        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieAppService _freebieAppService;

        private FreebieCreateDto FreebieCreateDto { get; set; } = new();
        public CreateFreebie(
            IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IFreebieAppService freebieAppService
            )
        {
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            _freebieAppService = freebieAppService;
            ImageList = new List<CreateImageDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                GroupBuyList = await _freebieAppService.GetGroupBuyLookupAsync();
                FreebieCreateDto.FreebieOrderReach = FreebieOrderReach.MinimumAmount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task OnImageUploadAsync(FileChangedEventArgs e)
        {
            if (e.Files.Length > TotalMaxAllowedFiles)
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.FilesExceedMaxAllowedPerUpload]);
                await FilePicker.Clear();
                return;
            }
            if (ImageList.Count > TotalMaxAllowedFiles)
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
                    var stream = file.OpenReadStream(long.MaxValue);
                    try
                    {
                        var memoryStream = new MemoryStream();

                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                        int sortNo = ImageList.LastOrDefault()?.SortNo ?? 0;

                        ImageList.Add(new CreateImageDto
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
                    ImageList = ImageList.Where(x => x.BlobImageName != blobImageName).ToList();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
            }
        }

        protected virtual async Task CreateFreebieAsync()
        {
            try
            {
                FreebieCreateDto.ItemDescription = await ItemDescription.GetHTML();
                ValidateForm();

                FreebieCreateDto.Images = ImageList;

                await _freebieAppService.CreateAsync(FreebieCreateDto);
                CancelToFreebieList();
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
            if (FreebieCreateDto.ItemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
            }
            if (FreebieCreateDto.ItemName.Length > MaxTextCount)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.InvalidItemName]);
            }
            if (!ImageList.Any())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SelectAtLeastOneImage]);
            }
            if (FreebieCreateDto.ItemDescription.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemDetailsCannotBeEmpty]);
            }
            if (FreebieCreateDto.ApplyToAllGroupBuy == false && !FreebieCreateDto.FreebieGroupBuys.Any())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SelectAtLeastOneGroupBuy]);
            }
            if (FreebieCreateDto.FreebieAmount <= 0)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.FreebieAmountCannotBeZero]);
            }
        }

        private void CancelToFreebieList()
        {
            NavigationManager.NavigateTo("Freebie/FreebieList");
        }

        Task OnGroupbuyCheckedValueChanged(bool value)
        {
            FreebieCreateDto.ApplyToAllGroupBuy = value;
            return Task.CompletedTask;
        }

        Task OnUnconditionCheckedValueChanged(bool value)
        {
            FreebieCreateDto.UnCondition = value;

            return Task.CompletedTask;
        }
    }

}
