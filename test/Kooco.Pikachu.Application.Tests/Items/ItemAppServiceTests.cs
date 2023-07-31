using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Kooco.Pikachu.Items;

public class ItemAppServiceTests : PikachuApplicationTestBase
{
    private readonly IItemAppService _itemAppService;

    public ItemAppServiceTests()
    {
        _itemAppService = GetRequiredService<IItemAppService>();
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

