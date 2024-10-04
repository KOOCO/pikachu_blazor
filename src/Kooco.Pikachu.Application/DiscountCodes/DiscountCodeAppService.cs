using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.GroupBuys;

namespace Kooco.Pikachu.DiscountCodes
{
    [RemoteService(IsEnabled = false)]
    [Authorize(PikachuPermissions.DiscountCodes.Default)] // Update this permission name as needed
    public class DiscountCodeAppService(IDiscountCodeRepository discountCodeRepository, DiscountCodeManager discountCodeManager) : PikachuAppService, IDiscountCodeAppService
    {
        [Authorize(PikachuPermissions.DiscountCodes.Create)] // Update this permission name as needed
        public async Task<DiscountCodeDto> CreateAsync(CreateUpdateDiscountCodeDto input)
        {
            var discountCode = await discountCodeManager.CreateAsync(input.EventName,input.Status,input.StartDate,input.EndDate,input.Code,input.SpecifiedCode,input.AvailableQuantity,input.MaxUsePerPerson,input.GroupbuysScope,input.ProductsScope,input.DiscountMethod,input.MinimumSpendAmount,input.ShippingDiscountScope,input.SpecificShippingMethods,input.DiscountPercentage,input.DiscountAmount,input.GroupbuyIds,input.ProductIds);
            return ObjectMapper.Map<DiscountCode, DiscountCodeDto>(discountCode);
        }

        [Authorize(PikachuPermissions.DiscountCodes.Delete)] // Update this permission name as needed
        public async Task DeleteAsync(Guid id)
        {
            await discountCodeRepository.DeleteAsync(id);
        }

        public async Task<DiscountCodeDto> GetAsync(Guid id)
        {
            var discountCode = await discountCodeRepository.GetWithDetailAsync(id);
            return ObjectMapper.Map<DiscountCode, DiscountCodeDto>(discountCode);
        }
        public async Task<List<ItemDto>> GetProductsAsync(Guid id)
        {
            var discountCode = await discountCodeRepository.GetProductsAsync(id);
            return ObjectMapper.Map<List<Item>, List<ItemDto>>(discountCode);
        }
        public async Task<List<GroupBuyDto>> GetGroupBuysAsync(Guid id)
        {
            var discountCode = await discountCodeRepository.GetGroupbuysAsync(id);
            return ObjectMapper.Map<List<GroupBuy>, List<GroupBuyDto>>(discountCode);
        }
        public async Task<PagedResultDto<DiscountCodeDto>> GetListAsync(GetDiscountCodeListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(DiscountCode.CreationTime) + " DESC";
            }

            var count = await discountCodeRepository.GetCountAsync(input.Filter,status:input.Status,startDate:input.StartDate,endDate: input.EndDate);
            var items = await discountCodeRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, status: input.Status, startDate: input.StartDate, endDate: input.EndDate);

            return new PagedResultDto<DiscountCodeDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<DiscountCode>, List<DiscountCodeDto>>(items)
            };
        }

        [Authorize(PikachuPermissions.DiscountCodes.Edit)] // Update this permission name as needed
        public async Task<DiscountCodeDto> UpdateAsync(Guid id, CreateUpdateDiscountCodeDto input)
        {
            var discountCode = await discountCodeManager.UpdateAsync(id,input.EventName, input.Status,input.StartDate, input.EndDate, input.Code, input.SpecifiedCode, input.AvailableQuantity, input.MaxUsePerPerson, input.GroupbuysScope, input.ProductsScope, input.DiscountMethod, input.MinimumSpendAmount, input.ShippingDiscountScope, input.SpecificShippingMethods, input.DiscountPercentage, input.DiscountAmount, input.GroupbuyIds, input.ProductIds);
            return ObjectMapper.Map<DiscountCode, DiscountCodeDto>(discountCode);
        }
        [Authorize(PikachuPermissions.DiscountCodes.Edit)] // Update this permission name as needed
        public async Task UpdateStatusAsync(Guid id)
        {
            var discountCode = await discountCodeRepository.GetAsync(id);
            discountCode.Status = !discountCode.Status;
            await discountCodeRepository.UpdateAsync(discountCode);
            return;
        }
    }
}