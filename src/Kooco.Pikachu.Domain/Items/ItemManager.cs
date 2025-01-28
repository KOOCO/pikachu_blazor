using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Items
{
    public class ItemManager : DomainService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IStringLocalizer<PikachuResource> _l;
        public ItemManager(
            IItemRepository itemRepository,
            IStringLocalizer<PikachuResource> l
            )
        {
            _itemRepository = itemRepository;
            _l = l;
        }

        public async Task<Item> CreateAsync(
            [NotNull] string itemName,
            string? itemBadge,
            string? itemDescriptionTitle,
            string? itemDescription,
            string? itemTags,
            DateTime? limitAvailableTimeStart,
            DateTime? limitAvailableTimeEnd,
            float shareProfit,
            bool isFreeShipping,
            bool isReturnable,
            int? shippingMethodId,
            int? taxTypeId,

            string? customField1Value,
            string? customField1Name,

            string? customField2Value,
            string? customField2Name,
            string? customField3Value,
            string? customField3Name,
            string? customField4Value,
            string? customField4Name,
            string? customField5Value,
            string? customField5Name,
            string? customField6Value,
            string? customField6Name,
            string? customField7Value,
            string? customField7Name,
            string? customField8Value,
            string? customField8Name,
            string? customField9Value,
            string? customField9Name,
            string? customField10Value,
            string? customField10Name,

            string? attribute1Name,
            string? attribute2Name,
            string? attribute3Name,

            ItemStorageTemperature? itemTemperature
            )
        {
            if (itemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(PikachuDomainErrorCodes.ItemNameCannotBeNull);
            }

            Check.NotNullOrWhiteSpace(itemName, nameof(itemName), maxLength: ItemConsts.MaxItemNameLength);

            var existing = await _itemRepository.FindByNameAsync(itemName);
            if (existing is not null)
            {
                throw new BusinessException(_l[PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists]);
            }

            return new Item(
                GuidGenerator.Create(),
                itemName,
                itemBadge,
                itemDescriptionTitle,
                itemDescription,
                itemTags,
                limitAvailableTimeStart,
                limitAvailableTimeEnd,
                shareProfit,
                isFreeShipping,
                isReturnable,
                shippingMethodId,
                taxTypeId,

                customField1Value,
                customField1Name,
                customField2Value,
                customField2Name,
                customField3Value,
                customField3Name,
                customField4Value,
                customField4Name,
                customField5Value,
                customField5Name,
                customField6Value,
                customField6Name,
                customField7Value,
                customField7Name,
                customField8Value,
                customField8Name,
                customField9Value,
                customField9Name,
                customField10Value,
                customField10Name,

                attribute1Name,
                attribute2Name,
                attribute3Name,

                itemTemperature
                
                );
        }

        public async Task AddItemDetailAsync(
            [NotNull] Item @item,
            [NotNull] string itemName,
            [NotNull] string sku,
            int? limitQuantity,
            float sellingPrice,
            float cost,
            float saleableQuantity,
            int? stockonHand,
            float? preOrderableQuantity,
            float? saleablePreOrderQuantity,
            float? groupBuyPrice,
            string? inventoryAccount,
            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value,
            string? image,
            string? itemDescription,
            int sortNo,
            bool status = false
            
        )
        {
            Check.NotNull(@item, nameof(Item));
            Check.NotNull(itemName, nameof(itemName));

            var existing = await _itemRepository.FindBySKUAsync(sku);
            if (existing is not null)
            {
                throw new BusinessException(_l[PikachuDomainErrorCodes.ItemWithSKUAlreadyExists]);
            }

            @item.AddItemDetail(
                GuidGenerator.Create(),
                itemName,
                sku,
                limitQuantity,
                sellingPrice,
                cost,
                saleableQuantity,
                stockonHand,
                preOrderableQuantity,
                saleablePreOrderQuantity,
                groupBuyPrice,
                inventoryAccount,
                attribute1Value,
                attribute2Value,
                attribute3Value,
                image,
                itemDescription,
                sortNo,
                status
            );
        }

        public void AddItemImage(
            [NotNull] Item @item,
            string name,
            string blobImageName,
            string imageUrl,
            ImageType imageType,
            int sortNo
            )
        {
            Check.NotNull(item, nameof(Item));
            @item.AddItemImage(
                GuidGenerator.Create(),
                name,
                blobImageName,
                imageUrl,
                imageType,
                @item.Id,
                sortNo
                );
        }

        public void RemoveItemDetailsAsync(
            [NotNull] Item @item,
            List<Guid?> itemDetailsIds
            )
        {
            if(itemDetailsIds != null && itemDetailsIds.Any())
            {
                foreach (var itemDetail in item.ItemDetails)
                {
                    if (!itemDetailsIds.Contains(itemDetail.Id))
                    {
                        item.ItemDetails.Remove(itemDetail);
                    }
                }
            }
        }
    }
}
