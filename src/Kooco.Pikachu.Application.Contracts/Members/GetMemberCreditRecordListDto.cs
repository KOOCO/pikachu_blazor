using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Members;

public class GetMemberCreditRecordListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public DateTime? UsageTimeFrom { get; set; }
    public DateTime? UsageTimeTo { get; set; }
    public DateTime? ExpiryDateFrom { get; set; }
    public DateTime? ExpiryDateTo { get; set; }
    public int? MinRemainingCredits { get; set; }
    public int? MaxRemainingCredits { get; set; }
    public int? MinAmount { get; set; }
    public int? MaxAmount { get; set; }
}