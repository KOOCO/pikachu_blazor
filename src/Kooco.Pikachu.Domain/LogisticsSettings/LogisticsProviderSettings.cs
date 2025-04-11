using Kooco.Pikachu.EnumValues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.LogisticsSettings;
public class LogisticsProviderSettings : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public bool IsEnabled { get; set; }

    public string? StoreCode { get; set; }

    public string? HashKey { get; set; }

    public string? HashIV { get; set; }

    public string? SenderName { get; set; }

    public string? SenderPhoneNumber { get; set; }

    public string? PlatFormId { get; set; }
    public string? SenderPostalCode { get; set; }
    public string? SenderAddress { get; set; }
    public MainlandCity City { get; set; }
    public decimal Weight { get; set; }
    public bool Payment { get; set; }
    public TCatPaymentMethod? TCatPaymentMethod { get; set; }
    public SizeEnum Size { get; set; }
    public int OuterIslandFreight { get; set; }
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

    public bool IsOuterIslands { get; set; }

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

    public string? CustomerId { get; set; }
    public string? CustomerToken { get; set; }
    public TCatShippingLabelForm? TCatShippingLabelForm { get; set; }
    public TCatPickingListForm? TCatPickingListForm { get; set; }
    public TCatShippingLabelForm711? TCatShippingLabelForm711 { get; set; }
    public bool DeclaredValue { get; set; }
    public ReverseLogisticShippingFee? ReverseLogisticShippingFee { get; set; }


    public int HualienAndTaitungShippingFee { get; set; }
    public int HolidaySurcharge { get; set; }
    public DateTime? HolidaySurchargeStartTime { get; set; }
    public DateTime? HolidaySurchargeEndTime { get; set; }
}
