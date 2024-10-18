using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ProductCategories;

public class CreateUpdateProductCategoryImageDto
{
    public Guid? Id { get; set; }

    [Required]
    public string Url { get; set; }

    [Required]
    public string BlobName { get; set; }

    public string? Name { get; set; }
}