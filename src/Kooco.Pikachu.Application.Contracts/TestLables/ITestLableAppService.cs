using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TestLables
{
    public interface ITestLableAppService:IApplicationService
    {
        Task<string>TestLableAsync(string logisticSubType);
    }
}
