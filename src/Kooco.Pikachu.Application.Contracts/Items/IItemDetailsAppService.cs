using System;
using System.Threading.Tasks;
using Kooco.Pikachu.Items.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Items;


/// <summary>
/// 
/// </summary>
public interface IItemDetailsAppService :
    ICrudAppService< 
                ItemDetailsDto, 
        Guid, 
        PagedAndSortedResultRequestDto,
        CreateItemDetailsDto,
        CreateItemDetailsDto>
{
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(InventroyExcelDownloadDto input);
    
}