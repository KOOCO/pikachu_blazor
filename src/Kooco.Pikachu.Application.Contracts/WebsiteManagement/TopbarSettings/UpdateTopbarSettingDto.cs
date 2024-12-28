using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public class UpdateTopbarSettingDto : IHasConcurrencyStamp
{
    [Required]
    public TopbarStyleSettings? TopbarStyleSettings { get; set; }

    [Required]
    public List<UpdateTopbarSettingLinkDto> Links { get; set; } = [];

    public string ConcurrencyStamp { get; set; }
}