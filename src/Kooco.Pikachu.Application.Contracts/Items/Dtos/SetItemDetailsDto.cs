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

    public ItemDto Item { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }
}