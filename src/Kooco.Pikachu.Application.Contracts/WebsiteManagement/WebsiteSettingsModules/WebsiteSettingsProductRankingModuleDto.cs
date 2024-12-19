using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsProductRankingModuleDto
{
    public Guid Id { get; set; }
    public Guid WebsiteSettingsId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string? Content { get; set; }
    public int? ModuleNumber { get; set; }
    public Guid? TenantId { get; set; }

    public List<CreateImageDto> Images { get; set; }
}
