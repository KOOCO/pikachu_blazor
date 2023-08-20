using System;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class ItemAppService : CrudAppService<Item, ItemDto, Guid, PagedAndSortedResultRequestDto, CreateItemDto, UpdateItemDto>,
    IItemAppService
{
    protected override string GetPolicyName { get; set; } = PikachuPermissions.Item.Default;
    protected override string GetListPolicyName { get; set; } = PikachuPermissions.Item.Default;
    protected override string CreatePolicyName { get; set; } = PikachuPermissions.Item.Create;
    protected override string UpdatePolicyName { get; set; } = PikachuPermissions.Item.Update;
    protected override string DeletePolicyName { get; set; } = PikachuPermissions.Item.Delete;

    private readonly IItemRepository _repository;

    public ItemAppService(IItemRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public override Task<ItemDto> CreateAsync(CreateItemDto input)
    {
        var a = ObjectMapper.Map<CreateItemDto, Item>(input);
        return base.CreateAsync(input);
    }
}
