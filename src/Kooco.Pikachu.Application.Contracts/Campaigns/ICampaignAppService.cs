using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Campaigns;

public interface ICampaignAppService : IApplicationService
{
    Task<CampaignDto> CreateAsync(CreateCampaignDto input);
}
