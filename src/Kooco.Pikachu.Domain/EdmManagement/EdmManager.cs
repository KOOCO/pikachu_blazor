using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.EdmManagement;

public class EdmManager(IEdmRepository edmRepository) : DomainService
{
    public async Task<Edm> CreateAsync(EdmTemplateType templateType, Guid? campaignId, bool applyToAllMembers,
        IEnumerable<string> memberTags, bool applyToAllGroupBuys, IEnumerable<Guid> groupBuyIds, DateTime startDate,
        DateTime? endDate, DateTime sendTime, EdmSendFrequency? sendFrequency, string subject, string message)
    {
        var edm = new Edm(GuidGenerator.Create(), templateType, campaignId, applyToAllMembers, memberTags,
            applyToAllGroupBuys, groupBuyIds, startDate, endDate, sendTime, sendFrequency, subject, message);

        await edmRepository.InsertAsync(edm);
        return edm;
    }

    public async Task<Edm> UpdateAsync(Edm edm, EdmTemplateType templateType, Guid? campaignId, bool applyToAllMembers,
        IEnumerable<string> memberTags, bool applyToAllGroupBuys, IEnumerable<Guid> groupBuyIds, DateTime startDate,
        DateTime? endDate, DateTime sendTime, EdmSendFrequency? sendFrequency, string subject, string message)
    {
        Check.NotNull(edm, nameof(Edm));

        edm.SetTemplateType(templateType, campaignId, sendFrequency);
        edm.SetApplyToAllMembers(applyToAllMembers, memberTags);
        edm.SetApplyToAllGroupBuys(applyToAllGroupBuys, groupBuyIds);
        edm.SetDateRange(templateType, startDate, endDate, sendTime);
        edm.SetSubject(subject);
        edm.SetMessage(message);

        await edmRepository.UpdateAsync(edm);
        return edm;
    }
}
