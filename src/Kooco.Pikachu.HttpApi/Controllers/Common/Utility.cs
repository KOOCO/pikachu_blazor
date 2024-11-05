using ECPay.Payment.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Controllers.Common;

public class Utility
{
}

public class PaymentResponse
{
    public bool IsSuccessStatusCode { get; set; }
    public string EcPayUrl { get; set; }
    public AllInOneResult Result { get; set; }
    public List<string> Errors { get; set; } = [];
}
