using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Kooco.Pikachu.Items;

public class SetItemAppServiceTests : PikachuApplicationTestBase
{
    private readonly ISetItemAppService _setItemAppService;

    public SetItemAppServiceTests()
    {
        _setItemAppService = GetRequiredService<ISetItemAppService>();
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

