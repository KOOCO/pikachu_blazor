using Kooco.Pikachu.ImageBlob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IImageBlobService _imageblobService;
        private readonly IRepository<Image, Guid> _repository;
        public ImageAppService(IRepository<Image, Guid> repository,
                               IImageBlobService imageBlobService) : base(repository)
        {
            _imageblobService = imageBlobService;
        }

        public async Task InsertManyImageAsync(List<CreateImageDto> images)
        {
            foreach (var item in images.ToList())
            {
                string dirName = item.ImageType.ToString() + "/" + item.TargetID.ToString();
                var imageString = await _imageblobService.UploadFileToBlob(item.FileInfo.FileName, item.FileInfo.FileData, item.FileInfo.FileMimeType, dirName);
                item.ImagePath = imageString;
            }
            var image = ObjectMapper.Map<List<CreateImageDto>, List<Image>>(images);
            await _repository.InsertManyAsync(image, true);
        }
    }
}
