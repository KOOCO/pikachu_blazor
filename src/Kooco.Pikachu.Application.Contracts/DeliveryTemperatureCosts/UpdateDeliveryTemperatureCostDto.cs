using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.DeliveryTemperatureCosts;

public class UpdateDeliveryTemperatureCostDto
{
    public Guid Id { get; set; }
    public ItemStorageTemperature Temperature { get; set; }
    public decimal Cost { get; set; }
    public LogisticProviders? LogisticProvider { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public bool IsAllowOffShoreIslands { get; set; }
    public bool IsLogisticProviderActivated { get; set; }
}
