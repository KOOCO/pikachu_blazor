using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class ItemDetailsAppService : CrudAppService<ItemDetails, ItemDetailsDto, Guid, PagedAndSortedResultRequestDto, CreateItemDetailsDto, CreateItemDetailsDto>,
    IItemDetailsAppService
{

    private readonly IItemDetailsRepository _repository;

    public ItemDetailsAppService(IItemDetailsRepository repository) : base(repository)
    {
        _repository = repository;
    }

}
