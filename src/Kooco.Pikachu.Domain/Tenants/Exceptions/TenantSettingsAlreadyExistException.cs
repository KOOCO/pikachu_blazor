using Volo.Abp;

namespace Kooco.Pikachu.Tenants.Exceptions;

public class TenantSettingsAlreadyExistException : BusinessException
{
    public TenantSettingsAlreadyExistException() : base(PikachuDomainErrorCodes.TenantSettingsAlreadyExist)
    {
        
    }
}
