using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Members.MemberTags;

public class MemberTagFilterDto
{
    public string Tag { get; set; }
    public int? AmountSpent { get; set; }
    public int? OrdersCompleted { get; set; }
    public DateTime? MinRegistrationDate { get; set; }
    public DateTime? MaxRegistrationDate { get; set; }
    public Guid? TenantId { get; set; }
    public IReadOnlyList<string> MemberTags { get; set; }
    public IReadOnlyList<string> MemberTypes { get; set; }
}
