using Asp.Versioning;
using Kooco.Pikachu.ShoppingCredits;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.ShoppingCredits
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("ShoppingCreditEarnSetting")]
    [Area("app")]
    [Route("api/app/shopping-credit-earn-setting")]
    public class ShoppingCreditEarnSettingController : PikachuController, IShoppingCreditEarnSettingAppService
    {
        private readonly IShoppingCreditEarnSettingAppService _shoppingCreditEarnSettingAppService;

        public ShoppingCreditEarnSettingController(IShoppingCreditEarnSettingAppService shoppingCreditEarnSettingAppService)
        {
            _shoppingCreditEarnSettingAppService = shoppingCreditEarnSettingAppService;
        }

        [HttpGet("{id}")]
        public async Task<ShoppingCreditEarnSettingDto> GetAsync(Guid id)
        {
            return await _shoppingCreditEarnSettingAppService.GetAsync(id);
        }

        [HttpGet("first")]
        public async Task<ShoppingCreditEarnSettingDto> GetFirstAsync()
        {
            return await _shoppingCreditEarnSettingAppService.GetFirstAsync();
        }

        [HttpPost]
        public async Task<ShoppingCreditEarnSettingDto> CreateAsync([FromBody] CreateUpdateShoppingCreditEarnSettingDto input)
        {
            return await _shoppingCreditEarnSettingAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<ShoppingCreditEarnSettingDto> UpdateAsync(Guid id, [FromBody] CreateUpdateShoppingCreditEarnSettingDto input)
        {
            return await _shoppingCreditEarnSettingAppService.UpdateAsync(id, input);
        }
        [HttpGet("shopping-credit-settings")]
        public Task<Dictionary<string, object>> GetShoppingCreditSettingsAsync(Guid groupBuyId)
        {
            return _shoppingCreditEarnSettingAppService.GetShoppingCreditSettingsAsync(groupBuyId);
        }
    }
}