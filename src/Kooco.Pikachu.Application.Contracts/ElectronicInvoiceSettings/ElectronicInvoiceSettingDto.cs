using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public class ElectronicInvoiceSettingDto : FullAuditedEntityDto<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public bool IsEnable { get; set; }
        public string StoreCode { get; set; }
        public string HashKey { get; set; }
        public string HashIV { get; set; }
        public string DisplayInvoiceName { get; set; }
        public DeliveryStatus StatusOnInvoiceIssue { get; set; }
        public int DaysAfterShipmentGenerateInvoice { get; set; }
    }
}
