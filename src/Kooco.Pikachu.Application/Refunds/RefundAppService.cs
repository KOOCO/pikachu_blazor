using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Refunds
{
    [Authorize(PikachuPermissions.Refund.Default)]
    public class RefundAppService : ApplicationService, IRefundAppService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IOrderRepository _orderRepository;

        public RefundAppService(
            IRefundRepository refundRepository,
            IOrderRepository orderRepository
            )
        {
            _refundRepository = refundRepository;
            _orderRepository = orderRepository;
        }

        [Authorize(PikachuPermissions.Refund.Create)]
        public async Task CreateAsync(Guid orderId)
        {
            var existing = await _refundRepository.FindAsync(x => x.OrderId == orderId);
            if (existing != null)
            {
                throw new BusinessException(PikachuDomainErrorCodes.RefundForSameOrderAlreadyExists);
            }
            var refund = new Refund(GuidGenerator.Create(), orderId);
            await _refundRepository.InsertAsync(refund);
            var order = await _orderRepository.GetAsync(orderId);
            order.IsRefunded = true;
            await _orderRepository.UpdateAsync(order);

            /// ToDo: Send Refund Email Here, and also change status for order
        }

        public async Task<PagedResultDto<RefundDto>> GetListAsync(GetRefundListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Refund.CreationTime)} desc";
            }
            var totalCount = await _refundRepository.GetCountAsync(input.Filter);
            var items = await _refundRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            return new PagedResultDto<RefundDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Refund>, List<RefundDto>>(items)
            };
        }
        public async Task<RefundDto> UpdateRefundReviewAsync(Guid id, RefundReviewStatus input)
        {
            var refund = await _refundRepository.GetAsync(id);
            refund.RefundReview = input;
            await _refundRepository.UpdateAsync(refund);
            return ObjectMapper.Map<Refund, RefundDto>(refund);

        }
    }
}
