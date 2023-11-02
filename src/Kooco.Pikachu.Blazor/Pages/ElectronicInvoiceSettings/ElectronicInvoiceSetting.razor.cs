using Blazorise;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu.Blazor.Pages.ElectronicInvoiceSettings
{
    public partial class ElectronicInvoiceSetting
    {
        private CreateUpdateElectronicInvoiceDto CreateUpdateElectronicInvoiceDto = new();
        protected Validations CreateValidationsRef;
        private readonly IObjectMapper _objectMapper;
        private readonly IUiMessageService _uiMessageService;
        private readonly IElectronicInvoiceSettingAppService _appService;
        public Guid Id { get; set; }
        public ElectronicInvoiceSetting(IElectronicInvoiceSettingAppService appService, IObjectMapper objectMapper, IUiMessageService uiMessageService) {
        
        _appService = appService;
            _objectMapper= objectMapper;
            _uiMessageService= uiMessageService;
        
        
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var setting = await _appService.GetSettingAsync();
              
                if (setting != null)
                {
                    Id = (Guid)(setting?.Id);
                    CreateUpdateElectronicInvoiceDto = setting != null ? _objectMapper.Map<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>(setting) : new();
                    StateHasChanged();
                }
               
              
              
            }
            catch (Exception ex)
            {
                 await _uiMessageService.Error(ex.GetType().ToString());

                
            }
        }
        protected virtual async Task CreateEntityAsync() {


            var validate = true;
            if (CreateValidationsRef != null)
            {
                validate = await CreateValidationsRef.ValidateAll();
            }
            if (validate)
            {
                if (Id == Guid.Empty)
                {
                 var result=   await _appService.CreateAsyc(CreateUpdateElectronicInvoiceDto);
                    CreateUpdateElectronicInvoiceDto = _objectMapper.Map<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>(result);
                    StateHasChanged();


                    _uiMessageService.Success(L["Setting Update Successfully"]);
                }
                else {

                  var result=  await _appService.UpdateAsyc(Id, CreateUpdateElectronicInvoiceDto);
                    CreateUpdateElectronicInvoiceDto = _objectMapper.Map<ElectronicInvoiceSettingDto, CreateUpdateElectronicInvoiceDto>(result);
                    StateHasChanged();
                    _uiMessageService.Success(L["Setting Update Successfully"]);

                }

               
            }

        }
    }
}
