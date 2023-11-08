using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class AutomaticEmailDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string? TradeName { get; set; }
        public string? Recipients { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SendTime { get; set; }
        public RecurrenceType RecurrenceType { get; set; }

        public List<string> RecipientsList { get; set; }
        public List<AutomaticEmailGroupBuysDto> GroupBuys { get; set; }
    }
}