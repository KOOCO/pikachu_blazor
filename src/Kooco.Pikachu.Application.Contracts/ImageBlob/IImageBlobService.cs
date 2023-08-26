using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.ImageBlob
{
    public interface IImageBlobService : IScopedDependency
    {
        Task<string> UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType, string dirName);
        Task DeleteBlobData(string fileUrl);
    }
}
