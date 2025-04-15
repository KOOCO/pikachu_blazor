using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members.MemberTags;

public class MemberTagDto : CreationAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsSystemAssigned { get; set; }
    public Guid? VipTierId { get; set; }
}
