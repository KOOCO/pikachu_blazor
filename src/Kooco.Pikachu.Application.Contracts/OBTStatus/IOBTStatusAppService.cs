using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.OBTStatus
{
	public interface IOBTStatusAppService:IApplicationService
	{
		Task<OBTStatusResponseDto> GetOBTStatusAsync(OBTStatusRequestDto input);
		Task UpdateOrderStatusesAsync();
	}
}
