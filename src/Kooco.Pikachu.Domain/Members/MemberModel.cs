using System;
using System.Collections.Generic;
using Volo.Abp.Data;

namespace Kooco.Pikachu.Members;

public class MemberModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? Birthday { get; set; }
    public int TotalOrders { get; set; }
    public int TotalSpent { get; set; }
    public string? LineId { get; set; }
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public List<string> MemberTags { get; set; }
}
