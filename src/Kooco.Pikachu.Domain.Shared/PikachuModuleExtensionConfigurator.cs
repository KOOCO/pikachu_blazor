using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Kooco.Pikachu;

public static class PikachuModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           IdentityUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        /* You can configure extra properties for the
         * entities defined in the modules used by your application.
         *
         * This class can be used to define these extra properties
         * with a high level, easy to use API.
         *
         * Example: Add a new property to the user entity of the identity module

           ObjectExtensionManager.Instance.Modules()
              .ConfigureIdentity(identity =>
              {
                  identity.ConfigureUser(user =>
                  {
                      user.AddOrUpdateProperty<string>( //property type: string
                          "SocialSecurityNumber", //property name
                          property =>
                          {
                              //validation rules
                              property.Attributes.Add(new RequiredAttribute());
                              property.Attributes.Add(new StringLengthAttribute(64) {MinimumLength = 4});
                              
                              property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;

                              //...other configurations for this property
                          }
                      );
                  });
              });

         * See the documentation for more:
         * https://docs.abp.io/en/abp/latest/Module-Entity-Extensions
         */
        ObjectExtensionManager.Instance.Modules()
           .ConfigureTenantManagement(tenantConfig =>
           {
               tenantConfig.ConfigureTenant(tenant =>
               {
                   tenant.AddOrUpdateProperty<Guid?>( //property type: string
                       Constant.TenantOwner, //property name
                       property =>
                       {
                           //validation rules
                           //property.Attributes.Add(new RequiredAttribute());
                           //property.Attributes.Add(new StringLengthAttribute(64) { MinimumLength = 4 });
                           property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                       }
                   );
                   tenant.AddOrUpdateProperty<Guid?>( //property type: string
                       Constant.TenantContactPerson, //property name
                       property =>
                       {
                           //validation rules
                           //property.Attributes.Add(new RequiredAttribute());
                           //property.Attributes.Add(new StringLengthAttribute(64) { MinimumLength = 4 });
                           property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                       }
                   );
                   tenant.AddOrUpdateProperty<Guid?>( //property type: string
                       Constant.TenantContactTitle, //property name
                       property =>
                       {
                           //validation rules
                           //property.Attributes.Add(new RequiredAttribute());
                           //property.Attributes.Add(new StringLengthAttribute(64) { MinimumLength = 4 });
                           property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                       }
                   );
                   tenant.AddOrUpdateProperty<Guid?>( //property type: string
                       Constant.TenantContactEmail, //property name
                       property =>
                       {
                           //validation rules
                           //property.Attributes.Add(new RequiredAttribute());
                           //property.Attributes.Add(new StringLengthAttribute(64) { MinimumLength = 4 });
                           property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                       }
                   );
                   tenant.AddOrUpdateProperty<int>(
                       Constant.ShareProfitPercent, //property name
                       property =>
                       {
                           //validation rules
                           //property.Attributes.Add(new RequiredAttribute());
                           //to do: need to make sure number is between 0 and 100
                           property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                       }
                   );
                   tenant.AddOrUpdateProperty<string>(
                      Constant.Logo, //property name
                      property =>
                      {
                          //validation rules
                          //property.Attributes.Add(new RequiredAttribute());
                          //to do: need to make sure number is between 0 and 100
                          property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                      }
                  );
                   tenant.AddOrUpdateProperty<TenantStatus>(
                    Constant.Status, //property name
                    property =>
                    {
                        //validation rules
                        //property.Attributes.Add(new RequiredAttribute());
                        //to do: need to make sure number is between 0 and 100
                        property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                    }
                );
                   tenant.AddOrUpdateProperty<string>(
                   Constant.BannerUrl, //property name
                   property =>
                   {
                       //validation rules
                       //property.Attributes.Add(new RequiredAttribute());
                       //to do: need to make sure number is between 0 and 100
                       property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                   }
               );
                   tenant.AddOrUpdateProperty<string>(
                 Constant.ShortCode, //property name
                 property =>
                 {
                     //validation rules
                     property.Attributes.Add(new RequiredAttribute());
                     //to do: need to make sure number is between 0 and 100
                     property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
                 }
             );
                   tenant.AddOrUpdateProperty<string>(
           Constant.TenantUrl, //property name
           property =>
           {
               //validation rules
               //property.Attributes.Add(new RequiredAttribute());
               //to do: need to make sure number is between 0 and 100
               property.Configuration[IdentityModuleExtensionConsts.ConfigurationNames.AllowUserToEdit] = true;
           }
       );
               });
           });

        ObjectExtensionManager.Instance.Modules()
            .ConfigureIdentity(identity =>
            {
                identity.ConfigureUser(user =>
                {
                    user.AddOrUpdateProperty<DateTime?>(
                        "Birthday",
                        property =>
                        {
                            //validation rules
                            property.Attributes.Add(new RequiredAttribute());
                        }
                    );
                });
            });
    }
}
