using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public class ItemDetailsAppService : CrudAppService<ItemDetails, ItemDetailsDto, Guid, PagedAndSortedResultRequestDto, CreateItemDetailsDto, CreateItemDetailsDto>,
    IItemDetailsAppService
{

    private readonly IItemDetailsRepository _repository;
    private readonly IDistributedCache<InventroyExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;

    public ItemDetailsAppService(IItemDetailsRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(InventroyExcelDownloadDto input)
    {

        //var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
        //if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        //{
        //    throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        //}
        var items = await _repository.GetPagedListAsync(input.SkipCount,input.MaxResultCount,input.Sorting);
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(ObjectMapper.Map<List<ItemDetails>, List<ItemDetailsDto>>(items));
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
