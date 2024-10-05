using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.AzureStorage.Image
{
    public class ImageContainerManager : DomainService
    {
        private readonly IBlobContainer<ImageContainer> _imageContainer;
        private readonly AzureStorageAccountOptions _azureStorageAccountOptions;

        public ImageContainerManager(
            IBlobContainer<ImageContainer> imageContainer,
            IOptions<AzureStorageAccountOptions> azureStorageAccountOptions
            )
        {
            _imageContainer = imageContainer;
            _azureStorageAccountOptions = azureStorageAccountOptions.Value;
        }

        public async Task<string> SaveAsync(string fileName, Stream stream, bool overrideExisting = false)
        {
            await _imageContainer.SaveAsync(fileName, stream, overrideExisting);

            return GetStorageUrl(fileName);
        }

        public async Task<string> SaveAsync(string fileName, byte[] byteArray, bool overrideExisting = false)
        {
            await _imageContainer.SaveAsync(fileName, byteArray, overrideExisting);

            return GetStorageUrl(fileName);
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            return await _imageContainer.DeleteAsync(fileName);
        }

        public string GetStorageUrl(string storageFileName)
        {
            if (CurrentTenant.IsAvailable)
            {
                return $"{_azureStorageAccountOptions.AccountUrl}/{ImageContainer.ImageContainerName}/tenants/{CurrentTenant.Id}/{storageFileName}";
            }
            return $"{_azureStorageAccountOptions.AccountUrl}/{ImageContainer.ImageContainerName}/host/{storageFileName}";
        }
    }
}
