namespace LAB.DataScanner.Components.Tests.Unit.Extractors.TextExtractor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.Extractors.TextExtractor;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using NSubstitute;
    using RabbitMQ.Client.Events;

    [TestClass]
    public class TextParserEngineTests
    {
        [TestMethod]
        public void ShouldSetQueueWithCertainData()
        {
            // Arrange
            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""TestQueueName"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Criteria"": ""Between"",
            ""TargetWord"": ""TargetWord"",
            ""StartWord"": ""TestStartWord"",
            ""StopWord"": ""TestStopWord""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var fakeConfigurationRoot = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();
                var configurationSection = fakeConfigurationRoot.GetSection("Config");

                var rmqConsumerMock = Substitute.For<IRmqConsumer>();
                var rmqPublisherMock = Substitute.For<IRmqPublisher>();
                var extractionMethodsMock = Substitute.For<IExtractionMethods>();

                var fakeQueueStringAsByte =
                    Encoding.UTF8.GetBytes("target string from queue that we need to parser");

                rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = fakeQueueStringAsByte
                        });
                    });

                var sut = new TextExtractorEngine(configurationSection, rmqConsumerMock, rmqPublisherMock, extractionMethodsMock);

                // Act
                sut.Start();

                // Assert
                rmqConsumerMock
                    .Received(1)
                    .SetQueue(Arg.Is<string>("TestQueueName"));
            }
        }

        [TestMethod]
        public void ShouldCallContainsMethodIfConfiguredFor()
        {
            // Arrange
            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""TestQueueName"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Criteria"": ""Contains"",
            ""TargetWord"": ""TestTargetWord"",
            ""StartWord"": ""TestStartWord"",
            ""StopWord"": ""TestStopWord""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var fakeConfigurationRoot = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();
                var configurationSection = fakeConfigurationRoot.GetSection("Config");

                var rmqConsumerMock = Substitute.For<IRmqConsumer>();
                var rmqPublisherMock = Substitute.For<IRmqPublisher>();
                var extractionMethodsMock = Substitute.For<IExtractionMethods>();

                var fakeQueueString = "target string from queue that we need to parser";
                var fakeQueueStringAsByte = Encoding.UTF8.GetBytes(fakeQueueString);

                rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = fakeQueueStringAsByte
                        });
                    });

                var sut = new TextExtractorEngine(configurationSection, rmqConsumerMock, rmqPublisherMock, extractionMethodsMock);

                // Act
                sut.Start();

                // Assert
                extractionMethodsMock
                    .Received(1)
                    .Contains(
                        Arg.Is<string>("TestTargetWord"),
                        Arg.Is<string>(fakeQueueString));
            }
        }

        [TestMethod]
        public void ShouldCallBetweenMethodIfConfiguredFor()
        {
            // Arrange
            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""TestQueueName"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Criteria"": ""between"",
            ""TargetWord"": ""TestTargetWord"",
            ""StartWord"": ""TestStartWord"",
            ""StopWord"": ""TestStopWord""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var fakeConfigurationRoot = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();
                var configurationSection = fakeConfigurationRoot.GetSection("Config");

                var rmqConsumerMock = Substitute.For<IRmqConsumer>();
                var rmqPublisherMock = Substitute.For<IRmqPublisher>();
                var extractionMethodsMock = Substitute.For<IExtractionMethods>();

                var fakeQueueString = "target string from queue that we need to parser";
                var fakeQueueStringAsByte = Encoding.UTF8.GetBytes(fakeQueueString);

                rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = fakeQueueStringAsByte
                        });
                    });

                var sut = new TextExtractorEngine(configurationSection, rmqConsumerMock, rmqPublisherMock, extractionMethodsMock);

                // Act
                sut.Start();

                // Assert
                extractionMethodsMock
                    .Received(1)
                    .Between(
                        Arg.Is<string>("TestStartWord"),
                        Arg.Is<string>("TestStopWord"),
                        Arg.Is<string>(fakeQueueString));
            }
        }

        [TestMethod]
        public void ShouldCallPublishWithCertainData()
        {
            // Arrange
            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""TestQueueName"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1,RoutingKey2,RoutingKey3""
        },
    ""Application"": {
            ""Criteria"": ""between"",
            ""TargetWord"": ""TestTargetWord"",
            ""StartWord"": ""TestStartWord"",
            ""StopWord"": ""TestStopWord""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var fakeConfigurationRoot = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();
                var configurationSection = fakeConfigurationRoot.GetSection("Config");

                var rmqConsumerMock = Substitute.For<IRmqConsumer>();
                var rmqPublisherMock = Substitute.For<IRmqPublisher>();
                var extractionMethodsMock = Substitute.For<IExtractionMethods>();

                var fakeQueueString = "target string from queue that we need to parser";
                var fakeQueueStringAsByte = Encoding.UTF8.GetBytes(fakeQueueString);
                string[] routingKeys = new string[3] { "RoutingKey1", "RoutingKey2", "RoutingKey3" };

                extractionMethodsMock
                    .Between(
                    Arg.Is<string>("TestStartWord"),
                    Arg.Is<string>("TestStopWord"),
                    Arg.Is<string>(fakeQueueString))
                    .Returns("parsedData");

                rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = fakeQueueStringAsByte
                        });
                    });

                var sut = new TextExtractorEngine(
                    configurationSection, 
                    rmqConsumerMock, 
                    rmqPublisherMock, 
                    extractionMethodsMock);

                // Act
                sut.Start();

                // Assert
                rmqPublisherMock
                    .Received(1)
                    .Publish(
                        Arg.Is<byte[]>(messsageBytes => 
                            Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject("parsedData"))
                            .SequenceEqual(messsageBytes)),
                        Arg.Is<string>("TestExchange"),
                        Arg.Is<string[]>(rk => routingKeys.SequenceEqual(rk)));
            }
        }

        [TestMethod]
        public void ShouldCallAck()
        {
            // Arrange
            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""TestQueueName"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Criteria"": ""between"",
            ""TargetWord"": ""TestTargetWord"",
            ""StartWord"": ""TestStartWord"",
            ""StopWord"": ""TestStopWord""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var fakeConfigurationRoot = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();
                var configurationSection = fakeConfigurationRoot.GetSection("Config");

                var rmqConsumerMock = Substitute.For<IRmqConsumer>();
                var rmqPublisherMock = Substitute.For<IRmqPublisher>();
                var extractionMethodsMock = Substitute.For<IExtractionMethods>();

                var fakeQueueString = "target string from queue that we need to parser";
                var fakeQueueStringAsByte = Encoding.UTF8.GetBytes(fakeQueueString);

                extractionMethodsMock
                    .Between(
                    Arg.Is<string>("TestStartWord"),
                    Arg.Is<string>("TestStopWord"),
                    Arg.Is<string>(fakeQueueString))
                    .Returns("parsedData");

                rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = fakeQueueStringAsByte
                        });
                    });

                var sut = new TextExtractorEngine(
                    configurationSection,
                    rmqConsumerMock,
                    rmqPublisherMock,
                    extractionMethodsMock);

                // Act
                sut.Start();

                // Assert
                rmqConsumerMock
                    .Received(1)
                    .Ack(Arg.Any<BasicDeliverEventArgs>());
            }
        }
    }
}
