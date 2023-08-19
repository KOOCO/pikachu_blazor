using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.EnumValues
{
    public class EnumValueDto : EntityDto<int>
    {
        public EnumType EnumType { get; set; }
        public string Text { get; set; }
    }
}
