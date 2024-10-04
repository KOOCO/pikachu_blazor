using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShoppingCredits
{
    public interface IShoppingCreditUsageSettingRepository:IRepository<ShoppingCreditUsageSetting,Guid>
    {
        Task<ShoppingCreditUsageSetting> GetWithDetailsAsync(Guid id);
    }
}
