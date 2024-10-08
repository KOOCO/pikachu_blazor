using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.GroupBuyOrderInstructions;

public class CreateUpdateGroupBuyOrderInstructionDto
{
    public Guid Id { get; set; }
    public Guid GroupBuyId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? BodyText { get; set; }
}
