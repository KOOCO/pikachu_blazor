using System.Threading.Tasks;

namespace Kooco.Pikachu.Data;

public interface IPikachuDbSchemaMigrator
{
    Task MigrateAsync();
}
