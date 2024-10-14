using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Encryption;

namespace Kooco.Pikachu.LoginConfigurations;

[Authorize]
[RemoteService(IsEnabled = false)]
public class LoginConfigurationAppService(IRepository<LoginConfiguration, Guid> loginConfigurationRepository,
    IStringEncryptionService stringEncryptionService) : PikachuAppService, ILoginConfigurationAppService
{
    public async Task DeleteAsync()
    {
        var loginConfigurations = await loginConfigurationRepository.FirstOrDefaultAsync();
        await loginConfigurationRepository.DeleteAsync(loginConfigurations);
    }

    public async Task<LoginConfigurationDto?> FirstOrDefaultAsync()
    {
        var loginConfigurations = await loginConfigurationRepository.FirstOrDefaultAsync();
        if (loginConfigurations == null) return default;

        var dto = ObjectMapper.Map<LoginConfiguration, LoginConfigurationDto>(loginConfigurations);
        dto.FacebookAppId = stringEncryptionService.Decrypt(dto.FacebookAppId);
        dto.FacebookAppSecret = stringEncryptionService.Decrypt(dto.FacebookAppSecret);
        dto.LineChannelId = stringEncryptionService.Decrypt(dto.LineChannelId);
        dto.LineChannelSecret = stringEncryptionService.Decrypt(dto.LineChannelSecret);
        return dto;
    }

    public async Task UpdateAsync(UpdateLoginConfigurationDto input)
    {
        var loginConfigurations = await loginConfigurationRepository.FirstOrDefaultAsync();

        var facebookAppId = stringEncryptionService.Encrypt(input.FacebookAppId);
        var facebookAppSecret = stringEncryptionService.Encrypt(input.FacebookAppSecret);
        var lineChannelId = stringEncryptionService.Encrypt(input.LineChannelId);
        var lineChannelSecret = stringEncryptionService.Encrypt(input.LineChannelSecret);

        if (loginConfigurations == null)
        {
            loginConfigurations = new LoginConfiguration(GuidGenerator.Create(), facebookAppId, facebookAppSecret,
                lineChannelId, lineChannelSecret);
            await loginConfigurationRepository.InsertAsync(loginConfigurations);
        }
        else
        {
            loginConfigurations.FacebookAppId = facebookAppId;
            loginConfigurations.FacebookAppSecret = facebookAppSecret;
            loginConfigurations.LineChannelId = lineChannelId;
            loginConfigurations.LineChannelSecret = lineChannelSecret;
            await loginConfigurationRepository.UpdateAsync(loginConfigurations);
        }
    }
}
