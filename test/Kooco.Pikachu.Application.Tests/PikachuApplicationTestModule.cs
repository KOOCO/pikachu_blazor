using Kooco.Pikachu.AzureStorage;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Images;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Volo.Abp.BlobStoring;
using Volo.Abp.Modularity;

namespace Kooco.Pikachu;

[DependsOn(
    typeof(PikachuApplicationModule),
    typeof(PikachuDomainTestModule)
    )]
public class PikachuApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        var imageContainerMock = new Mock<IBlobContainer<ImageContainer>>();
        var azureOptionsMock = Options.Create(new AzureStorageAccountOptions());

        var imageContainerManagerMock = new Mock<ImageContainerManager>(imageContainerMock.Object, azureOptionsMock);

        // Create and register mocks
        var imageAppServiceMock = new Mock<IImageAppService>();

        // Setup mock behavior if necessary
        imageAppServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<byte[]>(), true))
            .ReturnsAsync("https://fakeurl.com/sample-image.jpeg");

        // Register mocks
        services.AddSingleton(imageAppServiceMock.Object);
        services.AddSingleton(imageContainerManagerMock.Object);
    }
}
