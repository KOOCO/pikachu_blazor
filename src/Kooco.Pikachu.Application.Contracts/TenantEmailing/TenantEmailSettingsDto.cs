using System;

namespace Kooco.Pikachu.TenantEmailing
{
    public class TenantEmailSettingsDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string? SenderName { get; set; }
        public string? Subject { get; set; }
        public string? Greetings { get; set; }
        public string? Footer { get; set; }
    }
}
