using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;


namespace Kooco.Pikachu.ImageBlob
{
    public class ImageBlobService : IImageBlobService
    {
        private string _basePath;
        private string _bucketName;
        private string _rootPath;

        public ImageBlobService()
        {
                
        }
        public Task AddDirectory(string directoryName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string directoryName, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFilesAsync(string directoryName, List<string> fileNames)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetAsync(string directory, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task GetListAsync(string directory, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task InsertFileAsync(string directory, string fileName, byte[] file)
        {
            throw new NotImplementedException();
        }

        public Task InsertFilesAsync(string directory, string fileName, byte[] file)
        {
            throw new NotImplementedException();
        }
    }
}
