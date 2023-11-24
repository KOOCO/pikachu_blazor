using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Encryption;

namespace Kooco.Pikachu.PaymentGateways
{
    [RemoteService(IsEnabled = false)]
    public class PaymentGatewayAppService : ApplicationService, IPaymentGatewayAppService
    {
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
        private readonly IStringEncryptionService _stringEncryptionService;

        public PaymentGatewayAppService(
            IRepository<PaymentGateway, Guid> paymentGatewayRepository,
            IStringEncryptionService stringEncryptionService
            )
        {
            _paymentGatewayRepository = paymentGatewayRepository;
            _stringEncryptionService = stringEncryptionService;
        }
        public async Task UpdateChinaTrustAsync(UpdateChinaTrustDto input)
        {
            var chinaTrust = await _paymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.ChinaTrust);
            if (chinaTrust != null)
            {
                chinaTrust.IsEnabled = input.IsEnabled;
                chinaTrust.MerchantId = _stringEncryptionService.Encrypt(input.MerchantId);
                chinaTrust.Code = _stringEncryptionService.Encrypt(input.Code);
                chinaTrust.TerminalCode = _stringEncryptionService.Encrypt(input.TerminalCode);
                chinaTrust.CodeValue = _stringEncryptionService.Encrypt(input.CodeValue);
                await _paymentGatewayRepository.UpdateAsync(chinaTrust);
                return;
            }
            else
            {
                chinaTrust = new PaymentGateway
                {

                    PaymentIntegrationType = PaymentIntegrationType.ChinaTrust,
                    IsEnabled = input.IsEnabled,
                    MerchantId = _stringEncryptionService.Encrypt(input.MerchantId),
                    Code = _stringEncryptionService.Encrypt(input.Code),
                    TerminalCode = _stringEncryptionService.Encrypt(input.TerminalCode),
                    CodeValue = _stringEncryptionService.Encrypt(input.CodeValue),
                };

                await _paymentGatewayRepository.InsertAsync(chinaTrust);
            }
        }

        public async Task UpdateEcPayAsync(UpdateEcPayDto input)
        {
            var ecPay = await _paymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);

            if (ecPay != null)
            {
                ecPay.IsEnabled = input.IsEnabled;
                ecPay.MerchantId = _stringEncryptionService.Encrypt(input.MerchantId);
                ecPay.HashKey = _stringEncryptionService.Encrypt(input.HashKey);
                ecPay.HashIV = _stringEncryptionService.Encrypt(input.HashIV);
                ecPay.TradeDescription = _stringEncryptionService.Encrypt(input.TradeDescription);
                await _paymentGatewayRepository.UpdateAsync(ecPay);
            }
            else
            {
                var newEcPay = new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    IsEnabled = input.IsEnabled,
                    MerchantId = _stringEncryptionService.Encrypt(input.MerchantId),
                    HashKey = _stringEncryptionService.Encrypt(input.HashKey),
                    HashIV = _stringEncryptionService.Encrypt(input.HashIV),
                    TradeDescription = _stringEncryptionService.Encrypt(input.TradeDescription),
                };

                await _paymentGatewayRepository.InsertAsync(newEcPay);
            }
        }

        public async Task UpdateLinePayAsync(UpdateLinePayDto input)
        {
            var linePay = await _paymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
            if (linePay != null)
            {
                linePay.IsEnabled = input.IsEnabled;
                linePay.ChannelId = _stringEncryptionService.Encrypt(input.ChannelId);
                linePay.ChannelSecretKey = _stringEncryptionService.Encrypt(input.ChannelSecretKey);
                await _paymentGatewayRepository.UpdateAsync(linePay);
            }
            else
            {
                linePay = new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.LinePay,
                    IsEnabled = input.IsEnabled,
                    ChannelId = _stringEncryptionService.Encrypt(input.ChannelId),
                    ChannelSecretKey = _stringEncryptionService.Encrypt(input.ChannelSecretKey),
                };

                await _paymentGatewayRepository.InsertAsync(linePay);
            }
        }

        public async Task<List<PaymentGatewayDto>> GetAllAsync()
        {
            var paymentGateways = await _paymentGatewayRepository.GetListAsync();
            var paymentGatewayDtos = ObjectMapper.Map<List<PaymentGateway>, List<PaymentGatewayDto>>(paymentGateways);

            foreach (var paymentGatewayDto in paymentGatewayDtos)
            {
                var properties = typeof(PaymentGatewayDto).GetProperties();
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var value = (string?)property.GetValue(paymentGatewayDto);
                        if (!string.IsNullOrEmpty(value))
                        {
                            var decryptedValue = _stringEncryptionService.Decrypt(value);
                            property.SetValue(paymentGatewayDto, decryptedValue);
                        }
                    }
                }
            }

            return paymentGatewayDtos;
        }
    }
}
