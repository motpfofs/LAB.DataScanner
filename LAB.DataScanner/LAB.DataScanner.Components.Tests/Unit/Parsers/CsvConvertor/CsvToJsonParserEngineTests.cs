namespace LAB.DataScanner.Components.Tests.Unit.Parsers.CsvConvertor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Parsers.CsvConverter;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using RabbitMQ.Client.Events;

    [TestClass]
    public class CsvToJsonParserEngineTest
    {
        [TestMethod]
        public void ShouldSetSourceQueueWithParameterFromAppsettings()
        {
            var rmqPublisherMock = Substitute.For<IRmqPublisher>();
            var rmqConsumerMock = Substitute.For<IRmqConsumer>();
            var csvToJsonParserMock = Substitute.For<ICsvConverter>();

            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""S0urc3Qu3u3"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Columns"": ""1 2 3 4 5 6 7"",
            ""Rows"": ""76""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var builder = new ConfigurationBuilder().AddJsonStream(ms);
                var configurationSection = builder
                    .Build()
                    .GetSection("Config");

                var sut = new CsvToJsonConverterEngine(
                    configurationSection,
                    rmqConsumerMock,
                    rmqPublisherMock,
                    csvToJsonParserMock);

                // Act
                sut.Start();
            }

            // Assert
            rmqConsumerMock
                .Received(1)
                .SetQueue(Arg.Is("S0urc3Qu3u3"));
        }

        [TestMethod]
        public void ShouldSetCsvToJsonParserWithCertainData()
        {
            var rmqPublisherMock = Substitute.For<IRmqPublisher>();
            var rmqConsumerMock = Substitute.For<IRmqConsumer>();
            var csvToJsonParserMock = Substitute.For<ICsvConverter>();

            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""S0urc3Qu3u3"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Columns"": ""1 2 3 4 5 6 7"",
            ""Rows"": ""76""
        }
    }
}";

            string testData =
@"#,Name,City
1,Gauguin,Paris
2,Cezanne,Aix-en-Provence
3,Edgar,Paris
4.Claude,Paris 
5,Edouard,Paris
6,Renoir,Limotges";

            rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        byte[] fakeBytesUrl = Encoding.UTF8.GetBytes(testData);
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = Encoding.UTF8.GetBytes(testData)
                        });
                    });

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var builder = new ConfigurationBuilder().AddJsonStream(ms);
                var configurationSection = builder
                    .Build()
                    .GetSection("Config"); 

                var sut = new CsvToJsonConverterEngine(
                    configurationSection,
                    rmqConsumerMock,
                    rmqPublisherMock,
                    csvToJsonParserMock);

                // Act
                sut.Start();
            }

            // Assert
            csvToJsonParserMock
                .Received(1)
                .ConvertToJson(
                Arg.Any<string>(),
                Arg.Is<string>(testData),
                Arg.Is<int[]>(e => new int[] { 1, 2, 3, 4, 5, 6, 7 }.SequenceEqual(e)),
                Arg.Is<int>(76));
        }

        [TestMethod]
        public void ShouldSendAcknowledgetoBrocker()
        {
            var rmqPublisherMock = Substitute.For<IRmqPublisher>();
            var rmqConsumerMock = Substitute.For<IRmqConsumer>();
            var csvToJsonParserMock = Substitute.For<ICsvConverter>();

            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""S0urc3Qu3u3"",
            ""SenderExchange"": ""TestExchange"",
            ""SenderRoutingKeys"": ""RoutingKey1 RoutingKey2 RoutingKey3""
        },
    ""Application"": {
            ""Columns"": ""1 2 3 4 5"",
            ""Rows"": ""72""
        }
    }
}";

            string testData =
@"#,Name,City
1,Gauguin,Paris
2,Cezanne,Aix-en-Provence
3,Edgar,Paris
4.Claude,Paris 
5,Edouard,Paris
6,Renoir,Limotges";

            rmqConsumerMock
                    .When(w => w.StartListening(Arg.Any<EventHandler<BasicDeliverEventArgs>>()))
                    .Do((args) =>
                    {
                        byte[] fakeBytesUrl = Encoding.UTF8.GetBytes(testData);
                        args.Arg<EventHandler<BasicDeliverEventArgs>>().Invoke(this, new BasicDeliverEventArgs()
                        {
                            Body = Encoding.UTF8.GetBytes(testData)
                        });
                    });

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var builder = new ConfigurationBuilder().AddJsonStream(ms);
                var configurationSection = builder
                    .Build()
                    .GetSection("Config");

                var sut = new CsvToJsonConverterEngine(
                    configurationSection,
                    rmqConsumerMock,
                    rmqPublisherMock,
                    csvToJsonParserMock);

                // Act
                sut.Start();
            }

            // Assert
            rmqConsumerMock
                .Received(1)
                .Ack(Arg.Any<BasicDeliverEventArgs>());
        }
    }
}
