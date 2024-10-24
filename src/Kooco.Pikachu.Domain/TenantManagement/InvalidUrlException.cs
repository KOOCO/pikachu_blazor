using Volo.Abp;

namespace Kooco.Pikachu.TenantManagement;

public class InvalidUrlException : BusinessException
{
    public InvalidUrlException(string property) : base(PikachuDomainErrorCodes.InvalidUrl)
    {
        WithData("property", property);
    }
}
