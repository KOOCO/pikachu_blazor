using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Volo.Abp.Users;

namespace Kooco.Pikachu.ImageBlob
{
    public class ImageBlobService : IImageBlobService
    {
        private string _accessKey = string.Empty;
        private readonly ICurrentUser _currentUser;
        private readonly string _pathPrefix = string.Empty;
        public ImageBlobService(ICurrentUser currentUser)
        {
            this._accessKey = "";
            _currentUser = currentUser;
            _pathPrefix = (_currentUser.TenantId != null && _currentUser.TenantId != Guid.Empty) ? _currentUser.TenantId + "/" : "";
        }

        public async Task<string> UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType, string dirName)
        {
            return await UploadFileToBlobAsync(strFileName, fileData, fileMimeType, dirName);
        }

        public async Task DeleteBlobData(string fileUrl)
        {
            Uri uriObj = new Uri(fileUrl);
            string BlobName = Path.GetFileName(uriObj.LocalPath);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "uploads";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

            CloudBlobDirectory blobDirectory = cloudBlobContainer.GetDirectoryReference(_pathPrefix);
            // get block blob refarence    
            CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

            // delete blob from container        
            await blockBlob.DeleteAsync();
        }

        private string GenerateFileName(string fileName, string dirName)
        {
            string[] strName = fileName.Split('.');
            return _pathPrefix + dirName + "." + strName[strName.Length - 1];
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType, string dirName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "uploads";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            string fileName = this.GenerateFileName(strFileName, dirName);

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            if (fileName != null && fileData != null)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                cloudBlockBlob.Properties.ContentType = fileMimeType;
                await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                return cloudBlockBlob.Uri.AbsoluteUri;
            }
            return "";
        }
    }
}
