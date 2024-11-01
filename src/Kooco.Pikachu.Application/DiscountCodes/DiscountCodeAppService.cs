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
using Volo.Abp.Domain.Repositories;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.DiscountCodes
{
    [RemoteService(IsEnabled = false)]
    [Authorize(PikachuPermissions.DiscountCodes.Default)] // Update this permission name as needed
    public class DiscountCodeAppService(IDiscountCodeRepository discountCodeRepository, DiscountCodeManager discountCodeManager) : PikachuAppService, IDiscountCodeAppService
    {
        [Authorize(PikachuPermissions.DiscountCodes.Create)] // Update this permission name as needed
        public async Task<DiscountCodeDto> CreateAsync(CreateUpdateDiscountCodeDto input)
        {
            var discountCode = await discountCodeManager.CreateAsync(input.EventName,input.Status,input.StartDate.Value,input.EndDate.Value,input.Code,input.SpecifiedCode,input.AvailableQuantity,input.MaxUsePerPerson,input.GroupbuysScope,input.ProductsScope,input.DiscountMethod,input.MinimumSpendAmount,input.ShippingDiscountScope,input.SpecificShippingMethods,input.DiscountPercentage,input.DiscountAmount,input.GroupbuyIds,input.ProductIds);
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
            var discountCode = await discountCodeManager.UpdateAsync(id,input.EventName, input.Status,input.StartDate.Value, input.EndDate.Value, input.Code, input.SpecifiedCode, input.AvailableQuantity, input.MaxUsePerPerson, input.GroupbuysScope, input.ProductsScope, input.DiscountMethod, input.MinimumSpendAmount, input.ShippingDiscountScope, input.SpecificShippingMethods, input.DiscountPercentage, input.DiscountAmount, input.GroupbuyIds, input.ProductIds);
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

        public async Task<DiscountCheckOutputDto> CheckDiscountCodeAsync(DiscountCheckInputDto input)
        {
            var discount = await discountCodeRepository
                .FirstOrDefaultAsync(d => d.SpecifiedCode == input.Code);
           
          
            if (discount == null)
            {
                return new DiscountCheckOutputDto { ErrorMessage = "InvalidCode" };
            }
            if (discount.EndDate < DateTime.UtcNow)
            {
                return new DiscountCheckOutputDto { ErrorMessage = "DiscountExpired" };
            }
            if (!discount.Status)
            {
                return new DiscountCheckOutputDto { ErrorMessage = "DiscountClosed" };
            }
            await discountCodeRepository.EnsureCollectionLoadedAsync(discount, x => x.DiscountSpecificGroupbuys);
            if (discount.GroupbuysScope == "AllGroupbuys" ||
                discount.DiscountSpecificGroupbuys.Any(g => g.GroupbuyId == input.GroupbuyId))
            {
                var output = new DiscountCheckOutputDto
                {
                    DiscountType = discount.DiscountMethod
                };
                if (discount.DiscountAmount.HasValue&& discount.DiscountAmount>0)
                {
                    output.DiscountAmount = discount.DiscountAmount;
                }
                if (discount.DiscountPercentage.HasValue && discount.DiscountPercentage > 0)
                {
                    output.DiscountPercentage = discount.DiscountPercentage;
                }
                switch (discount.DiscountMethod)
                {
                    case "DirectDiscount":
                     
                        break;

                    case "RequireSpending":
                        output.MinimumSpendAmount = discount.MinimumSpendAmount;
                        break;

                    case "ShippingDiscount":
                        output.ShippingMethods = discount.ShippingDiscountScope == "AllMethods"
                            ? new[] { "AllMethods" }
                            : discount?.SpecificShippingMethods.Select(m => Enum.GetName(typeof(DeliveryMethod), m)).ToArray();
                        break;
                }

                return output;
            }
            else
            {
                return new DiscountCheckOutputDto { ErrorMessage = "DiscountNotForThisGroupbuy" };
            }
        }
    }
}