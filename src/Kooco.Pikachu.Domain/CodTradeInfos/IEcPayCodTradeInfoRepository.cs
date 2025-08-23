using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.CodTradeInfos;

public interface IEcPayCodTradeInfoRepository : IRepository<EcPayCodTradeInfoRecord, Guid>
{
    Task<List<(Guid OrderId, string MerchantTradeNo)>> GetMerchantTradeNos(DateTime cutoffDate, CancellationToken cancellationToken = default);
}
