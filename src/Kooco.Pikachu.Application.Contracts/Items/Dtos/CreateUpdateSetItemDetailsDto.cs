using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateUpdateSetItemDetailsDto
{
    public Guid? Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDetailsSetItemId")]
    public Guid SetItemId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDetailsSetItem")]
    public SetItemDto SetItem { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDetailsItem")]
    public ItemDto Item { get; set; } = new();

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }
}