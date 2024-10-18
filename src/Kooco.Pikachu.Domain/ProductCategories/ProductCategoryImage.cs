using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryImage : Entity<Guid>
{
    public string Name { get; set; }
    public string BlobName { get; set; }
    public string Url { get; set; }
    public Guid ProductCategoryId { get; set; }
    
    [ForeignKey(nameof(ProductCategoryId))]
    public ProductCategory? ProductCategory { get; set; }

    public ProductCategoryImage(
        Guid id,
        string name,
        string blobName,
        string url,
        Guid productCategoryId
        ) : base(id)
    {
        ProductCategoryId = productCategoryId;
        SetName(name);
        SetBlobName(blobName);
        SetUrl(url);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(Name));
    }

    public void SetBlobName(string blobName)
    {
        BlobName = Check.NotNullOrWhiteSpace(blobName, nameof(BlobName));
    }

    public void SetUrl(string url)
    {
        Url = Check.NotNullOrWhiteSpace(url, nameof(url));
    }
}
