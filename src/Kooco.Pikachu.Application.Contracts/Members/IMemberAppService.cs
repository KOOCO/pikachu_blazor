﻿using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Members;

public interface IMemberAppService : IApplicationService
{
    Task<MemberDto> GetAsync(Guid id);
    Task<PagedResultDto<MemberDto>> GetListAsync(GetMemberListDto input);
}