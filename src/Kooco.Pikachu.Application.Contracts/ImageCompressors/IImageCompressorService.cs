using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.ImageCompressors;

public interface IImageCompressorService : ITransientDependency
{
    Task<CompressImageResponse> CompressAsync(string base64);
    Task<CompressImageResponse> CompressAsync(byte[] bytes);
}
