using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Members.MemberTags;

public class AddTagForUsersDto
{
    public Guid? EditingId { get; set; }

    [MaxLength(MemberTagConsts.MemberTagNameMaxLength)]
    public string Name { get; set; } = "";

    public IEnumerable<string> MemberTypes { get; set; } = [];
    public IEnumerable<string> MemberTags { get; set; } = [];
    public int? AmountSpent { get; set; }
    public int? OrdersCompleted { get; set; }
    public DateTime?[] RegistrationDateRange { get; set; } = [null, null];
    public DateTime? MinRegistrationDate { get { return RegistrationDateRange?.Length > 0 ? RegistrationDateRange[0] : null; } }
    public DateTime? MaxRegistrationDate { get { return RegistrationDateRange?.Length > 1 ? RegistrationDateRange[1] : null; } }
}
