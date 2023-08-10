using System;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class SetItemDetailsAppService : CrudAppService<SetItemDetails, SetItemDetailsDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateSetItemDetailsDto, CreateUpdateSetItemDetailsDto>,
    ISetItemDetailsAppService
{
    protected override string GetPolicyName { get; set; } = PikachuPermissions.SetItemDetails.Default;
    protected override string GetListPolicyName { get; set; } = PikachuPermissions.SetItemDetails.Default;
    protected override string CreatePolicyName { get; set; } = PikachuPermissions.SetItemDetails.Create;
    protected override string UpdatePolicyName { get; set; } = PikachuPermissions.SetItemDetails.Update;
    protected override string DeletePolicyName { get; set; } = PikachuPermissions.SetItemDetails.Delete;

    private readonly ISetItemDetailsRepository _repository;

    public SetItemDetailsAppService(ISetItemDetailsRepository repository) : base(repository)
    {
        _repository = repository;
    }

}
