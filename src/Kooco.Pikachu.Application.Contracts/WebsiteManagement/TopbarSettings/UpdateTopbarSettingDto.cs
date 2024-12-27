using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class UpdateTopbarSettingDto
{
    [Required]
    public TopbarStyleSettings? TopbarStyleSettings { get; set; }

    [Required]
    public List<UpdateTopbarSettingLinkDto> Links { get; set; } = [];
}