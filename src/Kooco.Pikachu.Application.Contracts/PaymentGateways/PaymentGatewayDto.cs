using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.PaymentGateways
{
    public class PaymentGatewayDto
    {
        public PaymentIntegrationType PaymentIntegrationType { get; set; }
        public bool IsEnabled { get; set; }
        //OrderVAlidatePeriod
        public int? Period { get; set; } // Input value (e.g., 5 for 5 days, 5 hours, etc.)
        public string? Unit { get; set; }
        //LinePay
        public string ChannelId { get; set; }
        public string ChannelSecretKey { get; set; }

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
        public string? CreditCheckCode { get; set; }
    }
}
