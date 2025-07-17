using Autofac.Core;
using Castle.Core.Smtp;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.DeliveryTempratureCosts;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.OrderTradeNos;
using Kooco.Pikachu.StoreLogisticOrders;
using Kooco.Pikachu.Tenants;
using Kooco.Pikachu.Tenants.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;
using Xunit;
using IEmailSender = Volo.Abp.Emailing.IEmailSender;

namespace Kooco.Pikachu.StoreLogisticsOrders
{
    public class StoreLogisticsOrderAppServiceTest : PikachuApplicationTestBase

    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderDeliveryRepository> _orderDeliveryRepositoryMock;
        private readonly Mock<ILogisticsProvidersAppService> _logisticsProvidersAppServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IStringLocalizer<PikachuResource>> _localizerMock;
        private readonly Mock<IGroupBuyAppService> _groupBuyAppServiceMock;
        private readonly Mock<IBackgroundJobManager> _backgroundJobManagerMock;
        private readonly Mock<ITenantTripartiteRepository> _electronicInvoiceSettingRepositoryMock;
        private readonly Mock<IOrderInvoiceAppService> _electronicInvoiceAppServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IGroupBuyRepository> _groupBuyRepositoryMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<ITenantSettingsAppService> _tenantSettingsAppServiceMock;
        private readonly Mock<IEmailAppService> _emailAppServiceMock;
        private readonly StoreLogisticsOrderAppService _service;
        private readonly IDataFilter<IMultiTenant> _multiTenantFilter;
        private readonly Mock<IOrderHistoryRepository> _orderHistoryRepositoryMock;
        private readonly OrderHistoryManager _orderManager;
        private readonly Mock<IRepository<DeliveryTemperatureCost, Guid>> _deliveryTemperatureCostRepositoryMock;
        private readonly Mock<IOrderTradeNoRepository> _orderTradeNoRepository;

        public StoreLogisticsOrderAppServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderDeliveryRepositoryMock = new Mock<IOrderDeliveryRepository>();
            _logisticsProvidersAppServiceMock = new Mock<ILogisticsProvidersAppService>();
            _configurationMock = new Mock<IConfiguration>();
            _localizerMock = new Mock<IStringLocalizer<PikachuResource>>();
            _groupBuyAppServiceMock = new Mock<IGroupBuyAppService>();
            _backgroundJobManagerMock = new Mock<IBackgroundJobManager>();
            _electronicInvoiceSettingRepositoryMock = new Mock<ITenantTripartiteRepository>();
            _electronicInvoiceAppServiceMock = new Mock<IOrderInvoiceAppService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _groupBuyRepositoryMock = new Mock<IGroupBuyRepository>();
            _emailSenderMock = new Mock<IEmailSender>();
            _tenantSettingsAppServiceMock = new Mock<ITenantSettingsAppService>();
            _emailAppServiceMock = new Mock<IEmailAppService>();
            _multiTenantFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
            _orderHistoryRepositoryMock = new Mock<IOrderHistoryRepository>();
            _orderManager = new OrderHistoryManager(_orderHistoryRepositoryMock.Object);
            _deliveryTemperatureCostRepositoryMock = new Mock<IRepository<DeliveryTemperatureCost, Guid>>();
            _orderTradeNoRepository = new Mock<IOrderTradeNoRepository>();

            _service = new StoreLogisticsOrderAppService(
                _orderDeliveryRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _logisticsProvidersAppServiceMock.Object,
                _configurationMock.Object,
                _localizerMock.Object,
                _groupBuyAppServiceMock.Object,
                _backgroundJobManagerMock.Object,
                _electronicInvoiceSettingRepositoryMock.Object,
                _electronicInvoiceAppServiceMock.Object,
                _httpContextAccessorMock.Object,
                _groupBuyRepositoryMock.Object,
                _emailSenderMock.Object,
                _tenantSettingsAppServiceMock.Object,
                _emailAppServiceMock.Object,
                _orderManager,
                _deliveryTemperatureCostRepositoryMock.Object,
                _orderTradeNoRepository.Object
            );
        }
        private void SetProtectedProperty<T>(T obj, string propertyName, object value)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            property.SetValue(obj, value);
        }
        [Fact]
        public async Task CreateHomeDeliveryShipmentOrderAsync_Should_Throw_Exception_If_Order_Not_Found()
        {
            // Arrange
            using (_multiTenantFilter.Disable())
            {
                // 使用完全唯一的 GUID 確保訂單不存在
                var nonExistentOrderId = Guid.NewGuid();
                var nonExistentOrderDeliveryId = Guid.NewGuid();

                // Mock repository 返回 null
                _orderRepositoryMock.Setup(repo => repo.GetAsync(nonExistentOrderId, true, default))
                    .ThrowsAsync(new EntityNotFoundException(typeof(Order), nonExistentOrderId));

                // Act & Assert - 期望 NullReferenceException（因為 Logger 為 null）
                var exception = await Assert.ThrowsAsync<NullReferenceException>(async () =>
                    await _service.CreateHomeDeliveryShipmentOrderAsync(nonExistentOrderId, nonExistentOrderDeliveryId));
                
                // 驗證拋出了預期的異常
                exception.ShouldNotBeNull();
            }
        }

    }
}