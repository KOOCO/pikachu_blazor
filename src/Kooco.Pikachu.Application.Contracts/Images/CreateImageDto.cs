using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.Images
{
    public class CreateImageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BlobImageName { get; set; }
        public string ImageUrl { get; set; }
        public ImageType ImageType { get; set; }
        public Guid TargetId { get; set; }
        public int SortNo { get; set; }
        public string? Link { get; set; }
        public StyleForCarouselImages? CarouselStyle { get; set; }
        public int? ModuleNumber { get; set; }

        public CreateImageDto()
        {
            
        }
        public CreateImageDto(
            string name,
            string blobImageName,
            string imageUrl,
            ImageType imageType,
            Guid? targetId = null,
            int sortNo = 1
        )
        {
            Name = name;
            BlobImageName = blobImageName;
            ImageUrl = imageUrl;
            ImageType = imageType;
            TargetId = targetId ?? Guid.Empty;
            SortNo = sortNo;
        }
    }
}
