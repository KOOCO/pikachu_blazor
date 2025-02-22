using Hangfire;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.AutomaticEmails
{
    [RemoteService(IsEnabled = false)]
    public class AutomaticEmailAppService : ApplicationService, IAutomaticEmailAppService
    {
        private readonly IAutomaticEmailRepository _automaticEmailRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IGroupBuyRepository _groupBuyRepository;

        public AutomaticEmailAppService(
            IAutomaticEmailRepository automaticEmailRepository,
            IDataFilter dataFilter,
            IGroupBuyRepository groupBuyRepository
            )
        {
            _automaticEmailRepository = automaticEmailRepository;
            _dataFilter = dataFilter;
            _groupBuyRepository = groupBuyRepository;
        }
        public async Task CreateAsync(AutomaticEmailCreateUpdateDto input)
        {
            var automaticEmail = new AutomaticEmail(
                GuidGenerator.Create(),
                input.TradeName,
                JsonConvert.SerializeObject(input.RecipientsList),
                input.StartDate.Value,
                input.EndDate.Value,
                input.SendTime.Value,
                input.SendTimeUTC.Value,
                input.RecurrenceType
                );

            if (input.GroupBuyIds != null)
            {
                foreach (var groupBuyId in input.GroupBuyIds)
                {
                    automaticEmail.AddGroupBuy(GuidGenerator.Create(), groupBuyId);
                }
            }

            await _automaticEmailRepository.InsertAsync(automaticEmail);

            var dto = ObjectMapper.Map<AutomaticEmail, AutomaticEmailDto>(automaticEmail);

            DateTime utcTime = dto.SendTimeUTC;
            var cronExpression = $"{utcTime.Minute} {utcTime.Hour} * * {(input.RecurrenceType == RecurrenceType.Weekly ? 0 : "*")}";

            RecurringJob.AddOrUpdate<AutomaticEmailsJob>(
                    dto.Id.ToString(),
                    job => job.ExecuteAsync(dto.Id),
                    cronExpression);
        }

        public async Task<AutomaticEmailDto> GetAsync(Guid id)
        {
            var automaticEmail = await _automaticEmailRepository.GetAsync(id);
            await _automaticEmailRepository.EnsureCollectionLoadedAsync(automaticEmail, a => a.GroupBuys);
            return ObjectMapper.Map<AutomaticEmail, AutomaticEmailDto>(automaticEmail);
        }

        public async Task<List<string>> GetGroupBuyNamesAsync(List<Guid>? groupBuyIds)
        {
            groupBuyIds ??= [];
            var groupBuyNames = await (await _groupBuyRepository.GetQueryableAsync())
                .Where(g => groupBuyIds.Contains(g.Id))
                .Select(g => g.GroupBuyName)
                .ToListAsync();

            return groupBuyNames ?? [];
        }

        public async Task<PagedResultDto<AutomaticEmailDto>> GetListAsync(GetAutomaticEmailListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(AutomaticEmail.StartDate);
            }

            var totalCount = await _automaticEmailRepository.GetCountAsync();
            var items = await _automaticEmailRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);

            return new PagedResultDto<AutomaticEmailDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AutomaticEmail>, List<AutomaticEmailDto>>(items)
            };
        }

        public async Task<AutomaticEmailDto> GetWithDetailsByIdAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var item = await _automaticEmailRepository.GetAsync(id);
                await _automaticEmailRepository.EnsureCollectionLoadedAsync(item, a => a.GroupBuys);
                return ObjectMapper.Map<AutomaticEmail, AutomaticEmailDto>(item);
            }
        }

        public async Task UpdateAsync(Guid id, AutomaticEmailCreateUpdateDto input)
        {
            var automaticEmail = await _automaticEmailRepository.GetAsync(id);
            await _automaticEmailRepository.EnsureCollectionLoadedAsync(automaticEmail, a => a.GroupBuys);

            automaticEmail.TradeName = input.TradeName;
            automaticEmail.Recipients = JsonConvert.SerializeObject(input.RecipientsList);
            automaticEmail.StartDate = input.StartDate.Value;
            automaticEmail.EndDate = input.EndDate.Value;
            automaticEmail.SendTime = input.SendTime.Value;
            automaticEmail.SendTimeUTC = input.SendTimeUTC.Value;
            automaticEmail.RecurrenceType = input.RecurrenceType;

            var itemsToRemove = automaticEmail.GroupBuys
                .Where(groupBuy => !input.GroupBuyIds.Contains(groupBuy.GroupBuyId))
                .ToList();

            foreach (var groupBuy in itemsToRemove)
            {
                automaticEmail.GroupBuys.Remove(groupBuy);
            }

            if (input.GroupBuyIds != null)
            {
                foreach (var groupBuyId in input.GroupBuyIds)
                {
                    if (!automaticEmail.GroupBuys.Select(x => x.GroupBuyId).Contains(groupBuyId))
                    {
                        automaticEmail.AddGroupBuy(GuidGenerator.Create(), groupBuyId);
                    }
                }
            }

            await _automaticEmailRepository.UpdateAsync(automaticEmail);

            var dto = ObjectMapper.Map<AutomaticEmail, AutomaticEmailDto>(automaticEmail);

            DateTime utcTime = input.SendTimeUTC.Value;

            var cronExpression = $"{utcTime.Minute} {utcTime.Hour} * * {(input.RecurrenceType == RecurrenceType.Weekly ? 0 : "*")}";

            RecurringJob.AddOrUpdate<AutomaticEmailsJob>(
                    dto.Id.ToString(),
                    job => job.ExecuteAsync(dto.Id),
                    cronExpression);
        }

        public async Task UpdateJobStatusAsync(Guid id, JobStatus status, Guid? tenantId)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var job = await _automaticEmailRepository.GetAsync(id);
                job.LastJobStatus = status;
                await _automaticEmailRepository.UpdateAsync(job);
            }
        }
    }
}
