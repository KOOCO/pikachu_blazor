using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsSettings;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.OBTStatus;
public class OBTStatusAppService : PikachuAppService, IOBTStatusAppService
{
    public async Task<OBTStatusResponseDto> GetOBTStatusAsync(OBTStatusRequestDto input)
    {
        var apiUrl = Configuration["T-Cat:OBTStatus"];

        RestClient client = new(apiUrl);
        RestRequest request = new()
        {
            Method = Method.Post
        };

        request.AddJsonBody(input);

        var response = await client.ExecuteAsync<OBTStatusResponseDto>(request);

        // Check for HTTP errors
        if (!response.IsSuccessful)
        {
            return new OBTStatusResponseDto
            {
                SrvTranId = Guid.NewGuid().ToString(),
                IsOK = "N",
                Message = $"Error: {response.ErrorMessage ?? response.StatusCode.ToString()}",
                Data = null
            };
        }
        else
        {
            foreach (var delivery in input.OBTNumbers)
            {
                var orderDelivery = await OrderDeliveryRepository.FirstOrDefaultAsync(x => x.DeliveryNo == delivery);
                var responseObt = response?.Data?.Data?.OBTs.FirstOrDefault(x => x.OBTNumber == delivery);
                orderDelivery.StatusName = responseObt?.StatusName;
                orderDelivery.StatusId = responseObt?.StatusId;
                await OrderDeliveryRepository.UpdateAsync(orderDelivery);
            }
        }

        // Deserialize the response
        return response.Data;
    }

    public async Task UpdateOrderStatusesAsync()
    {
        using (DataFilter.Disable<IMultiTenant>())
        {
            // Fetch orders with DeliveryStatus = Shipped
            var orders = await OrderDeliveryRepository.GetListAsync(o =>
                o.DeliveryStatus == DeliveryStatus.Shipped &&
                (o.DeliveryMethod == DeliveryMethod.TCatDeliveryNormal ||
                    o.DeliveryMethod == DeliveryMethod.TCatDeliveryFrozen ||
                    o.DeliveryMethod == DeliveryMethod.TCatDeliveryFreeze ||
                    o.DeliveryMethod == DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                    o.DeliveryMethod == DeliveryMethod.TCatDeliverySevenElevenFrozen ||
                    o.DeliveryMethod == DeliveryMethod.TCatDeliverySevenElevenNormal));

            foreach (var order in orders)
            {
                // Get Tcat LogisticsProviderSettings for the order's tenant
                var logisticsProvider = await LogisticsProviderSettings.FirstOrDefaultAsync(l =>
                    l.LogisticProvider == LogisticProviders.TCat && l.TenantId == order.TenantId);

                if (logisticsProvider != null)
                {
                    var response = await GetOBTStatusAsync(new OBTStatusRequestDto
                    {
                        CustomerId = logisticsProvider.CustomerId,
                        CustomerToken = logisticsProvider.CustomerToken,
                        OBTNumbers = [order.DeliveryNo]
                    });

                    if (response.IsOK == "Y" && response.Data != null)
                    {
                        Logger.LogError($"Successful to update order");
                    }
                    else
                    {
                        Logger.LogError($"Failed to update order {order.Id}: {response.Message}");
                    }
                }
            }
        }
    }

    public IConfiguration Configuration { get; set; }
    public IOrderDeliveryRepository OrderDeliveryRepository { get; set; }
    public IRepository<LogisticsProviderSettings, Guid> LogisticsProviderSettings { get; set; }
}