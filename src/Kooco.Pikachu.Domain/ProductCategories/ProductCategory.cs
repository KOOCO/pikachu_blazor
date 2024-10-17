using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategory : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Name { get; private set; }
    public string Description { get; set; }
    public Guid? TenantId { get; set; }

    public ProductCategory(
        Guid id,
        string name,
        string description
        ) : base(id)
    {
        SetName(name);
        Description = description;
    }

    public ProductCategory ChangeName(string name)
    {
        SetName(name);
        return this;
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(Name), maxLength: ProductCategoryConsts.MaxNameLength);
    }
}
