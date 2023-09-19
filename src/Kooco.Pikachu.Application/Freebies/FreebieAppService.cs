using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebies
{
    public class FreebieAppService : ApplicationService, IFreebieAppService
    {

        private readonly FreebieManager _freebieManager;
        private readonly IFreebieRepository _freebieRepository;
        private readonly IGroupBuyRepositroy _groupBuyRepository;
        public FreebieAppService(
            FreebieManager freebieManager,
            IFreebieRepository freebieRepository,
            IGroupBuyRepositroy groupBuyRepository
            )
        {
            _freebieManager = freebieManager;
            _freebieRepository = freebieRepository;
            _groupBuyRepository = groupBuyRepository;
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

            if (input.FreebieGroupBuys!=null&& input.FreebieGroupBuys.Any())
            {
                foreach (var freeBuyGroupBuys in input.FreebieGroupBuys)
                {
                    result.AddFreebieGroupBuys(
                        result.Id, freeBuyGroupBuys.Value
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

        public async Task<List<KeyValueDto>> GetGroupBuyLookupAsync()
        {
            var groupbuys = await _groupBuyRepository.GetListAsync();
            return ObjectMapper.Map<List<GroupBuy>, List<KeyValueDto>>(groupbuys);

        }
    }
}
