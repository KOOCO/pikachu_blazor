using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Images
{
    public class ImageDto : EntityDto<Guid>
    {
        public string Name { get; set; }    
        public string BlobImageName { get; set; }
        public string ImageUrl { get; set; }
        public ImageType ImageType { get; set; }
        public Guid? TargetId { get; set; }
        public int SortNo { get; set; }
        public string? Link { get; set; }
        public StyleForCarouselImages? CarouselStyle { get; set; }
        public int? ModuleNumber { get; set; }
    }
}
