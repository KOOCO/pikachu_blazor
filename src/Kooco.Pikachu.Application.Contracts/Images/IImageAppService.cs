using System;
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
        
    }
}
