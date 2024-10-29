using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Kooco.Pikachu.Items;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore.Metadata;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.StoreComments;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.TenantEmailing;
using Kooco.Pikachu.AutomaticEmails;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.DeliveryTempratureCosts;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserShoppingCredits;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeOrders;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.ShopCarts;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.WebsiteManagement;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.LoginConfigurations;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.ProductCategories;

namespace Kooco.Pikachu.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class PikachuDbContext :
    AbpDbContext<PikachuDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DbSet<Item> Items { get; set; }
    public DbSet<EnumValue> EnumValues { get; set; }
    public DbSet<ItemDetails> ItemDetails { get; set; }
    public DbSet<SetItem> SetItems { get; set; }
    public DbSet<SetItemDetails> SetItemDetails { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<GroupBuy> GroupBuys { get; set; }
    public DbSet<GroupBuyItemGroup> GroupBuyItemGroups { get; set; }
    public DbSet<GroupBuyItemGroupDetails> GroupBuyItemGroupDetails { get; set; }

    public DbSet<Freebie> Freebies { get; set; }
    public DbSet<FreebieGroupBuys> FreebieGroupBuys { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<StoreComment> StoreComments { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<PaymentGateway> PaymentGateways { get; set; }
    public DbSet<ElectronicInvoiceSetting> ElectronicInvoiceSettings { get; set; }
    public DbSet<TenantEmailSettings> TenantEmailSettings { get; set; }
    public DbSet<AutomaticEmail> AutomaticEmails { get; set; }
    public DbSet<AutomaticEmailGroupBuys> AutomaticEmailGroupBuys { get; set; }
    public DbSet<LogisticsProviderSettings> LogisticsProviderSettings { get; set; }
    public DbSet<DeliveryTemperatureCost> DeliveryTemperatureCosts { get; set; }
    public DbSet<OrderDelivery> OrderDeliveries { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<UserShoppingCredit> UserShoppingCredits { get; set; }
    public DbSet<UserCumulativeCredit> UserCumulativeCredits { get; set; }
    public DbSet<UserCumulativeOrder> UserCumulativeOrders { get; set; }
    public DbSet<AddOnProduct> AddOnProducts { get; set; }
    public DbSet<AddOnProductSpecificGroupbuy> AddOnProductSpecificGroupbuys { get; set; }
    public DbSet<UserCumulativeFinancial> UserCumulativeFinancials { get; set; }
    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<DiscountSpecificGroupbuy> DiscountSpecificGroupbuys { get; set; }
    public DbSet<DiscountSpecificProduct> DiscountSpecificProducts { get; set; }
    public DbSet<DiscountCodeUsage> DiscountCodeUsages { get; set; }
    public DbSet<ShoppingCreditUsageSetting> ShoppingCreditUsageSettings { get; set; }
    public DbSet<ShoppingCreditUsageSpecificGroupbuy> ShoppingCreditUsageSpecificGroupbuys { get; set; }
    public DbSet<ShoppingCreditUsageSpecificProduct> ShoppingCreditUsageSpecificProducts { get; set; }
    public DbSet<ShoppingCreditEarnSetting> ShoppingCreditEarnSettings { get; set; }
    public DbSet<ShoppingCreditEarnSpecificGroupbuy> ShoppingCreditEarnSpecificGroupbuys { get; set; }
    public DbSet<ShoppingCreditEarnSpecificProduct> ShoppingCreditEarnSpecificProducts { get; set; }
    public DbSet<GroupPurchaseOverview> GroupPurchaseOverviews { get; set; }
    public DbSet<GroupBuyOrderInstruction> GroupBuyOrderInstructions { get; set; }
    public DbSet<GroupBuyProductRanking> GroupBuyProductRankings { get; set; }
    public DbSet<ShopCart> ShopCarts { get; set; }
    public DbSet<TenantSettings> TenantSettings { get; set; }

    public DbSet<OrderMessage> OrderMessages { get; set; }

    public DbSet<WebsiteSettings> WebsiteSettings { get; set; }
    public DbSet<LoginConfiguration> LoginConfigurations { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }

    public PikachuDbContext(DbContextOptions<PikachuDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        builder.Entity<Item>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Items", PikachuConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.HasKey(x => x.Id).HasName("PK_AppItems").IsClustered(false);  // 移除默认的聚集索引
            b.HasIndex(x => x.ItemNo).IsUnique().IsClustered();// 添加 ItemNo 的聚集索引
            b.Property(x => x.ItemNo).IsRequired().ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            b.Property(x => x.ItemName).IsRequired();
            b.Property(x => x.Returnable).IsRequired();
            b.HasMany(x => x.ItemDetails).WithOne();
        });

        builder.Entity<EnumValue>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "EnumValues", PikachuConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.EnumType).IsRequired();
            b.Property(x => x.Text).IsRequired();
        });

        builder.Entity<ItemDetails>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ItemDetails", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

        });

        builder.Entity<Image>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Images", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<DeliveryTemperatureCost>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "DeliveryTemperatureCosts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<SetItem>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "SetItems", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasMany(x => x.SetItemDetails).WithOne(d => d.SetItem);
        });

        builder.Entity<SetItemDetails>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "SetItemDetails", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<GroupBuy>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "GroupBuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(x => x.ItemGroups).WithOne();
        });

        builder.Entity<GroupBuyItemGroup>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "GroupBuyItemGroups", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(x => x.ItemGroupDetails).WithOne();

        });

        builder.Entity<GroupBuyItemGroupDetails>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "GroupBuyItemGroupDetails", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasOne(x => x.Item).WithMany().IsRequired(false).HasForeignKey(x => x.ItemId);
            b.HasOne(x => x.SetItem).WithMany().IsRequired(false).HasForeignKey(x => x.SetItemId);
        });

        builder.Entity<Freebie>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Freebie", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.Property(x => x.FreebieAmount).HasColumnType("money");
            b.Property(x => x.MinimumAmount).HasColumnType("money");
            b.HasMany(x => x.FreebieGroupBuys).WithOne();
        });

        builder.Entity<FreebieGroupBuys>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "FreebieGroupBuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasKey(x => new { x.FreebieId, x.GroupBuyId });
        });

        builder.Entity<Order>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Orders", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasOne(o => o.GroupBuy).WithMany().HasForeignKey(o => o.GroupBuyId);
            b.HasMany(o => o.OrderItems).WithOne().HasForeignKey(d => d.OrderId);
            b.HasMany(o => o.StoreComments).WithOne();
            b.Property(o => o.TotalAmount).HasColumnType("money");
            b.Property(p => p.RefundAmount).HasColumnType("decimal(18,2)");
        });
        builder.Entity<OrderDelivery>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "OrderDeliveries", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(o => o.Items).WithOne().HasForeignKey(d => d.DeliveryOrderId);

            b.HasOne<Order>().WithMany().HasForeignKey(d => d.OrderId);

        });
        builder.Entity<OrderItem>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "OrderItems", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasOne(x => x.Item).WithMany().IsRequired(false).HasForeignKey(x => x.ItemId);

            b.HasOne(x => x.SetItem).WithMany().IsRequired(false).HasForeignKey(x => x.SetItemId);
            b.HasOne(x => x.Freebie).WithMany().IsRequired(false).HasForeignKey(x => x.FreebieId);
            b.Property(x => x.ItemPrice).HasColumnType("money");
            b.Property(x => x.TotalAmount).HasColumnType("money");
        });

        builder.Entity<StoreComment>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "StoreComments", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.CreatorId);
        });

        builder.Entity<Refund>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Refunds", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId);
        });

        builder.Entity<PaymentGateway>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "PaymentGateways", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<ElectronicInvoiceSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ElectronicInvoiceSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<TenantEmailSettings>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "TenantEmailSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<AutomaticEmail>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "AutomaticEmails", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.Ignore(x => x.RecipientsList);
            b.HasMany(x => x.GroupBuys).WithOne().HasForeignKey(x => x.AutomaticEmailId);
        });

        builder.Entity<AutomaticEmailGroupBuys>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "AutomaticEmailGroupBuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.GroupBuy).WithMany().IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<LogisticsProviderSettings>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "LogisticsProviderSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            // b.Ignore(x => x.LogisticsSubTypesList);
            b.Ignore(x => x.MainIslandsList);
            b.Ignore(x => x.OuterIslandsList);
        });

        builder.Entity<UserAddress>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "UserAddresses", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<UserShoppingCredit>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "UserShoppingCredits", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<UserCumulativeCredit>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "UserCumulativeCredits", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<UserCumulativeOrder>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "UserCumulativeOrders", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<AddOnProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "AddOnProducts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<AddOnProductSpecificGroupbuy>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "AddOnProductSpecificGroupbuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<UserCumulativeFinancial>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "UserCumulativeFinancials", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<DiscountCode>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "DiscountCodes", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<DiscountSpecificGroupbuy>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "DiscountSpecificGroupbuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<DiscountSpecificProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "DiscountSpecificProducts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<DiscountCodeUsage>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "DiscountCodeUsages", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<ShopCart>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShopCarts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(x => x.CartItems).WithOne(x => x.ShopCart).HasForeignKey(x => x.ShopCartId);
        });

        builder.Entity<CartItem>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CartItems", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.ShopCart).WithMany(x => x.CartItems).HasForeignKey(x => x.ShopCartId);
            b.HasIndex(x => x.ItemDetailId);
        });
        builder.Entity<ShoppingCreditUsageSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShoppingCreditUsageSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<ShoppingCreditUsageSpecificGroupbuy>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShoppingCreditUsageSpecificGroupbuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<ShoppingCreditUsageSpecificProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShoppingCreditUsageSpecificProducts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<ShoppingCreditEarnSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShoppingCreditEarnSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<ShoppingCreditEarnSpecificGroupbuy>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShoppingCreditEarnSpecificGroupbuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<ShoppingCreditEarnSpecificProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ShoppingCreditEarnSpecificProducts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<TenantSettings>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "TenantSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });
        builder.Entity<OrderMessage>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "OrderMessages", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<WebsiteSettings>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<LoginConfiguration>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "LoginConfigurations", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<ProductCategory>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ProductCategories", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(x => x.ProductCategoryImages).WithOne(x => x.ProductCategory).HasForeignKey(x => x.ProductCategoryId);
            b.HasMany(x => x.CategoryProducts).WithOne(x => x.ProductCategory).HasForeignKey(x => x.ProductCategoryId);
        });

        builder.Entity<ProductCategoryImage>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ProductCategoryImages", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.ProductCategory).WithMany(x => x.ProductCategoryImages).HasForeignKey(x => x.ProductCategoryId);
        });

        builder.Entity<CategoryProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CategoryProducts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasKey(x => new { x.ProductCategoryId, x.ItemId });

            b.HasOne(x => x.ProductCategory).WithMany(x => x.CategoryProducts).HasForeignKey(x => x.ProductCategoryId);
            b.HasOne(x => x.Item).WithMany(x => x.CategoryProducts).HasForeignKey(x => x.ItemId);
        });

        #region GroupPurchaseOverviews
        builder.Entity<GroupPurchaseOverview>(b =>
        {
            b.ToTable(
                 PikachuConsts.DbTablePrefix + "GroupPurchaseOverviews",
                 PikachuConsts.DbSchema,
                 table => table.HasComment("")
            );
            b.ConfigureByConvention();
        });
        #endregion

        #region GroupBuyOrderInstructions
        builder.Entity<GroupBuyOrderInstruction>(b =>
        {
            b.ToTable(
                PikachuConsts.DbTablePrefix + "GroupBuyOrderInstructions",
                PikachuConsts.DbSchema,
                table => table.HasComment("")
            );
            b.ConfigureByConvention();
        });
        #endregion

        #region GroupBuyProductRankings
        builder.Entity<GroupBuyProductRanking>(b =>
        {
            b.ToTable(
                PikachuConsts.DbTablePrefix + "GroupBuyProductRankings",
                PikachuConsts.DbSchema,
                table => table.HasComment("")
            );
            b.ConfigureByConvention();
        });
        #endregion
    }
}

