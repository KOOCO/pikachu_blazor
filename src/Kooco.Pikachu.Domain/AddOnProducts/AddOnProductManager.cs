using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Kooco.Pikachu.AddOnProducts
{
    public class AddOnProductManager(IAddOnProductRepository addOnProductRepository,IRepository<AddOnProductSpecificGroupbuy,Guid> repository):DomainService
    {
        public async Task<AddOnProduct> CreateAsync(int addOnAmount,
        int addOnLimitPerOrder,
        string quantitySetting,
        int availableQuantity,
        string displayOriginalPrice,
        DateTime startDate,
        DateTime endDate,
        string addOnConditions,
        int minimumSpendAmount,
        string groupbuysScope,
        bool status,
        int sellingQuantity,
        Guid productId,
        List<Guid> groupbuyIds)
        {
            var addOnProduct = new AddOnProduct(GuidGenerator.Create(), addOnAmount, addOnLimitPerOrder, quantitySetting, availableQuantity, displayOriginalPrice, startDate, endDate, addOnConditions, minimumSpendAmount, groupbuysScope, status, sellingQuantity, productId);
            addOnProduct.AddGroupbuys(groupbuyIds);
            return await addOnProductRepository.InsertAsync(addOnProduct);
        
        }

        public async Task<AddOnProduct> UpdateAsync(
    Guid id,
    int addOnAmount,
    int addOnLimitPerOrder,
    string quantitySetting,
    int availableQuantity,
    string displayOriginalPrice,
    DateTime startDate,
    DateTime endDate,
    string addOnConditions,
    int minimumSpendAmount,
    string groupbuysScope,
    bool status,
    int sellingQuantity,
    Guid productId,
    List<Guid> groupbuyIds)
        {
            // Fetch the existing AddOnProduct
            var existingAddOnProduct = await addOnProductRepository.GetAsync(id);

            if (existingAddOnProduct == null)
            {
                throw new EntityNotFoundException(typeof(AddOnProduct), id);
            }

            // Update properties of the existing AddOnProduct
            existingAddOnProduct.AddOnAmount = addOnAmount;
            existingAddOnProduct.AddOnLimitPerOrder = addOnLimitPerOrder;
            existingAddOnProduct.QuantitySetting = quantitySetting;
            existingAddOnProduct.AvailableQuantity = availableQuantity;
            existingAddOnProduct.DisplayOriginalPrice = displayOriginalPrice;
            existingAddOnProduct.StartDate = startDate;
            existingAddOnProduct.EndDate = endDate;
            existingAddOnProduct.AddOnConditions = addOnConditions;
            existingAddOnProduct.MinimumSpendAmount = minimumSpendAmount;
            existingAddOnProduct.GroupbuysScope = groupbuysScope;
            existingAddOnProduct.Status = status;
            existingAddOnProduct.SellingQuantity = sellingQuantity;
            existingAddOnProduct.ProductId = productId;

            existingAddOnProduct.UpdateGroupbuys(groupbuyIds);
            await repository.InsertManyAsync(existingAddOnProduct.AddOnProductSpecificGroupbuys);
            // Update the AddOnProduct in the repository
            return await addOnProductRepository.UpdateAsync(existingAddOnProduct);
        }
    }
}
