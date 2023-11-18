using Kooco.Pikachu.ElectronicInvoiceSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.DeliveryTempratureCosts
{
    public interface IDeliveryTemperatureCostRepository : IRepository<DeliveryTemperatureCost>
    {
    }
}
