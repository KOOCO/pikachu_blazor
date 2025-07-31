using AutoMapper;
using Blazored.TextEditor;
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
using Kooco.Pikachu.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.Freebies;

public partial class EditFreebie
{
    #region Inject
    [Parameter]
    public string Id { get; set; }
    private FreebieDto ExistingItem { get; set; }
    private const int MaxTextCount = 60;
    private const int TotalMaxAllowedFiles = 10;
    private const int MaxAllowedFileSize = 1024 * 1024 * 10;
    private BlazoredTextEditor ItemDescription;
    private List<KeyValueDto> GroupBuyList { get; set; } = new();
    private List<KeyValueDto> ProductList { get; set; } = new();
    private FilePicker FilePicker { get; set; }
    private List<string> SelectedTexts { get; set; } = new();
    private List<string> SelectedProductTexts { get; set; } = new();
    private readonly IUiMessageService _uiMessageService;
    private readonly ImageContainerManager _imageContainerManager;
    private readonly IFreebieAppService _freebieAppService;
    private readonly IGroupBuyAppService _groupBuyAppService;
    private readonly IItemAppService _itemAppService;

    private UpdateFreebieDto UpdateFreebieDto = new();
    public Guid EditingId { get; private set; }

    int freebieAmount = 0;
    #endregion

    #region Constructor
    public EditFreebie(
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
    }
    #endregion

    #region Methods
    private void NavigateToList()
    {
        NavigationManager.NavigateTo("/Freebie/FreebieList");
    }
    protected override async Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (isFirstRender)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("updateDropText");
                EditingId = Guid.Parse(Id);

                ExistingItem = await _freebieAppService.GetAsync(EditingId, true);

                var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
                // Create a new mapper
                var mapper = config.CreateMapper();

                // Map ItemDto to UpdateItemDto
                UpdateFreebieDto = mapper.Map<UpdateFreebieDto>(ExistingItem);

                freebieAmount = Convert.ToInt32(UpdateFreebieDto.FreebieAmount);

                UpdateFreebieDto.Images = UpdateFreebieDto.Images.OrderBy(x => x.SortNo).ToList();

                GroupBuyList = await _groupBuyAppService.GetGroupBuyLookupAsync();
                ProductList = await _itemAppService.LookupAsync();
                await LoadHtmlContent();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }
    }

    public async void OnActivityDateChanged(DateTime? e, bool isActivityStartDate = false)
    {
        if (isActivityStartDate)
        {
            if (e <= UpdateFreebieDto.ActivityEndDate || UpdateFreebieDto.ActivityEndDate is null)
            {
                UpdateFreebieDto.ActivityStartDate = e;
            }

            else await _uiMessageService.Warn(L[PikachuResource.ActivityStartDateWarning]);
        }

        else
        {
            if (e >= UpdateFreebieDto.ActivityStartDate || UpdateFreebieDto.ActivityStartDate is null)
            {
                UpdateFreebieDto.ActivityEndDate = e;
            }

            else await _uiMessageService.Warn(L[PikachuResource.ActivityEndDateWarning]);
        }
    }

    private async Task LoadHtmlContent()
    {
        await Task.Delay(5);
        await ItemDescription.LoadHTMLContent(ExistingItem.ItemDescription);
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
            UpdateFreebieDto.FreebieAmount = freebieAmount;

            UpdateFreebieDto.ItemDescription = await ItemDescription.GetHTML();
            if (UpdateFreebieDto.ItemDescription.IsNullOrWhiteSpace() || UpdateFreebieDto.ItemDescription == "<p><br></p>")
            {
                await _uiMessageService.Error(L[PikachuDomainErrorCodes.ItemDescriptionIsRequired]);
                return;
            }

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
        if (UpdateFreebieDto.ApplyToAllProduct == false && !UpdateFreebieDto.FreebieProducts.Any())
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.SelectAtLeastOneProduct]);
        }
        if (!UpdateFreebieDto.UnCondition && UpdateFreebieDto.FreebieOrderReach is FreebieOrderReach.MinimumAmount && UpdateFreebieDto.MinimumAmount is null)
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.MinimumAmountReachCannotBeEmpty]);
        }
        if (!UpdateFreebieDto.UnCondition && UpdateFreebieDto.FreebieOrderReach is FreebieOrderReach.MinimumPiece && UpdateFreebieDto.MinimumPiece is null)
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.MinimumPieceCannotBeEmpty]);
        }
        if (!UpdateFreebieDto.UnCondition && UpdateFreebieDto.FreebieQuantity <= 0)
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.GetCannotBeEmptyOrZero]);
        }
        if (UpdateFreebieDto.UnCondition && UpdateFreebieDto.FreebieQuantity < 0)
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.GetCannotBeEmptyOrZero]);
        }
        if (!UpdateFreebieDto.UnCondition && UpdateFreebieDto.FreebieQuantity > freebieAmount)
        {
            throw new BusinessException(L[PikachuDomainErrorCodes.GetCannotBeGreaterThanGiftableQuantity]);
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
    Task OnProductCheckedValueChanged(bool value)
    {
        UpdateFreebieDto.ApplyToAllProduct = value;

        return Task.CompletedTask;
    }
    Task OnUnconditionCheckedValueChanged(bool value)
    {
        UpdateFreebieDto.UnCondition = value;

        return Task.CompletedTask;
    }
    #endregion
}
