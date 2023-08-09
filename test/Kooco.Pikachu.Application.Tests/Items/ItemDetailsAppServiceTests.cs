using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Kooco.Pikachu.Items;

public class ItemDetailsAppServiceTests : PikachuApplicationTestBase
{
    private readonly IItemDetailsAppService _itemDetailsAppService;

    public ItemDetailsAppServiceTests()
    {
        _itemDetailsAppService = GetRequiredService<IItemDetailsAppService>();
    }

    /*
    [Fact]
    public async Task Test1()
    {
        // Arrange

        // Act

        // Assert
    }
    */
}

