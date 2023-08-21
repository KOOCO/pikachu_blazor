using System.Threading.Tasks;

namespace Kooco.Pikachu.ImageBlob
{
    public interface IImageBlobService
    {
        Task<string> UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType, string dirName);
        Task DeleteBlobData(string fileUrl);
    }
}
