using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class UpdateFooterSettingDto
{
    [Required]
    public List<UpdateFooterSettingSectionDto> Sections { get; set; } = [];
}
