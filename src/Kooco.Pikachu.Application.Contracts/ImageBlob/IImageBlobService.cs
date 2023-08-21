using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.ImageBlob
{
    public interface IImageBlobService
    {
        Task AddDirectory(string directoryName);
        Task InsertFileAsync(string directory, string fileName, byte[] file);
        Task InsertFilesAsync(string directory, string fileName, byte[] file);
        Task DeleteFileAsync(string directoryName, string fileName);
        Task DeleteFilesAsync(string directoryName, List<string> fileNames);
        Task<byte[]> GetAsync(string directory, string fileName);
        Task GetListAsync(string directory, string fileName);
    }
}
