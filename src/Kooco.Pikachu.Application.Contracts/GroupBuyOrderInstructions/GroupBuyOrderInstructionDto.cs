using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.GroupBuyOrderInstructions;

public class GroupBuyOrderInstructionDto : AuditedEntityDto<Guid>
{
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? BodyText { get; set; }
    public int? ModuleNumber { get; set; }
}
