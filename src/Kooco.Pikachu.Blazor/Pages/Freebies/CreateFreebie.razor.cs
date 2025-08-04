using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using Kooco.Pikachu.GroupBuys;
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

namespace Kooco.Pikachu.Blazor.Pages.Freebies
{

    public partial class CreateFreebie
    {
        private const int MaxTextCount = 60;
        private const int TotalMaxAllowedFiles = 10;
        private const int MaxAllowedFileSize = 1024 * 1024 * 10;
        private Blazored.TextEditor.BlazoredTextEditor ItemDescription;
        private List<KeyValueDto> GroupBuyList { get; set; } = new();
        private List<KeyValueDto> ProductList { get; set; } = new();
        public List<CreateImageDto> ImageList { get; set; }
        private FilePicker FilePicker { get; set; }
        private List<string> SelectedTexts { get; set; } = new();

        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieAppService _freebieAppService;
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IItemAppService _itemAppService;

        private FreebieCreateDto FreebieCreateDto { get; set; } = new();
        public CreateFreebie(
            IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IFreebieAppService freebieAppService,
            IGroupBuyAppService groupBuyAppService,
            IItemAppService itemAppService
            )
        {
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            _freebieAppService = freebieAppService;
            _groupBuyAppService = groupBuyAppService;
            _itemAppService = itemAppService;
            ImageList = new List<CreateImageDto>();
        }
        private void NavigateToList()
        {
            NavigationManager.NavigateTo("/Freebie/FreebieList");
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                GroupBuyList = await _groupBuyAppService.GetGroupBuyLookupAsync();
                ProductList = await _itemAppService.LookupAsync();
                FreebieCreateDto.FreebieOrderReach = FreebieOrderReach.MinimumAmount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("updateDropText");
            }
        }

        private string LocalizeFilePicker(string key, object[] args)
        {
            return L[key];
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
                    string newFileName = Path.ChangeExtension(
                          Path.GetRandomFileName(),
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
                if (FreebieCreateDto.ItemDescription.IsNullOrWhiteSpace() || FreebieCreateDto.ItemDescription == "<p><br></p>")
                {
                    await _uiMessageService.Error(L[PikachuDomainErrorCodes.ItemDescriptionIsRequired]);
                    return;
                }
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
            if (FreebieCreateDto.ApplyToAllProducts == false && !FreebieCreateDto.FreebieProducts.Any())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.SelectAtLeastOneProduct]);
            }
            if (!FreebieCreateDto.UnCondition && FreebieCreateDto.FreebieOrderReach is FreebieOrderReach.MinimumAmount && FreebieCreateDto.MinimumAmount is null)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.MinimumAmountReachCannotBeEmpty]);
            }
            if (!FreebieCreateDto.UnCondition && FreebieCreateDto.FreebieOrderReach is FreebieOrderReach.MinimumPiece && FreebieCreateDto.MinimumPiece is null)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.MinimumPieceCannotBeEmpty]);
            }
            if (!FreebieCreateDto.UnCondition && FreebieCreateDto.FreebieQuantity <= 0)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.GetCannotBeEmptyOrZero]);
            }
            if (FreebieCreateDto.UnCondition && FreebieCreateDto.FreebieQuantity <= 0)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.GetCannotBeEmptyOrZero]);
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
        Task OnProductCheckedValueChanged(bool value)
        {
            FreebieCreateDto.ApplyToAllProducts = value;
            return Task.CompletedTask;
        }
        Task OnUnconditionCheckedValueChanged(bool value)
        {
            FreebieCreateDto.UnCondition = value;
            if (FreebieCreateDto.UnCondition)
            {
                FreebieCreateDto.MinimumAmount = null;
                FreebieCreateDto.MinimumPiece = null;
                FreebieCreateDto.FreebieOrderReach = null;

            }
            else
            {
                FreebieCreateDto.FreebieQuantity = 0;


            }
            return Task.CompletedTask;
        }
    }

}
