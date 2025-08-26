using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees;

public partial class TCatPaymentFeeConfigurations
{
    [Parameter] public Guid TenantId { get; set; }
    private readonly IReadOnlyList<EcPayFeeConfiguration> Configurations = TempModels.TCatConfigurations;
}