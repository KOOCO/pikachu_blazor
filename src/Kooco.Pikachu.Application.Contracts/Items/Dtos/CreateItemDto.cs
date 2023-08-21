using Kooco.Pikachu.AntBlazorModels.Upload;
using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateItemDto
{
    public string ItemNo { get; set; }
    public string ItemName { get; set; } = "";
    public List<CreateItemDetailsDto> ItemDetails { get; set; }
    public List<UploadFileItem> ItemImages { get; set; }
    public string ItemDescriptionTitle { get; set; } = "";
    public string ItemDescription { get; set; } = "";
    public string ItemTags { get; set; }
    public int LimitQuantity { get; set; }
    public bool IsReturnable { get; set; }
    public DateTime LimitAvaliableTimeStart { get; set; } = DateTime.UtcNow;
    public DateTime LimitAvaliableTimeEnd { get; set; } = DateTime.UtcNow;
    public float ShareProfit { get; set; }
    public bool IsFreeShipping { get; set; } = false;
    public int ShippingMethodId { get; set; }
    public int TaxTypeId { get; set; }
    public bool IsItemAvaliable { get; set; }
    public string? CustomField1Name { get; set; }
    public string? CustomField1Value { get; set; }
    public string? CustomField2Name { get; set; }
    public string? CustomField2Value { get; set; }
    public string? CustomField3Name { get; set; }
    public string? CustomField3Value { get; set; }
    public string? CustomField4Name { get; set; }
    public string? CustomField4Value { get; set; }
    public string? CustomField5Name { get; set; }
    public string? CustomField5Value { get; set; }
    public string? CustomField6Name { get; set; }
    public string? CustomField6Value { get; set; }
    public string? CustomField7Name { get; set; }
    public string? CustomField7Value { get; set; }
    public string? CustomField8Name { get; set; }
    public string? CustomField8Value { get; set; }
    public string? CustomField9Name { get; set; }
    public string? CustomField9Value { get; set; }
    public string? CustomField10Name { get; set; }
    public string? CustomField10Value { get; set; }
}