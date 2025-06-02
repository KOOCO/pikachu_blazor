using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.TierManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Members.MemberTags;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.Members.Default)]
public class MemberTagAppService : PikachuAppService, IMemberTagAppService
{
    private readonly MemberTagManager _memberTagManager;
    private readonly IMemberTagRepository _memberTagRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IRepository<VipTierSetting, Guid> _vipTierSettingRepository;
    private readonly IRepository<MemberTagFilter, Guid> _memberTagFilterRepository;

    public MemberTagAppService(
        IMemberTagRepository memberTagRepository,
        IMemberRepository memberRepository,
        MemberTagManager memberTagManager,
        IRepository<VipTierSetting, Guid> vipTierSettingRepository,
        IRepository<MemberTagFilter, Guid> memberTagFilterRepository
        )
    {
        _memberTagRepository = memberTagRepository;
        _memberRepository = memberRepository;
        _memberTagManager = memberTagManager;
        _vipTierSettingRepository = vipTierSettingRepository;
        _memberTagFilterRepository = memberTagFilterRepository;
    }

    public async Task AddTagForUsersAsync(AddTagForUsersDto input)
    {
        if (input.EditingId.HasValue)
        {
            var existingTag = await _memberTagRepository.FirstOrDefaultAsync(tag => tag.Id == input.EditingId)
                ?? throw new EntityNotFoundException(typeof(MemberTag), input.EditingId);

            var allExistingTags = await _memberTagRepository.GetListAsync(tag => tag.Name == existingTag.Name);
            await _memberTagRepository.DeleteManyAsync(allExistingTags);

            var existingFilter = await _memberTagFilterRepository.FindAsync(x => x.Tag == existingTag.Name);
            if (existingFilter != null)
            {
                await _memberTagFilterRepository.DeleteAsync(existingFilter);
            }

            await CurrentUnitOfWork!.SaveChangesAsync();
        }

        var existing = await _memberTagRepository.FirstOrDefaultAsync(tag => tag.Name == input.Name);

        if (existing != null)
        {
            throw new UserFriendlyException(L["MemberTagWithSameNameAlreadyExists"].Value);
        }

        var queryable = await GetForTagsAsync(input);

        var memberIds = await queryable.Select(member => member.Id).ToListAsync();

        await _memberTagManager.AddTagForUsersAsync(input.Name, memberIds);

        var filterCheck = await _memberTagFilterRepository.FindAsync(x => x.Tag == input.Name);

        if (filterCheck != null)
        {
            await _memberTagFilterRepository.DeleteAsync(filterCheck);
        }

        var memberTagFilter = new MemberTagFilter(GuidGenerator.Create(), input.Name, input.MemberTypes, input.MemberTags,
            input.AmountSpent, input.OrdersCompleted, input.MinRegistrationDate, input.MaxRegistrationDate);

        await _memberTagFilterRepository.InsertAsync(memberTagFilter);
    }

    public async Task<long> CountMembersAsync(AddTagForUsersDto input)
    {
        var queryable = await GetForTagsAsync(input);
        return await queryable.LongCountAsync();
    }

    public async Task<PagedResultDto<MemberTagDto>> GetListAsync(GetMemberTagsListDto input)
    {
        var totalCount = await _memberTagRepository.CountAsync(input.Filter, false);

        var items = await _memberTagRepository.GetListAsync(input.SkipCount, input.MaxResultCount,
            input.Sorting, input.Filter, false);

        return new PagedResultDto<MemberTagDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MemberTag>, List<MemberTagDto>>(items)
        };
    }

    public async Task DeleteManyAsync(List<string> tagsList)
    {
        var existing = await _memberTagRepository.GetListAsync(tag => tagsList.Contains(tag.Name));
        await _memberTagRepository.DeleteManyAsync(existing);

        var existingFilters = await _memberTagFilterRepository.GetListAsync(filter => tagsList.Contains(filter.Tag));
        await _memberTagFilterRepository.DeleteManyAsync(existingFilters);
    }

    public async Task SetIsEnabledAsync(string name, bool isEnabled)
    {
        await _memberTagRepository.SetIsEnabledAsync(name, isEnabled);
    }

    public async Task<MemberTagFilterDto> GetMemberTagFilterAsync(string tagName)
    {
        var memberTagFilter = await _memberTagFilterRepository.FindAsync(x => x.Tag == tagName);
        return ObjectMapper.Map<MemberTagFilter, MemberTagFilterDto>(memberTagFilter);
    }

    public async Task<List<string>> GetMemberTagNamesAsync()
    {
        var queryable = await _memberTagRepository.GetFilteredQueryableAsync(isSystemGenerated: false);
        var tagNames = await queryable.Select(q => q.Name).OrderBy(x => x).ToListAsync();

        var vipTiers = await _vipTierSettingRepository.FirstOrDefaultAsync();
        if (vipTiers != null)
        {
            await _vipTierSettingRepository.EnsureCollectionLoadedAsync(vipTiers, tier => tier.Tiers);
        }

        var vipTierNames = vipTiers?.Tiers.Select(t => t.TierName!).ToList();
        if (tagNames.Count > 0)
        {
            return [.. tagNames
            .Concat(vipTierNames)
            .Concat(MemberConsts.MemberTags.Names)
            .Distinct()
            .OrderBy(x => x)];
        }
        else
        {
            return [];
        }
    }

    private async Task<IQueryable<MemberModel>> GetForTagsAsync(AddTagForUsersDto input)
    {
        var queryable = await _memberRepository.GetFilteredQueryableAsync(minOrderCount: input.OrdersCompleted, minSpent: input.AmountSpent,
            selectedMemberTags: input.MemberTags, selectedMemberTypes: input.MemberTypes, minCreationTime: input.MinRegistrationDate,
            maxCreationTime: input.MaxRegistrationDate);

        return queryable;
    }
}
