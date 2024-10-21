using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.DeliveryTempratureCosts;

public class DeliveryTemperatureCost : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public ItemStorageTemperature Temperature { get; set; }
    public decimal Cost { get; set; }
    public LogisticProviders? LogisticProvider { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public Guid? TenantId { get; set; }
    public bool IsAllowOffShoreIslands { get; set; }
    public bool IsLogisticProviderActivated { get; set; }
}
