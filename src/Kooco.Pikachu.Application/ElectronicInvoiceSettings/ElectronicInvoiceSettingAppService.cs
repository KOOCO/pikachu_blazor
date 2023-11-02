using AutoMapper.Internal.Mappers;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Freebies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public class ElectronicInvoiceSettingAppService : ApplicationService, IElectronicInvoiceSettingAppService
    {
        private readonly IElectronicInvoiceSettingRepository _repository;

        public ElectronicInvoiceSettingAppService(IElectronicInvoiceSettingRepository repository) {
        
        _repository = repository;
        
        }

        public async Task<ElectronicInvoiceSettingDto> GetSettingAsync()
        {
            var query = await _repository.GetQueryableAsync();
            return ObjectMapper.Map<ElectronicInvoiceSetting, ElectronicInvoiceSettingDto>(query.FirstOrDefault());

        }
        public async Task<ElectronicInvoiceSettingDto> CreateAsyc(CreateUpdateElectronicInvoiceDto input)
        {
            var setting = new ElectronicInvoiceSetting(Guid.NewGuid(), input.IsEnable, input.StoreCode, input.HashKey, input.HashIV, input.DisplayInvoiceName, input.DaysAfterShipmentGenerateInvoice);
            var result=await _repository.InsertAsync(setting);
            return ObjectMapper.Map<ElectronicInvoiceSetting, ElectronicInvoiceSettingDto>(setting);

        }
        public async Task<ElectronicInvoiceSettingDto> UpdateAsyc(Guid Id,CreateUpdateElectronicInvoiceDto input)
        {
            var query = await _repository.GetQueryableAsync();
            var setting = query.Where(x => x.Id == Id).FirstOrDefault();
            setting.IsEnable = input.IsEnable;
            setting.StoreCode = input.StoreCode;
            setting.HashKey = input.HashKey;
            setting.HashIV = input.HashIV;
            setting.DisplayInvoiceName = input.DisplayInvoiceName;
            setting.DaysAfterShipmentGenerateInvoice= input.DaysAfterShipmentGenerateInvoice;
            var result = await _repository.UpdateAsync(setting);
            return ObjectMapper.Map<ElectronicInvoiceSetting, ElectronicInvoiceSettingDto>(setting);

        }
    }
}
