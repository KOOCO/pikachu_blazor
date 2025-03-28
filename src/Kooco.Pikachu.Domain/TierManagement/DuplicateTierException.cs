using Volo.Abp;

namespace Kooco.Pikachu.TierManagement;

public class DuplicateTierException : BusinessException
{
    public DuplicateTierException() : base(PikachuDomainErrorCodes.DuplicateTier)
    {
        
    }
}
