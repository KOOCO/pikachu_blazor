using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Members;

public class MemberMessagesWithCountDto
{
    public long TotalCount { get; set; }
    public Guid MemberId { get; set; }
    public List<MemberMessageDto> Messages { get; set; } = [];
}

public class MemberMessageDto
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
