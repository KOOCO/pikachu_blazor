using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for Order comment management operations
    /// Split from IOrderAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IOrderCommentService : IApplicationService
    {
        /// <summary>
        /// Add store comment to order
        /// </summary>
        Task AddStoreCommentAsync(Guid id, string comment);

        /// <summary>
        /// Update store comment on order
        /// </summary>
        Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment);
    }
}