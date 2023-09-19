using Blazorise;
using Blazorise.Components;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Items.Dtos;
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
        private Blazored.TextEditor.BlazoredTextEditor ItemDescription; //Item Discription Html
      //  private Radio<bool> ApplyToAllGroupBuy;
       // private Radio<bool> Uncondition;
        private List<KeyValueDto> GroupBuyList { get; set; } = new();
        public List<CreateImageDto> ImageList { get; set; }
        private FilePicker FilePicker { get; set; }
        private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; } 
        private List<string> SelectedTexts { get; set; } = new();
        private List<Guid?> SelectedItems { get; set; }

        private readonly IGroupBuyAppService _groupBuyAppService;
        private readonly IUiMessageService _uiMessageService;
        private readonly ImageContainerManager _imageContainerManager;
        private readonly IFreebieAppService _freebieAppService;
      
        private FreebieCreateDto FreebieCreateDto { get; set; } = new();
        public CreateFreebie(
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
            try
            {
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
                        ImageList = ImageList.Where(x => x.BlobImageName != blobImageName).ToList();
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

        protected virtual async Task CreateFreebieAsync()
        {
            try
            {
                
                //ValidateForm();
                //FreebieCreateDto.FreebieGroupBuys = new();

                FreebieCreateDto.ItemDescription = await ItemDescription.GetHTML();

                await _freebieAppService.CreateAsync(FreebieCreateDto);
                //NavigationManager.NavigateTo("/SetItem");
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
    }
}
