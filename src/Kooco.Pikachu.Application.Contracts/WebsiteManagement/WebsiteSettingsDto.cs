using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement;

public class WebsiteSettingsDto : FullAuditedEntityDto<Guid>
{
    public string NotificationBar { get; set; }
    public string LogoName { get; set; }
    public string LogoUrl { get; set; }
    public string StoreTitle { get; set; }
    public WebsiteTitleDisplayOptions TitleDisplayOption { get; set; }
    public string Facebook { get; set; }
    public string Instagram { get; set; }
    public string Line { get; set; }
    public string ReturnExchangePolicy { get; set; }
    public Guid? TenantId { get; set; }
}