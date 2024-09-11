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
    [RegularExpression(
        @"^[\u4e00-\u9fa5a-zA-Z0-9\s`~!@#$%^&*()_+\-=\[\]{};:'"",.?/]*$", 
        ErrorMessage = "Sender name can only include Chinese characters, English letters, numbers, and specified symbols.")
    ]
    public string SenderName { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    [RegularExpression(@"^09\d*$", ErrorMessage = "Phone number must start with 09.")]
    public string SenderPhoneNumber { get; set; }

    public string SenderPostalCode { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public string SenderAddress { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public TCatShippingLabelForm TCatShippingLabelForm { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public TCatPickingListForm TCatPickingListForm { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public TCatShippingLabelForm711 TCatShippingLabelForm711 { get; set; }
    public bool DeclaredValue { get; set; }

    [Required(ErrorMessage = "This Field is Required")]
    public ReverseLogisticShippingFee ReverseLogisticShippingFee { get; set; }
}
