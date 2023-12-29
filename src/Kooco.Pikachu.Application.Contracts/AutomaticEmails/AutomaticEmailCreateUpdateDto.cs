using System.Collections.Generic;
using System;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class AutomaticEmailCreateUpdateDto
    {
        public string TradeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? SendTimeUTC { get; set; }
        public RecurrenceType RecurrenceType { get; set; }

        public List<string> RecipientsList { get; set; }
        public List<Guid> GroupBuyIds { get; set; } = new List<Guid>();

        public AutomaticEmailCreateUpdateDto()
        {
            RecipientsList = new List<string>();
            GroupBuyIds = new List<Guid>();
        }
    }
}