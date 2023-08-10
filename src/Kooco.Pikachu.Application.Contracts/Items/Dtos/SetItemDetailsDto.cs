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

    /// <summary>
    /// 
    /// </summary>
    public SetItem SetItem { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Item Item { get; set; }
}