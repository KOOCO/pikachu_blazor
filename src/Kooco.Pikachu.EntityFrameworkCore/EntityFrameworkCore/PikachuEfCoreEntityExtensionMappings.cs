using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace Kooco.Pikachu.EntityFrameworkCore;

public static class PikachuEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        PikachuGlobalFeatureConfigurator.Configure();
        PikachuModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE PikachuModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:

                 ObjectExtensionManager.Instance
                     .MapEfCoreProperty<IdentityUser, string>(
                         "MyProperty",
                         (entityBuilder, propertyBuilder) =>
                         {
                             propertyBuilder.HasMaxLength(128);
                         }
                     );


             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */
            ObjectExtensionManager.Instance
                    .MapEfCoreProperty<Tenant, string>(
                       Constant.ShortCode,
                        (entityBuilder, propertyBuilder) =>
                        {
                            propertyBuilder.HasMaxLength(8);
                        }
                    );

            ObjectExtensionManager.Instance
                         .MapEfCoreProperty<Tenant, string>(
                             Constant.TenantContactPerson,
                             (entityBuilder, propertyBuilder) =>
                             {
                                 propertyBuilder.HasMaxLength(512);
                             }
                         );

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<Tenant, string>(
                Constant.TenantContactEmail,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(512);
                }
            );

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<Tenant, string>(
                Constant.Domain,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(512);
                }
            );

            #region IdentityUser
            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, DateTime?>(Constant.Birthday);

            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string?>(Constant.FacebookId,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(100);
                });

            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string?>(Constant.GoogleId,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(100);
                });

            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string?>(Constant.LineId,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(100);
                });

            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string?>(Constant.MobileNumber,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(100);
                });

            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string?>(Constant.Gender,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(20);
                });
            #endregion
        });
    }
}
