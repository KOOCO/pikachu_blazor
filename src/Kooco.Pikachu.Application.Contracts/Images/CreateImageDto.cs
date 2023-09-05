using System;

namespace Kooco.Pikachu.Images
{
    public class CreateImageDto
    {
        public string Name { get; set; }
        public string BlobImageName { get; set; }
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid TargetID { get; set; }
        public FileInfo FileInfo { get; set; }
    }
}
