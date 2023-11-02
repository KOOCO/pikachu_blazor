using Kooco.Pikachu.Freebies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public interface IElectronicInvoiceSettingRepository : IRepository<ElectronicInvoiceSetting>
    {
    }
}
