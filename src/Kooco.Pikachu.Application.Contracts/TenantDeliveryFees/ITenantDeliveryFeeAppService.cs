using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public interface ITenantDeliveryFeeAppService : IApplicationService
    {
        Task<PagedResultDto<TenantDeliveryFeeDto>> GetListAsync(TenantDeliveryFeeGetListInput input);
        /// <summary>
        /// Add/update many in one go: if (TenantId, DeliveryProvider) exists -> update; else -> create.
        /// </summary>
        Task UpsertManyAsync(UpsertTenantDeliveryFeesInput input);
        Task<PagedResultDto<TenantLogisticsFeeRowDto>> GetTenantLogisticsFeeOverviewAsync(
        TenantLogisticsFeeGetListInput input);
    }
}
