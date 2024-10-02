using System;
using Volo.Abp;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCartForUserAlreadyExistsException : BusinessException
{
    public ShopCartForUserAlreadyExistsException() : base(PikachuDomainErrorCodes.ShopCartForUserAlreadyExists)
    {
        
    }
}