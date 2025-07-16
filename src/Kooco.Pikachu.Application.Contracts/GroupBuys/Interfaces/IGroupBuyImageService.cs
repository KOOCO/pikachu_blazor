using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.GroupBuys.Interfaces
{
    /// <summary>
    /// Service responsible for GroupBuy image management operations
    /// Extracted from GroupBuyAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IGroupBuyImageService : IApplicationService
    {
        /// <summary>
        /// Upload and associate images with GroupBuy
        /// </summary>
        Task<List<ImageWithLinkDto>> UploadGroupBuyImagesAsync(Guid groupBuyId, List<IRemoteStreamContent> images);

        /// <summary>
        /// Update GroupBuy image configuration
        /// </summary>
        Task UpdateGroupBuyImageConfigurationAsync(Guid groupBuyId);

        /// <summary>
        /// Delete GroupBuy images
        /// </summary>
        Task DeleteGroupBuyImagesAsync(Guid groupBuyId, List<Guid> imageIds);

        /// <summary>
        /// Get GroupBuy image gallery
        /// </summary>
        Task<List<ImageWithLinkDto>> GetGroupBuyImageGalleryAsync(Guid groupBuyId);

        /// <summary>
        /// Upload GroupBuy item group images
        /// </summary>
        Task<List<ImageWithLinkDto>> UploadGroupBuyItemGroupImagesAsync(Guid groupBuyId, Guid itemGroupId, List<IRemoteStreamContent> images);

        /// <summary>
        /// Update GroupBuy item group image configuration
        /// </summary>
        Task UpdateGroupBuyItemGroupImageConfigurationAsync(Guid groupBuyId, Guid itemGroupId);

        /// <summary>
        /// Delete GroupBuy item group images
        /// </summary>
        Task DeleteGroupBuyItemGroupImagesAsync(Guid groupBuyId, Guid itemGroupId, List<Guid> imageIds);

        /// <summary>
        /// Get GroupBuy item group image gallery
        /// </summary>
        Task<List<ImageWithLinkDto>> GetGroupBuyItemGroupImageGalleryAsync(Guid groupBuyId, Guid itemGroupId);

        /// <summary>
        /// Set primary image for GroupBuy
        /// </summary>
        Task SetGroupBuyPrimaryImageAsync(Guid groupBuyId, Guid imageId);
    }
}