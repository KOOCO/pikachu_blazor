using Kooco.Pikachu.AddOnProducts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.OrderHistories
{
    public class OrderHistoryManager(IOrderHistoryRepository orderHistoryRepository) : DomainService
    {
        public async Task AddOrderHistoryAsync(Guid orderId, string actionType, object[] parameters, Guid? editorUserId = null, string? editorUserName = null)
        {
            var history = new OrderHistory(Guid.NewGuid(), orderId, actionType, JsonConvert.SerializeObject(parameters), editorUserId, editorUserName);
           ;

            await orderHistoryRepository.InsertAsync(history,autoSave:true);
        }

        /// <summary>
        /// Updates an existing OrderHistory entry (useful if required).
        /// </summary>
        public async Task UpdateOrderHistoryAsync(Guid historyId, string actionDetails, Guid? editorUserId, string? editorUserName)
        {
            var history = await orderHistoryRepository.GetAsync(historyId);
            if (history != null)
            {
                history.ActionDetails = actionDetails;
                history.EditorUserId = editorUserId;
                history.EditorUserName = editorUserName;

                await orderHistoryRepository.UpdateAsync(history);
            }
        }
    }
}
