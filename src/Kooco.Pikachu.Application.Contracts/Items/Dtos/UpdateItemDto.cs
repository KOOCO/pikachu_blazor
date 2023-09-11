using System;
using System.Collections.Generic;
using Kooco.Pikachu.Images;

namespace Kooco.Pikachu.Items.Dtos
{
    public class UpdateItemDto
    {
        public string ItemNo { get; set; }
        public string ItemName { get; set; } = "";
        public List<CreateItemDetailsDto> ItemDetails { get; set; }
        public List<CreateImageDto> Images { get; set; }
        public string ItemDescriptionTitle { get; set; } = "";
        public string ItemDescription { get; set; } = "";
        public string ItemTags { get; set; }
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

        public string? Attribute1Name { get; set; }
        public string? Attribute2Name { get; set; }
        public string? Attribute3Name { get; set; }
    }
}
