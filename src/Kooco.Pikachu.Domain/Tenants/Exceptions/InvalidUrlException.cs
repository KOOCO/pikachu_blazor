using Volo.Abp;

namespace Kooco.Pikachu.Tenants.Exceptions;

public class InvalidUrlException : BusinessException
{
    public InvalidUrlException(string property) : base(PikachuDomainErrorCodes.InvalidUrl)
    {
        WithData("property", property);
    }
}
