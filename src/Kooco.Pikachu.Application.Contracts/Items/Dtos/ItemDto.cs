using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class ItemDto : FullAuditedEntityDto<Guid>
{
    public string ItemName { get; set; } //商品名稱/ItemName
    public string ItemDescriptionTitle { get; set; } 
    public DateTime LimitAvaliableTimeStart { get; set; }
    public DateTime LimitAvaliableTimeEnd { get; set; }
    public int LimitQuantity { get; set; }
    public int ShareProfit { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool IsReturnable { get; set; }
    public bool IsItemAvaliable { get; set; }
}