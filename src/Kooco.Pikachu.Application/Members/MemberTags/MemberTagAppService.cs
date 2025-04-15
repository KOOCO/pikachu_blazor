using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Members.MemberTags;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.Members.Default)]
public class MemberTagAppService : PikachuAppService, IMemberTagAppService
{
    private readonly MemberTagManager _memberTagManager;
    private readonly IMemberTagRepository _memberTagRepository;
    private readonly IMemberRepository _memberRepository;

    public MemberTagAppService(
        IMemberTagRepository memberTagRepository,
        IMemberRepository memberRepository,
        MemberTagManager memberTagManager
        )
    {
        _memberTagRepository = memberTagRepository;
        _memberRepository = memberRepository;
        _memberTagManager = memberTagManager;
    }

    public async Task AddTagForUsersAsync(AddTagForUsersDto input)
    {
        if (!input.IsEdit)
        {
            var existing = await _memberTagRepository.FirstOrDefaultAsync(tag => tag.Name == input.Name);

            if (existing != null)
            {
                throw new UserFriendlyException("Member Tag with same name already exists");
            }
        }

        var queryable = await _memberRepository.GetFilteredQueryableAsync(minOrderCount: input.OrdersCompleted, minSpent: input.AmountSpent,
            selectedMemberTags: input.TypesAndTags, minCreationTime: input.MinRegistrationDate, maxCreationTime: input.MaxRegistrationDate);

        var memberIds = await queryable.Select(member => member.Id).ToListAsync();

        var existingTags = await _memberTagRepository.GetListAsync(tag => tag.Name == input.Name);

        await _memberTagRepository.DeleteManyAsync(existingTags);

        await _memberTagManager.AddTagForUsersAsync(input.Name, memberIds);
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
    }

    public async Task SetIsEnabledAsync(string name, bool isEnabled)
    {
        await _memberTagRepository.SetIsEnabledAsync(name, isEnabled);
    }
}
