using System;

namespace Kooco.Pikachu.ProductCategories;

public class ProductCategoryImagesMaxLimitException : Exception
{
    public ProductCategoryImagesMaxLimitException() : base(PikachuDomainErrorCodes.ProductCategoryImageMaxLimit)
    {
    }
}