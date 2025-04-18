using AutoMapper.Internal.Mappers;
using Kooco.Pikachu.GroupBuyItemsPriceses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyItemsPriceAppService : PikachuAppService, IGroupBuyItemsPriceAppService
    {
        private readonly IGroupBuyItemsPriceRepository _repository;
        private readonly GroupBuyItemsPriceManager _manager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GroupBuyItemsPriceAppService(
            IGroupBuyItemsPriceRepository repository,
            GroupBuyItemsPriceManager manager, IUnitOfWorkManager unitOfWorkManager)
        {
            _repository = repository;
            _manager = manager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<GroupBuyItemsPriceDto> CreateAsync(CreateUpdateGroupBuyItemsPriceDto input)
        {
            var entity = await _manager.CreateAsync(
                input.SetItemId,
                input.GroupBuyId,
                input.GroupBuyPrice,
                input.ItemDetailId
            );

            return ObjectMapper.Map<GroupBuyItemsPrice, GroupBuyItemsPriceDto>(entity);
        }

        public async Task<GroupBuyItemsPriceDto> UpdateAsync(Guid id, CreateUpdateGroupBuyItemsPriceDto input)
        {
            var entity = await _manager.UpdateAsync(
                id,
                input.SetItemId,
                input.GroupBuyId,
                input.GroupBuyPrice,
                input.ItemDetailId
            );

            return ObjectMapper.Map<GroupBuyItemsPrice, GroupBuyItemsPriceDto>(entity);
        }

        public async Task<GroupBuyItemsPriceDto> GetAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);
            return ObjectMapper.Map<GroupBuyItemsPrice, GroupBuyItemsPriceDto>(entity);
        }
        public async Task<GroupBuyItemsPriceDto> GetByItemIdAndGroupBuyIdAsync(Guid itemDetailId,Guid GroupBuyId)
        {
            var entity = await _repository.FirstOrDefaultAsync(x=>x.ItemDetailId==itemDetailId && x.GroupBuyId==GroupBuyId);
            return ObjectMapper.Map<GroupBuyItemsPrice, GroupBuyItemsPriceDto>(entity);
        }
        public async Task<GroupBuyItemsPriceDto> GetBySetItemIdAndGroupBuyIdAsync(Guid SetItemId, Guid GroupBuyId)
        {
            var entity = await _repository.FirstOrDefaultAsync(x => x.SetItemId == SetItemId && x.GroupBuyId == GroupBuyId);
            return ObjectMapper.Map<GroupBuyItemsPrice, GroupBuyItemsPriceDto>(entity);
        }
        public async Task<List<GroupBuyItemsPriceDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return ObjectMapper.Map<List<GroupBuyItemsPrice>, List<GroupBuyItemsPriceDto>>(list);
        }
        public async Task<List<GroupBuyItemsPriceDto>> GetListByGroupBuyAsync(Guid GroupBuyId)
        {
            var query = await _repository.GetQueryableAsync();
            var list=query.Where(x=>x.GroupBuyId==GroupBuyId).ToList();
            return ObjectMapper.Map<List<GroupBuyItemsPrice>, List<GroupBuyItemsPriceDto>>(list);
        }
       
        public async Task DeleteAllGroupByItemAsync(Guid GroupBuyId)
        {
            using (var retryUow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                await _repository.HardDeleteAsync(x => x.GroupBuyId == GroupBuyId);
                await retryUow.CompleteAsync();
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }

}
