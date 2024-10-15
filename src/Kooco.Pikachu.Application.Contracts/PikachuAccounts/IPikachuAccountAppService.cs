using Kooco.Pikachu.Members;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.PikachuAccounts;

public interface IPikachuAccountAppService : IApplicationService
{
    Task<PikachuLoginResponseDto> LoginAsync(PikachuLoginInputDto input);
    Task<IdentityUserDto> RegisterAsync(PikachuRegisterInputDto input);
    Task SendEmailVerificationCodeAsync(string email);
    Task<VerifyCodeResponseDto> VerifyEmailCodeAsync(string email, string code);
}
