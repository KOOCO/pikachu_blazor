using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Images
{
    public interface IImageAppService : ICrudAppService<ImageDto,
                                                        Guid,
                                                        PagedAndSortedResultRequestDto,
                                                        CreateImageDto,
                                                        UpdateImageDto>
    {
        Task ImagesModuleNoReindexingAsync(Guid targetId, ImageType imageType, int oldModuleNo, int newModuleNo);
        Task<List<CreateImageDto>> GetImageListByModuleNumberAsync(Guid groupBuyId, ImageType imageType, int moduleNumber);
        Task UpdateImageAsync(CreateImageDto image);
        Task InsertManyImageAsync(List<CreateImageDto> images);
        Task<List<ImageDto>> GetGroupBuyImagesAsync(Guid GroupBuyId, ImageType? imageType = null);
        Task DeleteGroupBuyImagesAsync(Guid GroupBuyId);

        Task<string> UploadImageAsync(string fileName, byte[] bytes, bool overrideExisting = true);
        Task<bool> DeleteImageAsync(string blobName, bool configureAwait = true);
        Task DeleteByGroupBuyIdAndImageTypeAsync(Guid groupBuyId, ImageType imageType, int moduleNumber);
        Task UpdateCarouselStyleAsync(CreateImageDto carouselImage);
    }
}
