using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.TenantPaymentFees;

public class TenantPaymentFeeAppService : PikachuAppService, ITenantPaymentFeeAppService
{
    private readonly TenantPaymentFeeManager _tenantPaymentFeeManager;
    private readonly ITenantPaymentFeeRepository _tenantPaymentFeeRepository;

    public TenantPaymentFeeAppService(
        TenantPaymentFeeManager tenantPaymentFeeManager,
        ITenantPaymentFeeRepository tenantPaymentFeeRepository
        )
    {
        _tenantPaymentFeeManager = tenantPaymentFeeManager;
        _tenantPaymentFeeRepository = tenantPaymentFeeRepository;
    }

    public Task UpdateEcPayPaymentAsync(Guid tenantId, List<UpdateTenantPaymentFeeDto> input)
    => UpdatePaymentAsync(tenantId, PaymentFeeType.EcPay, input);

    public Task UpdateTCatPaymentAsync(Guid tenantId, List<UpdateTenantPaymentFeeDto> input)
        => UpdatePaymentAsync(tenantId, PaymentFeeType.TCat, input);

    private async Task UpdatePaymentAsync(Guid tenantId, PaymentFeeType feeType, List<UpdateTenantPaymentFeeDto> input)
    {
        if (input == null || input.Count == 0) return;

        var filteredInput = input.Where(i => i.FeeType == feeType).ToList();
        if (filteredInput.Count == 0) return;

        var existing = await _tenantPaymentFeeRepository
            .GetListAsync(t => t.FeeType == feeType && t.TenantId == tenantId);

        var combinations = existing.ToDictionary(x => (x.FeeType, x.FeeSubType, x.PaymentMethod));

        foreach (var item in filteredInput)
        {
            if (combinations.TryGetValue((item.FeeType, item.FeeSubType, item.PaymentMethod), out var existingItem))
            {
                await _tenantPaymentFeeManager.UpdateAsync(
                    existingItem,
                    item.IsEnabled,
                    item.FeeKind,
                    item.Amount
                );
            }
            else
            {
                await _tenantPaymentFeeManager.CreateAsync(
                    item.FeeType,
                    item.FeeSubType,
                    item.PaymentMethod,
                    item.IsEnabled,
                    item.FeeKind,
                    item.Amount,
                    item.IsBaseFee,
                    tenantId
                );
            }
        }
    }

    public Task<List<TenantPaymentFeeDto>> GetEcPayPaymentAsync(Guid tenantId)
        => GetByFeeTypeAsync(tenantId, PaymentFeeType.EcPay);
    public Task<List<TenantPaymentFeeDto>> GetTCatPaymentAsync(Guid tenantId)
        => GetByFeeTypeAsync(tenantId, PaymentFeeType.TCat);

    private async Task<List<TenantPaymentFeeDto>> GetByFeeTypeAsync(Guid tenantId, PaymentFeeType feeType)
    {
        var entities = await _tenantPaymentFeeRepository.GetListAsync(t => t.TenantId == tenantId && t.FeeType == feeType);
        var result = ObjectMapper.Map<List<TenantPaymentFee>, List<TenantPaymentFeeDto>>(entities);
        return TenantPaymentFeeInitializer.InitByType(result, feeType);
    }
}
