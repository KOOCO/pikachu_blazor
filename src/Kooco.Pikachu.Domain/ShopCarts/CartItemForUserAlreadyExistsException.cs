using System;
using Volo.Abp;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemForUserAlreadyExistsException : BusinessException
{
    public CartItemForUserAlreadyExistsException() : base(PikachuDomainErrorCodes.CartItemForUserAlreadyExists)
    {
    }
}