using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.EnumValues
{
    public enum OrderReturnStatus
    {
        PendingReview,
        Approve =1,
        Reject = 2,
        Processing =3,
       
        
        Succeeded=4
    }
}
