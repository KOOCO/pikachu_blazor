using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Groupbuys;

public class GroupBuyItemGroupImageModule : Entity<Guid>
{
    public Guid GroupBuyItemGroupId { get; set; }

    public List<GroupBuyItemGroupImage> Images { get; set; } = [];

    [ForeignKey(nameof(GroupBuyItemGroupId))]
    public virtual GroupBuyItemGroup? GroupBuyItemGroup { get; set; }

    public GroupBuyItemGroupImageModule(Guid id) : base(id)
    {

    }
}

public class GroupBuyItemGroupImage : Entity<Guid>
{
    public Guid GroupBuyItemGroupImageModuleId { get; set; }
    public string Url { get; set; }
    public string BlobImageName { get; set; }
    public int SortNo { get; set; }

    [ForeignKey(nameof(GroupBuyItemGroupImageModuleId))]
    public virtual GroupBuyItemGroupImageModule? GroupBuyItemGroupImageModule { get; set; }
    public GroupBuyItemGroupImage(Guid id) : base(id)
    {

    }
}
