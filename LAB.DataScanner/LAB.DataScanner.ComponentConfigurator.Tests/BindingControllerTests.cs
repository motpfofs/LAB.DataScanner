namespace LAB.DataScanner.ComponentConfigurator.Tests
{
    using LAB.DataScanner.ComponentConfigurator.Controllers;
    using LAB.DataScanner.Components.Models;
    using LAB.DataScanner.Components.MessageBroker.RabbitMq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class BindingControllerTests
    {
        [TestMethod]
        public void ShouldCallConfigurationServiceBuild()
        {
            // Arrange
            BindingConfig data = new BindingConfig { ComponentName = "cmpName", Bindings = null };

            var configurationServiceMock = Substitute.For<IRmqBindingService>();
            var sut = new BindingsController(configurationServiceMock);

            // Act
            sut.Build(data).GetAwaiter().GetResult();

            // Assert
            configurationServiceMock
                .Received(1)
                .Build(Arg.Is<BindingConfig>(data));
        }
    }
}