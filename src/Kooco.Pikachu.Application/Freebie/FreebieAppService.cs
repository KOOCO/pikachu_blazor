using Kooco.Pikachu.Freebie.Dtos;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebie
{
    public class FreebieAppService : ApplicationService, IFreebieAppService
    {

        private readonly FreebieManager _freebieManager;
        private readonly IFreebieRepository _freebieRepository;
        public FreebieAppService(
            FreebieManager freebieManager,
            IFreebieRepository freebieRepository
            )
        {
            _freebieManager = freebieManager;
            _freebieRepository = freebieRepository;
        }

        public async Task<FreebieDto> CreateAsync(FreebieCreateDto input)
        {
            var result = await _freebieManager.CreateAsync(
                input.ItemName,
                input.ItemDescription,
                input.ApplyToAllGroupBuy,
                input.UnCondition,
                input.ActivityStartDate,
                input.ActivityEndDate,
                input.FreebieOrderReach,
                input.MinimumAmount,
                input.MinimumPiece,
                input.FreebieAmount
                );

            if (input.FreebieGroupBuys.Any())
            {
                foreach (var freeBuyGroupBuys in input.FreebieGroupBuys)
                {
                    result.AddFreebieGroupBuys(
                        freeBuyGroupBuys.FreebieId, freeBuyGroupBuys.GroupBuyId
                        );
                }
            }

            if (input.Images.Any())
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

    }
}
