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
            b.HasMany(x => x.ItemDetails).WithOne(d => d.Item);
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
            b.HasOne(x => x.Item).WithMany(d => d.ItemDetails).HasForeignKey(i => i.ItemId);
        });

        builder.Entity<Image>(b =>
        {
            b.ToTable(PikachuConsts.DbTablePrefix + "Images", PikachuConsts.DbSchema, table => table.HasComment(""));
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
            b.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);
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
        });
    }
}
