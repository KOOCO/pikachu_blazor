using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.AddOnProducts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(PikachuPermissions.AddOnProducts.Default)]
    public class AddOnProductAppService(IAddOnProductRepository addOnProductRepository, AddOnProductManager addOnProductManager) : PikachuAppService, IAddOnProductAppService
    {
        [Authorize(PikachuPermissions.AddOnProducts.Create)]
        public async Task<AddOnProductDto> CreateAsync(CreateUpdateAddOnProductDto input)
        {
            var addonProduct = await addOnProductManager.CreateAsync(input.AddOnAmount, input.AddOnLimitPerOrder, input.QuantitySetting, input.AvailableQuantity, input.DisplayOriginalPrice, input.StartDate, input.EndDate, input.AddOnConditions, input.MinimumSpendAmount, input.GroupbuysScope, input.Status, input.SellingQuantity, input.ProductId, input.GroupBuyIds);
            return ObjectMapper.Map<AddOnProduct, AddOnProductDto>(addonProduct);
        }
        [Authorize(PikachuPermissions.AddOnProducts.Delete)]
        public async Task DeleteAsync(Guid Id)
        {
            await addOnProductRepository.DeleteAsync(Id);
        }

        public async Task<AddOnProductDto> GetAsync(Guid Id)
        {
            var addOnProduct=await addOnProductRepository.GetWithDetailAsync(Id);
            
            return ObjectMapper.Map<AddOnProduct,AddOnProductDto>(addOnProduct);
        }

        public async Task<PagedResultDto<AddOnProductDto>> GetListAsync(GetAddOnProductListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(AddOnProduct.CreationTime) + " DESC";
            }
            var count = await addOnProductRepository.GetCountAsync(input.Filter, startDate: input.StartDate, endDate: input.EndDate,status:input.Status);
            var items = await addOnProductRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, startDate: input.StartDate, endDate: input.EndDate,status: input.Status);
            return new PagedResultDto<AddOnProductDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<AddOnProduct>, List<AddOnProductDto>>(items)

            };
        }
        [Authorize(PikachuPermissions.AddOnProducts.Edit)]
        public async Task<AddOnProductDto> UpdateAsync(Guid Id,CreateUpdateAddOnProductDto input)
            {
                var addonProduct = await addOnProductManager.UpdateAsync(Id,input.AddOnAmount, input.AddOnLimitPerOrder, input.QuantitySetting, input.AvailableQuantity, input.DisplayOriginalPrice, input.StartDate, input.EndDate, input.AddOnConditions, input.MinimumSpendAmount, input.GroupbuysScope, input.Status, input.SellingQuantity, input.ProductId, input.GroupBuyIds);
                return ObjectMapper.Map<AddOnProduct, AddOnProductDto>(addonProduct);
            }
        [Authorize(PikachuPermissions.DiscountCodes.Edit)] // Update this permission name as needed
        public async Task UpdateStatusAsync(Guid id)
        {
            var addOnProduct = await addOnProductRepository.GetAsync(id);
            addOnProduct.Status = !addOnProduct.Status;
            await addOnProductRepository.UpdateAsync(addOnProduct);
            return;
        }
    }
}
