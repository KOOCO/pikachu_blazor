using Kooco.Pikachu.AntBlazorModels.Upload;
using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateItemDto
{
    public string ItemName { get; set; } = "";
    public List<CreateItemDetailsDto> ItemDetails { get; set; }
    public List<UploadFileItem> ItemImages { get; set; }
    public string ItemDescriptionTitle { get; set; } = "";
    public string ItemDescriptionHtml { get; set; } = "";
    public int LimitQuantity { get; set; }
    public bool Returnable { get; set; }
    public DateTime LimitAvaliableTimeStart { get; set; }
    public DateTime LimitAvaliableTimeEnd { get; set; }
    public int ShareProfit { get; set; }
    public bool IsFreeShipping { get; set; } = false;
    public EnumValueDto ShippingMethod { get; set; }
    public EnumValueDto TaxType { get; set; }
}