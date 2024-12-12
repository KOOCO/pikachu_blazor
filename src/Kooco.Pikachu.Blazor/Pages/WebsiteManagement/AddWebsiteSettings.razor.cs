using Blazorise;
using Kooco.Pikachu.WebsiteManagement;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using Blazorise.Components;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.ProductCategories;
using Blazored.TextEditor;
using Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.GroupBuys;
using Microsoft.AspNetCore.Components;
using Kooco.Pikachu.AzureStorage.Image;
using Microsoft.AspNetCore.Components.Web;

namespace Kooco.Pikachu.Blazor.Pages.WebsiteManagement;

public partial class AddWebsiteSettings
{
    private CreateWebsiteSettingsDto NewEntity { get; set; }
    private Validations ValidationsRef;

    private bool IsLoading { get; set; }

    private string LogoBase64 { get; set; }
    private List<KeyValueDto> ProductCategoryLookup { get; set; } = [];
    private string SelectedAutoCompleteText { get; set; }
    private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
    private BlazoredTextEditor ArticlePageHtml;

    private List<CollapseItem> CollapseItem = [];
    private List<FilePicker> CarouselFilePickers = [];
    private List<FilePicker> BannerFilePickers = [];
    private List<FilePicker> GroupPurchaseOverviewFilePickers = [];
    private List<FilePicker> GroupBuyOrderInstructionPickers = [];
    private List<FilePicker> ProductRankingCarouselPickers = [];

    public List<List<CreateImageDto>> CarouselModules = [];
    public List<List<CreateImageDto>> BannerModules = [];
    public List<GroupPurchaseOverviewDto> GroupPurchaseOverviewModules = [];
    public List<GroupBuyOrderInstructionDto> GroupBuyOrderInstructionModules = [];
    public List<ProductRankingCarouselModule> ProductRankingCarouselModules = [];

    private CreateImageDto SelectedImageDto = new();
    private readonly List<string> ValidFileExtensions = [".jpg", ".png", ".svg", ".jpeg", ".webp"];
    private FilePicker LogoPickerCustom { get; set; }
    private const int MaxAllowedFileSize = 1024 * 1024 * 10;

    private Modal AddLinkModal { get; set; }
    private List<ItemWithItemTypeDto> ItemsList { get; set; } = [];
    private List<ItemWithItemTypeDto> SetItemList { get; set; } = [];
    int CurrentIndex { get; set; }

