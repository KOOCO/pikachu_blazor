using System.Collections.Generic;

namespace Kooco.Pikachu.AntBlazorModels.Upload
{
    public class UploadInfo
    {
        public IFormFile File { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}
