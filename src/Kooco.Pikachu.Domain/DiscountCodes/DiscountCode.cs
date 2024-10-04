using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCode:AuditedAggregateRoot<Guid>
    {
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Code { get; set; } 
        public string SpecifiedCode { get; set; }
        public int AvailableQuantity { get; set; }
        public int MaxUsePerPerson { get; set; }
        public string GroupbuysScope { get; set; }
        public string ProductsScope { get; set; }
        public string DiscountMethod { get; set; }
        public int? MinimumSpendAmount { get; set; }
        public string ShippingDiscountScope { get; set; }
        public int[] SpecificShippingMethods { get; set; } // Assuming this is an array
        public int? DiscountPercentage { get; set; }
        public int? DiscountAmount { get; set; }
        public bool Status { get; set; }
        // Navigation properties

        public ICollection<DiscountCodeUsage> DiscountCodeUsages { get; set; }
        public ICollection<DiscountSpecificGroupbuy> DiscountSpecificGroupbuys { get; set; }
        public ICollection<DiscountSpecificProduct> DiscountSpecificProducts { get; set; }

        private DiscountCode() { }
        public DiscountCode(
                  Guid id,
                  string eventName,
                  bool status,
                  DateTime startDate,
                  DateTime endDate,
                  string code,
                  string specifiedCode,
                  int availableQuantity,
                  int maxUsePerPerson,
                  string groupbuysScope,
                  string productsScope,
                  string discountMethod,
                  int minimumSpendAmount,
                  string shippingDiscountScope,
                  //int[] specificShippingMethods,
                  int? discountPercentage,
                  int? discountAmount) : base(id)
        {
            DiscountSpecificGroupbuys = new HashSet<DiscountSpecificGroupbuy>();
            DiscountSpecificProducts = new HashSet<DiscountSpecificProduct>();
           
            DiscountCodeUsages = new HashSet<DiscountCodeUsage>();
            Status = status;
            EventName = eventName;
            StartDate = startDate;
            EndDate = endDate;
            Code = code;
            SpecifiedCode = specifiedCode;
            SetAvailableQuantity(availableQuantity);
            SetMaxUsePerPerson(maxUsePerPerson);
            GroupbuysScope = groupbuysScope;
            ProductsScope = productsScope;
            DiscountMethod = discountMethod;
            SetMinimumSpendAmount(minimumSpendAmount);
            ShippingDiscountScope = shippingDiscountScope;
            //SpecificShippingMethods = specificShippingMethods;
            DiscountPercentage = discountPercentage;
            DiscountAmount = discountAmount;
        }

        public DiscountCode ChangeAvailableQuantity(int availableQuantity)
        {
            SetAvailableQuantity(availableQuantity);
            return this;
        }

        private void SetAvailableQuantity(int availableQuantity)
        {
            AvailableQuantity = Check.Range(availableQuantity, nameof(availableQuantity), 0, int.MaxValue);
        }

        public DiscountCode ChangeMaxUsePerPerson(int maxUsePerPerson)
        {
            SetMaxUsePerPerson(maxUsePerPerson);
            return this;
        }

        private void SetMaxUsePerPerson(int maxUsePerPerson)
        {
            MaxUsePerPerson = Check.Range(maxUsePerPerson, nameof(maxUsePerPerson), 0, int.MaxValue);
        }

        public DiscountCode ChangeMinimumSpendAmount(int minimumSpendAmount)
        {
            SetMinimumSpendAmount(minimumSpendAmount);
            return this;
        }

        private void SetMinimumSpendAmount(int minimumSpendAmount)
        {
            MinimumSpendAmount = Check.Range(minimumSpendAmount, nameof(minimumSpendAmount), 0, int.MaxValue);
        }

        

      

        public void AddSpecificGroupbuys(List<Guid> groupbuyIds)
        {
            if (groupbuyIds.Count > 0)
            {
                foreach (var id in groupbuyIds)
                {
                    DiscountSpecificGroupbuys.Add(new DiscountSpecificGroupbuy(Guid.NewGuid(), Id, id) );
                }
            }
        }

        public void UpdateSpecificGroupbuys(List<Guid> newGroupbuys)
        {
            DiscountSpecificGroupbuys.Clear();
            AddSpecificGroupbuys(newGroupbuys);
        }

        public void AddSpecificProducts(List<Guid> productIds)
        {
            if (productIds.Count > 0)
            {
                foreach (var id in productIds)
                {
                    DiscountSpecificProducts.Add(new DiscountSpecificProduct(Guid.NewGuid(),Id,id ));
                }
            }
        }

        public void UpdateSpecificProducts(List<Guid> newProducts)
        {
            DiscountSpecificProducts.Clear();
            AddSpecificProducts(newProducts);
        }
    }
}