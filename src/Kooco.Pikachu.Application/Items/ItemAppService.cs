using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Kooco.Pikachu.ProductCategories;
using Ganss.Xss;
using System.IO;
using Volo.Abp.Content;
using MiniExcelLibs;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;

namespace Kooco.Pikachu.Items;

[RemoteService(IsEnabled = false)]
public class ItemAppService :
    CrudAppService<
        Item,
        ItemDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateItemDto,
        UpdateItemDto
    >, IItemAppService
{
    #region Inject
    private readonly IItemRepository _itemRepository;
    private readonly ISetItemRepository _setItemRepository;
    private readonly ItemManager _itemManager;
    private readonly HtmlSanitizer _htmlSanitizer;
    private readonly IEnumValueAppService _enumValueService;
    private readonly IStringLocalizer<PikachuResource> _l;
    private readonly IItemDetailsRepository _itemDetailsRepository;
    #endregion

    #region Constructor
    public ItemAppService(
        IItemRepository repository,
        ISetItemRepository setItemRepository,
        ItemManager itemManager,
        IEnumValueAppService enumValueService,
        IStringLocalizer<PikachuResource> l,
        IItemDetailsRepository itemDetailsRepository
    )
        : base(repository)
    {
        _itemRepository = repository;
        _setItemRepository = setItemRepository;
        _itemManager = itemManager;
        _htmlSanitizer = new HtmlSanitizer();
        _enumValueService = enumValueService;
        _l = l;
        _itemDetailsRepository = itemDetailsRepository;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Create New Item
    /// </summary>
    /// <param name="input">Item</param>
    /// <returns>item</returns>
    public override async Task<ItemDto> CreateAsync(CreateItemDto input)
    {
        if (!input.ItemDescription.IsNullOrWhiteSpace())
        {
            input.ItemDescription = SanitizeItemDescription(input.ItemDescription);


        }
        var item = await _itemManager.CreateAsync(
            input.ItemName,
            input.ItemBadgeDto?.ItemBadge,
            input.ItemBadgeDto?.ItemBadgeColor,
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
        if (input.IsTest)
        {
            // For testing, generate a unique ItemNo to avoid conflicts
            item.ItemNo = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
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
                    itemDetail.Cost,
                    itemDetail.SaleableQuantity,
                    itemDetail.StockOnHand,
                    itemDetail.PreOrderableQuantity,
                    itemDetail.SaleablePreOrderQuantity,

                    itemDetail.InventoryAccount,
                    itemDetail.Attribute1Value,
                    itemDetail.Attribute2Value,
                    itemDetail.Attribute3Value,
                    itemDetail.Image,
                    itemDetail.ItemDescription,
                    itemDetail.SortNo,
                    itemDetail.Status
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

        if (input.ItemCategories != null)
        {
            foreach (var itemCategory in input.ItemCategories)
            {
                Check.NotDefaultOrNull(itemCategory.ProductCategoryId, nameof(itemCategory.ProductCategoryId));
                item.CategoryProducts.Add(new CategoryProduct(item.Id, itemCategory.ProductCategoryId.Value));
            }
        }
        item.IsItemAvaliable = input.IsItemAvaliable;
        await _itemRepository.InsertAsync(item);
        return ObjectMapper.Map<Item, ItemDto>(item);
    }

    public async Task<ItemDto> CopyAysnc(Guid Id)
    {
        try
        {
            var orignalItem = await _itemRepository.GetAsync(Id);

            await _itemRepository.EnsureCollectionLoadedAsync(orignalItem, i => i.ItemDetails);
            await _itemRepository.EnsureCollectionLoadedAsync(orignalItem, i => i.Images);

            var item = await _itemManager.CreateAsync(
                orignalItem.ItemName + "Copy",
                orignalItem.ItemBadge,
                orignalItem.ItemBadgeColor,
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
                        itemDetail.Cost,
                        itemDetail.SaleableQuantity ?? 0,
                        itemDetail.StockOnHand,
                        itemDetail.PreOrderableQuantity,
                        itemDetail.SaleablePreOrderQuantity,

                        itemDetail.InventoryAccount,
                        itemDetail.Attribute1Value,
                        itemDetail.Attribute2Value,
                        itemDetail.Attribute3Value,
                        itemDetail.Image,
                        itemDetail.ItemDescription,
                        itemDetail.SortNo,
                        itemDetail.Status
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
        await _itemRepository.EnsureCollectionLoadedAsync(item, i => i.CategoryProducts);

        item.ItemName = input.ItemName;
        item.ItemBadge = input.ItemBadgeDto?.ItemBadge;
        item.ItemBadgeColor = input.ItemBadgeDto?.ItemBadgeColor;
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
                    existing.StockOnHand = itemDetail.StockOnHand;
                    existing.Cost = itemDetail.Cost;
                    existing.PreOrderableQuantity = itemDetail.PreOrderableQuantity;
                    existing.SaleablePreOrderQuantity = itemDetail.SaleablePreOrderQuantity;

                    existing.InventoryAccount = itemDetail.InventoryAccount;
                    existing.Attribute1Value = itemDetail.Attribute1Value;
                    existing.Attribute2Value = itemDetail.Attribute2Value;
                    existing.Attribute3Value = itemDetail.Attribute3Value;
                    existing.Image = itemDetail.Image;
                    existing.ItemDescription = itemDetail.ItemDescription;
                    existing.Status = itemDetail.Status;
                    existing.SortNo = itemDetail.SortNo;
                }
                else
                {
                    await _itemManager.AddItemDetailAsync(
                        item,
                        itemDetail.ItemName,
                        itemDetail.Sku,
                        itemDetail.LimitQuantity,
                        itemDetail.SellingPrice,
                        itemDetail.Cost,
                        itemDetail.SaleableQuantity,
                        itemDetail.StockOnHand,
                        itemDetail.PreOrderableQuantity,
                        itemDetail.SaleablePreOrderQuantity,

                        itemDetail.InventoryAccount,
                        itemDetail.Attribute1Value,
                        itemDetail.Attribute2Value,
                        itemDetail.Attribute3Value,
                        itemDetail.Image,
                        itemDetail.ItemDescription,
                        itemDetail.SortNo,
                        itemDetail.Status
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

        item.CategoryProducts.Clear();
        if (input.ItemCategories != null)
        {
            foreach (var itemCategory in input.ItemCategories)
            {
                Check.NotDefaultOrNull(itemCategory.ProductCategoryId, nameof(itemCategory.ProductCategoryId));
                item.CategoryProducts.Add(new CategoryProduct(item.Id, itemCategory.ProductCategoryId.Value));
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
        var item = await _itemRepository.GetFirstImage(id);

        return item.Images.OrderBy(x => x.SortNo).FirstOrDefault()?.ImageUrl;
    }

    public async Task<List<ItemWithItemTypeDto>> GetItemsLookupAsync()
    {
        var items = await _itemRepository.GetItemsLookupAsync();
        return ObjectMapper.Map<List<ItemWithItemType>, List<ItemWithItemTypeDto>>(items);
    }
    public async Task<IRemoteStreamContent> ExportItemListToExcelAsync(List<Guid>itemIds)
    {
        var items = await _itemRepository.GetItemsWithDetailsByIdsAsync(itemIds);

        int itemIndex = 1;
        var headers = new Dictionary<string, string>
{
    { "ItemNo", _l["ItemNo"] },
    { "ItemName", _l["ItemName"] },
    { "ItemAttributes", _l["ItemAttributes"] },
    { "SKU", _l["SKU"] },
    { "InventoryName", _l["InventoryName"] },
    { "SellingPrice", _l["SellingPrice"] },
    { "CostPrice", _l["CostPrice"] },
    { "CurrentStock", _l["CurrentStock"] },
    { "Temperature", _l["Temperature"] },
    { "TaxType", _l["TaxType"] },
    { "Category", _l["Category"] },
    { "Availability", _l["Availability"] }
};
        var excelData = items
          .SelectMany(item =>
          {
              var rows = item.ItemDetails.Select(detail => new Dictionary<string, object>
              {
            { headers["ItemNo"], itemIndex },
            { headers["ItemName"], item.ItemName },
            { headers["ItemAttributes"], detail.ItemName },
            { headers["SKU"], detail.SKU },
            { headers["InventoryName"], detail.InventoryAccount },
            { headers["SellingPrice"], $"${detail.SellingPrice:F2}" },
            { headers["CostPrice"], $"${detail.Cost:F2}" },
            { headers["CurrentStock"], detail.StockOnHand ?? 0 },
            { headers["Temperature"], item.ItemStorageTemperature?.ToString() },
            { headers["TaxType"], item.TaxType?.Text},
            { headers["Category"],  string.Join(", ", item.CategoryProducts.Select(x => x.ProductCategory?.Name).Where(name => !string.IsNullOrWhiteSpace(name))) },
            { headers["Availability"], item.IsItemAvaliable ? L["Active"] : L["Disabled"] }
              }).ToList();

              itemIndex++;
              return rows;
          });

        var stream = new MemoryStream();
        await stream.SaveAsAsync(excelData);
        stream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(
            stream,
            "ItemsList.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );
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

    public async Task<List<KeyValueDto>> LookupAsync()
    {
        var list = (await _itemRepository.GetQueryableAsync())
            .Select(x => new KeyValueDto { Id = x.Id, Name = x.ItemName })
            .OrderBy(x => x.Name)
            .ToList();

        return list;
    }

    public async Task<List<KeyValueDto>> GetAllItemsLookupAsync()
    {
        var list = (await _itemRepository.GetListAsync()).Select(x => new KeyValueDto { Id = x.Id, Name = x.ItemName }).ToList();
         
        return list;
    }

    public async Task<List<KeyValueDto>> GetItemDetailLookupAsync(Guid itemId)
    {
        var itemDetails = (await _itemDetailsRepository.GetQueryableAsync())
            .Where(itemDetail => itemDetail.ItemId == itemId)
            .Select(x => new KeyValueDto { Id = x.Id, Name = x.SKU })
            .OrderBy(x => x.Name)
            .ToList();

        return itemDetails;
    }

    public async Task<ItemDto> GetSKUAndItemAsync(Guid itemId, Guid itemDetailId)
    {
        return MapToGetOutputDto(
            await _itemRepository.GetSKUAndItemAsync(itemId, itemDetailId)
        );
    }

    public async Task<List<ItemDto>> GetManyAsync(List<Guid> itemIds)
    {
        var items = await _itemRepository.GetManyAsync(itemIds);
        return ObjectMapper.Map<List<Item>, List<ItemDto>>(items);
    }

    public async Task<List<CategoryProductDto>> GetItemCategoriesAsync(Guid id)
    {
        var itemProducts = await _itemRepository.GetItemCategoriesAsync(id);
        return ObjectMapper.Map<List<CategoryProduct>, List<CategoryProductDto>>(itemProducts);
    }

    public async Task<List<ItemDto>> GetItemsWithAttributesAsync(List<Guid> ids)
    {
        var items = await _itemRepository.GetItemsWithAttributesAsync(ids);
        return ObjectMapper.Map<List<Item>, List<ItemDto>>(items);
    }
    public string SanitizeItemDescription(string itemDescription)
    {
        // Block dangerous tags like <script> etc.
        _htmlSanitizer.AllowedTags.Remove("script");
        // Block or sanitize all event handler attributes like onerror, onclick, etc.
        _htmlSanitizer.AllowedAttributes.Remove("onerror");
        _htmlSanitizer.AllowedAttributes.Remove("onclick");
        _htmlSanitizer.AllowedAttributes.Remove("onload");
        _htmlSanitizer.AllowedAttributes.Remove("onmouseover");
        _htmlSanitizer.AllowedAttributes.Remove("onfocus");
        if (string.IsNullOrEmpty(itemDescription))
        {
            return itemDescription;
        }

        // Sanitize and return the safe HTML
        return _htmlSanitizer.Sanitize(itemDescription);
    }

    public async Task<List<ItemBadgeDto>> GetItemBadgesAsync()
    {
        var items = await _itemRepository.GetQueryableAsync();
        var setItems = await _setItemRepository.GetQueryableAsync();
        var itemBadges = items
            .Where(x => !string.IsNullOrWhiteSpace(x.ItemBadge))
            .Select(x => new ItemBadgeDto { ItemBadge = x.ItemBadge!, ItemBadgeColor = x.ItemBadgeColor })
            .Distinct()
            .ToList();

        var setItemBadges = setItems
            .Where(x => !string.IsNullOrWhiteSpace(x.SetItemBadge))
            .Select(x => new ItemBadgeDto { ItemBadge = x.SetItemBadge!, ItemBadgeColor = x.SetItemBadgeColor })
            .Distinct()
            .ToList();

        List<ItemBadgeDto> badges = [.. itemBadges, .. setItemBadges];
        return [.. badges.Distinct()];
    }

    public async Task DeleteItemBadgeAsync(ItemBadgeDto input)
    {
        await _itemRepository.DeleteItemBadgeAsync(input.ItemBadge, input.ItemBadgeColor); 
    }
    public async Task ImportItemsFromExcelAsync(IRemoteStreamContent file)
    {
        using var stream = new MemoryStream();
        await file.GetStream().CopyToAsync(stream);

        ExcelPackage.License.SetNonCommercialPersonal("Pikachu");
        using var package = new ExcelPackage(stream);

        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension?.Rows ?? 0;

        if (rowCount <= 1)
            throw new UserFriendlyException("Excel file is empty or invalid");

        var enumValues = (await _enumValueService.GetEnums(new List<EnumType>
    {
        EnumType.ShippingMethod,
        EnumType.TaxType
    })).ToList();

        var ShippingMethods = enumValues
            .Where(x => x.EnumType == EnumType.ShippingMethod)
            .ToList();
        var taxs = enumValues
            .Where(x => x.EnumType == EnumType.TaxType)
            .ToList();

        var itemsDict = new Dictionary<long, CreateItemDto>();

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var itemNo = long.Parse(worksheet.Cells[row, 1].Text);
                var taxTypeText = worksheet.Cells[row, 12].Text?.Trim();
                var taxType = taxs.FirstOrDefault(x =>
                    string.Equals(x.Text, taxTypeText, StringComparison.OrdinalIgnoreCase));

                if (!itemsDict.ContainsKey(itemNo))
                {
                    itemsDict[itemNo] = new CreateItemDto
                    {
                        ItemName = worksheet.Cells[row, 2].Text,
                        ItemDescription = worksheet.Cells[row, 6].Text,
                        ItemBadgeDto = new ItemBadgeDto
                        {
                            ItemBadge = worksheet.Cells[row, 3].Text,
                            ItemBadgeColor = worksheet.Cells[row, 4].Text
                        },
                        ItemDescriptionTitle = worksheet.Cells[row, 5].Text,
                        Attribute1Name = worksheet.Cells[row, 7].Text,
                        Attribute2Name = worksheet.Cells[row, 8].Text,
                        Attribute3Name = worksheet.Cells[row, 9].Text,
                        ItemTags = null,
                        IsFreeShipping = worksheet.Cells[row, 10].Text.Equals("Yes", StringComparison.OrdinalIgnoreCase),
                        IsReturnable = worksheet.Cells[row, 11].Text.Equals("Yes", StringComparison.OrdinalIgnoreCase),
                        TaxTypeId = taxType?.Id ?? 0,
                        ShareProfit = 0,
                        ItemDetails = new List<CreateItemDetailsDto>()
                    };
                }

                itemsDict[itemNo].ItemDetails.Add(new CreateItemDetailsDto
                {
                    Sku = worksheet.Cells[row, 13].Text,
                    SellingPrice = float.TryParse(worksheet.Cells[row, 14].Text, out var sellingPrice) ? sellingPrice : 0,
                    Cost = float.TryParse(worksheet.Cells[row, 15].Text, out var cost) ? cost : 0,
                    LimitQuantity = int.TryParse(worksheet.Cells[row, 16].Text, out var limitQty) ? limitQty : null,
                    SaleableQuantity = float.TryParse(worksheet.Cells[row, 17].Text, out var saleableQty) ? saleableQty : 0,
                    StockOnHand = int.TryParse(worksheet.Cells[row, 18].Text, out var stock) ? stock : null,
                    PreOrderableQuantity = float.TryParse(worksheet.Cells[row, 19].Text, out var preOrderQty) ? preOrderQty : null,
                    SaleablePreOrderQuantity = float.TryParse(worksheet.Cells[row, 20].Text, out var saleablePreOrder) ? saleablePreOrder : null,
                    InventoryAccount = worksheet.Cells[row, 21].Text,
                    Attribute1Value = worksheet.Cells[row, 22].Text,
                    Attribute2Value = worksheet.Cells[row, 23].Text,
                    Attribute3Value = worksheet.Cells[row, 24].Text,
                    ItemName = string.Join("/",
                        new[] {
                        worksheet.Cells[row, 22].Text,
                        worksheet.Cells[row, 23].Text,
                        worksheet.Cells[row, 24].Text
                        }.Where(v => !string.IsNullOrWhiteSpace(v))
                    ),
                    Image = null,
                    ItemDescription = worksheet.Cells[row, 25].Text,
                    SortNo = int.TryParse(worksheet.Cells[row, 26].Text, out var sortNo) ? sortNo : 0,
                    Status = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"Error processing row {row}: {ex.Message}");
            }
        }

        foreach (var itemDto in itemsDict.Values)
        {
            try
            {
                var item = await _itemManager.CreateAsync(
                    itemDto.ItemName,
                    itemDto.ItemBadgeDto?.ItemBadge,
                    itemDto.ItemBadgeDto?.ItemBadgeColor,
                    itemDto.ItemDescriptionTitle,
                    itemDto.ItemDescription,
                    itemDto.ItemTags,
                    itemDto.LimitAvaliableTimeStart,
                    itemDto.LimitAvaliableTimeEnd,
                    itemDto.ShareProfit,
                    itemDto.IsFreeShipping,
                    itemDto.IsReturnable,
                    itemDto.ShippingMethodId,
                    itemDto.TaxTypeId,
                    itemDto.CustomField1Value,
                    itemDto.CustomField1Name,
                    itemDto.CustomField2Value,
                    itemDto.CustomField2Name,
                    itemDto.CustomField3Value,
                    itemDto.CustomField3Name,
                    itemDto.CustomField4Value,
                    itemDto.CustomField4Name,
                    itemDto.CustomField5Value,
                    itemDto.CustomField5Name,
                    itemDto.CustomField6Value,
                    itemDto.CustomField6Name,
                    itemDto.CustomField7Value,
                    itemDto.CustomField7Name,
                    itemDto.CustomField8Value,
                    itemDto.CustomField8Name,
                    itemDto.CustomField9Value,
                    itemDto.CustomField9Name,
                    itemDto.CustomField10Value,
                    itemDto.CustomField10Name,
                    itemDto.Attribute1Name,
                    itemDto.Attribute2Name,
                    itemDto.Attribute3Name,
                    itemDto.ItemStorageTemperature
                );

                item.ShippingMethodId = ShippingMethods.First().Id;

                foreach (var detailDto in itemDto.ItemDetails)
                {
                    await _itemManager.AddItemDetailAsync(
                        item,
                        detailDto.ItemName,
                        detailDto.Sku,
                        detailDto.LimitQuantity,
                        detailDto.SellingPrice,
                        detailDto.Cost,
                        detailDto.SaleableQuantity,
                        detailDto.StockOnHand,
                        detailDto.PreOrderableQuantity,
                        detailDto.SaleablePreOrderQuantity,
                        detailDto.InventoryAccount,
                        detailDto.Attribute1Value,
                        detailDto.Attribute2Value,
                        detailDto.Attribute3Value,
                        detailDto.Image,
                        detailDto.ItemDescription,
                        detailDto.SortNo,
                        detailDto.Status
                    );
                }

                await _itemRepository.InsertAsync(item);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"Error importing item {itemDto.ItemName}: {ex.Message}");
            }
        }
    }





    #endregion
}
