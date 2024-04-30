using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyList
    {
        public Guid Id { get; set; }
        public string GroupBuyName { get; set; }
         public DateTime?  StartTime { get; set; }
        public DateTime? EndTime { get; set; }
         public DateTime   CreationTime { get; set; }
         public bool IsGroupBuyAvaliable { get; set; }
    }
}
