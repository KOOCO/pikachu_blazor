﻿using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class LogisticsProviderSettingsDto
    {
        public bool IsEnabled { get; set; }

        public string? StoreCode { get; set; }

        public string? HashKey { get; set; }

        public string? HashIV { get; set; }

        public string? SenderName { get; set; }

        public string? SenderPhoneNumber { get; set; }

        public string? LogisticsType { get; set; }

        public string? LogisticsSubTypes { get; set; }

        public List<string>? LogisticsSubTypesList { get; set; }

        public int FreeShippingThreshold { get; set; }

        public int Freight { get; set; }

        public string? CustomTitle { get; set; }

        public string? MainIslands { get; set; }

        public List<string>? MainIslandsList { get; set; }

        public string? OuterIslands { get; set; }

        public List<string>? OuterIslandsList { get; set; }

        public LogisticProviders LogisticProvider { get; set; }

        public Guid? TenantId { get; set; }
        public string? PlatFormId { get; set; }
        public string? SenderPostalCode { get; set; }
        public string? SenderAddress { get; set; }
        public MainlandCity City { get; set; }
        public decimal Weight { get; set; }
        public bool Payment { get; set; }
        public SizeEnum Size { get; set; }
        public int OuterIslandFreight { get; set; }
    }
}
