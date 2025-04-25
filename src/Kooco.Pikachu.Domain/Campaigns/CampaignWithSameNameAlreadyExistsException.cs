using Volo.Abp;

namespace Kooco.Pikachu.Campaigns;

public class CampaignWithSameNameAlreadyExistsException : BusinessException
{
    public CampaignWithSameNameAlreadyExistsException(string name) : base(PikachuDomainErrorCodes.CampaignWithSameNameAlreadyExists)
    {
        WithData("name", name);
    }
}
