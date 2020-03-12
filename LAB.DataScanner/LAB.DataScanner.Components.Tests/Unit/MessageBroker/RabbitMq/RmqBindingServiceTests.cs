namespace LAB.DataScanner.Components.Tests.Unit.MessageBroker.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using LAB.DataScanner.Components.MessageBroker.RabbitMq;
    using LAB.DataScanner.Components.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class RmqBindingServiceTests
    {
        [TestMethod]
        public void ShouldAuthBeforeManaging()
        {
            // Arrange
            BindingConfig testData = new BindingConfig()
            {
                ComponentName = "exchange_1",
                Bindings = new List<BindingConfigEntry>
                {
                    new BindingConfigEntry { ComponentName = "queue_1", RoutingKey = "#1" },
                    new BindingConfigEntry { ComponentName = "queue_2", RoutingKey = "#2" }
                }
            };
            var managementProvider = Substitute.For<IRmqManagementService>();
            var configurationService = new RmqBindingService(managementProvider);

            // Act
            configurationService.Build(testData);

            // Assert
            Received.InOrder(() =>
            {
                managementProvider.Auth();
                managementProvider.CreateDirectExchange(Arg.Any<string>());
                managementProvider.CreateQueue(Arg.Any<string>());
                managementProvider.BindQueueToExchange(Arg.Any<string>(), Arg.Any<BindingConfigEntry>());
                managementProvider.CreateQueue(Arg.Any<string>());
                managementProvider.BindQueueToExchange(Arg.Any<string>(), Arg.Any<BindingConfigEntry>());
            });
        }

        [TestMethod]
        public void ShouldNotCallManagementProviderIfExchangeNameIsEmpty()
        {
            // Arrange
            BindingConfig testData = new BindingConfig()
            {
                ComponentName = string.Empty,
                Bindings = new List<BindingConfigEntry>
                {
                    new BindingConfigEntry { ComponentName = "queue_1", RoutingKey = "#1" },
                    new BindingConfigEntry { ComponentName = "queue_2", RoutingKey = "#2" }
                }
            };
            var managementProvider = Substitute.For<IRmqManagementService>();
            var configurationService = new RmqBindingService(managementProvider);

            // Act
            try
            {
                configurationService.Build(testData);
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(
                    "Exchange name cannot be empty",
                    ex.InnerException.Message);
            }
        }

        [TestMethod]
        public void ShouldNotCallManagementProviderIfExchangeNameIsWhitespace()
        {
            // Arrange
            BindingConfig testData = new BindingConfig()
            {
                ComponentName = " ",
                Bindings = new List<BindingConfigEntry>
                {
                    new BindingConfigEntry { ComponentName = "queue_1", RoutingKey = "#1" },
                    new BindingConfigEntry { ComponentName = "queue_2", RoutingKey = "#2" }
                }
            };
            var managementProvider = Substitute.For<IRmqManagementService>();
            var configurationService = new RmqBindingService(managementProvider);

            // Act
            try
            {
                configurationService.Build(testData);
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(
                    "Exchange name cannot be empty",
                    ex.InnerException.Message);
            }
        }

        [TestMethod]
        public void ShouldNotCallManagementProviderIfQueueNameIsEmpty()
        {
            // Arrange
            BindingConfig testData = new BindingConfig()
            {
                ComponentName = "exchange_1",
                Bindings = new List<BindingConfigEntry>
                {
                    new BindingConfigEntry { ComponentName = string.Empty, RoutingKey = "#1" },
                    new BindingConfigEntry { ComponentName = "queue_2", RoutingKey = "#2" }
                }
            };
            var managementProvider = Substitute.For<IRmqManagementService>();
            var configurationService = new RmqBindingService(managementProvider);

            // Act
            try
            {
                configurationService.Build(testData);
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(
                    "Queue name cannot be empty",
                    ex.InnerException.Message);
            }
        }

        [TestMethod]
        public void ShouldNotCallManagementProviderIfQueueNameIsWhitespace()
        {
            // Arrange
            BindingConfig testData = new BindingConfig()
            {
                ComponentName = "exchange_1",
                Bindings = new List<BindingConfigEntry>
                {
                    new BindingConfigEntry { ComponentName = "queue_1", RoutingKey = "#1" },
                    new BindingConfigEntry { ComponentName = " ", RoutingKey = "#2" }
                }
            };
            var managementProvider = Substitute.For<IRmqManagementService>();
            var configurationService = new RmqBindingService(managementProvider);

            // Act
            try
            {
                configurationService.Build(testData);
            }
            catch (AggregateException ex)
            {
                Assert.AreEqual(
                    "Queue name cannot be empty",
                    ex.InnerException.Message);
            }
        }
    }
}
