using System;
using System.Threading.Tasks;
using Kooco.Pikachu.Items;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.EntityFrameworkCore.Items;

public class ItemDetailsRepositoryTests : PikachuEntityFrameworkCoreTestBase
{
    private readonly IItemDetailsRepository _itemDetailsRepository;

    public ItemDetailsRepositoryTests()
    {
        _itemDetailsRepository = GetRequiredService<IItemDetailsRepository>();
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
