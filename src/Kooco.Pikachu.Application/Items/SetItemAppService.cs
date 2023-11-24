using System;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Threading.Tasks;
using System.Linq;
using Volo.Abp;
using System.Collections.Generic;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

[RemoteService(IsEnabled = false)]
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
            input.SetItemDescriptionTitle,
            input.Description,
            input.SetItemMainImageURL,
            input.SetItemPrice,
            input.LimitQuantity,
            input.LimitAvaliableTimeStart,
            input.LimitAvaliableTimeEnd,
            input.ShareProfit,
            input.IsFreeShipping
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
        setItem.SetItemDescriptionTitle = input.SetItemDescriptionTitle;
        setItem.Description = input.Description;
        setItem.SetItemMainImageURL = input.SetItemMainImageURL;
        setItem.SetItemStatus = input.SetItemStatus;
        setItem.SetItemPrice = input.SetItemPrice;
        setItem.LimitQuantity = input.LimitQuantity;
        setItem.LimitAvaliableTimeStart = input.LimitAvaliableTimeStart;
        setItem.LimitAvaliableTimeEnd = input.LimitAvaliableTimeEnd;
        setItem.ShareProfit = input.ShareProfit;
        setItem.IsFreeShipping = input.IsFreeShipping;
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
    public async Task<List<ItemWithItemTypeDto>> GetItemsLookupAsync()
    {
        var setItems = await _setItemRepository.GetItemsLookupAsync();
        return ObjectMapper.Map<List<ItemWithItemType>, List<ItemWithItemTypeDto>>(setItems);
    }
}
