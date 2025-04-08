using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public class GroupBuyItemsPriceAppService : PikachuAppService, IGroupBuyItemsPriceAppService
    {
        private readonly IGroupBuyItemsPriceRepository _repository;
        private readonly GroupBuyItemsPriceManager _manager;

        public GroupBuyItemsPriceAppService(
            IGroupBuyItemsPriceRepository repository,
            GroupBuyItemsPriceManager manager)
        {
            _repository = repository;
            _manager = manager;
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
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }

}
