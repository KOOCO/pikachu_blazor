using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.EnumValues;
public class EnumValue : FullAuditedAggregateRoot<int>
{
    public EnumType EnumType { get; set; }
    public string Text { get; set; }
}