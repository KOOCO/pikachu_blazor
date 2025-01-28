using System;

namespace Kooco.Pikachu.Items;

public class ItemListDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; }
    public string ItemBadge { get; set; }
    public string ItemDescriptionTitle { get; set; }
    public DateTime LimitAvaliableTimeStart { get; set; }
    public DateTime LimitAvaliableTimeEnd { get; set; }
    public DateTime CreationTime { get; set; }
    public int ShareProfit { get; set; }
    public bool IsFreeShipping { get; set; }
    public bool IsItemAvaliable { get; set; }
    public bool IsSelected { get; set; } = false;
}