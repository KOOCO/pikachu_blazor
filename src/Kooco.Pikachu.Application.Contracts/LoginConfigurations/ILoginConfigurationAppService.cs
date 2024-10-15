using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.LoginConfigurations;

public interface ILoginConfigurationAppService : IApplicationService
{
    Task UpdateAsync(UpdateLoginConfigurationDto input);
    Task<LoginConfigurationDto?> FirstOrDefaultAsync(Guid? tenantId);
    Task DeleteAsync();
}
