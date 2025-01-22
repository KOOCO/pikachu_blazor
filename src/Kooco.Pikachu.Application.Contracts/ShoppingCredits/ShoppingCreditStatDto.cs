using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditStatDto
    {
        public double TodayIssueAmount { get; set; }
        public double ThisWeekIssueAmount { get; set; }
        public double ThisMonthIssueAmount { get; set; }
        public double TodayRedeemedAmount { get; set; }
        public double ThisWeekRedeemedAmount { get; set; }
        public double ThisMonthRedeemedAmount { get; set; }
    }
}
