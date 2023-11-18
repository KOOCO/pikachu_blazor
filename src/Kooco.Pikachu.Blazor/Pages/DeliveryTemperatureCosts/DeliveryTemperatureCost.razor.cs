using Blazorise;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using System.Threading.Tasks;
using System;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;
using System.Collections.Generic;
using Kooco.Pikachu.DeliveryTemperatureCosts;

namespace Kooco.Pikachu.Blazor.Pages.DeliveryTemperatureCosts
{
    public partial class DeliveryTemperatureCost
    {
       private List<DeliveryTemperatureCostDto> temperatureCosts = new List<DeliveryTemperatureCostDto>();
        protected Validations CreateValidationsRef;
        private readonly IObjectMapper _objectMapper;
        private readonly IUiMessageService _uiMessageService;
        private readonly IDeliveryTemperatureCostAppService _appService;
        public Guid Id { get; set; }
        public DeliveryTemperatureCost(IDeliveryTemperatureCostAppService appService, IObjectMapper objectMapper, IUiMessageService uiMessageService)
        {

            _appService = appService;
            _objectMapper = objectMapper;
            _uiMessageService = uiMessageService;


        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
               await GetCostsAysnc();
                

                StateHasChanged();
                



            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());


            }
        }
        private async Task GetCostsAysnc() { 
        
        temperatureCosts=await _appService.GetListAsync();
        
        }
        protected virtual async Task UpdateCostAsync()
        {
            var costs =  _objectMapper.Map<List<DeliveryTemperatureCostDto>, List<UpdateDeliveryTemperatureCostDto>>(temperatureCosts);

            await _appService.UpdateCostAsync(costs);
            StateHasChanged();


           await _uiMessageService.Success(L["CostUpdateSuccessfully"]);

        }

        }
    }
