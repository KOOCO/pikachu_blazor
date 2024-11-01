using Kooco.Pikachu.ProductCategories;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Items;

public class CreateUpdateItemCategoryDto
{
    [Required]
    public Guid? ProductCategoryId { get; set; }

    public string? ProductCategoryName { get; set; }

    public string ImageUrl { get; set; }
}
