using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.ImageBlob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu.Images;

public class ImageAppService : CrudAppService<Image,
                                                ImageDto,
                                                Guid,
                                                PagedAndSortedResultRequestDto,
                                                CreateImageDto,
                                                UpdateImageDto>, IImageAppService

{
    #region Inject
    private readonly IImageBlobService _imageblobService;
    private readonly IRepository<Image, Guid> _repository;
    private readonly ImageContainerManager _imageContainerManager;
    #endregion

    #region Constructor
    public ImageAppService(IRepository<Image, Guid> repository,
                           IImageBlobService imageBlobService,
                           ImageContainerManager imageContainerManager) : base(repository)
    {
        _imageblobService = imageBlobService;
        _repository = repository;
        _imageContainerManager = imageContainerManager;
    }
    #endregion

    public async Task ImagesModuleNoReindexingAsync(Guid targetId, ImageType imageType, int oldModuleNo, int newModuleNo)
    {
        List<CreateImageDto> images = ObjectMapper.Map<List<Image>, List<CreateImageDto>>(
            [.. (await _repository.GetQueryableAsync()).Where(w => w.TargetId == targetId && 
                                                                   w.ImageType == imageType && 
                                                                   w.ModuleNumber == oldModuleNo)]
        );

        foreach (CreateImageDto image in images) 
        {
            image.ModuleNumber = newModuleNo;

            await UpdateImageAsync(image);
        }
    }

    public async Task<List<CreateImageDto>> GetImageListByModuleNumberAsync(Guid groupBuyId, ImageType imageType, int moduleNumber)
    {
        return ObjectMapper.Map<List<Image>, List<CreateImageDto>>(
            [.. (await _repository.GetQueryableAsync()).Where(w => w.TargetId == groupBuyId &&
                                                                   w.ImageType == imageType &&
                                                                   w.ModuleNumber == moduleNumber)]
        );
    }

    public async Task UpdateImageAsync(CreateImageDto image)
    {
        await UpdateAsync(image.Id, ObjectMapper.Map<CreateImageDto, UpdateImageDto>(image));
    }

    public async Task DeleteGroupBuyImagesAsync(Guid GroupBuyId)
    {
        await _repository.DeleteAsync(x => x.TargetId == GroupBuyId);
    }

    public async Task<List<ImageDto>> GetGroupBuyImagesAsync(Guid GroupBuyId, ImageType? imageType = null)
    {
        List<ImageDto> result = new List<ImageDto>();
        var query = await _repository.GetQueryableAsync();
        if (imageType.HasValue)
        {
            query = query.Where(x => x.ImageType == imageType);
        }
        result = ObjectMapper.Map<List<Image>, List<ImageDto>>(query.Where(x => x.TargetId == GroupBuyId).ToList());
        return result;
    }

    public async Task InsertManyImageAsync(List<CreateImageDto> images)
    {
        foreach (var item in images.ToList())
        {
            string dirName = item.ImageType.ToString() + "/" + item.TargetId.ToString();
            //var imageString = await _imageblobService.UploadFileToBlob(item.FileInfo.FileName, item.FileInfo.FileData, item.FileInfo.FileMimeType, dirName);
            item.ImageUrl = "";
        }
        var image = ObjectMapper.Map<List<CreateImageDto>, List<Image>>(images);
        await _repository.InsertManyAsync(image, true);
    }

    public async Task<string> UploadImageAsync(string fileName, byte[] bytes, bool overrideExisting = true)
    {
        var blobName = GuidGenerator.Create().ToString().Replace("-", "") + Path.GetExtension(fileName);
        return await _imageContainerManager.SaveAsync(blobName, bytes);
    }

    public async Task<bool> DeleteImageAsync(string blobName, bool configureAwait = true)
    {
        var isDeleted = await _imageContainerManager.DeleteAsync(blobName).ConfigureAwait(configureAwait);
        return isDeleted;
    }

    public async Task DeleteByGroupBuyIdAndImageTypeAsync(Guid groupBuyId, ImageType imageType, int moduleNumber)
    {
        await _repository.DeleteDirectAsync(d => d.TargetId == groupBuyId && d.ImageType == imageType && d.ModuleNumber == moduleNumber);
    }
    public async Task UpdateCarouselStyleAsync(CreateImageDto carouselImage)
    {
        List<ImageDto> existingImages = await GetGroupBuyImagesAsync(carouselImage.TargetId);
        if (existingImages != null)
        {
            existingImages = existingImages.Where(x => x.ModuleNumber == carouselImage.ModuleNumber).ToList();

            if (existingImages.Any())
            {
                foreach (var image in existingImages)
                {
                    image.CarouselStyle = carouselImage.CarouselStyle;
                }

                List<Image> updatedImages = ObjectMapper.Map<List<ImageDto>, List<Image>>(existingImages);
                await _repository.UpdateManyAsync(updatedImages, true);
            }
        }
    }

}
