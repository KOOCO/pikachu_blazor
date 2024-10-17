using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.GroupBuyProductRankings;

public class GroupBuyProductRankingDto : AuditedEntityDto<Guid>
{
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string? Content { get; set; }
    public int? ModuleNumber { get; set; }
}
