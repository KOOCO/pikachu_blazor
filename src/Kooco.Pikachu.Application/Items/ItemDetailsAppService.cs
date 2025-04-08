using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Items;

[RemoteService(IsEnabled = false)]
public class ItemDetailsAppService : CrudAppService<ItemDetails, ItemDetailsDto, Guid, PagedAndSortedResultRequestDto, CreateItemDetailsDto, CreateItemDetailsDto>,
    IItemDetailsAppService
{

    private readonly IItemDetailsRepository _repository;
    private readonly IDistributedCache<InventroyExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
    private readonly IStringLocalizer<PikachuResource> _l;

    public ItemDetailsAppService(IItemDetailsRepository repository, IStringLocalizer<PikachuResource> l) : base(repository)
    {
        _repository = repository;
        _l = l;
    }
    public async Task<List<ItemDetailsDto>> GetItemDetailByItemId(Guid itemId)
    {
        var query =await _repository.GetWithItemId(itemId);
        return ObjectMapper.Map<List<ItemDetails>, List<ItemDetailsDto>>(query);
    }
    public async Task<PagedResultDto<ItemDetailsDto>> GetInventroyReport(GetInventroyInputDto input)
    {
        var count = await _repository.GetInventroyListCountAsync(input.FilterText);
        var items = await _repository.GetInventroyListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.FilterText);
        return new PagedResultDto<ItemDetailsDto>
        {
            TotalCount = count,
            Items = ObjectMapper.Map<List<ItemDetails>, List<ItemDetailsDto>>(items)
        };
    }

    public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(InventroyExcelDownloadDto input)
    {
        var items = await _repository.GetInventroyListAsync(0, int.MaxValue, input.Sorting, input.FilterText);

        // Create a dictionary for localized headers
        var headers = new Dictionary<string, string>
        {
            { "ItemName", _l["ItemName"] },
            { "SKU", _l["SKU"] },
            { "SellingPrice", _l["SellingPrice"] },
            { "GroupBuyPrice", _l["GroupBuyPrice"] },
            { "CurrentStock", _l["CurrentStock"] },
            { "AvailableStock", _l["AvailableStock"] },
            { "PreorderQuantity", _l["PreorderQuantity"] },
            { "AvailablePreorderQuantity", _l["AvailablePreorderQuantity"] }
        };

        var excelData = items.Select(x => new Dictionary<string, object>
        {
            { headers["ItemName"], x.ItemName },
            { headers["SKU"], x.SKU },
            { headers["SellingPrice"], x.SellingPrice },
            
            { headers["CurrentStock"], x.StockOnHand },
            { headers["AvailableStock"], x.SaleableQuantity },
            { headers["PreorderQuantity"], x.PreOrderableQuantity },
            { headers["AvailablePreorderQuantity"], x.SaleablePreOrderQuantity }
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(excelData);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");
        await _excelDownloadTokenCache.SetAsync(
            token,
            new InventroyExcelDownloadTokenCacheItem { Token = token },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });
        return new DownloadTokenResultDto
        {
            Token = token
        };
    }
}
