﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Campaigns;

public interface ICampaignRepository : IRepository<Campaign, Guid>
{
    Task<Campaign> GetWithDetailsAsync(Guid id);
    Task<long> CountAsync(
        string? filter, 
        bool? isEnabled, 
        DateTime? startDate, 
        DateTime? endDate,
        bool onlyAvailable = false
        );

    Task<List<Campaign>> GetListAsync(
        int skipCount, 
        int maxResultCount, 
        string? sorting, 
        string? filter, 
        bool? isEnabled, 
        DateTime? startDate, 
        DateTime? endDate,
        bool onlyAvailable = false
        );
    
    Task<IQueryable<Campaign>> GetFilteredQueryableAsync(
        string? filter,
        bool? isEnabled,
        DateTime? startDate,
        DateTime? endDate,
        bool onlyAvailable = false
        );

    Task<long> GetActiveCampaignsCountAsync();
    Task<List<Campaign>> GetListAsync(List<Guid> campaignIds);
}
