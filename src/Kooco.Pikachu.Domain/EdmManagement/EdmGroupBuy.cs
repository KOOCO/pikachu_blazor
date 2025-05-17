using Kooco.Pikachu.GroupBuys;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.EdmManagement;

public class EdmGroupBuy : Entity<Guid>
{
    public Guid EdmId { get; set; }
    public Guid GroupBuyId { get; set; }

    [NotMapped]
    public string? GroupBuyName { get; set; }

    [ForeignKey(nameof(EdmId))]
    public virtual Edm Edm { get; set; }

    [ForeignKey(nameof(GroupBuyId))]
    public virtual GroupBuy GroupBuy { get; set; }

    private EdmGroupBuy() { }

    internal EdmGroupBuy(Guid id, Guid edmId, Guid groupBuyId) : base(id)
    {
        EdmId = edmId;
        GroupBuyId = groupBuyId;
    }
}
