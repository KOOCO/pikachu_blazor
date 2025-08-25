using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Members;

public class MemberMessagesWithCount
{
    public long TotalCount { get; set; }
    public Guid MemberId { get; set; }
    public List<MemberMessageModel> Messages { get; set; } = [];
}

public class MemberMessageModel
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public Guid MessageId { get; set; }
    public Guid GroupBuyId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public bool IsMerchant { get; set; }
    public DateTime CreationTime { get; set; }
}