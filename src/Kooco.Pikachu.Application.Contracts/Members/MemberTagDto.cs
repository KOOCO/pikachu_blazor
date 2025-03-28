using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class MemberTagDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; private set; }
    public Guid? VipTierId { get; set; }
}
