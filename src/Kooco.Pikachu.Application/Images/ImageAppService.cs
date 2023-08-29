using Kooco.Pikachu.ImageBlob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

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
            _repository = repository;
        }

        public async Task DeleteGroupBuyImagesAsync(Guid GroupBuyId)
        {
            await _repository.DeleteAsync(x => x.TargetID == GroupBuyId);
        }

        public async Task<List<ImageDto>> GetGroupBuyImagesAsync(Guid GroupBuyId)
        {
            List<ImageDto> result=new List<ImageDto>();
            var query = await _repository.GetQueryableAsync();
            result =ObjectMapper.Map<List<Image>,List<ImageDto>>(query.Where(x => x.TargetID == GroupBuyId).ToList());
            return result;
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
