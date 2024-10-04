using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCodeManager:DomainService
    {
        private readonly IDiscountCodeRepository _discountCodeRepository;
        private readonly IRepository<DiscountSpecificGroupbuy, Guid> _groupbuyRepository;
        private readonly IRepository<DiscountSpecificProduct, Guid> _productRepository;

        public DiscountCodeManager(
            IDiscountCodeRepository discountCodeRepository,
            IRepository<DiscountSpecificGroupbuy, Guid> groupbuyRepository,
            IRepository<DiscountSpecificProduct, Guid> productRepository)
        {
            _discountCodeRepository = discountCodeRepository;
            _groupbuyRepository = groupbuyRepository;
            _productRepository = productRepository;
        }

        public async Task<DiscountCode> CreateAsync(
            string eventName,
            bool status,
            DateTime startDate,
            DateTime endDate,
            string discountCode,
            string specifiedCode,
            int availableQuantity,
            int maxUsePerPerson,
            string groupbuysScope,
            string productsScope,
            string discountMethod,
            int minimumSpendAmount,
            string shippingDiscountScope,
            List<int> specificShippingMethods,
            int discountPercentage,
            int discountAmount,
            List<Guid> groupbuyIds,
            List<Guid> productIds)
        {
            var discount = new DiscountCode(
                GuidGenerator.Create(),
                eventName,
                status,
                startDate,
                endDate,
                discountCode,
                specifiedCode,
                availableQuantity,
                maxUsePerPerson,
                groupbuysScope,
                productsScope,
                discountMethod,
                minimumSpendAmount,
                shippingDiscountScope,
                //specificShippingMethods.ToArray(),
                discountPercentage,
                discountAmount);
            discount.SpecificShippingMethods = specificShippingMethods.ToArray();
            // Add associated group buys and products
            discount.AddSpecificGroupbuys(groupbuyIds);
            discount.AddSpecificProducts(productIds);

            return await _discountCodeRepository.InsertAsync(discount);
        }

        public async Task<DiscountCode> UpdateAsync(
            Guid id,
            string eventName,
            bool status,
            DateTime startDate,
            DateTime endDate,
            string discountCode,
            string specifiedCode,
            int availableQuantity,
            int maxUsePerPerson,
            string groupbuysScope,
            string productsScope,
            string discountMethod,
            int minimumSpendAmount,
            string shippingDiscountScope,
            List<int> specificShippingMethods,
            int? discountPercentage,
            int? discountAmount,
            List<Guid> groupbuyIds,
            List<Guid> productIds)
        {
            var existingDiscount = await _discountCodeRepository.GetWithDetailAsync(id);

            if (existingDiscount == null)
            {
                throw new EntityNotFoundException(typeof(DiscountCode), id);
            }

            // Update properties of the existing DiscountCode
            existingDiscount.EventName = eventName;
            existingDiscount.Status = status;
            existingDiscount.StartDate = startDate;
            existingDiscount.EndDate = endDate;
            existingDiscount.Code = discountCode;
            existingDiscount.SpecifiedCode = specifiedCode;
            existingDiscount.ChangeAvailableQuantity(availableQuantity)
                            .ChangeMaxUsePerPerson(maxUsePerPerson);
            existingDiscount.GroupbuysScope = groupbuysScope;
            existingDiscount.ProductsScope = productsScope;
            existingDiscount.DiscountMethod = discountMethod;
            existingDiscount.ChangeMinimumSpendAmount(minimumSpendAmount);
            existingDiscount.ShippingDiscountScope = shippingDiscountScope;
            existingDiscount.SpecificShippingMethods = specificShippingMethods.ToArray();
            existingDiscount.DiscountPercentage = discountPercentage;
            existingDiscount.DiscountAmount= discountAmount;

            // Update group buys and products
            await _groupbuyRepository.DeleteManyAsync(existingDiscount.DiscountSpecificGroupbuys, autoSave: true);
            await _productRepository.DeleteManyAsync(existingDiscount.DiscountSpecificProducts, autoSave: true);
            existingDiscount.UpdateSpecificGroupbuys(groupbuyIds);
            existingDiscount.UpdateSpecificProducts(productIds);
            await _groupbuyRepository.InsertManyAsync(existingDiscount.DiscountSpecificGroupbuys,autoSave:true);
            await _productRepository.InsertManyAsync(existingDiscount.DiscountSpecificProducts,autoSave: true);
            return await _discountCodeRepository.UpdateAsync(existingDiscount);
        }
    }
}
