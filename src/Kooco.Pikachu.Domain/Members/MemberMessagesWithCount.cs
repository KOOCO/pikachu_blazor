using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Members;

public class MemberMessagesWithCount
{
    public long TotalCount { get; set; }
    public Guid MemberId { get; set; }
    public List<OrderMessage> Messages { get; set; } = [];
}
