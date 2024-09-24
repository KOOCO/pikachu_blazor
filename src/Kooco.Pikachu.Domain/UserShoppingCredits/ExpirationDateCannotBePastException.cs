using Volo.Abp;

namespace Kooco.Pikachu.UserShoppingCredits;

public class ExpirationDateCannotBePastException : BusinessException
{
    public ExpirationDateCannotBePastException() : base(PikachuDomainErrorCodes.ExpirationDateCannotBePast)
    {

    }
}
