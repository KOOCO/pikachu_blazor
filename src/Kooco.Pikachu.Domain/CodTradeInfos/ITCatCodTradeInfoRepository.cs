using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.CodTradeInfos;

public interface ITCatCodTradeInfoRepository : IRepository<TCatCodTradeInfoRecord, Guid>
{
}
