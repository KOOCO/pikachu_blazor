using System;
using System.ComponentModel;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateUpdateSetItemDetailsDto
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDetailsSetItemId")]
    public Guid SetItemId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDetailsSetItem")]
    public SetItem SetItem { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("SetItemDetailsItem")]
    public Item Item { get; set; }
}