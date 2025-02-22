using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.AutomaticEmails
{
    public interface IAutomaticEmailAppService : IApplicationService
    {
        Task CreateAsync(AutomaticEmailCreateUpdateDto input);
        Task UpdateAsync(Guid id, AutomaticEmailCreateUpdateDto input);
        Task<AutomaticEmailDto> GetAsync(Guid id);
        Task<PagedResultDto<AutomaticEmailDto>> GetListAsync(GetAutomaticEmailListDto input);
        Task<AutomaticEmailDto> GetWithDetailsByIdAsync(Guid id);
        Task UpdateJobStatusAsync(Guid id, JobStatus status, Guid? tenantId);
        Task<List<string>> GetGroupBuyNamesAsync(List<Guid>? groupBuyIds);
    }
}
