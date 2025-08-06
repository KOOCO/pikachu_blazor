using Kooco.Pikachu.AzureStorage.Image;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.AzureStorage.LogisticsFiles
{
    public class LogisticsFileContainerManager : DomainService
    {
        private readonly IBlobContainer<LogisticsFileContainer> _logisticsFileContainer;
        private readonly AzureStorageAccountOptions _azureStorageAccountOptions;

        public LogisticsFileContainerManager(
            IBlobContainer<LogisticsFileContainer> logisticsFileContainer,
            IOptions<AzureStorageAccountOptions> azureStorageAccountOptions
            )
        {
            _logisticsFileContainer = logisticsFileContainer;
            _azureStorageAccountOptions = azureStorageAccountOptions.Value;
        }

        public async Task<string> SaveAsync(string fileName, Stream stream, bool overrideExisting = false)
        {
            await _logisticsFileContainer.SaveAsync(fileName, stream, overrideExisting);

            return GetStorageUrl(fileName);
        }

        public async Task<string> SaveAsync(string fileName, byte[] byteArray, bool overrideExisting = false)
        {
            await _logisticsFileContainer.SaveAsync(fileName, byteArray, overrideExisting);

            return GetStorageUrl(fileName);
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            return await _logisticsFileContainer.DeleteAsync(fileName);
        }

        public string GetStorageUrl(string storageFileName)
        {
            if (CurrentTenant.IsAvailable)
            {
                return $"{_azureStorageAccountOptions.AccountUrl}/{LogisticsFileContainer.LogisticsFileContainerName}/tenants/{CurrentTenant.Id}/{storageFileName}";
            }
            return $"{_azureStorageAccountOptions.AccountUrl}/{LogisticsFileContainer.LogisticsFileContainerName}/host/{storageFileName}";
        }
    }
}


