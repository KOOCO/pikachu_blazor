using Volo.Abp;

namespace Kooco.Pikachu.TierManagement;

public class VipTierConditionCheckedException : BusinessException
{
    public VipTierConditionCheckedException() : base(PikachuDomainErrorCodes.VipTierConditionChecked)
    {
        
    }
}
