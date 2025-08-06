using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class RetryBatchResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<RetryRecordResult> Results { get; set; } = new();
    }
}
