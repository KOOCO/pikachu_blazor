using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.GroupBuyOrderInstructions;

public class GroupBuyOrderInstruction : Entity<Guid>
{
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? BodyText { get; set; }
    public int? ModuleNumber { get; set; }
}
