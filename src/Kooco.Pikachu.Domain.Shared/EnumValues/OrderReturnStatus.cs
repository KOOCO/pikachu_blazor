using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.EnumValues;

public enum OrderReturnStatus
{
    Pending,
    Approve = 1,
    Reject = 2,
    Processing = 3,
    Succeeded = 4,
    Cancelled = 5
}
