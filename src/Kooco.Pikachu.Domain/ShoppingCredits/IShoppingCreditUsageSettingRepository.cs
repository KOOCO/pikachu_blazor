using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShoppingCredits
{
    public interface IShoppingCreditUsageSettingRepository:IRepository<ShoppingCreditUsageSetting,Guid>
    {
        Task<ShoppingCreditUsageSetting> GetWithDetailsAsync(Guid id);
        Task<ShoppingCreditUsageSetting> GetFirstByGroupBuyIdAsync(Guid groupBuyId);
    }
}
