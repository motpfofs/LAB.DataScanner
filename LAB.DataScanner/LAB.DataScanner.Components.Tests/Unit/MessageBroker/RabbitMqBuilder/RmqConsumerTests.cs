namespace LAB.DataScanner.Components.Tests.Unit.MessageBroker.RabbitMqBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    [TestClass]
    public class RmqConsumerTests
    {
        [TestMethod]
        public void ShouldSendAck()
        {
            // Arange
            string queueName = "TestQueueName";
            var amqpChannelMock = Substitute.For<IModel>();

            var rmqConsumer = new RmqConsumer(amqpChannelMock, queueName);
            var basicProperties = Substitute.For<IBasicProperties>();
            string consumerTag = "TestConsumeTag";
            ulong deliveryTag = 1;
            bool redelivered = false;
            string exchange = "TestExchange";
            const string RoutingKey = "TestRoute";
            byte[] message = Encoding.UTF8.GetBytes("Test message");
            var args = new BasicDeliverEventArgs(consumerTag, deliveryTag, redelivered, exchange, RoutingKey, basicProperties, message);

            // Act
            rmqConsumer.Ack(args);

            // Assert
            amqpChannelMock
               .Received(1)
                .BasicAck(Arg.Is((ulong)1), Arg.Is(false));
        }
        [TestMethod]
        public void ShouldStartListening()
        {
            // Arange
            string queueName = "TestQueueName";
            var amqpChannelMock = Substitute.For<IModel>();

            var rmqConsumer = new RmqConsumer(amqpChannelMock, queueName); var basicProperties = Substitute.For<IBasicProperties>();
            
            string consumerTag = "TestConsumeTag";
            ulong deliveryTag = 1;
            bool redelivered = false;
            string exchange = "TestExchange";
            const string RoutingKey = "TestRoute";
            byte[] message = Encoding.UTF8.GetBytes("Test message");
            
            var args = new BasicDeliverEventArgs(consumerTag, deliveryTag, redelivered, exchange, RoutingKey, basicProperties, message);
            var consumer = rmqConsumer.basicConsumer;
            var wasCalled = false;
            EventHandler<BasicDeliverEventArgs> onRecievedHandler = new EventHandler<BasicDeliverEventArgs>(
                 (sender, args) => { wasCalled = true; });

            // Act
            rmqConsumer.StartListening(onRecievedHandler);
            onRecievedHandler.Invoke(amqpChannelMock, args);

            // Assert
            amqpChannelMock
               .Received(1)
               .BasicConsume(
                   Arg.Is<string>(str => str == "TestQueueName"),
                   Arg.Is(false),
                   Arg.Any<string>(),
                   Arg.Any<bool>(),
                   Arg.Any<bool>(),
                   Arg.Any<IDictionary<String, object>>(),
                   Arg.Is<EventingBasicConsumer>(rmqcs => rmqcs.Equals(consumer)));
            Assert.AreEqual<bool>(true, wasCalled);
        }

        [TestMethod]
        public void ShouldStopListening()
        {
            // Arange
            string queueName = "TestQueueName";
            var amqpChannelMock = Substitute.For<IModel>();

            var rmqConsumer = new RmqConsumer(amqpChannelMock, queueName); 
            var consumer = rmqConsumer.basicConsumer;
            consumer.ConsumerTag = "Test";

            // Act
            rmqConsumer.StartListening((model, ea) =>
            {
                var body = ea.Body;
            });
            rmqConsumer.StopListening();

            // Assert
            amqpChannelMock
               .Received(1)
                .BasicCancel(Arg.Is(consumer.ConsumerTag));
        }

        [TestMethod]
        public void ShouldStartListenUsingChangedQueueName()
        {
            // Arange
            string queueName = "TestQueueName";
            var amqpChannelMock = Substitute.For<IModel>();

            var rmqConsumer = new RmqConsumer(amqpChannelMock, queueName); var basicProperties = Substitute.For<IBasicProperties>();

            string consumerTag = "TestConsumeTag";
            ulong deliveryTag = 1;
            bool redelivered = false;
            string exchange = "TestExchange";
            const string RoutingKey = "TestRoute";
            byte[] message = Encoding.UTF8.GetBytes("Test message");

            var args = new BasicDeliverEventArgs(consumerTag, deliveryTag, redelivered, exchange, RoutingKey, basicProperties, message);
            var consumer = rmqConsumer.basicConsumer;
            var wasCalled = false;
            EventHandler<BasicDeliverEventArgs> onRecievedHandler = new EventHandler<BasicDeliverEventArgs>(
                 (sender, args) => { wasCalled = true; });

            // Act
            rmqConsumer.SetQueue("NewQueueName");
            rmqConsumer.StartListening(onRecievedHandler);
            onRecievedHandler.Invoke(amqpChannelMock, args);

            // Assert
            amqpChannelMock
               .Received(1)
               .BasicConsume(
                   Arg.Is<string>(str => str == "NewQueueName"),
                   Arg.Is(false),
                   Arg.Any<string>(),
                   Arg.Any<bool>(),
                   Arg.Any<bool>(),
                   Arg.Any<IDictionary<String, object>>(),
                   Arg.Is<EventingBasicConsumer>(rmqcs => rmqcs.Equals(consumer)));
            Assert.AreEqual<bool>(true, wasCalled);
        }

    }
}
