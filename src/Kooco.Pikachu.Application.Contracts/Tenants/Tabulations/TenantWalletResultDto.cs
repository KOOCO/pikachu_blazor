using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Tenants.Tabulations;
public class TenantWalletResultDto : CreationAuditedEntityDto<Guid>
{
    public required string TenantName { get; set; }
    public required decimal Balance { get; set; }
    public required int AwaitingReviewCount { get; set; }
}