using Volo.Abp;

namespace Kooco.Pikachu.TenantManagement;

public class InvalidServiceHoursException : BusinessException
{
    public InvalidServiceHoursException() : base(PikachuDomainErrorCodes.InvalidServiceHoursException)
    {
        
    }
}
