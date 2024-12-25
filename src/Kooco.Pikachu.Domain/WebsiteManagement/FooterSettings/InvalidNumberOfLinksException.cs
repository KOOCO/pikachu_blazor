using Volo.Abp;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public class InvalidNumberOfLinksException : BusinessException
{
    public InvalidNumberOfLinksException(int maxAllowedLinks) : base(PikachuDomainErrorCodes.InvalidNumberOfLinks)
    {
        WithData("0", maxAllowedLinks);
    }
}
