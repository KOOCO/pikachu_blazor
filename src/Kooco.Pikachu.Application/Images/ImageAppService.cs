using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Images
{
    public class ImageAppService : CrudAppService<Image,
                                                    ImageDto,
                                                    Guid,
                                                    PagedAndSortedResultRequestDto,
                                                    CreateImageDto,
                                                    UpdateImageDto>, IImageAppService

    {
        public ImageAppService(IRepository<Image, Guid> repository) : base(repository)
        {
        }

    }
}
