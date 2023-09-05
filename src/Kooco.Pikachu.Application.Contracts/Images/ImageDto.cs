using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Images
{
    public class ImageDto : EntityDto<Guid>
    {
        public string Name { get; set; }    
        public string BlobImageName { get; set; }
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid? TargetID { get; set; }
    }
}
