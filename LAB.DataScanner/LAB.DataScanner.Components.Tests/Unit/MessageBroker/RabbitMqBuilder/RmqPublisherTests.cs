namespace LAB.DataScanner.Components.Tests.Unit.MessageBroker.RabbitMqBuilder
{
    using System.Text;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using RabbitMQ.Client;

    [TestClass]
    public class RmqPublisherTests
    {
        [TestMethod]
        public void ShouldPublishMessageToDefaultExchange()
        {
            // Arrange
            const IBasicProperties BasicProperties = null;

            string testExchange = "testExchange";
            string routingKey = "testRoute";

            byte[] message = Encoding.UTF8.GetBytes("Test message");

            var amqpChannelMock = Substitute.For<IModel>();

            amqpChannelMock
                .CreateBasicProperties()
                .Returns((IBasicProperties)null);

            var sut = new RmqPublisher(amqpChannelMock, testExchange, routingKey);

            // Act
            sut.Publish(message);

            // Assert
            amqpChannelMock
                .Received(1)
                .BasicPublish(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Is<IBasicProperties>(bp => bp == BasicProperties),
                    Arg.Is<byte[]>(body => body == message));
        }

        [TestMethod]
        public void ShouldPublishMessageToCertainRoutingKey()
        {
            // Arrange
            const IBasicProperties BasicProperties = null;

            string testExchange = "testExchange";
            string routingKey = "testRoute";

            byte[] message = Encoding.UTF8.GetBytes("Test message");

            var amqpChannelMock = Substitute.For<IModel>();

            amqpChannelMock
                .CreateBasicProperties()
                .Returns((IBasicProperties)null);

            var sut = new RmqPublisher(amqpChannelMock, testExchange, routingKey);

            // Act
            sut.Publish(message, routingKey);

            // Assert
            amqpChannelMock
                .Received(1)
                .BasicPublish(
                    Arg.Any<string>(),
                    Arg.Is<string>(rk => rk == routingKey),
                    Arg.Is<IBasicProperties>(bp => bp == BasicProperties),
                    Arg.Is<byte[]>(body => body == message));
        }

        [TestMethod]
        public void ShouldPublishMessageToCertainExchangeAndRoutingKey()
        {
            // Arrange
            const IBasicProperties BasicProperties = null;

            string testExchange = "testExchange";
            string routingKey = "testRoute";

            byte[] message = Encoding.UTF8.GetBytes("Test message");

            var amqpChannelMock = Substitute.For<IModel>();

            amqpChannelMock
                .CreateBasicProperties()
                .Returns((IBasicProperties)null);

            var sut = new RmqPublisher(amqpChannelMock, testExchange, routingKey);

            // Act
            sut.Publish(message, testExchange, routingKey);

            // Assert
            amqpChannelMock
                .Received(1)
                .BasicPublish(
                    Arg.Is<string>(exch => exch == testExchange),
                    Arg.Is<string>(rk => rk == routingKey),
                    Arg.Is<IBasicProperties>(bp => bp == BasicProperties),
                    Arg.Is<byte[]>(body => body == message));
        }
    }
}
