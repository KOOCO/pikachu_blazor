using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public class GroupBuyItemsPriceManager:DomainService
    {
        private readonly IGroupBuyItemsPriceRepository _repository;

        public GroupBuyItemsPriceManager(IGroupBuyItemsPriceRepository repository)
        {
            _repository = repository;
        }

        public async Task<GroupBuyItemsPrice> CreateAsync(
            Guid? setItemId,
            Guid? groupBuyId,
            float groupBuyPrice,
            Guid? itemDetailId)
        {
            var entity = new GroupBuyItemsPrice(
                GuidGenerator.Create(), // or use Guid.NewGuid()
                setItemId,
                groupBuyId,
                groupBuyPrice,
                itemDetailId
            );

            return await _repository.InsertAsync(entity);
        }

        public async Task<GroupBuyItemsPrice> UpdateAsync(
            Guid id,
            Guid? setItemId,
            Guid? groupBuyId,
            float groupBuyPrice,
            Guid? itemDetailId)
        {
            var entity = await _repository.GetAsync(id);
            entity.Update(setItemId, groupBuyId, groupBuyPrice, itemDetailId);
            return await _repository.UpdateAsync(entity);
        }
    }
}
