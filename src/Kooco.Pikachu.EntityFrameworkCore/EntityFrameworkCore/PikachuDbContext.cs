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
using Kooco.Pikachu.StoreComments;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.AutomaticEmails;
using Kooco.Pikachu.DeliveryTempratureCosts;
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
using Kooco.Pikachu.WebsiteManagement;
using Kooco.Pikachu.GroupBuyOrderInstructions;
using Kooco.Pikachu.LoginConfigurations;
using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.ProductCategories;
using Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using Kooco.Pikachu.WebsiteManagement.FooterSettings;
using Kooco.Pikachu.WebsiteManagement.TopbarSettings;
using Kooco.Pikachu.TierManagement;
using System.Reflection;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.GroupBuyItemsPriceses;
using Kooco.Pikachu.LogisticsSettings;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Members.MemberTags;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.EdmManagement;
using Kooco.Pikachu.Domain.LogisticStatusRecords;
using Kooco.Pikachu.InventoryManagement;
using Kooco.Pikachu.OrderTradeNos;
using Kooco.Pikachu.LogisticsFeeManagements;
using System.Reflection.Emit;
using Kooco.Pikachu.Reconciliations;
using Kooco.Pikachu.InboxManagement;

namespace Kooco.Pikachu.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class PikachuDbContext(DbContextOptions<PikachuDbContext> options) :
    AbpDbContext<PikachuDbContext>(options),
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
    public DbSet<FreebieProducts> FreebieProducts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ManualBankTransferRecord> ManualBankTransferRecords { get; set; }
    public DbSet<AppliedCampaign> AppliedCampaigns { get; set; }
    public DbSet<StoreComment> StoreComments { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<PaymentGateway> PaymentGateways { get; set; }
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

    // Order
    public DbSet<OrderMessage> OrderMessages { get; set; }
    public DbSet<OrderInvoice> OrderInvoices { get; set; }
    public DbSet<OrderTradeNo> OrderTradeNos { get; set; }
    public DbSet<LogisticStatusRecord> LogisticStatusRecords { get; set; }

    public DbSet<WebsiteBasicSetting> WebsiteBasicSettings { get; set; }
    public DbSet<WebsiteSettings> WebsiteSettings { get; set; }
    public DbSet<WebsiteSettingsModule> WebsiteSettingsModules { get; set; }
    public DbSet<WebsiteSettingsModuleItem> WebsiteSettingsModuleItems { get; set; }
    public DbSet<WebsiteSettingsOverviewModule> WebsiteSettingsOverviewModules { get; set; }
    public DbSet<WebsiteSettingsInstructionModule> WebsiteSettingsInstructionModules { get; set; }
    public DbSet<WebsiteSettingsProductRankingModule> WebsiteSettingsProductRankingModules { get; set; }

    public DbSet<FooterSetting> FooterSettings { get; set; }
    public DbSet<TopbarSetting> TopbarSettings { get; set; }

    public DbSet<LoginConfiguration> LoginConfigurations { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<OrderHistory> OrderHistories { get; set; }
    public DbSet<OrderTransaction> OrderTransactions { get; set; }

    public DbSet<IdentitySession> Sessions { get; set; }

    public DbSet<VipTierSetting> VipTierSettings { get; set; }
    public DbSet<MemberTag> MemberTags { get; set; }
    public DbSet<MemberTagFilter> MemberTagFilters { get; set; }
    public DbSet<GroupBuyItemsPrice> GroupBuyItemsPriceses { get; set; }

    // Tenant
    public DbSet<TenantTripartite> TenantTripartites { get; set; }
    public DbSet<TenantEmailSettings> TenantEmailSettings { get; set; }
    public DbSet<TenantWallet> TenantWallets { get; set; }
    public DbSet<TenantWalletTransaction> TenantWalletTransactions { get; set; }
    public DbSet<GroupBuyItemGroupImageModule> GroupBuyItemGroupImageModules { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignDiscount> CampaignDiscounts { get; set; }
    public DbSet<CampaignShoppingCredit> CampaignShoppingCredits { get; set; }
    public DbSet<CampaignAddOnProduct> CampaignAddOnProducts { get; set; }

    public DbSet<Edm> Edms { get; set; }
    public DbSet<InventoryLog> InventoryLogs { get; set; }

    public DbSet<LogisticsFeeFileImport> LogisticsFeeFileImports { get; set; }
    public DbSet<TenantLogisticsFeeFileProcessingSummary> TenantLogisticsFeeFileProcessingSummaries { get; set; }
    public DbSet<TenantLogisticsFeeRecord> TenantLogisticsFeeRecord { get; set; }

    public DbSet<EcPayReconciliationRecord> EcPayReconciliationRecords { get; set; }

    public DbSet<Notification> Notifications { get; set; }

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
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

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
        builder.Entity<OrderTradeNo>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "OrderTradeNos", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();


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

        builder.Entity<GroupBuyItemGroupImageModule>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "GroupBuyItemGroupImageModules", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.GroupBuyItemGroup).WithMany(x => x.ImageModules).HasForeignKey(x => x.GroupBuyItemGroupId);
            b.HasMany(x => x.Images).WithOne(x => x.GroupBuyItemGroupImageModule).HasForeignKey(x => x.GroupBuyItemGroupImageModuleId);
        });

        builder.Entity<GroupBuyItemGroupImage>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "GroupBuyItemGroupImages", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.GroupBuyItemGroupImageModule).WithMany(x => x.Images).HasForeignKey(x => x.GroupBuyItemGroupImageModuleId);
        });

        builder.Entity<Freebie>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Freebie", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.Property(x => x.FreebieAmount).HasColumnType("money");
            b.Property(x => x.MinimumAmount).HasColumnType("money");
            b.HasMany(x => x.FreebieGroupBuys).WithOne();
            b.HasMany(x => x.FreebieProducts).WithOne();
        });

        builder.Entity<FreebieGroupBuys>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "FreebieGroupBuys", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasKey(x => new { x.FreebieId, x.GroupBuyId });
        });
        builder.Entity<FreebieProducts>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "FreebieProducts", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
            b.HasKey(x => new { x.FreebieId, x.ProductId });
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
            b.Property(p => p.cashback_amount).HasColumnType("decimal(18,2)");

            if (Database.IsSqlServer())
            {
                b.Property(o => o.RowVersion).IsRowVersion();
            }

            b.HasMany(x => x.OrderTransactions).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
            b.HasMany(x => x.AppliedCampaigns).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
            b.HasOne(x => x.ManualBankTransferRecord).WithOne(x => x.Order).HasForeignKey<ManualBankTransferRecord>(x => x.OrderId).IsRequired(false);
        });

        builder.Entity<ManualBankTransferRecord>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "ManualBankTransferRecords", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Order).WithOne(x => x.ManualBankTransferRecord).HasForeignKey<ManualBankTransferRecord>(x => x.OrderId).IsRequired(false);
        });

        builder.Entity<AppliedCampaign>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "AppliedCampaigns", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Order).WithMany(x => x.AppliedCampaigns).HasForeignKey(x => x.OrderId);
            b.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignId);
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

        builder.Entity<LogisticStatusRecord>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "LogisticStatusRecords", PikachuConsts.DbSchema);
            b.Property(x => x.OrderId).IsRequired().HasMaxLength(32);
            b.Property(x => x.Reference).HasMaxLength(64);
            b.Property(x => x.Location).HasMaxLength(128);
            b.Property(x => x.Datetime).HasMaxLength(32); // 原始字串型別的日期時間
            b.Property(x => x.DatetimeParsed).HasColumnType("datetime2"); // 新增的 DateTime 型別欄位
            b.Property(x => x.Code).HasMaxLength(32);
            b.Property(x => x.StatusCode).HasMaxLength(16);
            b.Property(x => x.StatusMessage).HasMaxLength(128);
            b.Property(x => x.RawLine).HasMaxLength(512);
            b.Property(x => x.CreateTime).IsRequired();
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

        builder.Entity<WebsiteBasicSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteBasicSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<WebsiteSettings>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettings", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(x => x.Modules).WithOne(x => x.WebsiteSettings).HasForeignKey(x => x.WebsiteSettingsId);
            b.HasMany(x => x.OverviewModules).WithOne(x => x.WebsiteSettings).HasForeignKey(x => x.WebsiteSettingsId);
            b.HasMany(x => x.InstructionModules).WithOne(x => x.WebsiteSettings).HasForeignKey(x => x.WebsiteSettingsId);
            b.HasMany(x => x.ProductRankingModules).WithOne(x => x.WebsiteSettings).HasForeignKey(x => x.WebsiteSettingsId);
        });

        builder.Entity<WebsiteSettingsModule>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettingsModules", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasMany(x => x.ModuleItems).WithOne().HasForeignKey(x => x.WebsiteSettingsModuleId);
            b.HasOne(x => x.WebsiteSettings).WithMany(x => x.Modules).HasForeignKey(x => x.WebsiteSettingsId);
        });

        builder.Entity<WebsiteSettingsModuleItem>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettingsModuleItems", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.WebsiteSettingsModule).WithMany(x => x.ModuleItems).HasForeignKey(x => x.WebsiteSettingsModuleId);
        });

        builder.Entity<WebsiteSettingsOverviewModule>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettingsOverviewModules", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.WebsiteSettings).WithMany(x => x.OverviewModules).HasForeignKey(x => x.WebsiteSettingsId);
        });

        builder.Entity<WebsiteSettingsInstructionModule>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettingsInstructionModules", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.WebsiteSettings).WithMany(x => x.InstructionModules).HasForeignKey(x => x.WebsiteSettingsId);
        });

        builder.Entity<WebsiteSettingsProductRankingModule>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "WebsiteSettingsProductRankingModules", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();

            b.HasOne(x => x.WebsiteSettings).WithMany(x => x.ProductRankingModules).HasForeignKey(x => x.WebsiteSettingsId);
        });

        builder.Entity<FooterSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "FooterSettings", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasMany(x => x.Sections).WithOne(x => x.FooterSetting).HasForeignKey(x => x.FooterSettingId);
        });

        builder.Entity<FooterSettingSection>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "FooterSettingSections", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.FooterSetting).WithMany(x => x.Sections).HasForeignKey(x => x.FooterSettingId);
            b.HasMany(x => x.Links).WithOne(x => x.Section).HasForeignKey(x => x.SectionId);
        });

        builder.Entity<FooterSettingLink>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "FooterSettingLinks", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Section).WithMany(x => x.Links).HasForeignKey(x => x.SectionId);
        });

        builder.Entity<TopbarSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "TopbarSettings", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasMany(x => x.Links).WithOne(x => x.TopbarSetting).HasForeignKey(x => x.TopbarSettingId);
        });

        builder.Entity<TopbarSettingLink>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "TopbarSettingLinks", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.TopbarSetting).WithMany(x => x.Links).HasForeignKey(x => x.TopbarSettingId);
            b.HasMany(x => x.CategoryOptions).WithOne(x => x.TopbarSettingLink).HasForeignKey(x => x.TopbarSettingLinkId);
        });

        builder.Entity<TopbarSettingCategoryOption>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "TopbarSettingCategoryOptions", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.TopbarSettingLink).WithMany(x => x.CategoryOptions).HasForeignKey(x => x.TopbarSettingLinkId);
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
        builder.Entity<OrderHistory>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "OrderHistories", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();


        });

        builder.Entity<OrderTransaction>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "OrderTransactions", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Amount).HasColumnType("money");
            b.HasOne(x => x.Order).WithMany(x => x.OrderTransactions).HasForeignKey(x => x.OrderId);
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

        builder.Entity<VipTierSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "VipTierSettings", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasMany(x => x.Tiers).WithOne(x => x.TierSetting).HasForeignKey(x => x.TierSettingId);
        });

        builder.Entity<VipTier>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "VipTiers", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.TierSetting).WithMany(x => x.Tiers).HasForeignKey(x => x.TierSettingId);
        });

        builder.Entity<MemberTag>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "MemberTags", PikachuConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<MemberTagFilter>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "MemberTagFilters", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Tag).HasMaxLength(MemberTagConsts.MemberTagNameMaxLength);
            b.HasIndex(x => x.Tag);
        });

        builder.Entity<GroupBuyItemsPrice>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "GroupBuyItemsPriceses", PikachuConsts.DbSchema, table => table.HasComment(""));
            b.ConfigureByConvention();
        });

        builder.Entity<Campaign>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Campaigns", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(CampaignConsts.MaxNameLength);
            b.Property(x => x.Description).HasMaxLength(CampaignConsts.MaxDescriptionLength);

            b.HasOne(c => c.Discount)
                .WithOne(x => x.Campaign)
                .HasForeignKey<CampaignDiscount>(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(c => c.ShoppingCredit)
                .WithOne(x => x.Campaign)
                .HasForeignKey<CampaignShoppingCredit>(s => s.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(c => c.AddOnProduct)
                .WithOne(x => x.Campaign)
                .HasForeignKey<CampaignAddOnProduct>(a => a.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.GroupBuys)
                .WithOne(x => x.Campaign)
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Products)
                .WithOne(x => x.Campaign)
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UseableCampaign>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "UseableCampaigns", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Campaign)
                .WithMany(x => x.UseableCampaigns)
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CampaignDiscount>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CampaignDiscounts", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.DiscountCode).HasMaxLength(CampaignConsts.MaxDiscountCodeLength);
        });

        builder.Entity<CampaignShoppingCredit>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CampaignShoppingCredits", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasMany(x => x.StageSettings).WithOne(x => x.ShoppingCredit).HasForeignKey(x => x.ShoppingCreditId);
        });

        builder.Entity<CampaignAddOnProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CampaignAddOnProducts", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });

        builder.Entity<CampaignStageSetting>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CampaignStageSettings", PikachuConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<CampaignGroupBuy>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CampaignGroupBuys", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.GroupBuy).WithMany().HasForeignKey(x => x.GroupBuyId);
        });

        builder.Entity<CampaignProduct>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "CampaignProducts", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });

        builder.Entity<Edm>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Edms", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.GroupBuy).WithMany().HasForeignKey(x => x.GroupBuyId);
            b.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignId);
        });

        builder.Entity<InventoryLog>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "InventoryLogs", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.ItemDetail).WithMany().HasForeignKey(x => x.ItemDetailId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        });
        // FileImport configuration (NOT tenant-specific)
        builder.Entity<LogisticsFeeFileImport>(entity =>
        {
            entity.ToTable(PikachuConsts.DbTablePrefix + "LogisticsFeeFileImports", PikachuConsts.DbSchema);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.OriginalFileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ProcessingNotes).HasMaxLength(1000);

            entity.HasIndex(e => e.UploadDate);
            entity.HasIndex(e => e.BatchStatus);
            entity.HasIndex(e => e.FileType);
            entity.HasIndex(e => e.UploadedByUserId);
            entity.HasIndex(e => new { e.FileType, e.UploadDate });
        });

        // TenantFileProcessingSummary configuration
        builder.Entity<TenantLogisticsFeeFileProcessingSummary>(entity =>
        {
            entity.ToTable(PikachuConsts.DbTablePrefix + "TenantLogisticsFeeFileProcessingSummaries", PikachuConsts.DbSchema);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantTotalAmount).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.LogisticsFeeFileImport)
                  .WithMany(f => f.LogisticsFeeTenantSummaries)
                  .HasForeignKey(e => e.FileImportId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.FileImportId);
            entity.HasIndex(e => new { e.TenantId, e.FileImportId }).IsUnique();
        });

        // TenantFeeRecord configuration
        builder.Entity<TenantLogisticsFeeRecord>(entity =>
        {
            entity.ToTable(PikachuConsts.DbTablePrefix + "TenantLogisticsFeeRecords", PikachuConsts.DbSchema);
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LogisticFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FailureReason).HasMaxLength(500);

            entity.HasOne(e => e.LogisticsFeeFileImport)
                  .WithMany(f => f.TenantLogisticsFeeRecords)
                  .HasForeignKey(e => e.FileImportId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TenantWalletTransaction)
                  .WithMany(t => t.TenantLogisticsFeeRecords)
                  .HasForeignKey(e => e.TenantWalletTransactionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.OrderNumber);
            entity.HasIndex(e => e.DeductionStatus);
            entity.HasIndex(e => e.FileImportId);
            entity.HasIndex(e => e.FileType);
            entity.HasIndex(e => new { e.TenantId, e.FileImportId });
            entity.HasIndex(e => new { e.TenantId, e.FileType });
            entity.HasIndex(e => new { e.TenantId, e.FileType, e.DeductionStatus });
        });

        builder.Entity<EcPayReconciliationRecord>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "EcPayReconciliationRecord", PikachuConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Notification>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Notifications", PikachuConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.ReadBy).WithMany().HasForeignKey(x => x.ReadById).IsRequired(false);
            b.Property(x => x.Title).IsRequired().HasMaxLength(NotificationConsts.MaxTitleLength);
            b.Property(x => x.Message).HasMaxLength(NotificationConsts.MaxMessageLength);
            b.Property(x => x.TitleParamsJson).HasMaxLength(NotificationConsts.MaxParamsJsonLength);
            b.Property(x => x.MessageParamsJson).HasMaxLength(NotificationConsts.MaxParamsJsonLength);
            b.Property(x => x.UrlParamsJson).HasMaxLength(NotificationConsts.MaxParamsJsonLength);
            b.Property(x => x.EntityId).HasMaxLength(NotificationConsts.MaxEntityIdLength);
        });
    }
}