using System;

namespace Kooco.Pikachu.Tenants
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
