using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.ShopCarts;

public class InvalidCartItemException : BusinessException
{
    public InvalidCartItemException() : base(PikachuDomainErrorCodes.InvalidCartItem)
    {
        
    }
}
