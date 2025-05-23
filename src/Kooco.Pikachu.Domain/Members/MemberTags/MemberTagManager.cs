﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Members.MemberTags;

public class MemberTagManager : DomainService
{
    private readonly IRepository<MemberTag, Guid> _repository;

    public MemberTagManager(IRepository<MemberTag, Guid> repository)
    {
        _repository = repository;
    }

    private async Task<MemberTag> AddTagAsync(Guid userId, string tagName, bool isSystemAssigned, bool clearNewExisting = false)
    {
        if (clearNewExisting)
        {
            await DeleteTagAsync(userId, MemberConsts.MemberTags.New);
            await DeleteTagAsync(userId, MemberConsts.MemberTags.Existing);
        }
        else
        {
            await DeleteTagAsync(userId, tagName);
        }

        return await CreateTagAsync(userId, tagName, true, isSystemAssigned);
    }

    private async Task<MemberTag> CreateTagAsync(Guid userId, string tagName, bool isEnabled, bool isSystemAssigned, Guid? vipTierId = null, bool autoSave = true)
    {
        var tag = new MemberTag(GuidGenerator.Create(), userId, tagName, isEnabled, isSystemAssigned, vipTierId);
        if (autoSave)
        {
            await _repository.InsertAsync(tag);
        }
        return tag;
    }

    public async Task<bool> TagExistsAsync(Guid userId, string tagName)
    {
        return await _repository.AnyAsync(x => x.UserId == userId && x.Name == tagName);
    }

    public async Task DeleteTagAsync(Guid userId, string tagName)
    {
        var tag = await _repository.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == tagName);
        if (tag != null)
        {
            await _repository.DeleteAsync(tag);
        }
    }

    public Task<MemberTag> AddNewAsync(Guid userId) => AddTagAsync(userId, MemberConsts.MemberTags.New, true, true);
    public Task<MemberTag> AddExistingAsync(Guid userId) => AddTagAsync(userId, MemberConsts.MemberTags.Existing, true, true);
    public Task<MemberTag> AddBlacklistedAsync(Guid userId) => AddTagAsync(userId, MemberConsts.MemberTags.Blacklisted, true);

    public Task<bool> IsNewAsync(Guid userId) => TagExistsAsync(userId, MemberConsts.MemberTags.New);
    public Task<bool> IsExistingAsync(Guid userId) => TagExistsAsync(userId, MemberConsts.MemberTags.Existing);
    public Task<bool> IsBlacklistedAsync(Guid userId) => TagExistsAsync(userId, MemberConsts.MemberTags.Blacklisted);

    public Task DeleteNewAsync(Guid userId) => DeleteTagAsync(userId, MemberConsts.MemberTags.New);
    public Task DeleteExistingAsync(Guid userId) => DeleteTagAsync(userId, MemberConsts.MemberTags.Existing);
    public Task DeleteBlacklistedAsync(Guid userId) => DeleteTagAsync(userId, MemberConsts.MemberTags.Blacklisted);

    public Task SetNewOrExistingAsync(Guid userId, long orderCount) => orderCount > 0 ? AddExistingAsync(userId) : AddNewAsync(userId);

    public async Task AddVipTierAsync(Guid userId, string name, Guid vipTierId)
    {
        var tags = await _repository.GetListAsync(tag => tag.UserId == userId && tag.VipTierId.HasValue);
        await _repository.DeleteManyAsync(tags);
        await CreateTagAsync(userId, name, true, true, vipTierId);
    }

    public async Task AddTagForUsersAsync(string name, List<Guid> userIds)
    {
        List<MemberTag> tagsToAdd = [];

        foreach (var userId in userIds)
        {
            tagsToAdd.Add(await CreateTagAsync(userId, name, true, false, autoSave: false));
        }

        await _repository.InsertManyAsync(tagsToAdd);
    }
}

