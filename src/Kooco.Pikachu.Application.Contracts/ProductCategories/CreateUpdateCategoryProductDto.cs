using Kooco.Pikachu.Items.Dtos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ProductCategories;

public class CreateUpdateCategoryProductDto
{
    [Required]
    public Guid? ItemId { get; set; }

    public ItemDto? Item { get; set; }

    public string? ItemImageUrl { get; set; }
}