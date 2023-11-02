using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Freebies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public class ElectronicInvoiceSettingRepository : EfCoreRepository<PikachuDbContext, ElectronicInvoiceSetting, Guid>, IElectronicInvoiceSettingRepository
    {
        public ElectronicInvoiceSettingRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
