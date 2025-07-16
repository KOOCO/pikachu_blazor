using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys.Interfaces;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.GroupBuys
{
    /// <summary>
    /// Main GroupBuy application service that combines all GroupBuy-related operations
    /// Uses Facade pattern to provide unified access to focused services
    /// Follows Interface Segregation Principle by composing smaller interfaces
    /// </summary>
    public interface IGroupBuyAppService : 
        IGroupBuyCrudService,
        IGroupBuyItemGroupService, 
        IGroupBuyDisplayService,
        IGroupBuyPricingService,
        IGroupBuyReportingService,
        IGroupBuyImageService
    {
        /// <summary>
        /// Get delivery temperature cost for specific storage temperature
        /// </summary>
        Task<DeliveryTemperatureCostDto> GetTemperatureCostAsync(ItemStorageTemperature itemStorageTemperature);

        /// <summary>
        /// Get attachment for GroupBuy
        /// </summary>
        Task<IRemoteStreamContent> GetAttachmentAsync(Guid id, Guid? tenantId, DateTime sendTime, RecurrenceType recurrenceType);
    }
}
