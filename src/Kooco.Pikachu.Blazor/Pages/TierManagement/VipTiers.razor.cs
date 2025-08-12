using Kooco.Pikachu.TierManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TierManagement;

public partial class VipTiers
{
    private UpdateVipTierSettingDto Entity { get; set; }
    private DateTime InitialStartDate { get; set; }
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
            if (!Entity.StartDate.HasValue)
            {
                return;
            }
            if (Entity.IsResetEnabled && !Entity.ResetFrequency.HasValue)
            {
                await Message.Error(L["The {0} field is required", L[nameof(Entity.ResetFrequency)]]);
                return;
            }
            if (!Entity.IsResetEnabled)
            {
                Entity.ResetFrequency = null;
            }
            bool confirmation = true;
            if ((InitialStartDate != Entity.StartDate || Entity.IsResetEnabled) && !Entity.IsResetConfigured)
            {
                confirmation = await Message.Confirm(L["VipTierTimeAndResetConfigWarning"], L["VipTierTimeAndResetConfigWarningTitle"]);
            }
            if (!confirmation)
            {
                return;
            }
            await VipTierSettingAppService.UpdateAsync(Entity);
            await ResetAsync();
            await Notify.Success("Updated");
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
            if (Entity.StartDate == null || Entity.StartDate == DateTime.MinValue)
            {
                Entity.StartDate = vipTierSetting?.CreationTime ?? DateTime.Today;
            }
            
            InitialStartDate = Entity.StartDate.Value;

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

    void AutoResetEnabledChange(bool e)
    {
        Entity.IsResetEnabled = e;
        if (!e)
        {
            Entity.ResetFrequency = null;
        }
    }
}