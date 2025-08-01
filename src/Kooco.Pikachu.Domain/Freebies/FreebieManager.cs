﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.Freebies
{
    public class FreebieManager : DomainService
    {
        private readonly IFreebieRepository _freebieRepository;
        private readonly IStringLocalizer<PikachuResource> L;
        public FreebieManager(
            IFreebieRepository freebieRepository,
            IStringLocalizer<PikachuResource> l
            )
        {
            L = l;
            _freebieRepository = freebieRepository;
        }

        public void AddFreebieImage(Freebie freebie, string name, string blobImageName, string imageUrl, int sortNo)
        {
            freebie.AddImage(GuidGenerator.Create(), name, blobImageName, imageUrl, sortNo);
        }

        public async Task<Freebie> CreateAsync(
            [NotNull] string itemName,
            string? itemDescription,
            bool applyToAllGroupBuy,
            bool applyToAllProduct,
            bool unCondition,
            DateTime? activityStartDate,
            DateTime? activityEndtDate,
            FreebieOrderReach? freebieOrderReach,
            decimal? minimumAmount,
            int? minimumPiece,
            int freebieQuantity,
            decimal? freebieAmount
            )
        {

            if (itemName.IsNullOrWhiteSpace())
            {
                throw new BusinessException(PikachuDomainErrorCodes.ItemNameCannotBeNull);
            }

            Check.NotNullOrWhiteSpace(itemName, nameof(itemName), maxLength: ItemConsts.MaxItemNameLength);

            var existing = await _freebieRepository.FindByNameAsync(itemName);
            if (existing is not null)
            {
                throw new BusinessException(L[PikachuDomainErrorCodes.FreebieWithSameNameAlreadyExists]);
            }
            return new Freebie(
                GuidGenerator.Create(),
                itemName,
                itemDescription,
                applyToAllGroupBuy,
                applyToAllProduct,
                unCondition,
                freebieOrderReach,
                activityStartDate,
                activityEndtDate,
                minimumAmount,
                minimumPiece,
                freebieQuantity,
                freebieAmount
            );
        }


        public void RemoveFreebieGroupBuys(
         [NotNull] Freebie freebie,
         List<Guid> freebieGroupBuysIds
         )
        {
            if (freebieGroupBuysIds != null && freebieGroupBuysIds.Any())
            {
                foreach (var freebieGroupBuy in freebie.FreebieGroupBuys)
                {
                    if (!freebieGroupBuysIds.Contains(freebieGroupBuy.GroupBuyId))
                    {
                        freebie.FreebieGroupBuys.Remove(freebieGroupBuy);
                    }
                }
            }
            else
            {
                freebie.FreebieGroupBuys.Clear();
            }
        }
        public void RemoveFreebieProducts(
        [NotNull] Freebie freebie,
        List<Guid> freebieProductIds
        )
        {
            if (freebieProductIds != null && freebieProductIds.Any())
            {
                foreach (var freebieProducts in freebie.FreebieProducts)
                {
                    if (!freebieProductIds.Contains(freebieProducts.ProductId))
                    {
                        freebie.FreebieProducts.Remove(freebieProducts);
                    }
                }
            }
            else
            {
                freebie.FreebieGroupBuys.Clear();
            }
        }
    }
}
