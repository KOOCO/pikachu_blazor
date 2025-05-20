using Kooco.Pikachu.Campaigns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.EdmManagement;

public class Edm : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public EdmTemplateType TemplateType { get; private set; }
    public Guid? CampaignId { get; private set; }
    public EdmMemberType MemberType { get; private set; }
    public string? MemberTagsJson { get; private set; }
    public bool ApplyToAllGroupBuys { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime SendTime { get; private set; }
    public DateTime SendTimeUtc { get; set; }
    public EdmSendFrequency? SendFrequency { get; private set; }

    [MaxLength(EdmConsts.MaxSubjectLength)]
    public string Subject { get; private set; }
    public string Message { get; private set; }
    public Guid? TenantId { get; private set; }
    public string? JobId { get; set; }
    public virtual ICollection<EdmGroupBuy> GroupBuys { get; private set; } = [];

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [NotMapped]
    public IEnumerable<string> MemberTags
    {
        get
        {
            return !string.IsNullOrWhiteSpace(MemberTagsJson)
                    ? JsonSerializer.Deserialize<List<string>>(MemberTagsJson) ?? []
                    : [];
        }
    }
    private Edm() { }

    internal Edm(
        Guid id,
        EdmTemplateType templateType,
        Guid? campaignId,
        EdmMemberType memberType,
        IEnumerable<string> memberTags,
        bool applyToAllGroupBuys,
        IEnumerable<Guid> groupBuyIds,
        DateTime startDate,
        DateTime? endDate,
        DateTime sendTime,
        EdmSendFrequency? sendFrequency,
        string subject,
        string message
        ) : base(id)
    {
        SetTemplateType(templateType, campaignId, sendFrequency);
        SetMemberType(memberType, memberTags);
        SetApplyToAllGroupBuys(applyToAllGroupBuys, groupBuyIds);
        SetDateRange(templateType, startDate, endDate, sendTime);
        SetSubject(subject);
        SetMessage(message);
    }

    public void SetTemplateType(EdmTemplateType templateType, Guid? campaignId, EdmSendFrequency? sendFrequency)
    {
        TemplateType = templateType;

        CampaignId = templateType == EdmTemplateType.Campaign
            ? Check.NotDefaultOrNull(campaignId, nameof(CampaignId))
            : null;

        SendFrequency = templateType == EdmTemplateType.ShoppingCart
            ? Check.NotNull(sendFrequency, nameof(SendFrequency))
            : null;
    }

    public void SetMemberType(EdmMemberType memberType, IEnumerable<string> memberTags)
    {
        MemberType = memberType;

        if (MemberType == EdmMemberType.SpecificMemberTags)
        {
            Check.NotNull(memberTags, nameof(MemberTags));
            MemberTagsJson = JsonSerializer.Serialize(memberTags);
        }
        else
        {
            MemberTagsJson = null;
        }
    }

    public void SetApplyToAllGroupBuys(bool applyToAllGroupBuys, IEnumerable<Guid> groupBuyIds)
    {
        ApplyToAllGroupBuys = applyToAllGroupBuys;

        if (!applyToAllGroupBuys)
        {
            if (!groupBuyIds?.Any() ?? true)
                throw new UserFriendlyException("The field Group Buys is required.");

            GroupBuys = [.. groupBuyIds!.Distinct().Select(id => new EdmGroupBuy(Guid.NewGuid(), Id, id))];
        }
        else
        {
            GroupBuys.Clear();
        }
    }

    public void SetDateRange(EdmTemplateType templateType, DateTime startDate, DateTime? endDate, DateTime sendTime)
    {
        TemplateType = templateType;

        if (startDate < DateTime.Today)
        {
            throw new UserFriendlyException("Start date cannot be in the past.");
        }

        StartDate = startDate;

        if (templateType == EdmTemplateType.ShoppingCart)
        {
            if (!endDate.HasValue || endDate.Value.Date < startDate.Date)
                throw new UserFriendlyException("End date must be after or equal to start date.");
            EndDate = endDate;
        }
        else
        {
            EndDate = null;
        }

        if (StartDate.Date == DateTime.Today)
        {
            var now = DateTime.Now.AddMinutes(1);
            var nowTime = new TimeSpan(now.Hour, now.Minute, 0);
            var sendTimeOnly = new TimeSpan(sendTime.Hour, sendTime.Minute, 0);

            if (sendTimeOnly < nowTime)
            {
                throw new UserFriendlyException("Send time must be atleast 1 minute ahead of current time.");
            }
        }

        SendTime = Check.NotNull(sendTime, nameof(sendTime));
        SendTimeUtc = SendTime.ToUniversalTime();
    }

    public void SetSubject(string subject)
    {
        Subject = Check.NotNullOrWhiteSpace(subject, nameof(Subject), EdmConsts.MaxSubjectLength);
    }

    public void SetMessage(string message)
    {
        Message = Check.NotNullOrWhiteSpace(message, nameof(Message));
    }

    public void SetJobId(string? jobId)
    {
        JobId = jobId;
    }
}
