using Kooco.Pikachu.Orders.Interfaces;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders
{
    /// <summary>
    /// Service responsible for Order comment management operations
    /// Placeholder implementation - methods delegate to existing OrderAppService logic
    /// </summary>
    public class OrderCommentService : ApplicationService, IOrderCommentService
    {
        public async Task AddStoreCommentAsync(Guid id, string comment)
        {
            // TODO: Extract store comment addition logic from OrderAppService
            throw new NotImplementedException("Store comment addition logic needs to be extracted from OrderAppService");
        }

        public async Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
        {
            // TODO: Extract store comment update logic from OrderAppService
            throw new NotImplementedException("Store comment update logic needs to be extracted from OrderAppService");
        }
    }
}