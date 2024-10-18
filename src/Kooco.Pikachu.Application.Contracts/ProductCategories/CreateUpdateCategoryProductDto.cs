using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ProductCategories;

public class CreateUpdateCategoryProductDto
{
    [Required]
    public Guid? ItemId { get; set; }
}