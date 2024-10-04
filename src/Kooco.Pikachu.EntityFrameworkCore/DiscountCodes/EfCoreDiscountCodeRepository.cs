using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.DiscountCodes
{
    public class EfCoreDiscountCodeRepository : EfCoreRepository<PikachuDbContext, DiscountCode, Guid>, IDiscountCodeRepository
    {
        public EfCoreDiscountCodeRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(
            string? filter,
            bool? status,
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
            int? discountAmount = null)
        {
            var queryable = await GetFilteredQueryableAsync(filter,status, eventName, startDate, endDate, discountCode, specifiedCode, availableQuantity, maxUsePerPerson, groupbuysScope, productsScope, discountMethod, minimumSpendAmount, shippingDiscountScope, specificShippingMethods, discountPercentage, discountAmount);
            return await queryable.LongCountAsync();
        }

        public async Task<List<DiscountCode>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string? filter,
            bool? status,
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
            int? discountAmount = null)
        {
            var queryable = await GetFilteredQueryableAsync(filter,status, eventName, startDate, endDate, discountCode, specifiedCode, availableQuantity, maxUsePerPerson, groupbuysScope, productsScope, discountMethod, minimumSpendAmount, shippingDiscountScope, specificShippingMethods, discountPercentage, discountAmount);
            return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
        }

        public async Task<IQueryable<DiscountCode>> GetFilteredQueryableAsync(
            string? filter,
            bool? status,
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
            int? discountAmount = null)
        {
            var queryable = await GetQueryableAsync();

            return queryable
                .WhereIf(!string.IsNullOrWhiteSpace(eventName), x => x.EventName.Contains(eventName))
                .WhereIf(status.HasValue, x => x.Status == status)
                .WhereIf(startDate.HasValue, x => x.StartDate >= startDate)
                .WhereIf(endDate.HasValue, x => x.EndDate <= endDate)
                .WhereIf(!string.IsNullOrWhiteSpace(discountCode), x => x.Code.Contains(discountCode))
                .WhereIf(!string.IsNullOrWhiteSpace(specifiedCode), x => x.SpecifiedCode.Contains(specifiedCode))
                .WhereIf(availableQuantity.HasValue, x => x.AvailableQuantity >= availableQuantity)
                .WhereIf(maxUsePerPerson.HasValue, x => x.MaxUsePerPerson <= maxUsePerPerson)
                .WhereIf(!string.IsNullOrWhiteSpace(groupbuysScope), x => x.GroupbuysScope.Contains(groupbuysScope))
                .WhereIf(!string.IsNullOrWhiteSpace(productsScope), x => x.ProductsScope.Contains(productsScope))
                .WhereIf(!string.IsNullOrWhiteSpace(discountMethod), x => x.DiscountMethod.Contains(discountMethod))
                .WhereIf(minimumSpendAmount.HasValue, x => x.MinimumSpendAmount >= minimumSpendAmount)
                .WhereIf(!string.IsNullOrWhiteSpace(shippingDiscountScope), x => x.ShippingDiscountScope.Contains(shippingDiscountScope))
                .WhereIf(specificShippingMethods.HasValue, x => x.SpecificShippingMethods.Contains(specificShippingMethods.Value))
                .WhereIf(discountPercentage.HasValue, x => x.DiscountPercentage == discountPercentage)
                .WhereIf(discountAmount.HasValue, x => x.DiscountAmount == discountAmount)
                .WhereIf(!string.IsNullOrWhiteSpace(filter), x =>
                    (x.EventName != null && x.EventName.Contains(filter)) ||
                    (x.Code != null && x.Code.Contains(filter)) ||
                    (x.SpecifiedCode != null && x.SpecifiedCode.Contains(filter)));
        }

        public async Task<DiscountCode> GetWithDetailAsync(Guid id)
        {
            return await (await GetQueryableAsync())
                .Include(x => x.DiscountSpecificGroupbuys) // Assuming a similar relationship as AddOnProduct
                .Include(x => x.DiscountSpecificProducts) // Assuming a relationship with Products
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Item?>> GetProductsAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            var products = await (from dsp in dbContext.DiscountSpecificProducts
                                  join p in dbContext.Items on dsp.ProductId equals p.Id
                                  where dsp.DiscountCodeId == id
                                  select p).ToListAsync();

            return products;


        }
        public async Task<List<GroupBuy?>> GetGroupbuysAsync(Guid id)
        {
            return await (await GetDbContextAsync()).DiscountSpecificGroupbuys.Where(x => x.DiscountCodeId == id).Include(x => x.Groupbuy).Select(x => x.Groupbuy).ToListAsync();


        }
    }
}