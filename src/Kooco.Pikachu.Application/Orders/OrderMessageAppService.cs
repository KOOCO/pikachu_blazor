using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace Kooco.Pikachu.Orders
{
    public class OrderMessageAppService : ApplicationService, IOrderMessageAppService
    {
        private readonly IOrderMessageRepository _orderMessageRepository;

        public OrderMessageAppService(IOrderMessageRepository orderMessageRepository)
        {
            _orderMessageRepository = orderMessageRepository;
        }

        // Retrieve an OrderMessage by ID
        public async Task<OrderMessageDto> GetAsync(Guid id)
        {
            var orderMessage = await _orderMessageRepository.GetAsync(id);
            return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
        }

        // Retrieve a paginated list of OrderMessages with filtering and sorting
        public async Task<PagedResultDto<OrderMessageDto>> GetListAsync(GetOrderMessageListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = nameof(OrderMessage.CreationTime) + " DESC";
            
            }
            var totalCount = await _orderMessageRepository.GetCountAsync(
                input.Filter,
                input.OrderId,
                input.SenderId,
                input.IsMerchant,
                input.Timestamp);

            var orderMessages = await _orderMessageRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter,
                input.OrderId,
                input.SenderId,
                input.IsMerchant,
                input.Timestamp);

            return new PagedResultDto<OrderMessageDto>(
                totalCount,
                ObjectMapper.Map<List<OrderMessage>, List<OrderMessageDto>>(orderMessages)
            );
        }

        public async Task<List<OrderMessageDto>> GetOrderMessagesAsync(Guid orderId)
        {
            return ObjectMapper.Map<List<OrderMessage>, List<OrderMessageDto>>(
                await _orderMessageRepository.GetOrderMessagesAsync(orderId)
            );
        }

        // Create a new OrderMessage
        public async Task<OrderMessageDto> CreateAsync(CreateUpdateOrderMessageDto input)
        {
            var orderMessage = new OrderMessage(
                GuidGenerator.Create(),
                input.OrderId,
                input.SenderId,
                input.Message,
                input.IsMerchant);

            await _orderMessageRepository.InsertAsync(orderMessage);

            return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
        }

        // Update an existing OrderMessage
        public async Task<OrderMessageDto> UpdateAsync(Guid id, CreateUpdateOrderMessageDto input)
        {
            var orderMessage = await _orderMessageRepository.GetAsync(id);

            orderMessage.Message = input.Message;
            orderMessage.IsMerchant = input.IsMerchant;
            orderMessage.SenderId = input.SenderId;
            orderMessage.OrderId = input.OrderId;

            await _orderMessageRepository.UpdateAsync(orderMessage);

            return ObjectMapper.Map<OrderMessage, OrderMessageDto>(orderMessage);
        }

        // Delete an OrderMessage by ID
        public async Task DeleteAsync(Guid id)
        {
            await _orderMessageRepository.DeleteAsync(id);
        }
    }
}