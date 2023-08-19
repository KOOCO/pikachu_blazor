
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.EnumValues
{
    public class EnumValueDto : FullAuditedEntityDto<int>
    {
        public EnumType EnumType { get; set; }
        public string Text { get; set; }
    }
}
