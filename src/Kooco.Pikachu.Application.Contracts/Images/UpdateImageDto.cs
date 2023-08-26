using System;

namespace Kooco.Pikachu.Images
{
    public class UpdateImageDto
    {
        public string ImagePath { get; set; }
        public ImageType ImageType { get; set; }
        public Guid? TargetID { get; set; }
    }
}
