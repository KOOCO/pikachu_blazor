using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kooco.Pikachu.Images;
using System.Linq;

namespace Kooco.Pikachu.Items;

public class ItemAppService : CrudAppService<Item, ItemDto, Guid, PagedAndSortedResultRequestDto, CreateItemDto, UpdateItemDto>,
    IItemAppService
{
    private readonly IItemRepository _repository;
    private readonly IItemDetailsRepository _itemDetailrepository;
    private readonly IImageAppService _imageAppService;

    public ItemAppService(IItemRepository repository, 
                          IItemDetailsRepository itemDetailrepository,
                          IImageAppService imageAppService) : base(repository)
    {
        _repository = repository;
        _itemDetailrepository = itemDetailrepository;
        _imageAppService = imageAppService;
    }

    /// <summary>
    /// Create New Item
    /// </summary>
    /// <param name="input">Item</param>
    /// <returns>item</returns>
    public override async Task<ItemDto> CreateAsync(CreateItemDto input)
    {
        var item = ObjectMapper.Map<CreateItemDto, Item>(input);
        var itemDetail = ObjectMapper.Map<List<CreateItemDetailsDto>, List<ItemDetails>>(input.ItemDetails);
        item.ItemDetails = new List<ItemDetails>();
        var res = await _repository.InsertAsync(item, true);
        if (itemDetail.Any())
        {
            itemDetail.ForEach(x => x.ItemId = res.Id);
            await _itemDetailrepository.InsertManyAsync(itemDetail, true);
        }
        if (input.ItemImages != null && input.ItemImages.Any() )
        {
            var imageFiles = input.ItemImages.Select(x => new CreateImageDto
            {
                FileInfo = x,
                TargetID = item.Id,
                ImageType = ImageType.Item
            }).ToList();
            await _imageAppService.InsertManyImageAsync(imageFiles);
        }
        return ObjectMapper.Map<Item, ItemDto>(res);
    }

    /// <summary>
    /// Chnage Item Availability
    /// </summary>
    /// <param name="itemId">Item Id</param>
    public async Task ChangeItemAvailability(Guid itemId)
    {
        var item = await _repository.FindAsync(x => x.Id == itemId);
        item.IsItemAvaliable = !item.IsItemAvaliable;
        await _repository.UpdateAsync(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIds"></param>
    /// <returns></returns>
    public async Task DeleteManyItems(List<Guid> itemIds)
    {
        await _repository.DeleteManyAsync(itemIds);
    }
}
