using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.GroupBuyProductRankings;

public class GroupBuyProductRanking : Entity<Guid>
{
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string? Content { get; set; }
    public int? ModuleNumber { get; set; }
}
