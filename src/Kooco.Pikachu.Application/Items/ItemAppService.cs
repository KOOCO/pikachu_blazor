using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;

namespace Kooco.Pikachu.Items;

[RemoteService(IsEnabled = false)]
public class ItemAppService : CrudAppService<Item, ItemDto, Guid, PagedAndSortedResultRequestDto, CreateItemDto, UpdateItemDto>,
    IItemAppService
{
    private readonly IItemRepository _itemRepository;
    private readonly ItemManager _itemManager;

    public ItemAppService(
        IItemRepository repository,
        ItemManager itemManager
        ) : base(repository)
    {
        _itemRepository = repository;
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
            input.Attribute3Name,
            input.ItemStorageTemperature
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
                    itemDetail.GroupBuyPrice,
                    itemDetail.InventoryAccount,
                    itemDetail.Attribute1Value,
                    itemDetail.Attribute2Value,
                    itemDetail.Attribute3Value
                    );
            }
        }

        if (input.ItemImages != null && input.ItemImages.Any())
        {
            foreach (var image in input.ItemImages)
            {
                if (item.Images.Any(x => x.BlobImageName == image.BlobImageName))
                {
                    continue;
                }
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

    public  async Task<ItemDto> CopyAysnc(Guid Id)
    {
        try
        {
            var orignalItem = await _itemRepository.GetAsync(Id);

            await _itemRepository.EnsureCollectionLoadedAsync(orignalItem, i => i.ItemDetails);
            await _itemRepository.EnsureCollectionLoadedAsync(orignalItem, i => i.Images);

            var item = await _itemManager.CreateAsync(
                orignalItem.ItemName + "Copy",
                orignalItem.ItemDescriptionTitle,
                orignalItem.ItemDescription,
                orignalItem.ItemTags,
                orignalItem.LimitAvaliableTimeStart,
                orignalItem.LimitAvaliableTimeEnd,
                orignalItem.ShareProfit,
                orignalItem.IsFreeShipping,
                orignalItem.IsReturnable,
                orignalItem.ShippingMethodId,
                orignalItem.TaxTypeId,
                orignalItem.CustomField1Value,
                orignalItem.CustomField1Name,
                orignalItem.CustomField2Value,
                orignalItem.CustomField2Name,
                orignalItem.CustomField3Value,
                orignalItem.CustomField3Name,
                orignalItem.CustomField4Value,
                orignalItem.CustomField4Name,
                orignalItem.CustomField5Value,
                orignalItem.CustomField5Name,
                orignalItem.CustomField6Value,
                orignalItem.CustomField6Name,
                orignalItem.CustomField7Value,
                orignalItem.CustomField7Name,
                orignalItem.CustomField8Value,
                orignalItem.CustomField8Name,
                orignalItem.CustomField9Value,
                orignalItem.CustomField9Name,
                orignalItem.CustomField10Value,
                orignalItem.CustomField10Name,
                orignalItem.Attribute1Name,
                orignalItem.Attribute2Name,
                orignalItem.Attribute3Name,
                orignalItem.ItemStorageTemperature
                );

            if (orignalItem.ItemDetails != null && orignalItem.ItemDetails.Any())
            {
                foreach (var itemDetail in orignalItem.ItemDetails)
                {
                    await _itemManager.AddItemDetailAsync(
                        item,
                        itemDetail.ItemName,
                        itemDetail.SKU + "Copy",
                        itemDetail.LimitQuantity,
                        itemDetail.SellingPrice,
                        itemDetail.SaleableQuantity ?? 0,
                        itemDetail.PreOrderableQuantity,
                        itemDetail.SaleablePreOrderQuantity,
                        itemDetail.GroupBuyPrice,
                        itemDetail.InventoryAccount,
                        itemDetail.Attribute1Value,
                        itemDetail.Attribute2Value,
                        itemDetail.Attribute3Value
                        );
                }
            }

            if (orignalItem.Images != null && orignalItem.Images.Any())
            {
                foreach (var image in orignalItem.Images)
                {
                    if (item.Images.Any(x => x.BlobImageName == image.BlobImageName))
                    {
                        continue;
                    }
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
        catch (Exception e)
        {

            throw;
        }
    }

    public override async Task<ItemDto> UpdateAsync(Guid id, UpdateItemDto input)
    {
        var sameName = await _itemRepository.FindByNameAsync(input.ItemName);

        if (sameName != null && sameName.Id != id)
        {
            throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);
        }

        var item = await _itemRepository.GetAsync(id);
        await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.ItemDetails);
        await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.Images);

        item.ItemName = input.ItemName;
        item.ItemDescriptionTitle = input.ItemDescriptionTitle;
        item.ItemDescription = input.ItemDescription;
        item.ItemTags = input.ItemTags;
        item.LimitAvaliableTimeStart = input.LimitAvaliableTimeStart;
        item.LimitAvaliableTimeEnd = input.LimitAvaliableTimeEnd;
        item.ShareProfit = input.ShareProfit;
        item.IsFreeShipping = input.IsFreeShipping;
        item.IsReturnable = input.IsReturnable;
        item.ShippingMethodId = input.ShippingMethodId;
        item.TaxTypeId = input.TaxTypeId;

        item.CustomField1Value = input.CustomField1Value;
        item.CustomField1Name = input.CustomField1Name;
        item.CustomField2Value = input.CustomField2Value;
        item.CustomField2Name = input.CustomField2Name;
        item.CustomField3Value = input.CustomField3Value;
        item.CustomField3Name = input.CustomField3Name;
        item.CustomField4Value = input.CustomField4Value;
        item.CustomField4Name = input.CustomField4Name;
        item.CustomField5Value = input.CustomField5Value;
        item.CustomField5Name = input.CustomField5Name;
        item.CustomField6Value = input.CustomField6Value;
        item.CustomField6Name = input.CustomField6Name;
        item.CustomField7Value = input.CustomField7Value;
        item.CustomField7Name = input.CustomField7Name;
        item.CustomField8Value = input.CustomField8Value;
        item.CustomField8Name = input.CustomField8Name;
        item.CustomField9Value = input.CustomField9Value;
        item.CustomField9Name = input.CustomField9Name;
        item.CustomField10Value = input.CustomField10Value;
        item.CustomField10Name = input.CustomField10Name;

        item.Attribute1Name = input.Attribute1Name;
        item.Attribute2Name = input.Attribute2Name;
        item.Attribute3Name = input.Attribute3Name;

        item.ItemStorageTemperature = input.ItemStorageTemperature;

        var itemDetailsIds = input.ItemDetails.Select(x => x.Id).ToList();
        _itemManager.RemoveItemDetailsAsync(item, itemDetailsIds);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        if (input.ItemDetails.Any())
        {
            foreach (var itemDetail in input.ItemDetails)
            {
                if (itemDetail.Id.HasValue)
                {
                    var existing = item.ItemDetails.First(x => x.Id == itemDetail.Id);
                    existing.ItemName = itemDetail.ItemName;
                    existing.SKU = itemDetail.Sku;
                    existing.LimitQuantity = itemDetail.LimitQuantity;
                    existing.SellingPrice = itemDetail.SellingPrice;
                    existing.SaleableQuantity = itemDetail.SaleableQuantity;
                    existing.PreOrderableQuantity = itemDetail.PreOrderableQuantity;
                    existing.SaleablePreOrderQuantity = itemDetail.SaleablePreOrderQuantity;
                    existing.GroupBuyPrice = itemDetail.GroupBuyPrice;
                    existing.InventoryAccount = itemDetail.InventoryAccount;
                    existing.Attribute1Value = itemDetail.Attribute1Value;
                    existing.Attribute2Value = itemDetail.Attribute2Value;
                    existing.Attribute3Value = itemDetail.Attribute3Value;
                }
                else
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
                        itemDetail.GroupBuyPrice,
                        itemDetail.InventoryAccount,
                        itemDetail.Attribute1Value,
                        itemDetail.Attribute2Value,
                        itemDetail.Attribute3Value
                        );
                }
            }
        }

        if (input.Images != null && input.Images.Any())
        {
            foreach (var image in input.Images)
            {
                if (!item.Images.Any(x => x.BlobImageName == image.BlobImageName))
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
                else
                {
                    var itemImage = item.Images.First(x => x.BlobImageName == image.BlobImageName);
                    itemImage.SortNo = image.SortNo;
                }
            }
        }

        return ObjectMapper.Map<Item, ItemDto>(item);
    }

    public async Task DeleteSingleImageAsync(Guid itemId, string blobImageName)
    {
        var item = await _itemRepository.GetAsync(itemId);
        await _itemRepository.EnsureCollectionLoadedAsync(item, x => x.Images);

        item.Images.RemoveAll(item.Images.Where(x => x.BlobImageName == blobImageName).ToList());
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
    public async Task DeleteManyItemsAsync(List<Guid> itemIds)
    {
        await _itemRepository.DeleteManyAsync(itemIds);
    }

    public async Task<ItemDto> GetAsync(Guid id, bool includeDetails = false)
    {
        var item = await _itemRepository.GetAsync(id);
        if (includeDetails)
        {
            await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.ItemDetails);
            await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.Images);
        }
        return ObjectMapper.Map<Item, ItemDto>(item);
    }

    public async Task<string?> GetFirstImageUrlAsync(Guid id)
    {
        var item = await _itemRepository.GetAsync(id);
        await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.Images);
        return item.Images.OrderBy(x => x.SortNo).FirstOrDefault()?.ImageUrl;
    }

    public async Task<List<ItemWithItemTypeDto>> GetItemsLookupAsync()
    {
        var items = await _itemRepository.GetItemsLookupAsync();
        return ObjectMapper.Map<List<ItemWithItemType>, List<ItemWithItemTypeDto>>(items);
    }

    public async Task<List<KeyValueDto>> GetAllItemsLookupAsync()
    {
        var items = await _itemRepository.GetQueryableAsync();
        return items.Select(x => new KeyValueDto { Id = x.Id, Name = x.ItemName }).ToList();
    }

    /// <summary>
    /// This Method Returns the Desired Result For the Store Front End.
    /// Do not change unless you want to make changes in the Store Front End Code
    /// </summary>
    /// <returns></returns>
    public async Task<List<ItemDto>> GetListForStoreAsync()
    {
        var data = await _itemRepository.GetWithImagesAsync(3);
        return ObjectMapper.Map<List<Item>, List<ItemDto>>(data.ToList());
    }

    public async Task<PagedResultDto<ItemListDto>> GetItemsListAsync(GetItemListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = $"{nameof(ItemListViewModel.CreationTime)} DESC";
        }
        var totalCount = await _itemRepository.LongCountAsync(
                                                input.ItemId,
                                                input.MinAvailableTime,
                                                input.MaxAvailableTime,
                                                input.IsFreeShipping,
                                                input.IsAvailable
                                                );

        var items = await _itemRepository.GetItemsListAsync(
                                                input.SkipCount,
                                                input.MaxResultCount,
                                                input.Sorting,
                                                input.ItemId,
                                                input.MinAvailableTime,
                                                input.MaxAvailableTime,
                                                input.IsFreeShipping,
                                                input.IsAvailable
                                                );

        return new PagedResultDto<ItemListDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<ItemListViewModel>, List<ItemListDto>>(items)
        };
    }
}
