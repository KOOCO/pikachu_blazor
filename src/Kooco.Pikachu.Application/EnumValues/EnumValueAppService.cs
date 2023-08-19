using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.EnumValues
{
    public class EnumValueAppService : CrudAppService<EnumValue,
                                       EnumValueDto,
                                       int,
                                       PagedAndSortedResultRequestDto,
                                       CreateUpdateEnumValueDto>,
                                       IEnumValueAppService
    {
        IRepository<EnumValue, int> _repository;
        public EnumValueAppService(IRepository<EnumValue, int> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EnumValueDto>> GetEnums(List<EnumType> enumTypes)
        {
            if (enumTypes.Any())
            {
                var query = await _repository.GetQueryableAsync();
                query = query.Where(x => enumTypes.Contains(x.EnumType));
                IEnumerable<EnumValue> enumValues = query.ToList();
                return ObjectMapper.Map<IEnumerable<EnumValue>, IEnumerable<EnumValueDto>>(enumValues);
            }
            else
                return new List<EnumValueDto>();
        }
    }
}
