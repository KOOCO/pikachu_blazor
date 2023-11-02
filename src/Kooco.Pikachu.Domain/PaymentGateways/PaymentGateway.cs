using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.PaymentGateways
{
    public class PaymentGateway : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public PaymentIntegrationType PaymentIntegrationType { get; set; }
        public bool IsEnabled { get; set; }

        //LinePay
        public string? ChannelId { get; set; }
        public string? ChannelSecretKey { get; set; }

        //Common for ChinaTrust and GreenSector
        public string? MerchantId { get; set; }

        //ChinaTrust
        public string? Code { get; set; }
        public string? TerminalCode { get; set; }
        public string? CodeValue { get; set; }

        //GreenSector
        public string? HashKey { get; set; }
        public string? HashIV { get; set; }
        public string? TradeDescription { get; set; }
    }
}
