using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TenantEmailing
{
    public class CreateUpdateTenantEmailSettingsDto
    {
        [Required(ErrorMessage = "This Field Is Required")]
        public string? SenderName { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? Greetings { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]
        public string? Footer { get; set; }
    }
}
