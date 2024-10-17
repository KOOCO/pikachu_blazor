using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuyProductRankings;

public class CreateUpdateGroupBuyProductRankingDto
{
    public Guid Id { get; set; }
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string? Content { get; set; }
    public int? ModuleNumber { get; set; }
}
