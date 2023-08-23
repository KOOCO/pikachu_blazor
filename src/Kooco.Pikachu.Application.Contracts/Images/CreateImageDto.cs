using System;

namespace Kooco.Pikachu.Images
{
    public class CreateImageDto
    {
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid FileId { get; set; }
        public FileInfo FileInfo { get; set; }
    }
}
