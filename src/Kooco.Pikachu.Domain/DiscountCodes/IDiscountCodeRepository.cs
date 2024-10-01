using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.DiscountCodes
{
    public interface IDiscountCodeRepository : IRepository<DiscountCode, Guid>
    {
        Task<long> GetCountAsync(
            string? filter,
            string? eventName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? discountCode = null,
            string? specifiedCode = null,
            int? availableQuantity = null,
            int? maxUsePerPerson = null,
            string? groupbuysScope = null,
            string? productsScope = null,
            string? discountMethod = null,
            int? minimumSpendAmount = null,
            string? shippingDiscountScope = null,
            int? specificShippingMethods = null,
            int? discountPercentage = null,
            int? discountAmount = null
        );

        Task<DiscountCode> GetWithDetailAsync(Guid id);

        Task<List<DiscountCode>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string? filter,
            string? eventName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? discountCode = null,
            string? specifiedCode = null,
            int? availableQuantity = null,
            int? maxUsePerPerson = null,
            string? groupbuysScope = null,
            string? productsScope = null,
            string? discountMethod = null,
            int? minimumSpendAmount = null,
            string? shippingDiscountScope = null,
            int? specificShippingMethods = null,
            int? discountPercentage = null,
            int? discountAmount = null
        );
        Task<List<Item?>> GetProductsAsync(Guid id);
        Task<List<GroupBuy?>> GetGroupbuysAsync(Guid id);
    }
}
