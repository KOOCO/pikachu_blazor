using System;
using Volo.Abp.Application.Dtos;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Items.Dtos;

/// <summary>
/// 
/// </summary>
[Serializable]
public class SetItemDetailsDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 
    /// </summary>
    public Guid SetItemId { get; set; }
    public string? ItemName { get; set; }
    public ItemDto Item { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public bool IsSelected { get; set; }
    public string? Attribute1Value { get; set; }
    public string? Attribute2Value { get; set; }
    public string? Attribute3Value { get; set; }
}