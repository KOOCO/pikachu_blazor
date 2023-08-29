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
        Task InsertManyImageAsync(List<CreateImageDto> images);
        Task<List<ImageDto>> GetGroupBuyImagesAsync(Guid GroupBuyId);
        Task DeleteGroupBuyImagesAsync(Guid GroupBuyId);
    }
}
