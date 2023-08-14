using System.Collections.Generic;

namespace Kooco.Pikachu.AntBlazorModels.Upload
{
    public class UploadInfo
    {
        public UploadFileItem File { get; set; }
        public List<UploadFileItem> FileList { get; set; }
    }
}
