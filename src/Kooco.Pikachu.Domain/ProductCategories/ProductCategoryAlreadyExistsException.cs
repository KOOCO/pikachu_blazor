using Volo.Abp;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryAlreadyExistsException : BusinessException
{
    public ProductCategoryAlreadyExistsException(string name) : base(PikachuDomainErrorCodes.ProductCategoryAlreadyExists)
    {
        WithData("name", name);
    }
}