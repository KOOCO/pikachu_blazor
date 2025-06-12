using Kooco.Pikachu.EnumValues;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Identity;

public interface IMyIdentityUserRepository : IIdentityUserRepository
{
    Task<IdentityUser?> FindByExternalIdAsync(LoginMethod loginMethod, string externalId, bool exception = false);
    Task<IdentityUser?> FindByNameOrEmailAsync(string userNameOrEmail);
}
