using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class RetryRecordResult
    {
        public Guid RecordId { get; set; }
        public bool Success { get; set; }
        public string Reason { get; set; }
    }

}
