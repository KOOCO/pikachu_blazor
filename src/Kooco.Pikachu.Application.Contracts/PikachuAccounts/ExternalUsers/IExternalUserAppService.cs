using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PikachuAccounts.ExternalUsers;

public interface IExternalUserAppService : IApplicationService
{
    Task<FacebookUserDto?> GetFacebookUserDetailsAsync(string token);
    Task<GoogleUserDto?> GetGoogleUserDetailsAsync(string accessToken);
    Task<LineUserDto?> GetLineUserDetailsAsync(string accessToken);
}
