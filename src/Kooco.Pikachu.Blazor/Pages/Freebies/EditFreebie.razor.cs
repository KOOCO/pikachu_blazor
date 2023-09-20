using Blazorise.Components;
using Blazorise;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.GroupBuys;
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
using Kooco.Pikachu.EnumValues;
using AutoMapper;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.FreeBies.Dtos;

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
        private Blazored.TextEditor.BlazoredTextEditor ItemDescription;
        //Item Discription Html
        private Radio<bool> ApplyAllRadio;
        // private Radio<bool> Uncondition;
        private List<KeyValueDto> GroupBuyList { get; set; } = new();
        public List<CreateImageDto> ImageList { get; set; }
        private FilePicker FilePicker { get; set; }
        private Autocomplete<KeyValueDto, Guid> AutocompleteField { get; set; }
        private List<string> SelectedTexts { get; set; } = new();
        private List<Guid> SelectedItems { get; set; }
        private List<CreateFreebieGroupBuysDto> CreateFreebieGroupBuysDto { get; set; } = new();
        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieAppService _freebieAppService;
        private readonly IEnumValueAppService _enumValueService;
        private UpdateFreebieDto UpdateFreebieDto = new();
        private FreebieCreateDto freebieCreateDto { get; set; } = new();
        public Guid EditingId { get; private set; }

        public EditFreebie(
            IUiMessageService uiMessageService,
            ImageContainerManager imageContainerManager,
            IGroupBuyAppService groupBuyAppService,
            IFreebieAppService freebieAppService
            )
        {
            _uiMessageService = uiMessageService;
            _imageContainerManager = imageContainerManager;
            _groupBuyAppService = groupBuyAppService;
            _freebieAppService = freebieAppService;
            ImageList = new List<CreateImageDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            EditingId = Guid.Parse(Id);

            try
            {
                ExistingItem = await _freebieAppService.GetAsync(EditingId, true);

                var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
                // Create a new mapper
                var mapper = config.CreateMapper();

                // Map ItemDto to UpdateItemDto
                UpdateFreebieDto = mapper.Map<UpdateFreebieDto>(ExistingItem);
                UpdateFreebieDto.Images = UpdateFreebieDto.Images.OrderBy(x => x.SortNo).ToList();

                SelectedItems = UpdateFreebieDto.FreebieGroupBuys.Select(x => x.GroupBuyId).ToList();

                if (!ExistingItem.ItemDescription.IsNullOrWhiteSpace())
                {
                    await ItemDescription.LoadHTMLContent(ExistingItem.ItemDescription);
                }
                GroupBuyList = await _freebieAppService.GetGroupBuyLookupAsync();
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
                    var stream = file.OpenReadStream();
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
                    confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                    if (confirmed)
                    {
                        confirmed = await _imageContainerManager.DeleteAsync(blobImageName);
                        await _freebieAppService.DeleteSingleImageAsync(EditingId, blobImageName);
                        UpdateFreebieDto.Images = UpdateFreebieDto.Images.Where(x => x.BlobImageName != blobImageName).ToList();
                        //ImageList = ImageList.Where(x => x.BlobImageName != blobImageName).ToList();
                        //StateHasChanged();
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

        protected virtual async Task UpdateFreebieAsync()
        {
            try
            {
                ValidateForm();
                //UpdateFreebieDto.SetItemDetails = new();
                //ItemDetails.ForEach(item =>
                //{
                //    CreateUpdateSetItemDto.SetItemDetails.Add(
                //        new CreateUpdateSetItemDetailsDto
                //        {
                //            ItemId = item.ItemId,
                //            Quantity = item.Quantity
                //        });
                //});

                UpdateFreebieDto.ItemDescription = await ItemDescription.GetHTML();

                await _freebieAppService.UpdateAsync(EditingId, UpdateFreebieDto);

                NavigationManager.NavigateTo("/Freebielist");
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
            if (UpdateFreebieDto.ItemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemNameCannotBeNull]);
            }
            if (UpdateFreebieDto.ItemDescription.IsNullOrWhiteSpace())
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.ItemDetailsCannotBeEmpty]);
            }
            if (UpdateFreebieDto.FreebieAmount <= 0)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.FreebieAmountCannotBeZero]);
            }
            
        }

        //private void OnRadioChange(ChangeEventArgs e)
        //{
        //    freebieCreateDto.ApplyToAllGroupBuy = (bool)e.Value;
        //}
    }
}
