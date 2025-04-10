using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Scriban.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.OBTStatus
{
	public class OBTStatusAppService:PikachuAppService,IOBTStatusAppService
	{
		private readonly IConfiguration _configuration;
		private readonly IOrderDeliveryRepository _orderDeliveryRepository;
		private readonly IRepository<LogisticsProviderSettings, Guid> _logisticsProviderRepository;
		private readonly IDataFilter _dataFilte;
		public OBTStatusAppService(IConfiguration configuration,IOrderDeliveryRepository orderDeliveryRepository,
			IRepository<LogisticsProviderSettings, Guid> logisticsProviderRepository, IDataFilter dataFilte)
		{
			_configuration = configuration;
			_orderDeliveryRepository = orderDeliveryRepository;
			_logisticsProviderRepository = logisticsProviderRepository;
			_dataFilte= dataFilte;
		}

		public async Task<OBTStatusResponseDto> GetOBTStatusAsync(OBTStatusRequestDto input)
		{
			// Determine environment and get the URL
			
			var apiUrl = _configuration["T-Cat:OBTStatus"];

			// Initialize RestSharp Client
			var client = new RestClient(apiUrl);
			var request = new RestRequest();
			request.Method = Method.Post;

				request.AddJsonBody(input); // Add the request body as JSON

			// Execute the request
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
			else {
				
				foreach (var delivery in input.OBTNumbers)
				{
					var orderDelivery = await _orderDeliveryRepository.FirstOrDefaultAsync(x => x.DeliveryNo == delivery);
					var responseObt = response?.Data?.Data?.OBTs.FirstOrDefault(x => x.OBTNumber == delivery);
					orderDelivery.StatusName = responseObt?.StatusName;
					orderDelivery.StatusId = responseObt?.StatusId;
					await _orderDeliveryRepository.UpdateAsync(orderDelivery);
				}
			}

			// Deserialize the response
			return response.Data;
		}

		public async Task UpdateOrderStatusesAsync()
		{
			using (_dataFilte.Disable<IMultiTenant>())
			{
				// Fetch orders with DeliveryStatus = Shipped
				var orders = await _orderDeliveryRepository.GetListAsync(o => o.DeliveryStatus == DeliveryStatus.Shipped &&
																		(o.DeliveryMethod == DeliveryMethod.TCatDeliveryNormal ||
																		  o.DeliveryMethod == DeliveryMethod.TCatDeliveryFrozen ||
																		  o.DeliveryMethod == DeliveryMethod.TCatDeliveryFreeze ||
																		  o.DeliveryMethod == DeliveryMethod.TCatDeliverySevenElevenFreeze ||
																		  o.DeliveryMethod == DeliveryMethod.TCatDeliverySevenElevenFrozen ||
																		  o.DeliveryMethod == DeliveryMethod.TCatDeliverySevenElevenNormal));

				foreach (var order in orders)
				{
					// Get Tcat LogisticsProviderSettings for the order's tenant
					var logisticsProvider = await _logisticsProviderRepository.FirstOrDefaultAsync(l =>
						l.LogisticProvider == LogisticProviders.TCat && l.TenantId == order.TenantId);

					if (logisticsProvider != null)
					{




						var response = await GetOBTStatusAsync(new OBTStatusRequestDto
						{
							CustomerId = logisticsProvider.CustomerId,
							CustomerToken = logisticsProvider.CustomerToken,
							OBTNumbers = new List<string> { order.DeliveryNo }

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
	}
}
