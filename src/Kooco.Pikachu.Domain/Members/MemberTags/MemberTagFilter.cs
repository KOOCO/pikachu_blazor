using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Members.MemberTags;

public class MemberTagFilter : Entity<Guid>, IMultiTenant
{
    public string Tag { get; set; }
    public string? MemberTypesJson { get; private set; }
    public string? MemberTagsJson { get; private set; }
    public int? AmountSpent { get; set; }
    public int? OrdersCompleted { get; set; }
    public DateTime? MinRegistrationDate { get; set; }
    public DateTime? MaxRegistrationDate { get; set; }
    public Guid? TenantId { get; set; }

    [NotMapped]
    public IReadOnlyList<string> MemberTags => Deserialize(MemberTagsJson);

    [NotMapped]
    public IReadOnlyList<string> MemberTypes => Deserialize(MemberTypesJson);


    private MemberTagFilter() { }

    public MemberTagFilter(
        Guid id,
        string tag,
        IEnumerable<string> memberTypes,
        IEnumerable<string> memberTags,
        int? amountSpent,
        int? ordersCompleted,
        DateTime? minRegistrationDate,
        DateTime? maxRegistrationDate
        ) : base(id)
    {
        Tag = Check.NotNullOrWhiteSpace(tag, nameof(Tag), maxLength: MemberTagConsts.MemberTagNameMaxLength);
        SetMemberTypes(memberTypes);
        SetTags(memberTags);
        AmountSpent = amountSpent;
        OrdersCompleted = ordersCompleted;
        MinRegistrationDate = minRegistrationDate;
        MaxRegistrationDate = maxRegistrationDate;
    }

    public void SetMemberTypes(IEnumerable<string>? types)
    {
        var typesList = types?.Where(type => !string.IsNullOrWhiteSpace(type)).Distinct().ToList() ?? [];
        MemberTypesJson = JsonSerializer.Serialize(typesList);
    }

    public void SetTags(IEnumerable<string>? tags)
    {
        var tagList = tags?.Where(tag => !string.IsNullOrWhiteSpace(tag)).Distinct().ToList() ?? [];
        MemberTagsJson = JsonSerializer.Serialize(tagList);
    }

    private static List<string> Deserialize(string? input)
    {
        return string.IsNullOrWhiteSpace(input)
                    ? []
                    : JsonSerializer.Deserialize<List<string>>(input) ?? [];
    }
}
