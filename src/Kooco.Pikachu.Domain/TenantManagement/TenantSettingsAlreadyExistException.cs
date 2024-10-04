using Volo.Abp;

namespace Kooco.Pikachu.TenantManagement;

public class TenantSettingsAlreadyExistException : BusinessException
{
    public TenantSettingsAlreadyExistException() : base(PikachuDomainErrorCodes.TenantSettingsAlreadyExist)
    {
        
    }
}
