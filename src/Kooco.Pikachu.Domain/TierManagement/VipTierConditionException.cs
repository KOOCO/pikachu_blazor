using Volo.Abp;

namespace Kooco.Pikachu.TierManagement;

public class VipTierConditionException : BusinessException
{
    public VipTierConditionException() : base(PikachuDomainErrorCodes.VipTierCondition)
    {
        
    }
}
