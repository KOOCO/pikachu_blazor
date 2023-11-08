using System.Collections.Generic;
using System;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class AutomaticEmailCreateUpdateDto
    {
        public string TradeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SendTime { get; set; }

        public List<string> RecipientsList { get; set; }
        public List<Guid> GroupBuyIds { get; set; }

        public AutomaticEmailCreateUpdateDto()
        {
            RecipientsList = new List<string>();
            GroupBuyIds = new List<Guid>();
        }
    }
}