using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public class RetryBatchInput
    {
        public List<Guid> RecordIds { get; set; } = new();
    }
}
