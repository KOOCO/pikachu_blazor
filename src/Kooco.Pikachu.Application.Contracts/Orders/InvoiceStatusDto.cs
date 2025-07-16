using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class InvoiceStatusDto : EntityDto<Guid>
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}