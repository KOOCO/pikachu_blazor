using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Orders.Services;
public class OrderMessageManager(IOrderMessageRepository orderMessageRepository) : DomainService
{
    /// <summary>
    /// Creates a new order message.
    /// </summary>
    /// <param name="orderId">The ID of the order associated with the message.</param>
    /// <param name="senderId">The optional ID of the sender (either customer or merchant).</param>
    /// <param name="message">The content of the message.</param>
    /// <param name="isMerchant">Indicates if the sender is the merchant (true) or customer (false).</param>
    /// <returns>The created OrderMessage entity.</returns>
    public async Task<OrderMessage> CreateAsync(Guid orderId, Guid? senderId, string message, bool isMerchant)
    {
        var orderMessage = new OrderMessage(
            GuidGenerator.Create(),
            orderId,
            senderId,
            message,
            isMerchant
        );

        return await orderMessageRepository.InsertAsync(orderMessage);
    }

    /// <summary>
    /// Updates an existing order message.
    /// </summary>
    /// <param name="id">The ID of the message to update.</param>
    /// <param name="message">The updated content of the message.</param>
    /// <returns>The updated OrderMessage entity.</returns>
    public async Task<OrderMessage> UpdateAsync(Guid id, string message)
    {
        var orderMessage = await orderMessageRepository.GetAsync(id);
        orderMessage.Message = message ?? throw new ArgumentNullException(nameof(message));

        return await orderMessageRepository.UpdateAsync(orderMessage);
    }
}