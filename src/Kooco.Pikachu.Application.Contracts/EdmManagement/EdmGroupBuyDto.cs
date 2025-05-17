using System;

namespace Kooco.Pikachu.EdmManagement;

public class EdmGroupBuyDto
{
    public Guid Id { get; set; }
    public Guid EdmId { get; set; }
    public Guid GroupBuyId { get; set; }
    public string GroupBuyName { get; set; } = string.Empty;
}
