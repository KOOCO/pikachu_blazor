﻿using Kooco.Pikachu.GroupBuyProductRankings;
using Kooco.Pikachu.GroupBuyProductRankings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyProductRankingAppService :
    CrudAppService<
        GroupBuyProductRanking,
        GroupBuyProductRankingDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateGroupBuyProductRankingDto
    >, IGroupBuyProductRankingAppService
{
    #region Inject
    private readonly IGroupBuyProductRankingRepository _GroupBuyProductRankingRepository;
    #endregion

    #region Constructor
    public GroupBuyProductRankingAppService(
        IRepository<GroupBuyProductRanking, Guid> Repository,
        IGroupBuyProductRankingRepository GroupBuyProductRankingRepository
    ) 
        : base(Repository)
    {
        _GroupBuyProductRankingRepository = GroupBuyProductRankingRepository;
    }
    #endregion

    #region Methods
    public async Task<GroupBuyProductRankingDto> CreateGroupBuyProductRankingAsync(GroupBuyProductRankingDto groupBuyProductRanking)
    {
        return await CreateAsync(
            ObjectMapper.Map<GroupBuyProductRankingDto, CreateUpdateGroupBuyProductRankingDto>(groupBuyProductRanking)
        );
    }

    public async Task<GroupBuyProductRankingDto> UpdateGroupBuyProductRankingAsync(GroupBuyProductRankingDto groupBuyProductRanking)
    {
        return await UpdateAsync(
            groupBuyProductRanking.Id,
            ObjectMapper.Map<GroupBuyProductRankingDto, CreateUpdateGroupBuyProductRankingDto>(groupBuyProductRanking)
        );
    }

    public async Task<List<GroupBuyProductRankingDto>> GetListByGroupBuyIdAsync(Guid groupBuyId)
    {
        return await MapToGetListOutputDtosAsync(
            await _GroupBuyProductRankingRepository.GetListByGroupBuyIdAsync(groupBuyId)
        );
    }

    public async Task DeleteByGroupBuyIdAsync(Guid groupBuyId)
    {
        await _GroupBuyProductRankingRepository.DeleteDirectAsync(d => d.GroupBuyId == groupBuyId);
    }
    #endregion
}
