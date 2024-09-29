using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.AddOnProducts
{
    public interface IAddOnProductRepository : IRepository<AddOnProduct, Guid>
    {
        Task<long> GetCountAsync(string?filter,int? minAddOnAmount = null,
        int? maxAddOnAmount = null,
        int? minAddOnLimitPerOrder = null,
        int? maxAddOnLimitPerOrder = null,
        string? quantitySetting = null,
        int? minAvailableQuantity = null,
        int? maxAvailableQuantity = null,
        string? displayOriginalPrice = null,
        DateTime? startDate = null,
        
        DateTime? endDate = null,
        string? addOnConditions = null,
        int? minMinimumSpendAmount = null,
        int? maxMinimumSpendAmount = null,
        string? groupbuysScope = null,
        bool? status = null,
        int? minSellingQuantity = null,
        int? maxSellingQuantity = null,
        Guid? productId = null);

        Task<AddOnProduct> GetWithDetailAsync(Guid id);
        Task<List<AddOnProduct>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
           string? filter, int? minAddOnAmount = null,
        int? maxAddOnAmount = null,
        int? minAddOnLimitPerOrder = null,
        int? maxAddOnLimitPerOrder = null,
        string? quantitySetting = null,
        int? minAvailableQuantity = null,
        int? maxAvailableQuantity = null,
        string? displayOriginalPrice = null,
        DateTime? startDate = null,
         
        DateTime? endDate = null,
        string? addOnConditions = null,
        int? minMinimumSpendAmount = null,
        int? maxMinimumSpendAmount = null,
        string? groupbuysScope = null,
        bool? status = null,
        int? minSellingQuantity = null,
        int? maxSellingQuantity = null,
        Guid? productId = null);
    }
}
