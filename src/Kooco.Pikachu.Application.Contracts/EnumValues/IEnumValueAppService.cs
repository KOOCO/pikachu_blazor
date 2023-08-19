using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.EnumValues
{
    public interface IEnumValueAppService : ICrudAppService<EnumValueDto,
                                                            int,
                                                            PagedAndSortedResultRequestDto,
                                                            CreateUpdateEnumValueDto>
    {
        Task<IEnumerable<EnumValueDto>> GetEnums(List<EnumType> enumTypes);
    }
}
