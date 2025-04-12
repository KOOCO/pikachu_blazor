using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Validators;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.WebsiteManagement.FooterSettings)]
public class FooterSettingAppService(IFooterSettingRepository repository, ITenantSettingsAppService tenantSettingsAppService) : PikachuAppService, IFooterSettingAppService
{
    [AllowAnonymous]
    public async Task<FooterSettingDto?> FirstOrDefaultAsync()
    {
        var footer = await repository.FirstOrDefaultAsync();
        var dto = ObjectMapper.Map<FooterSetting, FooterSettingDto>(footer);

        if (dto != null && dto.Sections.Any(x => x.FooterSettingsType == FooterSettingsType.SocialMedia || x.FooterSettingsType == FooterSettingsType.CompanyAndCustomerServiceInformation))
        {
            var socialMedia = await tenantSettingsAppService.GetTenantSocialMediaAsync();
            var customerService = await tenantSettingsAppService.GetTenantCustomerServiceAsync();

            dto.Sections.ForEach(section =>
            {
                if (section.FooterSettingsType == FooterSettingsType.SocialMedia)
                {
                    section.SocialMedia = socialMedia;
                }
                if (section.FooterSettingsType == FooterSettingsType.CompanyAndCustomerServiceInformation)
                {
                    section.CustomerService = customerService;
                }
            });
        }

        return dto;
    }

    public async Task<FooterSettingDto> UpdateAsync(UpdateFooterSettingDto input)
    {
        var footer = await repository.FirstOrDefaultAsync();
        if (footer is null)
        {
            footer ??= new FooterSetting(GuidGenerator.Create());
            await repository.InsertAsync(footer);
        }

        footer.Sections.Clear();

        foreach (var section in input.Sections.OrderBy(s => s.FooterSettingsPosition))
        {
            MyCheck.NotUndefinedOrNull<FooterSettingsPosition>(section.FooterSettingsPosition, nameof(section.FooterSettingsPosition));

            var newSection = footer.AddSection(GuidGenerator.Create(), section.FooterSettingsPosition, section.Title,
                section.FooterSettingsType.Value, section.Text, section.ImageUrl, section.ImageName);

            if (newSection.FooterSettingsType == FooterSettingsType.Link)
            {
                if (section.Links == null || section.Links.Count == 0 || section.Links.Count >= FooterSettingsConsts.MaxAllowedLinks)
                {
                    throw new InvalidNumberOfLinksException(FooterSettingsConsts.MaxAllowedLinks);
                }
                foreach (var link in section.Links.OrderBy(l => l.Index))
                {
                    newSection.AddLink(GuidGenerator.Create(), link.Index, link.Title, link.Url);
                }
            }
        }

        await repository.UpdateAsync(footer);

        return ObjectMapper.Map<FooterSetting, FooterSettingDto>(footer);
    }
}
