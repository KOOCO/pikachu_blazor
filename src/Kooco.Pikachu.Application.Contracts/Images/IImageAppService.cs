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
        Task UpdateImageAsync(CreateImageDto image);
        Task InsertManyImageAsync(List<CreateImageDto> images);
        Task<List<ImageDto>> GetGroupBuyImagesAsync(Guid GroupBuyId, ImageType? imageType = null);
        Task DeleteGroupBuyImagesAsync(Guid GroupBuyId);

        Task<string> UploadImageAsync(string fileName, byte[] bytes, bool overrideExisting = true);
        Task<bool> DeleteImageAsync(string blobName, bool configureAwait = true);
        Task DeleteByGroupBuyIdAndImageTypeAsync(Guid groupBuyId, ImageType imageType);
        Task UpdateCarouselStyleAsync(CreateImageDto carouselImage);
    }
}
