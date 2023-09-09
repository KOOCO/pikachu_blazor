using System;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Threading.Tasks;
using System.Linq;
using static Kooco.Pikachu.Permissions.PikachuPermissions;
using Volo.Abp;
using System.Collections.Generic;

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

    public async Task DeleteManyItemsAsync(List<Guid> setItemIds)
    {
        await _setItemRepository.DeleteManyAsync(setItemIds);
    }
}
