using Kooco.Pikachu.Application.ShippingStrategies;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.ShippingStrategies;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Kooco.Pikachu.ShippingStrategies
{
    public class ShippingStrategyFactoryTests : PikachuApplicationTestBase
    {
        private readonly IShippingStrategyFactory _shippingStrategyFactory;
        private readonly IServiceProvider _serviceProvider;

        public ShippingStrategyFactoryTests()
        {
            _serviceProvider = GetRequiredService<IServiceProvider>();
            _shippingStrategyFactory = GetRequiredService<IShippingStrategyFactory>();
        }

        [Fact]
        public void CreateStrategy_WithSevenEleven_ShouldReturnSevenElevenStrategy()
        {
            // Act
            var strategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.SevenToEleven1);

            // Assert
            strategy.ShouldNotBeNull();
            strategy.ShouldBeOfType<SevenElevenShippingStrategy>();
            strategy.DeliveryMethod.ShouldBe(DeliveryMethod.SevenToEleven1);
            strategy.LogisticProvider.ShouldBe(LogisticProviders.SevenToEleven);
        }

        [Fact]
        public void CreateStrategy_WithFamilyMart_ShouldReturnFamilyMartStrategy()
        {
            // Act
            var strategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.FamilyMart1);

            // Assert
            strategy.ShouldNotBeNull();
            strategy.ShouldBeOfType<FamilyMartShippingStrategy>();
            strategy.DeliveryMethod.ShouldBe(DeliveryMethod.FamilyMart1);
            strategy.LogisticProvider.ShouldBe(LogisticProviders.FamilyMart);
        }

        [Fact]
        public void CreateStrategy_WithBlackCat_ShouldReturnBlackCatStrategy()
        {
            // Act
            var strategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.BlackCat1);

            // Assert
            strategy.ShouldNotBeNull();
            strategy.ShouldBeOfType<BlackCatShippingStrategy>();
            strategy.DeliveryMethod.ShouldBe(DeliveryMethod.BlackCat1);
            strategy.LogisticProvider.ShouldBe(LogisticProviders.GreenWorldLogistics);
        }

        [Fact]
        public void CreateStrategy_WithHomeDelivery_ShouldReturnHomeDeliveryStrategy()
        {
            // Act
            var strategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.HomeDelivery);

            // Assert
            strategy.ShouldNotBeNull();
            strategy.ShouldBeOfType<HomeDeliveryShippingStrategy>();
            strategy.DeliveryMethod.ShouldBe(DeliveryMethod.HomeDelivery);
            strategy.LogisticProvider.ShouldBe(LogisticProviders.GreenWorldLogistics);
        }

        [Fact]
        public void CreateStrategy_WithSelfPickup_ShouldReturnSelfPickupStrategy()
        {
            // Act
            var strategy = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.SelfPickup);

            // Assert
            strategy.ShouldNotBeNull();
            strategy.ShouldBeOfType<SelfPickupShippingStrategy>();
            strategy.DeliveryMethod.ShouldBe(DeliveryMethod.SelfPickup);
            strategy.LogisticProvider.ShouldBe(LogisticProviders.HomeDelivery); // Self pickup uses HomeDelivery provider
        }

        [Fact]
        public void CreateStrategy_WithUnsupportedDeliveryMethod_ShouldReturnNull()
        {
            // Act
            var strategy = _shippingStrategyFactory.CreateStrategy((DeliveryMethod)999);

            // Assert
            strategy.ShouldBeNull();
        }

        [Fact]
        public void GetSupportedDeliveryMethods_ShouldReturnAllSupportedMethods()
        {
            // Act
            var supportedMethods = _shippingStrategyFactory.GetSupportedDeliveryMethods().ToList();

            // Assert
            supportedMethods.ShouldNotBeEmpty();
            supportedMethods.ShouldContain(DeliveryMethod.SevenToEleven1);
            supportedMethods.ShouldContain(DeliveryMethod.FamilyMart1);
            supportedMethods.ShouldContain(DeliveryMethod.BlackCat1);
            supportedMethods.ShouldContain(DeliveryMethod.HomeDelivery);
            supportedMethods.ShouldContain(DeliveryMethod.SelfPickup);
        }

        [Theory]
        [InlineData(DeliveryMethod.SevenToEleven1, true)]
        [InlineData(DeliveryMethod.FamilyMart1, true)]
        [InlineData(DeliveryMethod.BlackCat1, true)]
        [InlineData(DeliveryMethod.HomeDelivery, true)]
        [InlineData(DeliveryMethod.SelfPickup, true)]
        public void IsDeliveryMethodSupported_WithValidMethods_ShouldReturnExpectedResult(
            DeliveryMethod deliveryMethod, bool expectedResult)
        {
            // Act
            var isSupported = _shippingStrategyFactory.IsDeliveryMethodSupported(deliveryMethod);

            // Assert
            isSupported.ShouldBe(expectedResult);
        }

        [Fact]
        public void CreateStrategy_MultipleCalls_ShouldReturnNewInstancesEachTime()
        {
            // Act
            var strategy1 = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.SevenToEleven1);
            var strategy2 = _shippingStrategyFactory.CreateStrategy(DeliveryMethod.SevenToEleven1);

            // Assert
            strategy1.ShouldNotBeNull();
            strategy2.ShouldNotBeNull();
            strategy1.ShouldNotBeSameAs(strategy2); // Factory should create new instances
        }

        [Fact]
        public void CreateStrategy_AllSupportedMethods_ShouldReturnValidStrategies()
        {
            // Arrange
            var supportedMethods = _shippingStrategyFactory.GetSupportedDeliveryMethods();

            // Act & Assert
            foreach (var method in supportedMethods)
            {
                var strategy = _shippingStrategyFactory.CreateStrategy(method);
                strategy.ShouldNotBeNull();
                strategy.DeliveryMethod.ShouldBe(method);
            }
        }
    }
}