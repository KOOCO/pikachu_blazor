using Kooco.Pikachu.EnumValues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class LogisticsProviderSettings : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public bool IsEnabled { get; set; }

        public string? StoreCode { get; set; }

        public string? HashKey { get; set; }

        public string? HashIV { get; set; }

        public string? SenderName { get; set; }

        public string? SenderPhoneNumber { get; set; }

        public string? LogisticsType { get; set; }

        public string? LogisticsSubTypes { get; set; }

        [NotMapped]
        public List<string>? LogisticsSubTypesList
        {
            get
            {
                return string.IsNullOrEmpty(LogisticsSubTypes)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(LogisticsSubTypes);
            }
        }

        //public int FreeShippingThreshold { get; set; }

        public int Freight { get; set; }

        public string? CustomTitle { get; set; }

        public string? MainIslands { get; set; }

        [NotMapped]
        public List<string>? MainIslandsList
        {
            get
            {
                return string.IsNullOrEmpty(MainIslands)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(MainIslands);
            }
        }

        public string? OuterIslands { get; set; }

        [NotMapped]
        public List<string>? OuterIslandsList
        {
            get
            {
                return string.IsNullOrEmpty(OuterIslands)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(OuterIslands);
            }
        }

        public LogisticProviders LogisticProvider { get; set; }
    }
}
