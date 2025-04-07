using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TenantManagement.TenantWallet;
public class AwaitingAuditResultDto : CreationAuditedEntityDto<Guid>
{
    public required string TenantName { get; set; }
    public required decimal Balance { get; set; }
    public required int AwaitingReviewCount { get; set; }
}
