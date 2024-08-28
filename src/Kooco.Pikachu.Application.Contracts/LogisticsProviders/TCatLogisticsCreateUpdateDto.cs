using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.LogisticsProviders;

public class TCatLogisticsCreateUpdateDto
{
    public bool IsEnabled { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public string CustomerToken { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public string SenderName { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public string SenderPhoneNumber { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public string SenderAddress { get; set; }

    public TCatShippingLabelForm ShippingLabelForm { get; set; }
    public TCatPickingListForm PickingListForm { get; set; }
    public TCatShippingLabelForm711 ShippingLabelForm711 { get; set; }
    public bool DeclaredValue { get; set; }
    public ReverseLogisticShippingFee ReverseLogisticShippingFee { get; set; }
}
