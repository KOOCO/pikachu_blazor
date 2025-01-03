using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class UpdateFooterSettingLinkDto
{
    [Required]
    public int Index { get; set; }

    [Required]
    [MaxLength(FooterSettingsConsts.MaxTitleLength)]
    public string Title { get; set; }

    [Required]
    [Url]
    public string Url { get; set; }

    public Guid FooterSettingSectionId { get; set; }

    public UpdateFooterSettingLinkDto(int index)
    {
        Index = index;
    }
}
