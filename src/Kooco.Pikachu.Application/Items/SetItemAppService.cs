using System;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class SetItemAppService : CrudAppService<SetItem, SetItemDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateSetItemDto, CreateUpdateSetItemDto>,
    ISetItemAppService
{
    protected override string GetPolicyName { get; set; } = PikachuPermissions.SetItem.Default;
    protected override string GetListPolicyName { get; set; } = PikachuPermissions.SetItem.Default;
    protected override string CreatePolicyName { get; set; } = PikachuPermissions.SetItem.Create;
    protected override string UpdatePolicyName { get; set; } = PikachuPermissions.SetItem.Update;
    protected override string DeletePolicyName { get; set; } = PikachuPermissions.SetItem.Delete;

    private readonly ISetItemRepository _repository;

    public SetItemAppService(ISetItemRepository repository) : base(repository)
    {
        _repository = repository;
    }

}
