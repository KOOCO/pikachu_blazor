using Asp.Versioning;
using Kooco.Pikachu.ShoppingCredits;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.ShoppingCredits
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("ShoppingCreditUsageSetting")]
    [Area("app")]
    [Route("api/app/shopping-credit-usage-setting")]
    public class ShoppingCreditUsageSettingController : PikachuController, IShoppingCreditUsageSettingAppService
    {
        private readonly IShoppingCreditUsageSettingAppService _shoppingCreditUsageSettingAppService;

        public ShoppingCreditUsageSettingController(IShoppingCreditUsageSettingAppService shoppingCreditUsageSettingAppService)
        {
            _shoppingCreditUsageSettingAppService = shoppingCreditUsageSettingAppService;
        }

        [HttpGet("{id}")]
        public async Task<ShoppingCreditUsageSettingDto> GetAsync(Guid id)
        {
            return await _shoppingCreditUsageSettingAppService.GetAsync(id);
        }

        [HttpGet("first")]
        public async Task<ShoppingCreditUsageSettingDto> GetFirstAsync()
        {
            return await _shoppingCreditUsageSettingAppService.GetFirstAsync();
        }

        [HttpPost]
        public async Task<ShoppingCreditUsageSettingDto> CreateAsync([FromBody] CreateUpdateShoppingCreditUsageSettingDto input)
        {
            return await _shoppingCreditUsageSettingAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<ShoppingCreditUsageSettingDto> UpdateAsync(Guid id, [FromBody] CreateUpdateShoppingCreditUsageSettingDto input)
        {
            return await _shoppingCreditUsageSettingAppService.UpdateAsync(id, input);
        }


    }
}