using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupPurchaseOverviews;
using Kooco.Pikachu.Images;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.GroupBuys.Interfaces
{
    /// <summary>
    /// Service responsible for GroupBuy display and presentation operations
    /// Split from IGroupBuyAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IGroupBuyDisplayService : IApplicationService
    {
        /// <summary>
        /// Get carousel images for a GroupBuy
        /// </summary>
        Task<List<ImageWithLinkDto>> GetCarouselImagesAsync(Guid id);

        /// <summary>
        /// Get carousel images module-wise
        /// </summary>
        Task<Tuple<List<string>, string?>> GetCarouselImagesModuleWiseAsync(Guid id, int moduleNumber);

        /// <summary>
        /// Get banner images module-wise
        /// </summary>
        Task<List<string>> GetBannerImagesModuleWiseAsync(Guid id, int moduleNumber);

        /// <summary>
        /// Get banner images for a GroupBuy
        /// </summary>
        Task<List<ImageDto>> GetBannerImagesAsync(Guid id);

        /// <summary>
        /// Get group purchase overviews for a GroupBuy
        /// </summary>
        Task<List<GroupPurchaseOverviewDto>> GetGroupPurchaseOverviewsAsync(Guid groupBuyId);

        /// <summary>
        /// Get freebies available for store display
        /// </summary>
        Task<List<FreebieDto>> GetFreebieForStoreAsync(Guid groupBuyId);
    }
}