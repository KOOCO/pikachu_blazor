namespace Kooco.Pikachu.TenantManagement;

public class UploadImageDto
{
    public string Base64 { get; set; }
    public string FileName { get; set; }

    public UploadImageDto(string base64, string fileName)
    {
        Base64 = base64;
        FileName = fileName;
    }
}