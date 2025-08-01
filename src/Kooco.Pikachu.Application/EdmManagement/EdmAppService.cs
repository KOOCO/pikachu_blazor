﻿using FluentValidation;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.EdmManagement;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.EdmManagement.Default)]
public class EdmAppService : PikachuAppService, IEdmAppService
{
    private readonly IEdmRepository _edmRepository;
    private readonly EdmManager _edmManager;
    private readonly IValidator<CreateEdmDto> _validator;
    private readonly EdmEmailService _edmEmailService;

    public EdmAppService(
        IEdmRepository edmRepository,
        EdmManager edmManager,
        IValidator<CreateEdmDto> validator,
        EdmEmailService edmEmailService
        )
    {
        _edmRepository = edmRepository;
        _edmManager = edmManager;
        _validator = validator;
        _edmEmailService = edmEmailService;
    }

    [Authorize(PikachuPermissions.EdmManagement.Create)]
    public async Task<EdmDto> CreateAsync(CreateEdmDto input)
    {
        await _validator.ValidateAndThrowAsync(input);

        var edm = await _edmManager.CreateAsync(input.TemplateType.Value, input.CampaignId,
            input.ApplyToAllMembers.Value, input.MemberTags, input.GroupBuyId.Value,
            input.StartDate.Value, input.EndDate, input.SendTime.Value, input.SendFrequency,
            input.Subject, input.Message);

        await _edmEmailService.EnqueueJob(edm);

        return ObjectMapper.Map<Edm, EdmDto>(edm);
    }

    [Authorize(PikachuPermissions.EdmManagement.Edit)]
    public async Task<EdmDto> UpdateAsync(Guid id, CreateEdmDto input)
    {
        await _validator.ValidateAndThrowAsync(input);

        var edm = await _edmRepository.GetAsync(id);
        
        _edmEmailService.CancelJob(edm);

        await _edmManager.UpdateAsync(edm, input.TemplateType.Value, input.CampaignId,
            input.ApplyToAllMembers.Value, input.MemberTags, input.GroupBuyId.Value,
            input.StartDate.Value, input.EndDate, input.SendTime.Value, input.SendFrequency,
            input.Subject, input.Message);

        await _edmEmailService.EnqueueJob(edm);

        return ObjectMapper.Map<Edm, EdmDto>(edm);
    }

    public async Task<EdmDto> GetAsync(Guid id)
    {
        var edm = await _edmRepository.GetAsync(id);
        return ObjectMapper.Map<Edm, EdmDto>(edm);
    }

    public async Task<PagedResultDto<EdmDto>> GetListAsync(GetEdmListDto input)
    {
        var totalCount = await _edmRepository.CountAsync(input.Filter, input.TemplateType, input.CampaignId,
            input.ApplyToAllMembers, input.MemberTags, input.GroupBuyId, input.StartDate, input.EndDate,
            input.MinSendTime, input.MaxSendTime, input.SendFrequency);

        var items = await _edmRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting,
            input.Filter, input.TemplateType, input.CampaignId, input.ApplyToAllMembers, input.MemberTags,
            input.GroupBuyId, input.StartDate, input.EndDate, input.MinSendTime, input.MaxSendTime, input.SendFrequency);

        return new PagedResultDto<EdmDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Edm>, List<EdmDto>>(items)
        };
    }

    [Authorize(PikachuPermissions.EdmManagement.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var edm = await _edmRepository.GetAsync(id);
        _edmEmailService.CancelJob(edm);
        await _edmRepository.DeleteAsync(edm);
    }

    public async Task SendEmailAsync(Guid id)
    {
        var edm = await _edmRepository.GetAsync(id);
        await _edmEmailService.SendEmailAsync(edm);
    }
}
