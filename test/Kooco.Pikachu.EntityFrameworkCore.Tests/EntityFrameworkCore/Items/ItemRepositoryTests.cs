using System;
using System.Threading.Tasks;
using Kooco.Pikachu.Items;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.EntityFrameworkCore.Items;

public class ItemRepositoryTests : PikachuEntityFrameworkCoreTestBase
{
    private readonly IItemRepository _itemRepository;

    public ItemRepositoryTests()
    {
        _itemRepository = GetRequiredService<IItemRepository>();
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
