using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Threading.Tasks;
using System.Linq;
using Volo.Abp;
using System.Collections.Generic;
using Volo.Abp.Domain.Repositories;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class SetItemAppService : CrudAppService<SetItem, SetItemDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateSetItemDto>, ISetItemAppService
{
    private readonly ISetItemRepository _setItemRepository;

    public SetItemAppService(ISetItemRepository repository) : base(repository)
    {
        _setItemRepository = repository;
    }

    public override async Task<SetItemDto> CreateAsync(CreateUpdateSetItemDto input)
    {
        var existing = await _setItemRepository.FindAsync(x => x.SetItemName == input.SetItemName);
        if (existing != null)
        {
            throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);
        }

        var setItem = new SetItem(
            GuidGenerator.Create(),
            CurrentTenant?.Id,
            input.SetItemName,
            input.SetItemNo,
            input.SetItemDescriptionTitle,
            input.Description,
            input.SetItemMainImageURL,
            input.SetItemStatus,
            input.SetItemSaleableQuantity,
            input.SellingPrice,
            input.GroupBuyPrice,
            input.SaleableQuantity,
            input.PreOrderableQuantity,
            input.SaleablePreOrderQuantity,
            input.SalesAccount,
            input.Returnable,
            input.LimitAvaliableTimeStart,
            input.LimitAvaliableTimeEnd,
            input.ShareProfit,
            input.IsFreeShipping,
            input.TaxName,
            input.TaxPercentage,
            input.TaxType,
            input.ItemCategory
            );

        if (input.SetItemDetails.Any())
        {
            input.SetItemDetails.ForEach(item =>
            {
                setItem.AddSetItemDetails(
                    GuidGenerator.Create(),
                    CurrentTenant?.Id,
                    setItem.Id,
                    item.ItemId,
                    item.Quantity
                    );
            });
        }

        if (input.Images.Any())
        {
            input.Images.ForEach(image =>
            {
                setItem.AddImage(
                    GuidGenerator.Create(),
                    image.Name,
                    image.BlobImageName,
                    image.ImageUrl,
                    image.SortNo
                );
            });
        }

        await _setItemRepository.InsertAsync(setItem);
        return ObjectMapper.Map<SetItem, SetItemDto>(setItem);
    }

    public override async Task<SetItemDto> UpdateAsync(Guid id, CreateUpdateSetItemDto input)
    {
        var sameName = await _setItemRepository.FirstOrDefaultAsync(item => item.SetItemName == input.SetItemName);
        if (sameName != null && sameName.Id != id)
        {
            throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);
        }

        var setItem = await _setItemRepository.GetAsync(id);
        setItem.SetItemName = input.SetItemName;
        setItem.SetItemNo = input.SetItemNo;
        setItem.SetItemDescriptionTitle = input.SetItemDescriptionTitle;
        setItem.Description = input.Description;
        setItem.SetItemMainImageURL = input.SetItemMainImageURL;
        setItem.SetItemStatus = input.SetItemStatus;
        setItem.SetItemSaleableQuantity = input.SetItemSaleableQuantity;
        setItem.SellingPrice = input.SellingPrice;
        setItem.GroupBuyPrice = input.GroupBuyPrice;
        setItem.SaleableQuantity = input.SaleableQuantity;
        setItem.PreOrderableQuantity = input.PreOrderableQuantity;
        setItem.SaleablePreOrderQuantity = input.SaleablePreOrderQuantity;
        setItem.SalesAccount = input.SalesAccount;
        setItem.Returnable = input.Returnable;
        setItem.LimitAvaliableTimeStart = input.LimitAvaliableTimeStart;
        setItem.LimitAvaliableTimeEnd = input.LimitAvaliableTimeEnd;
        setItem.ShareProfit = input.ShareProfit;
        setItem.IsFreeShipping = input.IsFreeShipping;
        setItem.TaxName = input.TaxName;
        setItem.TaxPercentage = input.TaxPercentage;
        setItem.TaxType = input.TaxType;
        setItem.ItemCategory = input.ItemCategory;

        await _setItemRepository.EnsureCollectionLoadedAsync(setItem, x => x.SetItemDetails);
        var setItemDetailIds = input.SetItemDetails.Select(x => x.Id).ToList();
        setItem.RemoveItemDetailsAsync(setItemDetailIds);

        foreach (var itemDetail in input.SetItemDetails)
        {
            if (itemDetail.Id.HasValue)
            {
                var existing = setItem.SetItemDetails.First(x => x.Id == itemDetail.Id);
                existing.ItemId = itemDetail.ItemId;
                existing.Quantity = itemDetail.Quantity;
            }
            else
            {
                setItem.AddSetItemDetails(GuidGenerator.Create(), CurrentTenant?.Id, setItem.Id, itemDetail.ItemId, itemDetail.Quantity);
            }
        }


        if (input.Images != null && input.Images.Any())
        {
            await _setItemRepository.EnsureCollectionLoadedAsync(setItem, s => s.Images);

            foreach (var image in input.Images)
            {
                if (!setItem.Images.Any(x => x.BlobImageName == image.BlobImageName))
                {
                    setItem.AddImage(
                    GuidGenerator.Create(),
                    image.Name,
                    image.BlobImageName,
                    image.ImageUrl,
                    image.SortNo
                    );
                }
            }
        }

        return ObjectMapper.Map<SetItem, SetItemDto>(setItem);
    }

    public async Task DeleteManyItemsAsync(List<Guid> setItemIds)
    {
        await _setItemRepository.DeleteManyAsync(setItemIds);
    }

    public async Task DeleteSingleImageAsync(Guid id, string blobImageName)
    {
        var item = await _setItemRepository.GetAsync(id);
        await _setItemRepository.EnsureCollectionLoadedAsync(item, x => x.Images);

        item.Images.RemoveAll(item.Images.Where(x => x.BlobImageName == blobImageName).ToList());
    }

    public async Task<SetItemDto> GetAsync(Guid id, bool includeDetails = false)
    {
        var setItem = await _setItemRepository.GetWithDetailsAsync(id);

        return ObjectMapper.Map<SetItem, SetItemDto>(setItem);
    }
}
