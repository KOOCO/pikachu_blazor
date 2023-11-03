using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantEmailing
{
    public class TenantEmailSettingsAppService : ApplicationService, ITenantEmailSettingsAppService
    {
        private readonly IRepository<TenantEmailSettings, Guid> _emailSettingsRepository;

        public TenantEmailSettingsAppService(
            IRepository<TenantEmailSettings, Guid> emailSettingsRepository
            )
        {
            _emailSettingsRepository = emailSettingsRepository;
        }

        public async Task<TenantEmailSettingsDto> GetEmailSettingsAsync()
        {
            var emailSettings = await _emailSettingsRepository.FirstOrDefaultAsync();
            return ObjectMapper.Map<TenantEmailSettings, TenantEmailSettingsDto>(emailSettings);
        }

        public async Task UpdateEmailSettingsAsync(CreateUpdateTenantEmailSettingsDto input)
        {
            var emailSettings = await _emailSettingsRepository.FirstOrDefaultAsync();
            if (emailSettings != null)
            {
                emailSettings.SenderName = input.SenderName;
                emailSettings.Subject = input.Subject;
                emailSettings.Greetings = input.Greetings;
                emailSettings.Footer = input.Footer;
                await _emailSettingsRepository.UpdateAsync(emailSettings);
            }
            else
            {
                emailSettings = new TenantEmailSettings
                {
                    SenderName = input.SenderName,
                    Subject = input.Subject,
                    Greetings = input.Greetings,
                    Footer = input.Footer
                };
                await _emailSettingsRepository.InsertAsync(emailSettings);
            }
        }
    }
}
