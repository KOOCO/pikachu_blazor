using Kooco.Pikachu.EnumValues;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.PikachuAccounts;

public interface IPikachuAccountAppService : IApplicationService
{
    Task<PikachuLoginResponseDto> LoginAsync(PikachuLoginInputDto input);
    Task<IdentityUserDto> RegisterAsync(PikachuRegisterInputDto input);
    Task<GenericResponseDto> SendEmailVerificationCodeAsync(string email);
    Task<VerifyCodeResponseDto> VerifyEmailCodeAsync(string email, string code);
    Task<GenericResponseDto> SendPasswordResetCodeAsync(string email);
    Task<VerifyCodeResponseDto> VerifyPasswordResetCodeAsync(string email, string code);
    Task<GenericResponseDto> ResetPasswordAsync(PikachuResetPasswordDto input);
    Task<GenericResponseDto> FindByTokenAsync(LoginMethod method, string thirdPartyToken);
}
