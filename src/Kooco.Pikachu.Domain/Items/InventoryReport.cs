using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Items
{
    public class InventoryReport
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public int CurrentStock { get; set; }
        public int AvailableStock { get; set; }
        public int PreorderQuantity { get; set; }
        public int AvailablePreorderQuantity { get; set; }

    }
}
