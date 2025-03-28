using Kooco.Pikachu.TierManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TierManagement;

public partial class VipTiers
{
    private UpdateVipTierSettingDto Entity { get; set; }
    private bool Loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ResetAsync();
    }

    private async Task UpdateAsync()
    {
        try
        {
            Loading = true;
            if (!Entity.BasedOnCount || !Entity.BasedOnAmount)
            {
                Entity.TierCondition = null;
            }
            await VipTierSettingAppService.UpdateAsync(Entity);
            await ResetAsync();
            await Message.Success("");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task ResetAsync()
    {
        try
        {
            var vipTierSetting = await VipTierSettingAppService.FirstOrDefaultAsync();
            Entity = ObjectMapper.Map<VipTierSettingDto, UpdateVipTierSettingDto>(vipTierSetting);

            Entity.Tiers.AddRange(
                [.. VipTierConsts.TierOptions
                .Where(tier => !Entity.Tiers.Any(t => tier == t.Tier))
                .Select(tier => new UpdateVipTierDto
                {
                    Tier = tier
                })]
                );
            Entity.Tiers = [.. Entity.Tiers.OrderBy(tier => tier.Tier)];
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}