using System;
using System.Threading.Tasks;
using Kooco.Pikachu.Items;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.EntityFrameworkCore.Items;

public class SetItemRepositoryTests : PikachuEntityFrameworkCoreTestBase
{
    private readonly ISetItemRepository _setItemRepository;

    public SetItemRepositoryTests()
    {
        _setItemRepository = GetRequiredService<ISetItemRepository>();
    }

    /*
    [Fact]
    public async Task Test1()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange

            // Act

            //Assert
        });
    }
    */
}
