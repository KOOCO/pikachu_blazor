using Volo.Abp.BlobStoring;

namespace Kooco.Pikachu.AzureStorage.Image
{
    [BlobContainerName(ImageContainerName)]
    public class ImageContainer
    {
        public const string ImageContainerName = "images";
    }
}
