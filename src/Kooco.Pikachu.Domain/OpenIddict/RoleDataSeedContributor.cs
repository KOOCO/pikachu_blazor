using Kooco.Pikachu.Members;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Kooco.Pikachu.OpenIddict;

public class RoleDataSeedContributor(
    ICurrentTenant CurrentTenant,
    IIdentityRoleRepository roleRepository,
    PermissionDefinitionManager permissionDefinitionManager,
    PermissionDataSeeder permissionDataSeeder
    ) : IDataSeedContributor, ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        using (CurrentTenant.Change(CurrentTenant.Id))
        {
            /* Member Role */
            #region MemberRole
            var memberRole = await roleRepository.FindByNormalizedNameAsync(MemberConsts.Role.ToUpper());
            if (memberRole is null)
            {
                memberRole = new IdentityRole(Guid.NewGuid(), MemberConsts.Role, CurrentTenant.Id)
                {
                    IsPublic = true
                };
                await roleRepository.InsertAsync(memberRole, autoSave: true);
            }

            var multiTenancySide = CurrentTenant.GetMultiTenancySide();

            var memberPermissionNames = (await permissionDefinitionManager.GetPermissionsAsync())
                .Where(p => p.Providers.Count == 0 || p.Providers.Contains(RolePermissionValueProvider.ProviderName))
                .Where(p => p.MultiTenancySide.HasFlag(multiTenancySide))
                .Where(p => p.Name == MemberConsts.Members.Default || p.Name == MemberConsts.Members.Edit
                || p.Name == MemberConsts.ShopCarts.Default || p.Name == MemberConsts.UserAddresses.Default
                || p.Name == MemberConsts.UserAddresses.Create || p.Name == MemberConsts.UserAddresses.Edit
                || p.Name == MemberConsts.UserAddresses.Delete || p.Name == MemberConsts.UserAddresses.SetIsDefault
                || p.Name == MemberConsts.UserShoppingCredits.Default)
                .Select(p => p.Name)
                .ToArray();

            await permissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                MemberConsts.Role,
                memberPermissionNames,
                context?.TenantId
               );
            #endregion
        }
    }
}
