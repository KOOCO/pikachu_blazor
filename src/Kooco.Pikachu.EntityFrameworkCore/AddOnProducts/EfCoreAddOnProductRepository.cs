using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.AddOnProducts;
public class EfCoreAddOnProductRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : 
    EfCoreRepository<PikachuDbContext, AddOnProduct, Guid>(dbContextProvider), IAddOnProductRepository
{
    public async Task<long> GetCountAsync(string? filter, int? minAddOnAmount = null, int? maxAddOnAmount = null, int? minAddOnLimitPerOrder = null, int? maxAddOnLimitPerOrder = null, string? quantitySetting = null, int? minAvailableQuantity = null, int? maxAvailableQuantity = null, string? displayOriginalPrice = null, DateTime? startDate = null, DateTime? endDate = null, string? addOnConditions = null, int? minMinimumSpendAmount = null, int? maxMinimumSpendAmount = null, string? groupbuysScope = null, bool? status = null, int? minSellingQuantity = null, int? maxSellingQuantity = null, Guid? productId = null)
    {
        var quaryable = await GetFilteredQueryableAsync(filter, minAddOnAmount, maxAddOnAmount, minAddOnLimitPerOrder, maxAddOnLimitPerOrder, quantitySetting, minAvailableQuantity, maxAvailableQuantity, displayOriginalPrice, startDate, endDate, addOnConditions, minMinimumSpendAmount, maxMinimumSpendAmount, groupbuysScope, status, minSellingQuantity, maxSellingQuantity, productId);
        return await quaryable.LongCountAsync();

    }
    public async Task<List<AddOnProduct>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter, int? minAddOnAmount = null, int? maxAddOnAmount = null, int? minAddOnLimitPerOrder = null, int? maxAddOnLimitPerOrder = null, string? quantitySetting = null, int? minAvailableQuantity = null, int? maxAvailableQuantity = null, string? displayOriginalPrice = null, DateTime? startDate = null, DateTime? endDate = null, string? addOnConditions = null, int? minMinimumSpendAmount = null, int? maxMinimumSpendAmount = null, string? groupbuysScope = null, bool? status = null, int? minSellingQuantity = null, int? maxSellingQuantity = null, Guid? productId = null)
    {
        var quaryable = await GetFilteredQueryableAsync(filter, minAddOnAmount, maxAddOnAmount, minAddOnLimitPerOrder, maxAddOnLimitPerOrder, quantitySetting, minAvailableQuantity, maxAvailableQuantity, displayOriginalPrice, startDate, endDate, addOnConditions, minMinimumSpendAmount, maxMinimumSpendAmount, groupbuysScope, status, minSellingQuantity, maxSellingQuantity, productId);
        return await quaryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }
    public async Task<IQueryable<AddOnProduct>> GetFilteredQueryableAsync(string? filter, int? minAddOnAmount = null, int? maxAddOnAmount = null, int? minAddOnLimitPerOrder = null, int? maxAddOnLimitPerOrder = null, string? quantitySetting = null, int? minAvailableQuantity = null, int? maxAvailableQuantity = null, string? displayOriginalPrice = null, DateTime? startDate = null, DateTime? endDate = null, string? addOnConditions = null, int? minMinimumSpendAmount = null, int? maxMinimumSpendAmount = null, string? groupbuysScope = null, bool? status = null, int? minSellingQuantity = null, int? maxSellingQuantity = null, Guid? productId = null)
    {
        var queryable = await GetQueryableAsync();
        return queryable.Include(x => x.AddOnProductSpecificGroupbuys).Include(x => x.Product)
            .WhereIf(productId.HasValue, x => x.ProductId == productId)
            .WhereIf(minAddOnAmount.HasValue, x => x.AddOnAmount >= minAddOnAmount)
            .WhereIf(maxAddOnAmount.HasValue, x => x.AddOnAmount <= maxAddOnAmount)
            .WhereIf(minAddOnLimitPerOrder.HasValue, x => x.AddOnLimitPerOrder >= minAddOnLimitPerOrder)
            .WhereIf(maxAddOnLimitPerOrder.HasValue, x => x.AddOnLimitPerOrder <= maxAddOnLimitPerOrder)
            .WhereIf(!string.IsNullOrWhiteSpace(quantitySetting), x => x.QuantitySetting.Contains(quantitySetting))
            .WhereIf(minAvailableQuantity.HasValue, x => x.AvailableQuantity >= minAvailableQuantity)
            .WhereIf(maxAvailableQuantity.HasValue, x => x.AvailableQuantity <= maxAvailableQuantity)
            .WhereIf(!string.IsNullOrWhiteSpace(displayOriginalPrice), x => x.DisplayOriginalPrice.Contains(displayOriginalPrice))
            .WhereIf(startDate.HasValue, x => x.StartDate >= startDate)

            .WhereIf(endDate.HasValue, x => x.EndDate <= endDate)
            .WhereIf(!string.IsNullOrWhiteSpace(addOnConditions), x => x.AddOnConditions.Contains(addOnConditions))
            .WhereIf(minMinimumSpendAmount.HasValue, x => x.MinimumSpendAmount >= minMinimumSpendAmount)
            .WhereIf(maxMinimumSpendAmount.HasValue, x => x.MinimumSpendAmount <= maxMinimumSpendAmount)
            .WhereIf(!string.IsNullOrWhiteSpace(groupbuysScope), x => x.GroupbuysScope.Contains(groupbuysScope))
            .WhereIf(status.HasValue, x => x.Status == status)
            .WhereIf(minSellingQuantity.HasValue, x => x.SellingQuantity >= minSellingQuantity)
            .WhereIf(maxSellingQuantity.HasValue, x => x.SellingQuantity <= maxSellingQuantity)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x =>
                (x.QuantitySetting != null && x.QuantitySetting.Contains(filter)) ||
                (x.AddOnConditions != null && x.AddOnConditions.Contains(filter)) ||
                (x.DisplayOriginalPrice != null && x.DisplayOriginalPrice.Contains(filter)));
    }
    public async Task<AddOnProduct> GetWithDetailAsync(Guid id)
    {
        return await (await GetQueryableAsync()).Include(x => x.AddOnProductSpecificGroupbuys).Where(x => x.Id == id).FirstOrDefaultAsync();
    }
}