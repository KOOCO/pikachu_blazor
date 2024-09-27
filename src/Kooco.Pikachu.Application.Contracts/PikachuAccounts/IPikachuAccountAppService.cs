using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PikachuAccounts;

public interface IPikachuAccountAppService : IApplicationService
{
    Task<PikachuLoginResponseDto> LoginAsync(PikachuLoginInputDto input);
}
