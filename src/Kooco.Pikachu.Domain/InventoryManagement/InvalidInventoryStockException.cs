using Volo.Abp;

namespace Kooco.Pikachu.InventoryManagement;

public class InvalidInventoryStockException : BusinessException
{
    public InvalidInventoryStockException(string field1, string field2) : base(PikachuDomainErrorCodes.InvalidInventoryStock)
    {
        WithData("field1", field1)
            .WithData("field2", field2);
    }
}
