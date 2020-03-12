namespace LAB.DataScanner.Components.Tests.Unit.Persisters.SimpleDataPersister
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Persisters.SimpleDataPersister;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using RabbitMQ.Client.Events;

    [TestClass]
    public class SimpleDataPersisterEngineTests
    {
        [TestMethod]
        public void ShouldSetSourceQueueWithParameterFromAppsettings()
        {
            var rmqConsumerMock = Substitute.For<IRmqConsumer>();
            var dbDataWriter = Substitute.For<IDbDataWriter>();

            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""QueueDataPersister""
        },
    ""Application"": {
            ""DataSchemaBase64"": ""ewogICJUYWJsZU5hbWUiOiAiTmFtZUJhc2VkNjQiLAogICJIZWFkZXIiOiB7CiAgICAiRmllbGQxIjogInN0cmluZyIsCiAgICAiRmllbGQyIjogIklOVCIsCiAgICAiRmllbGQzIjogInN0cmluZyIKICB9Cn0="",
            ""ConnectionString"": ""Server=***;Database=ProcessedDataCh2;User ID=***;Password=***;""
        }
    }
}";
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var builder = new ConfigurationBuilder().AddJsonStream(ms);
                var configurationSection = builder
                    .Build()
                    .GetSection("Config");


                var sut = new SimpleDataPersisterEngine(
                    configurationSection,
                    rmqConsumerMock,
                    dbDataWriter);

                // Act
                sut.Start();
            }

            // Assert
            rmqConsumerMock
                .Received(1)
                .SetQueue(Arg.Is("QueueDataPersister"));
        }

        [TestMethod]
        public void ShouldCallDbWriterWithCertainDataFromMessageBroker()
        {
            var rmqConsumerMock = Substitute.For<IRmqConsumer>();
            var dbDataWriterMock = Substitute.For<IDbDataWriter>();

            var configSample =
@"{
""Config"":{
    ""Binding"": {
            ""ReceiverQueue"": ""QueueDataPersister""
        },
    ""Application"": {
            ""DataSchemaBase64"": ""ewogICJUYWJsZU5hbWUiOiAiTmFtZUJhc2VkNjQiLAogICJIZWFkZXIiOiB7CiAgICAiRmllbGQxIjogInN0cmluZyIsCiAgICAiRmllbGQyIjogIklOVCIsCiAgICAiRmllbGQzIjogInN0cmluZyIKICB9Cn0="",
            ""ConnectionString"": ""Server=***;Database=ProcessedDataCh2;User ID=***;Password=***;""
        }
    }
}";

            var testData = @"{ ""Field"": ""Value""}";
            var expectedStringArray = new string[] { @"{ ""Field"": ""Value""}" };
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(configSample)))
            {
                var builder = new ConfigurationBuilder().AddJsonStream(ms);
                var configurationSection = builder
                    .Build()
                    .GetSection("Config");


                var sut = new SimpleDataPersisterEngine(
                    configurationSection,
                    rmqConsumerMock,
                    dbDataWriterMock);

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

                // Act
                sut.Start();
            }

            // Assert
            dbDataWriterMock
                .Received(1)
                .SaveMessagesToDb(Arg.Is<string[]>(e => expectedStringArray.SequenceEqual(e)));
        }
    }
}

