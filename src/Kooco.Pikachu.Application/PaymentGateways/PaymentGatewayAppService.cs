using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                ecPay.CreditCheckCode = _stringEncryptionService.Encrypt(input.CreditCheckCode);
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
                    CreditCheckCode = _stringEncryptionService.Encrypt(input.CreditCheckCode)
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
        public async Task UpdateOrderValidityAsync(UpdateOrderValidityDto input)
        {
            var validity = await _paymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod);
            if (validity != null)
            {
                validity.Period = input.Period;
                validity.Unit = input.Unit;
                await _paymentGatewayRepository.UpdateAsync(validity);
            }
            else
            {
                validity = new PaymentGateway
                {
                    PaymentIntegrationType= PaymentIntegrationType.OrderValidatePeriod,
                    Period = input.Period,
                Unit = input.Unit
            };

                await _paymentGatewayRepository.InsertAsync(validity);
            }
        }
        public async Task<List<PaymentGatewayDto>> GetAllAsync()
        {
            List<PaymentGateway> paymentGateways = await _paymentGatewayRepository.GetListAsync();

            List<PaymentGatewayDto> paymentGatewayDtos = ObjectMapper.Map<List<PaymentGateway>, List<PaymentGatewayDto>>(paymentGateways);

            foreach (PaymentGatewayDto paymentGatewayDto in paymentGatewayDtos)
            {
                PropertyInfo[] properties = typeof(PaymentGatewayDto).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) && property.Name!="Unit")
                    {
                        string? value = (string?)property.GetValue(paymentGatewayDto);

                        if (!string.IsNullOrEmpty(value))
                        {
                            string? decryptedValue = _stringEncryptionService.Decrypt(value);

                            property.SetValue(paymentGatewayDto, decryptedValue);
                        }
                    }
                }
            }

            return paymentGatewayDtos;
        }

        public async Task<string?> GetCreditCheckCodeAsync()
        {
            string? creditCheckCode = (await _paymentGatewayRepository.GetQueryableAsync()).Where(w => w.PaymentIntegrationType == PaymentIntegrationType.EcPay)
                                                                                           .Select(s => s.CreditCheckCode)
                                                                                           .FirstOrDefault();

            return _stringEncryptionService.Decrypt(creditCheckCode);
        }
    }
}
