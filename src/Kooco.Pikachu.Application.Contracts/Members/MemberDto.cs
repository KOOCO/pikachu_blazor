﻿using Kooco.Pikachu.Members.MemberTags;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.Members;

public class MemberDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? Birthday { get; set; }
    public string? MobileNumber { get; set; }
    public string? Gender { get; set; }
    public int TotalOrders { get; set; }
    public int TotalSpent { get; set; }
    public string? LineId { get; set; }
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public DateTime CreationTime { get; set; }
    public List<MemberTagDto> MemberTags { get; set; }
    public bool IsNew
    {
        get
        {
            return MemberTags?.Any(tag => tag.Name == MemberConsts.MemberTags.New) ?? false;
        }
    }

    public bool IsBlacklisted
    {
        get
        {
            return MemberTags?.Any(tag => tag.Name == MemberConsts.MemberTags.Blacklisted) ?? false;
        }
    }
}