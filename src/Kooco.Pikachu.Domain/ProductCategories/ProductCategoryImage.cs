using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryImage : Entity<Guid>
{
    public string Url { get; set; }
    public string BlobName { get; set; }
    public string? Name { get; set; }
    public Guid ProductCategoryId { get; set; }
    public int SortNo { get; set; }

    [ForeignKey(nameof(ProductCategoryId))]
    public ProductCategory? ProductCategory { get; set; }

    public ProductCategoryImage(
        Guid id,
        Guid productCategoryId,
        string url,
        string blobName,
        string? name,
        int sortNo
        ) : base(id)
    {
        ProductCategoryId = productCategoryId;
        SetUrl(url);
        SetBlobName(blobName);
        Name = name;
        SortNo = sortNo;
    }

    public void SetBlobName(string blobName)
    {
        BlobName = Check.NotNullOrWhiteSpace(blobName, nameof(BlobName));
    }

    public void SetUrl(string url)
    {
        Url = Check.NotNullOrWhiteSpace(url, nameof(Url));
    }
}
