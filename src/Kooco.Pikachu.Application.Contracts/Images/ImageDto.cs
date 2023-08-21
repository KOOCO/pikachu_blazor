using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Images
{
    public class ImageDto :FullAuditedEntityDto<Guid>
    {
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid? ItemId { get; set; }
    }
}
