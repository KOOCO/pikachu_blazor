using Kooco.Pikachu.TenantPaymentFees;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees;

public partial class TCatPaymentFeeConfigurations
{
    [Parameter] public Guid TenantId { get; set; }

    private IReadOnlyList<UpdateTenantPaymentFeeDto> Fees
        => [.. TenantPaymentFeeInitializer.Initialize().Where(x => x.FeeType == PaymentFeeType.TCat)];
}