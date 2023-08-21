using System;

namespace Kooco.Pikachu.Images
{
    public class CreateImageDto
    {
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid? ItemId { get; set; }
    }
}
