using Kooco.Pikachu.Members;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Blazor.Pages.Account;

[ExposeServices(typeof(LoginModel))]
[Dependency(ReplaceServices = true)]
public class CustomLoginModel : LoginModel
{
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "<Pending>")]
    public CustomLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IOptions<IdentityOptions> identityOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache
        ) : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache)
    {
    }

    public override async Task<IActionResult> OnPostAsync(string action)
    {
        var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
                    await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);

        if (user != null)
        {
            var isMember = await UserManager.IsInRoleAsync(user, MemberConsts.Role);
            if (isMember)
            {
                Alerts.Danger("Login is not allowed.");
                return Page();
            }
        }

        return await base.OnPostAsync(action);
    }
}
