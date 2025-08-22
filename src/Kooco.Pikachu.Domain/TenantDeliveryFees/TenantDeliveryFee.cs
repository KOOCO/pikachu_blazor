using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public class TenantDeliveryFee : FullAuditedEntity<Guid>
    {
        public Guid? TenantId { get; set; }

        public DeliveryProvider DeliveryProvider { get; private set; }
        public bool IsEnabled { get; private set; }
        public FeeKind FeeKind { get; private set; }
        public decimal? PercentValue { get; private set; }   // 0–100 when Percentage
        public decimal? FixedAmount { get; private set; }    // >= 0 when Fixed

        private TenantDeliveryFee()
        {
            // EF Core constructor
        }

        public TenantDeliveryFee(
            Guid id,
            Guid? tenantId,

            DeliveryProvider deliveryProvider,
            bool isEnabled,
            FeeKind feeKind,
            decimal? percentValue = 0,
            decimal? fixedAmount = 0) : base(id)
        {
            TenantId = tenantId;

            DeliveryProvider = deliveryProvider;
            IsEnabled = isEnabled;
            FeeKind = feeKind;
            PercentValue = percentValue;
            FixedAmount = fixedAmount;

            ValidateFeeValues();
        }

        public void Update(
            bool isEnabled,
            FeeKind feeKind,
            decimal? percentValue = 0,
            decimal? fixedAmount = 0)
        {
            IsEnabled = isEnabled;
            FeeKind = feeKind;
            PercentValue = percentValue;
            FixedAmount = fixedAmount;

            ValidateFeeValues();
        }

        private void ValidateFeeValues()
        {
            if (FeeKind == FeeKind.Percentage)
            {
                if (PercentValue == null || PercentValue < 0 || PercentValue > 100)
                    throw new BusinessException("Percent value must be between 0 and 100 when fee kind is percentage");
                FixedAmount = 0;
            }
            else if (FeeKind == FeeKind.FixedAmount)
            {
                if (FixedAmount == null || FixedAmount < 0)
                    throw new BusinessException("Fixed amount must be >= 0 when fee kind is fixed amount");
                PercentValue = 0;
            }
        }
    }
}
