using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.LogisticsProviders;

public class EcPayHomeDeliveryCreateUpdateDto
{
    public bool IsEnabled { get; set; }

    [Required(ErrorMessage = "This Field Is Required")]
    public string StoreCode { get; set; }

    [Required(ErrorMessage = "This Field Is Required")]
    public string HashKey { get; set; }

    [Required(ErrorMessage = "This Field Is Required")]
    public string HashIV { get; set; }

    [Required(ErrorMessage = "This Field Is Required")]
    public string SenderName { get; set; }

    [Required(ErrorMessage = "This Field Is Required")]
    public string SenderPhoneNumber { get; set; }
    public string? PlatFormId { get; set; }
    public string? SenderPostalCode { get; set; }
    public string? SenderAddress { get; set; }
    public MainlandCity City { get; set; }
}
