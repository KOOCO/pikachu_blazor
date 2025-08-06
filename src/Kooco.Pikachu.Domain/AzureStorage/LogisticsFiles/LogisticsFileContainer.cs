using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace Kooco.Pikachu.AzureStorage.LogisticsFiles
{
    [BlobContainerName(LogisticsFileContainerName)]
    public class LogisticsFileContainer
    {
        public const string LogisticsFileContainerName = "LogisticsFiles";
    }
}
