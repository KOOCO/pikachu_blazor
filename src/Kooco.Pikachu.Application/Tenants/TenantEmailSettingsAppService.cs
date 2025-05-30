﻿using Kooco.Pikachu.Tenants.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Tenants;

[RemoteService(IsEnabled = false)]
public class TenantEmailSettingsAppService(IRepository<TenantEmailSettings, Guid> emailSettingsRepository) :
    ApplicationService, ITenantEmailSettingsAppService
{
    public async Task<TenantEmailSettingsDto> GetEmailSettingsAsync()
    {
        var emailSettings = await emailSettingsRepository.FirstOrDefaultAsync();
        return ObjectMapper.Map<TenantEmailSettings, TenantEmailSettingsDto>(emailSettings);
    }

    public async Task UpdateEmailSettingsAsync(CreateUpdateTenantEmailSettingsDto input)
    {
        var emailSettings = await emailSettingsRepository.FirstOrDefaultAsync();
        if (emailSettings != null)
        {
            emailSettings.SenderName = input.SenderName;
            emailSettings.Subject = input.Subject;
            emailSettings.Greetings = input.Greetings;
            emailSettings.Footer = input.Footer;
            await emailSettingsRepository.UpdateAsync(emailSettings);
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
            await emailSettingsRepository.InsertAsync(emailSettings);
        }
    }
}
