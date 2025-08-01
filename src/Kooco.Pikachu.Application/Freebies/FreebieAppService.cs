﻿using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.FreeBies.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Freebies
{
    [RemoteService(IsEnabled = false)]
    public class FreebieAppService : CrudAppService<Freebie, FreebieDto, Guid, PagedAndSortedResultRequestDto, FreebieCreateDto, UpdateFreebieDto>, IFreebieAppService
    {

        private readonly FreebieManager _freebieManager;
        private readonly IFreebieRepository _freebieRepository;
        private readonly ImageContainerManager _imageContainerManager;
        public FreebieAppService(
            FreebieManager freebieManager,
            IFreebieRepository freebieRepository,
            ImageContainerManager imageContainerManager
            ) : base(freebieRepository)
        {
            _freebieManager = freebieManager;
            _freebieRepository = freebieRepository;
            _imageContainerManager = imageContainerManager;
        }

        public override async Task<FreebieDto> CreateAsync(FreebieCreateDto input)
        {
            var result = await _freebieManager.CreateAsync(
                input.ItemName,
                input.ItemDescription,
                input.ApplyToAllGroupBuy,
                input.ApplyToAllProducts,
                input.UnCondition,
                input.ActivityStartDate,
                input.ActivityEndDate,
                input.FreebieOrderReach,
                input.MinimumAmount,
                input.MinimumPiece,
                input.FreebieQuantity,
                input.FreebieAmount
                );

            if (input.FreebieGroupBuys != null && input.FreebieGroupBuys.Any())
            {
                foreach (var freeBuyGroupBuys in input.FreebieGroupBuys)
                {
                    result.AddFreebieGroupBuys(
                        result.Id, freeBuyGroupBuys.Value
                        );
                }
            }
            if (input.FreebieProducts != null && input.FreebieProducts.Any())
            {
                foreach (var freebieProducts in input.FreebieProducts)
                {
                    result.AddFreebieProducts(
                        result.Id, freebieProducts.Value
                        );
                }
            }
            if (input.Images != null && input.Images.Any())
            {
                foreach (var image in input.Images)
                {
                    _freebieManager.AddFreebieImage(
                        result,
                        image.Name,
                        image.BlobImageName,
                        image.ImageUrl,
                        image.SortNo
                        );
                }
            }

            await _freebieRepository.InsertAsync(result);
            return ObjectMapper.Map<Freebie, FreebieDto>(result);
        }
        public async Task<List<FreebieDto>> GetListAsync()
        {
            var data = await _freebieRepository.GetListAsync();
            return ObjectMapper.Map<List<Freebie>, List<FreebieDto>>(data);
        }
        public async Task<FreebieDto> GetAsync(Guid id, bool includeDetails = false)
        {
            var freebie = await _freebieRepository.GetAsync(id);
            if (includeDetails)
            {
                await _freebieRepository.EnsureCollectionLoadedAsync(freebie, i => i.FreebieGroupBuys);
                await _freebieRepository.EnsureCollectionLoadedAsync(freebie, i => i.FreebieProducts);
                await _freebieRepository.EnsureCollectionLoadedAsync(freebie, i => i.Images);
            }
            return ObjectMapper.Map<Freebie, FreebieDto>(freebie);
        }
        public override async Task<FreebieDto> UpdateAsync(Guid id, UpdateFreebieDto input)
        {
            Freebie? sameName = await _freebieRepository.FirstOrDefaultAsync(item => item.ItemName == input.ItemName);

            if (sameName is not null && sameName.Id != id) throw new BusinessException(PikachuDomainErrorCodes.ItemWithSameNameAlreadyExists);

            Freebie freebie = await _freebieRepository.GetAsync(id);

            freebie.ItemName = input.ItemName;
            freebie.ItemDescription = input.ItemDescription;
            freebie.ApplyToAllGroupBuy = input.ApplyToAllGroupBuy;
            freebie.ApplyToAllProducts = input.ApplyToAllProducts;
            freebie.ActivityStartDate = input.ActivityStartDate;
            freebie.ActivityEndDate = input.ActivityEndDate;
            freebie.UnCondition = input.UnCondition;
            freebie.FreebieAmount = input.FreebieAmount;
            freebie.MinimumAmount = input.MinimumAmount;
            freebie.MinimumPiece = input.MinimumPiece;
            freebie.FreebieOrderReach = input.FreebieOrderReach;
            freebie.FreebieQuantity = input.FreebieQuantity;

            await _freebieRepository.EnsureCollectionLoadedAsync(freebie, x => x.FreebieGroupBuys);
            await _freebieRepository.EnsureCollectionLoadedAsync(freebie, x => x.FreebieProducts);

            _freebieManager.RemoveFreebieGroupBuys(freebie, input.FreebieGroupBuys);
            _freebieManager.RemoveFreebieProducts(freebie, input.FreebieProducts);

            foreach (var groupBuyId in input.FreebieGroupBuys)
            {
                freebie.AddFreebieGroupBuys(freebie.Id, groupBuyId);
            }
            foreach (var productId in input.FreebieProducts)
            {
                freebie.AddFreebieProducts(freebie.Id, productId);
            }
            if (input.Images != null && input.Images.Any())
            {
                await _freebieRepository.EnsureCollectionLoadedAsync(freebie, s => s.Images);

                foreach (var image in input.Images)
                {
                    if (!freebie.Images.Any(x => x.BlobImageName == image.BlobImageName))
                    {
                        freebie.AddImage(
                        GuidGenerator.Create(),
                        image.Name,
                        image.BlobImageName,
                        image.ImageUrl,
                        image.SortNo
                        );
                    }
                }
            }
            await _freebieRepository.UpdateAsync(freebie);

            return ObjectMapper.Map<Freebie, FreebieDto>(freebie);
        }
        public async Task DeleteSingleImageAsync(Guid itemId, string blobImageName)
        {
            var item = await _freebieRepository.GetAsync(itemId);
            await _freebieRepository.EnsureCollectionLoadedAsync(item, x => x.Images);
            item.Images.RemoveAll(item.Images.Where(x => x.BlobImageName == blobImageName).ToList());
        }
        public async Task DeleteManyItemsAsync(List<Guid> freebieIds)
        {
            foreach (var id in freebieIds)
            {
                var freebie = await _freebieRepository.GetAsync(id);
                await _freebieRepository.EnsureCollectionLoadedAsync(freebie, x => x.Images);
                if (freebie.Images == null) continue;

                foreach (var image in freebie.Images)
                {
                    await _imageContainerManager.DeleteAsync(image.BlobImageName);
                }
            }
            await _freebieRepository.DeleteManyAsync(freebieIds);

        }
        public async Task ChangeFreebieAvailability(Guid freebieId)
        {
            var freebie = await _freebieRepository.FindAsync(x => x.Id == freebieId);
            freebie.IsFreebieAvaliable = !freebie.IsFreebieAvaliable;
            await _freebieRepository.UpdateAsync(freebie);
        }
        public async Task DeductFreebieAmountAsync(Guid freebieId, int quantity)
        {
            var freebie = await _freebieRepository.FindAsync(x => x.Id == freebieId);
            freebie.FreebieAmount -= quantity;
            await _freebieRepository.UpdateAsync(freebie);
        }

    }
}

