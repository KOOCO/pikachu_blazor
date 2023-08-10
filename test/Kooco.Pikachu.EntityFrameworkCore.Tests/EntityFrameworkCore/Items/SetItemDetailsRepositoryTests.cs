using System;
using System.Threading.Tasks;
using Kooco.Pikachu.Items;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.EntityFrameworkCore.Items;

public class SetItemDetailsRepositoryTests : PikachuEntityFrameworkCoreTestBase
{
    private readonly ISetItemDetailsRepository _setItemDetailsRepository;

    public SetItemDetailsRepositoryTests()
    {
        _setItemDetailsRepository = GetRequiredService<ISetItemDetailsRepository>();
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
