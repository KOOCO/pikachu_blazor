using System;

namespace Kooco.Pikachu.Images
{
    public class CreateImageDto
    {
        public string Name { get; set; }
        public string BlobImageName { get; set; }
        public string ImageUrl { get; set; }
        public ImageType ImageType { get; set; }
        public Guid TargetId { get; set; }
        public int SortNo { get; set; }
    }
}
