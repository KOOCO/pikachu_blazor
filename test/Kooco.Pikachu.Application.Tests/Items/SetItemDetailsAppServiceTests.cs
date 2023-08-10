using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Kooco.Pikachu.Items;

public class SetItemDetailsAppServiceTests : PikachuApplicationTestBase
{
    private readonly ISetItemDetailsAppService _setItemDetailsAppService;

    public SetItemDetailsAppServiceTests()
    {
        _setItemDetailsAppService = GetRequiredService<ISetItemDetailsAppService>();
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

