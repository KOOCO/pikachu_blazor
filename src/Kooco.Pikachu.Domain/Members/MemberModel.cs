using System;
using Volo.Abp.Identity;

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
}
