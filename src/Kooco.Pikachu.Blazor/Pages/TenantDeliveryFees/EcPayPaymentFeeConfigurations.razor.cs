using Blazorise;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees;

public partial class EcPayPaymentFeeConfigurations
{
    [Parameter] public Guid TenantId { get; set; }

    private readonly IReadOnlyList<EcPayFeeConfiguration> Configurations = TempModels.Configurations;
}