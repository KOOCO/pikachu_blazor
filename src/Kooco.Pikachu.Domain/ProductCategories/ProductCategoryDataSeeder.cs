using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.ProductCategories
{
    public class ProductCategoryDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        private readonly IRepository<Tenant, Guid> _tenantRepository;
        private readonly ICurrentTenant _currentTenant;

        public ProductCategoryDataSeeder(
            IRepository<ProductCategory, Guid> productCategoryRepository,
            IRepository<Tenant, Guid> tenantRepository,
            ICurrentTenant currentTenant)
        {
            _productCategoryRepository = productCategoryRepository;
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var tenants = await _tenantRepository.GetListAsync();

            foreach (var tenant in tenants)
            {
                using (_currentTenant.Change(tenant.Id))
                {
                    bool hasData = await _productCategoryRepository.AnyAsync();
                    if (hasData)
                        continue;

                    var categories = new List<ProductCategory>
                {
                    new(Guid.NewGuid(), "Apparel & Accessories", "服飾及配件", null),
                    new(Guid.NewGuid(), "Health & Beauty", "美妝保健", null),
                    new(Guid.NewGuid(), "Home & Living", "居家生活", null),
                    new(Guid.NewGuid(), "Electronic Devices & Accessories", "3C產品及周邊配件", null),
                    new(Guid.NewGuid(), "Home Appliances", "家用電器", null),
                    new(Guid.NewGuid(), "Sports & Outdoors", "運動戶外", null),
                    new(Guid.NewGuid(), "Food & Beverage", "食品飲料", null),
                    new(Guid.NewGuid(), "Maternity & Nursing", "孕婦與哺乳用品", null),
                    new(Guid.NewGuid(), "Babies & Kids", "嬰幼兒與兒童用品", null),
                    new(Guid.NewGuid(), "Home Improvement & Tools", "居家修繕", null),
                    new(Guid.NewGuid(), "Automotive", "汽機車與配件", null),
                    new(Guid.NewGuid(), "Pet Supplies", "寵物用品", null),
                    new(Guid.NewGuid(), "Office & Stationery", "辦公用品及文具", null),
                    new(Guid.NewGuid(), "Toys & Games", "玩具遊戲", null),
                    new(Guid.NewGuid(), "Jewelry & Watches", "珠寶手錶", null),
                    new(Guid.NewGuid(), "Gifts & Occasions", "禮品節慶", null)
                };

                    foreach (var category in categories)
                    {
                        category.TenantId = tenant.Id;
                        await _productCategoryRepository.InsertAsync(category, autoSave: true);
                    }
                }
            }
        }
    }

}
