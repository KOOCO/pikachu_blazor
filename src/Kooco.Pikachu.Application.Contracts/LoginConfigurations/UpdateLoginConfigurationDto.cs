using System;

namespace Kooco.Pikachu.LoginConfigurations;

public class UpdateLoginConfigurationDto
{
    public string? FacebookAppId { get; set; }
    public string? FacebookAppSecret { get; set; }

    public string? LineChannelId { get; set; }
    public string? LineChannelSecret { get; set; }
}