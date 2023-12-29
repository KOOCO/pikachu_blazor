using Kooco.Pikachu.EnumValues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class AutomaticEmail : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string? TradeName { get; set; }
        public string? Recipients { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime SendTimeUTC { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public JobStatus LastJobStatus { get; set; }
        public List<AutomaticEmailGroupBuys> GroupBuys { get; set; }

        [NotMapped]
        public List<string>? RecipientsList
        {
            get
            {
                return string.IsNullOrEmpty(Recipients)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(Recipients);
            }
        }

        public AutomaticEmail(
            Guid id,
            string? tradeName,
            string? recipients,
            DateTime startDate,
            DateTime endDate,
            DateTime sendTime,
            DateTime sendTimeUTC,
            RecurrenceType recurrenceType
            ) : base(id)
        {
            TradeName = tradeName;
            Recipients = recipients;
            StartDate = startDate;
            EndDate = endDate;
            SendTime = sendTime;
            SendTimeUTC = sendTimeUTC;
            RecurrenceType = recurrenceType;
            GroupBuys = [];
        }

        public void AddGroupBuy(Guid id, Guid groupBuyId)
        {
            GroupBuys.Add(new AutomaticEmailGroupBuys(id, Id, groupBuyId));
        }
    }
}
