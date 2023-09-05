using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kooco.Pikachu.Images;
using System.Linq;
using Volo.Abp.Validation.Localization;
using Volo.Abp.Data;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

public class ItemAppService : CrudAppService<Item, ItemDto, Guid, PagedAndSortedResultRequestDto, CreateItemDto, UpdateItemDto>,
    IItemAppService
{
    private readonly IItemRepository _itemRepository;
    private readonly IImageAppService _imageAppService;
    private readonly ItemManager _itemManager;

    public ItemAppService(
        IItemRepository repository,
        IImageAppService imageAppService,
        ItemManager itemManager
        ) : base(repository)
    {
        _itemRepository = repository;
        _imageAppService = imageAppService;
        _itemManager = itemManager;
    }

    /// <summary>
    /// Create New Item
    /// </summary>
    /// <param name="input">Item</param>
    /// <returns>item</returns>
    public override async Task<ItemDto> CreateAsync(CreateItemDto input)
    {
        var item = await _itemManager.CreateAsync(
            input.ItemName,
            input.ItemDescriptionTitle,
            input.ItemDescription,
            input.ItemTags,
            input.LimitAvaliableTimeStart,
            input.LimitAvaliableTimeEnd,
            input.ShareProfit,
            input.IsFreeShipping,
            input.IsReturnable,
            input.ShippingMethodId,
            input.TaxTypeId,

            input.CustomField1Value,
            input.CustomField1Name,
            input.CustomField2Value,
            input.CustomField2Name,
            input.CustomField3Value,
            input.CustomField3Name,
            input.CustomField4Value,
            input.CustomField4Name,
            input.CustomField5Value,
            input.CustomField5Name,
            input.CustomField6Value,
            input.CustomField6Name,
            input.CustomField7Value,
            input.CustomField7Name,
            input.CustomField8Value,
            input.CustomField8Name,
            input.CustomField9Value,
            input.CustomField9Name,
            input.CustomField10Value,
            input.CustomField10Name,

            input.Attribute1Name,
            input.Attribute2Name,
            input.Attribute3Name
            );

        if (input.ItemDetails != null && input.ItemDetails.Any())
        {
            foreach (var itemDetail in input.ItemDetails)
            {
                await _itemManager.AddItemDetailAsync(
                    item,
                    itemDetail.ItemName,
                    itemDetail.Sku,
                    itemDetail.LimitQuantity,
                    itemDetail.SellingPrice,
                    itemDetail.SaleableQuantity,
                    itemDetail.PreOrderableQuantity,
                    itemDetail.SaleablePreOrderQuantity,
                    itemDetail.InventoryAccount,

                    itemDetail.Attribute1Value,
                    itemDetail.Attribute2Value,
                    itemDetail.Attribute3Value
                    );
            }
        }

        if(input.ItemImages != null && input.ItemImages.Any())
        {
            foreach(var image in input.ItemImages)
            {
                _itemManager.AddItemImage(
                    item,
                    image.Name,
                    image.BlobImageName,
                    image.ImageUrl,
                    image.ImageType,
                    image.SortNo
                    );
            }
        }

        await _itemRepository.InsertAsync(item);
        return ObjectMapper.Map<Item, ItemDto>(item);
    }


    /// <summary>
    /// Chnage Item Availability
    /// </summary>
    /// <param name="itemId">Item Id</param>
    public async Task ChangeItemAvailability(Guid itemId)
    {
        var item = await _itemRepository.FindAsync(x => x.Id == itemId);
        item.IsItemAvaliable = !item.IsItemAvaliable;
        await _itemRepository.UpdateAsync(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIds"></param>
    /// <returns></returns>
    public async Task DeleteManyItems(List<Guid> itemIds)
    {
        await _itemRepository.DeleteManyAsync(itemIds);
    }

}