    public AddWebsiteSettings()
    {
        NewEntity = new();
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            SetItemList = await _setItemAppService.GetItemsLookupAsync();
            ItemsList = await _itemAppService.GetItemsLookupAsync();
            ItemsList.AddRange(SetItemList);
            ProductCategoryLookup = await ProductCategoryAppService.GetProductCategoryLookupAsync();
        }
    }

    void NavigateToWebsiteSettings()
    {
        NavigationManager.NavigateTo("/Website-Settings");
    }

    async Task OnFileUploadAsync(FileChangedEventArgs e)
    {
        var file = e.Files.FirstOrDefault();

        if (file != null)
        {
            string extension = Path.GetExtension(file.Name);

            if (file.Size > Constant.MaxImageSizeInBytes)
            {
                await UiNotificationService.Error(L["Pikachu:ImageSizeExceeds", Constant.MaxImageSizeInBytes.FromBytesToMB()]);
                return;
            }

            if (!Constant.ValidImageExtensions.Contains(extension))
            {
                await UiNotificationService.Error(L["Pikachu:InvalidImageExtension", string.Join(", ", Constant.ValidImageExtensions)]);
                return;
            }

            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            LogoBase64 = Convert.ToBase64String(fileBytes);
            NewEntity.LogoName = file.Name;

            await InvokeAsync(StateHasChanged);
        }
    }

    async Task CreateAsync()
    {
        var validate = await ValidationsRef.ValidateAll();
        if (!validate) return;
        try
        {
            IsLoading = true;

            var bytes = Convert.FromBase64String(LogoBase64);
            NewEntity.LogoUrl = await ImageAppService.UploadImageAsync(NewEntity.LogoName, bytes);

            await WebsiteSettingsAppService.CreateAsync(NewEntity);
            NavigateToWebsiteSettings();
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task WebsitePageTypeChanged(WebsitePageType? websitePageType)
    {
        NewEntity.WebsitePageType = websitePageType;
    }

    async Task OnSelectedValueChanged(Guid? id)
    {
        //try
        //{
        //    if (id != null)
        //    {
        //        RowLoading = true;
        //        StateHasChanged();
        //        await AutocompleteField.Clear();
        //        var productCategory = ProductCategoryLookup.Where(x => x.Id == id).FirstOrDefault();

        //        if (productCategory == null) return;

        //        var itemCategory = new CreateUpdateItemCategoryDto
        //        {
        //            ProductCategoryId = productCategory.Id,
        //            ProductCategoryName = productCategory.Name,
        //            ImageUrl = await ProductCategoryAppService.GetDefaultImageUrlAsync(productCategory.Id)
        //        };

        //        CreateItemDto.ItemCategories.Add(itemCategory);
        //        RowLoading = false;

        //        ProductCategoryLookup = ProductCategoryLookup.Where(x => x.Id != id).ToList();
        //        StateHasChanged();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    RowLoading = false;
        //    await HandleErrorAsync(ex);
        //}
    }

    void AddProductItem(GroupBuyModuleType? moduleType)
    {
        if (!moduleType.HasValue) return;
        NewEntity.SelectedGroupBuyModuleType = null;
        var groupBuyModuleType = moduleType.Value;
        if (CollapseItem.Count >= 20)
        {
            UiNotificationService.Error(L[PikachuDomainErrorCodes.CanNotAddMoreThan20Modules]);
            return;
        }

        if (groupBuyModuleType is GroupBuyModuleType.ProductDescriptionModule
            || groupBuyModuleType is GroupBuyModuleType.IndexAnchor)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected =
                [
                    new ItemWithItemTypeDto()
                ]
            };

            CollapseItem.Add(collapseItem);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.CarouselImages)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                ModuleNumber = CollapseItem.Any(c => c.GroupBuyModuleType is GroupBuyModuleType.CarouselImages) ?
                               CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.CarouselImages) + 1 : 1
            };

            CollapseItem.Add(collapseItem);

            CarouselFilePickers.Add(new());

            CarouselModules.Add([new() { ModuleNumber = collapseItem.ModuleNumber }]);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.BannerImages)
        {

            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                ModuleNumber = CollapseItem.Any(c => c.GroupBuyModuleType is GroupBuyModuleType.BannerImages) ?
                               CollapseItem.Count(c => c.GroupBuyModuleType is GroupBuyModuleType.BannerImages) + 1 : 1
            };

            CollapseItem.Add(collapseItem);

            BannerFilePickers.Add(new());

            BannerModules.Add([new() { ModuleNumber = collapseItem.ModuleNumber }]);
        }

        else if (groupBuyModuleType is GroupBuyModuleType.GroupPurchaseOverview)
        {
            if (GroupPurchaseOverviewModules is { Count: 0 })
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            GroupPurchaseOverviewFilePickers.Add(new());

            GroupPurchaseOverviewModules.Add(new());
        }

        else if (groupBuyModuleType is GroupBuyModuleType.OrderInstruction)
        {
            if (GroupBuyOrderInstructionModules is { Count: 0 })
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            GroupBuyOrderInstructionPickers.Add(new());

            GroupBuyOrderInstructionModules.Add(new());
        }

        else if (groupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
        {
            if (ProductRankingCarouselModules is { Count: 0 })
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }

            ProductRankingCarouselPickers.Add(new());

            ProductRankingCarouselModules.Add(new()
            {
                Selected = [
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto()
                ]
            });
        }

        else if (groupBuyModuleType is GroupBuyModuleType.CountdownTimer)
        {
            if (!CollapseItem.Any(a => a.GroupBuyModuleType is GroupBuyModuleType.CountdownTimer))
            {
                CollapseItem collapseItem = new()
                {
                    Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                    SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                    GroupBuyModuleType = groupBuyModuleType
                };

                CollapseItem.Add(collapseItem);
            }
        }

        else if (groupBuyModuleType is GroupBuyModuleType.ProductRankingCarouselModule)
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected =
                [
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto()
                ]
            };
        }

        else
        {
            CollapseItem collapseItem = new()
            {
                Index = CollapseItem.Count > 0 ? CollapseItem.Count + 1 : 1,
                SortOrder = CollapseItem.Count > 0 ? CollapseItem.Max(c => c.SortOrder) + 1 : 1,
                GroupBuyModuleType = groupBuyModuleType,
                Selected =
                [
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto(),
                    new ItemWithItemTypeDto()
                ]
            };

            CollapseItem.Add(collapseItem);
        }

        List<GroupBuyModuleType> templateModules = new();

        if (NewEntity.GroupBuyTemplateType == GroupBuyTemplateType.PikachuOne) templateModules = [.. GroupBuyExtensions.GetPikachuOneList()];

        else if (NewEntity.GroupBuyTemplateType == GroupBuyTemplateType.PikachuTwo) templateModules = [.. GroupBuyExtensions.GetPikachuTwoList()];

        if (templateModules is { Count: > 0 })
        {
            foreach (CollapseItem module in CollapseItem)
            {
                if (templateModules.Contains(module.GroupBuyModuleType)) module.IsWarnedForInCompatible = false;

                else module.IsWarnedForInCompatible = true;
            }
        }
    }

    private void RemoveCollapseItem(int index)
    {
        CollapseItem? collapseItem = CollapseItem.FirstOrDefault(f => f.Index == index);

        int moduleNumber = collapseItem.ModuleNumber.HasValue ? collapseItem.ModuleNumber.Value : 0;

        if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.CarouselImages)
        {
            CarouselFilePickers.RemoveAt(moduleNumber - 1);

            CarouselModules.RemoveAll(r => r.Any(w => w.ModuleNumber == moduleNumber));
        }

        else if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
        {
            BannerFilePickers.RemoveAt(moduleNumber - 1);

            BannerModules.RemoveAll(r => r.Any(w => w.ModuleNumber == moduleNumber));
        }

        CollapseItem.Remove(collapseItem);

        if (collapseItem.GroupBuyModuleType is GroupBuyModuleType.CarouselImages ||
            collapseItem.GroupBuyModuleType is GroupBuyModuleType.BannerImages)
            ReindexingCollapseItem(moduleNumber, collapseItem.GroupBuyModuleType);
    }

    private void ReindexingCollapseItem(int moduleNumber, GroupBuyModuleType groupBuyModuleType)
    {
        foreach (CollapseItem collapseItem in CollapseItem.Where(w => w.GroupBuyModuleType == groupBuyModuleType && w.ModuleNumber > moduleNumber).ToList())
        {
            int oldModuleNumber = (int)collapseItem.ModuleNumber!;

            collapseItem.ModuleNumber = collapseItem.ModuleNumber - 1;

            if (groupBuyModuleType is GroupBuyModuleType.CarouselImages)
            {
                foreach (List<CreateImageDto> images in CarouselModules.Select(s => s.Where(w => w.ModuleNumber == oldModuleNumber && s.Count > 0).ToList()).ToList())
                {
                    foreach (CreateImageDto image in images)
                    {
                        image.ModuleNumber = image.ModuleNumber - 1;
                    }
                }
            }

            else if (groupBuyModuleType is GroupBuyModuleType.BannerImages)
            {
                foreach (List<CreateImageDto> images in BannerModules.Select(s => s.Where(w => w.ModuleNumber == oldModuleNumber && s.Count > 0).ToList()).ToList())
                {
                    foreach (CreateImageDto image in images)
                    {
                        image.ModuleNumber = image.ModuleNumber - 1;
                    }
                }
            }

        }
    }

    private static void OnProductGroupValueChange(ChangeEventArgs e, CollapseItem collapseItem)
    {
        int takeCount = int.Parse(e?.Value.ToString());
        if (collapseItem.Selected.Count > takeCount)
        {
            collapseItem.Selected = collapseItem.Selected.Take(takeCount).ToList();
        }
        else
        {
            collapseItem.Selected.Add(new ItemWithItemTypeDto());
        }
    }

    async Task OnImageModuleUploadAsync(
        FileChangedEventArgs e,
        List<CreateImageDto> carouselImages,
        int carouselModuleNumber,
        FilePicker carouselPicker,
        ImageType imageType
    )
    {
        if (carouselImages.Count >= 5)
        {
            await carouselPicker.Clear();
            return;
        }

        int count = 0;
        try
        {
            foreach (var file in e.Files.Take(5))
            {
                if (!ValidFileExtensions.Contains(Path.GetExtension(file.Name)))
                {
                    await carouselPicker.RemoveFile(file);
                    await UiNotificationService.Error(L["InvalidFileType"]);
                    continue;
                }
                if (file.Size > MaxAllowedFileSize)
                {
                    count++;
                    await carouselPicker.RemoveFile(file);
                    continue;
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

                    int sortNo = carouselImages.LastOrDefault()?.SortNo ?? 0;

                    if (sortNo is 0 && carouselImages.Any(a => a.Id != Guid.Empty && a.CarouselStyle != null && a.BlobImageName == string.Empty))
                    {
                        carouselImages[0].Name = file.Name;
                        carouselImages[0].BlobImageName = newFileName;
                        carouselImages[0].ImageUrl = url;
                        carouselImages[0].ImageType = imageType;
                        carouselImages[0].SortNo = sortNo + 1;
                        carouselImages[0].ModuleNumber = carouselModuleNumber;
                    }

                    else
                    {
                        //int indexInCarouselModules = CarouselModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));
                        int indexInCarouselModules = 0;

                        if (imageType is ImageType.GroupBuyCarouselImage)
                            indexInCarouselModules = CarouselModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));

                        else if (imageType is ImageType.GroupBuyBannerImage)
                            indexInCarouselModules = BannerModules.FindIndex(module => module.Any(img => img.ModuleNumber == carouselModuleNumber));

                        if (indexInCarouselModules >= 0)
                        {
                            // List<CreateImageDto> originalCarouselImages = CarouselModules[indexInCarouselModules];

                            List<CreateImageDto> originalCarouselImages = new();

                            if (imageType is ImageType.GroupBuyCarouselImage)
                                originalCarouselImages = CarouselModules[indexInCarouselModules];

                            else if (imageType is ImageType.GroupBuyBannerImage)
                                originalCarouselImages = BannerModules[indexInCarouselModules];

                            if (originalCarouselImages.Any(a => a.SortNo is 0))
                            {
                                int index = originalCarouselImages.IndexOf(originalCarouselImages.First(f => f.SortNo == 0));

                                originalCarouselImages[index].Name = file.Name;
                                originalCarouselImages[index].BlobImageName = newFileName;
                                originalCarouselImages[index].ImageUrl = url;
                                originalCarouselImages[index].ImageType = imageType;
                                originalCarouselImages[index].SortNo = sortNo + 1;
                                originalCarouselImages[index].ModuleNumber = carouselModuleNumber;
                            }

                            else
                            {
                                originalCarouselImages.Add(new CreateImageDto
                                {
                                    Name = file.Name,
                                    BlobImageName = newFileName,
                                    ImageUrl = url,
                                    ImageType = imageType,
                                    SortNo = sortNo + 1,
                                    ModuleNumber = carouselModuleNumber,
                                    CarouselStyle = originalCarouselImages.FirstOrDefault(f => f.CarouselStyle != null)?.CarouselStyle ?? null
                                });
                            }
                        }
                    }

                    await carouselPicker.Clear();
                }
                finally
                {
                    stream.Close();
                }
            }
            if (count > 0)
            {
                await UiNotificationService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            await UiNotificationService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    async Task OnBannerImageModuleUploadAsync(
        FileChangedEventArgs e,
        List<CreateImageDto> bannerImages,
        int carouselModuleNumber,
        FilePicker bannerPicker,
        ImageType imageType
    )
    {

        if (e.Files.Length > 1)
        {
            await UiNotificationService.Error("Select Only 1 Banner to Upload");
            await LogoPickerCustom.Clear();
            return;
        }
        if (e.Files.Length == 0)
        {
            return;
        }

        int count = 0;
        try
        {
            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await UiNotificationService.Error(L["InvalidFileType"]);
                await LogoPickerCustom.Clear();
                return;
            }
            if (e.Files[0].Size > MaxAllowedFileSize)
            {
                await LogoPickerCustom.RemoveFile(e.Files[0]);
                await UiNotificationService.Error(L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
                return;
            }
            string newFileName = Path.ChangeExtension(
                      Guid.NewGuid().ToString().Replace("-", ""),
                      Path.GetExtension(e.Files[0].Name));
            var stream = e.Files[0].OpenReadStream(long.MaxValue);
            try
            {
                var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                int sortNo = bannerImages[0].SortNo is 0 ? bannerImages[0].SortNo + 1 : 1;

                bannerImages[0].Name = e.Files[0].Name;
                bannerImages[0].BlobImageName = newFileName;
                bannerImages[0].ImageUrl = url;
                bannerImages[0].ImageType = imageType;
                bannerImages[0].SortNo = sortNo;
                bannerImages[0].ModuleNumber = carouselModuleNumber;

                SelectedImageDto.Link = string.Empty;

                await bannerPicker.Clear();
            }
            finally
            {
                stream.Close();
            }

            if (count > 0)
            {
                await UiNotificationService.Error(count + ' ' + L[PikachuDomainErrorCodes.FilesAreGreaterThanMaxAllowedFileSize]);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            await UiNotificationService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    void OnStyleCarouselChange(ChangeEventArgs e, List<CreateImageDto> carouselImages, int carouselModuleNumber)
    {
        if (carouselImages is { Count: 0 })
        {
            carouselImages.Add(new CreateImageDto()
            {
                Name = string.Empty,
                ImageUrl = string.Empty,
                ImageType = ImageType.GroupBuyCarouselImage,
                BlobImageName = string.Empty,
                CarouselStyle = e.Value is not null ? Enum.Parse<StyleForCarouselImages>(e.Value.ToString()) : null,
                ModuleNumber = carouselModuleNumber,
                SortNo = 0
            });
        }

        else
        {
            foreach (CreateImageDto image in carouselImages)
            {
                image.CarouselStyle = e.Value is not null ? Enum.Parse<StyleForCarouselImages>(e.Value.ToString()) : null;
            }
        }

        StateHasChanged();
    }

    async Task DeleteImageAsync(string blobImageName, List<CreateImageDto> carouselImages, ImageType imageType)
    {
        try
        {
            bool confirmed = await Message.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);
            if (confirmed)
            {
                await _imageContainerManager.DeleteAsync(blobImageName);

                if (imageType is ImageType.GroupBuyBannerImage)
                {
                    foreach (List<CreateImageDto> bannerModule in BannerModules)
                    {
                        int? moduleNumber = bannerModule
                                               .Where(r => r.BlobImageName == blobImageName)
                                               .Select(s => s.ModuleNumber)
                                               .FirstOrDefault();

                        bannerModule.RemoveAll(r => r.BlobImageName == blobImageName);

                        if (!bannerModule.Any() && moduleNumber.HasValue)
                            bannerModule.Add(new CreateImageDto
                            {
                                Name = string.Empty,
                                ImageUrl = string.Empty,
                                ImageType = ImageType.GroupBuyBannerImage,
                                BlobImageName = string.Empty,
                                CarouselStyle = null,
                                ModuleNumber = moduleNumber.Value,
                                SortNo = 0
                            });
                    }
                }

                else if (imageType is ImageType.GroupBuyCarouselImage)
                {
                    foreach (List<CreateImageDto> carouselModule in CarouselModules)
                    {
                        int? moduleNumber = carouselModule
                                                .Where(r => r.BlobImageName == blobImageName)
                                                .Select(s => s.ModuleNumber)
                                                .FirstOrDefault();

                        carouselModule.RemoveAll(r => r.BlobImageName == blobImageName);

                        if (!carouselModule.Any() && moduleNumber.HasValue)
                            carouselModule.Add(new CreateImageDto
                            {
                                Name = string.Empty,
                                ImageUrl = string.Empty,
                                ImageType = ImageType.GroupBuyCarouselImage,
                                BlobImageName = string.Empty,
                                CarouselStyle = null,
                                ModuleNumber = moduleNumber.Value,
                                SortNo = 0
                            });
                    }
                }

                else if (imageType is ImageType.GroupBuyProductRankingCarousel)
                {
                    foreach (ProductRankingCarouselModule module in ProductRankingCarouselModules)
                    {
                        module.Images.RemoveAll(r => r.BlobImageName == blobImageName);
                    }
                }

                carouselImages = carouselImages.Where(x => x.BlobImageName != blobImageName).ToList();
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await UiNotificationService.Error(L[PikachuDomainErrorCodes.SomethingWentWrongWhileDeletingImage]);
        }
    }

    private void OpenAddLinkModal(CreateImageDto createImageDto)
    {
        SelectedImageDto = createImageDto;

        AddLinkModal.Show();
    }
    private void CloseAddLinkModal()
    {
        AddLinkModal.Hide();
    }
    private async Task ApplyAddLinkAsync()
    {
        await AddLinkModal.Hide();

        await InvokeAsync(StateHasChanged);
    }
    private Task OnModalClosing(ModalClosingEventArgs e)
    {
        return Task.CompletedTask;
    }
    private string LocalizeFilePicker(string key, object[] args)
    {
        return L[key];
    }

    public async Task OnImageUploadAsync(
        FileChangedEventArgs e,
        GroupBuyOrderInstructionDto module,
        FilePicker filePicker
    )
    {
        try
        {
            if (e.Files.Length is 0) return;

            if (e.Files.Length > 1)
            {
                await Message.Error("Cannot add more 1 image.");

                await filePicker.Clear();

                return;
            }

            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await Message.Error(L["InvalidFileType"]);

                await filePicker.Clear();

                return;
            }

            string newFileName = Path.ChangeExtension(
                Guid.NewGuid().ToString().Replace("-", ""),
                Path.GetExtension(e.Files[0].Name)
            );

            Stream stream = e.Files[0].OpenReadStream(long.MaxValue);

            try
            {
                MemoryStream memoryStream = new();

                await stream.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                string url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                module.Image = url;

                await filePicker.Clear();
            }
            finally
            {
                stream.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            await Message.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    private async Task OnSelectedValueChanged(Guid? id, CollapseItem collapseItem, ItemWithItemTypeDto? selectedItem = null)
    {
        try
        {
            var item = ItemsList.FirstOrDefault(x => x.Id == id);
            var index = collapseItem.Selected.IndexOf(selectedItem);
            if (item != null)
            {
                if (item.ItemType == ItemType.Item)
                {
                    selectedItem.Item = await _itemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.Item.Id;
                    selectedItem.Name = selectedItem.Item.ItemName;
                    selectedItem.ItemType = ItemType.Item;
                }
                if (item.ItemType == ItemType.SetItem)
                {
                    selectedItem.SetItem = await _setItemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.SetItem.Id;
                    selectedItem.Name = selectedItem.SetItem.SetItemName;
                    selectedItem.ItemType = ItemType.SetItem;
                }
                collapseItem.Selected[index] = selectedItem;
            }
            else
            {
                if (collapseItem.GroupBuyModuleType != GroupBuyModuleType.IndexAnchor || collapseItem.Selected[index].Id != Guid.Empty)
                {
                    collapseItem.Selected[index] = new();
                }
            }
        }
        catch (Exception ex)
        {
            await Message.Error(ex.GetType().ToString());
        }
    }

    private async Task OnSelectedValueChanged(Guid? id, ProductRankingCarouselModule module, ItemWithItemTypeDto? selectedItem = null)
    {
        try
        {
            var item = ItemsList.FirstOrDefault(x => x.Id == id);
            var index = module.Selected.IndexOf(selectedItem);
            if (item != null)
            {
                if (item.ItemType == ItemType.Item)
                {
                    selectedItem.Item = await _itemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.Item.Id;
                    selectedItem.Name = selectedItem.Item.ItemName;
                    selectedItem.ItemType = ItemType.Item;
                }
                if (item.ItemType == ItemType.SetItem)
                {
                    selectedItem.SetItem = await _setItemAppService.GetAsync(item.Id, true);
                    selectedItem.Id = selectedItem.SetItem.Id;
                    selectedItem.Name = selectedItem.SetItem.SetItemName;
                    selectedItem.ItemType = ItemType.SetItem;
                }
                module.Selected[index] = selectedItem;
            }
            else
            {
                if (module.Selected[index].Id != Guid.Empty)
                {
                    module.Selected[index] = new();
                }
            }
        }
        catch (Exception ex)
        {
            await Message.Error(ex.GetType().ToString());
        }
    }

    public async Task OnImageUploadAsync(
       FileChangedEventArgs e,
       GroupPurchaseOverviewDto module,
       FilePicker filePicker
   )
    {
        try
        {
            if (e.Files.Length is 0) return;

            if (e.Files.Length > 1)
            {
                await Message.Error("Cannot add more 1 image.");

                await filePicker.Clear();

                return;
            }

            if (!ValidFileExtensions.Contains(Path.GetExtension(e.Files[0].Name)))
            {
                await Message.Error(L["InvalidFileType"]);

                await filePicker.Clear();

                return;
            }

            string newFileName = Path.ChangeExtension(
                Guid.NewGuid().ToString().Replace("-", ""),
                Path.GetExtension(e.Files[0].Name)
            );

            Stream stream = e.Files[0].OpenReadStream(long.MaxValue);

            try
            {
                MemoryStream memoryStream = new();

                await stream.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                string url = await _imageContainerManager.SaveAsync(newFileName, memoryStream);

                module.Image = url;

                await filePicker.Clear();
            }
            finally
            {
                stream.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            await UiNotificationService.Error(L[PikachuDomainErrorCodes.SomethingWrongWhileFileUpload]);
        }
    }

    public async Task DeleteImageAsync(MouseEventArgs e, GroupPurchaseOverviewDto module)
    {
        bool confirmed = await Message.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);

        if (confirmed)
        {
            await _imageContainerManager.DeleteAsync(module.Image);

            module.Image = string.Empty;

            StateHasChanged();
        }
    }

    public async Task DeleteImageAsync(MouseEventArgs e, GroupBuyOrderInstructionDto module)
    {
        bool confirmed = await Message.Confirm(L[PikachuDomainErrorCodes.AreYouSureToDeleteImage]);

        if (confirmed)
        {
            await _imageContainerManager.DeleteAsync(module.Image);

            module.Image = string.Empty;

            StateHasChanged();
        }
    }

    void StartDrag(CollapseItem item)
    {
        CurrentIndex = CollapseItem.IndexOf(item);
    }

    void Drop(CollapseItem item)
    {
        if (item != null)
        {
            var index = CollapseItem.IndexOf(item);

            var current = CollapseItem[CurrentIndex];

            CollapseItem.RemoveAt(CurrentIndex);
            CollapseItem.Insert(index, current);

            CurrentIndex = index;

            for (int i = 0; i < CollapseItem.Count; i++)
            {
                CollapseItem[i].Index = i;
                CollapseItem[i].SortOrder = i + 1;
            }
            StateHasChanged();
        }
    }
}