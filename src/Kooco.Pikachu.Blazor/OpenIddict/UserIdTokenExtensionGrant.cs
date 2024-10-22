using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Controllers;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;
using Volo.Abp.OpenIddict;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kooco.Pikachu.Blazor.OpenIddict;

public class UserIdTokenExtensionGrant : AbpOpenIdDictControllerBase, ITokenExtensionGrant
{
    public const string ExtensionGrantName = "user_id";
    public string Name => ExtensionGrantName;

    public async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        var currentUserManager = context.HttpContext.RequestServices.GetService<IdentityUserManager>();
        var _dataFilter = context.HttpContext.RequestServices.GetService<IDataFilter>();
        var userId = context.Request.GetParameter("user_id")?.ToString();

        if (userId.IsNullOrWhiteSpace())
        {
            return new ForbidResult(
                [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest
                }!));
        }
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var user = await currentUserManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new ForbidResult(
                    [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest
                    }));
            }
            var userClaimsPrincipalFactory = context.HttpContext.RequestServices.GetRequiredService<IUserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser>>();
            var claimsPrincipal = await userClaimsPrincipalFactory.CreateAsync(user);
            ImmutableArray<string> scopes = [context.Request.GetParameter("scope")?.Value?.ToString()];
            claimsPrincipal.SetScopes(scopes);
            claimsPrincipal.SetAudiences(scopes);
            await context.HttpContext.RequestServices.GetRequiredService<AbpOpenIddictClaimsPrincipalManager>().HandleAsync(context.Request, claimsPrincipal);

            return SignIn(claimsPrincipal, new AuthenticationProperties()
            {
                ExpiresUtc = DateTime.Now.AddDays(7),
                IsPersistent = true,
            }, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
