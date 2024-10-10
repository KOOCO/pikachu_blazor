using Volo.Abp.Identity;

namespace Kooco.Pikachu.Identity;

public class GetCategorizedListDto : GetIdentityUsersInput
{
    public UserTypes? UserTypes { get; set; }
}