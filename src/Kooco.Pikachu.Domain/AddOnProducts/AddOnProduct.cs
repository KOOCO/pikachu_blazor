using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using Volo.Abp.Identity;
using Kooco.Pikachu.Items;
using Volo.Abp;

namespace Kooco.Pikachu.AddOnProducts
{
    public class AddOnProduct:AuditedAggregateRoot<Guid>
    {
       
        public int AddOnAmount { get; set; }
        public int AddOnLimitPerOrder { get; set; }
        public string QuantitySetting { get; set; }
        public int AvailableQuantity { get; set; }
        public string DisplayOriginalPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AddOnConditions { get; set; }
        public int MinimumSpendAmount { get; set; }
        public string GroupbuysScope { get; set; }
        public bool Status { get; set; }
        public int SellingQuantity { get; set; }
      
        public Guid ProductId { get; set; }
        public virtual ICollection<AddOnProductSpecificGroupbuy> AddOnProductSpecificGroupbuys { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Item? Product { get; set; }

        public AddOnProduct(
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
        Guid productId):base(id)
        {
            AddOnProductSpecificGroupbuys = new HashSet<AddOnProductSpecificGroupbuy>();
            ProductId = productId;
            SetAddOnAmount(addOnAmount);
            SetAddOnLimitPerOrder(addOnLimitPerOrder);
            QuantitySetting = quantitySetting;
            SetAvailableQuantity(availableQuantity);
            DisplayOriginalPrice = displayOriginalPrice;
            StartDate = startDate;
            EndDate = endDate;
            AddOnConditions = addOnConditions;
            SetMinimumSpendAmount(minimumSpendAmount);
            GroupbuysScope = groupbuysScope;
            Status = status;
            SetSellingQuantity(sellingQuantity);
            
        }

        public AddOnProduct ChangeAddOnAmount(int addOnAmount)
        {
            SetAddOnAmount(addOnAmount);
            return this;
        }

        private void SetAddOnAmount(int addOnAmount)
        {
            AddOnAmount = Check.Range(addOnAmount, nameof(addOnAmount), 0, int.MaxValue);
        }

        public AddOnProduct ChangeAddOnLimitPerOrder(int addOnLimitPerOrder)
        {
            SetAddOnLimitPerOrder(addOnLimitPerOrder);
            return this;
        }

        private void SetAddOnLimitPerOrder(int addOnLimitPerOrder)
        {
            AddOnLimitPerOrder = Check.Range(addOnLimitPerOrder, nameof(addOnLimitPerOrder), 0, int.MaxValue);
        }

        public AddOnProduct ChangeAvailableQuantity(int availableQuantity)
        {
            SetAvailableQuantity(availableQuantity);
            return this;
        }

        private void SetAvailableQuantity(int availableQuantity)
        {
            AvailableQuantity = Check.Range(availableQuantity, nameof(availableQuantity), 0, int.MaxValue);
        }

        public AddOnProduct ChangeMinimumSpendAmount(int minimumSpendAmount)
        {
            SetMinimumSpendAmount(minimumSpendAmount);
            return this;
        }

        private void SetMinimumSpendAmount(int minimumSpendAmount)
        {
            MinimumSpendAmount = Check.Range(minimumSpendAmount, nameof(minimumSpendAmount), 0, int.MaxValue);
        }

        public AddOnProduct ChangeSellingQuantity(int sellingQuantity)
        {
            SetSellingQuantity(sellingQuantity);
            return this;
        }

        private void SetSellingQuantity(int sellingQuantity)
        {
            SellingQuantity = Check.Range(sellingQuantity, nameof(sellingQuantity), 0, int.MaxValue);
        }

        public void AddGroupbuys(List<Guid> groupbuyIds)
        {
            if (groupbuyIds.Count>0)
            {
                foreach (var id in groupbuyIds)
                {
                    
                        AddOnProductSpecificGroupbuys.Add(new AddOnProductSpecificGroupbuy(Guid.NewGuid(), Id,id));
                    
                }
            }
        }
        public void UpdateGroupbuys(List<Guid> newGroupbuys)
        {
            // Clear existing group buys
            AddOnProductSpecificGroupbuys.Clear();

            // Add the new group buys
            AddGroupbuys(newGroupbuys);
        }
    }
}
