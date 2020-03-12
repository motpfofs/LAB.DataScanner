namespace LAB.DataScanner.Components.Tests.Unit.Scrapers.HttpGetGrabber
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.DataRetriever;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class GrabberEngineTests
    {
        [TestMethod]
        public void PageDownloaderReceivingLinks()
        {
            // Arange
            var dataRetrieverMock = Substitute.For<IDataRetriever>();
            var rmqPublisherMock = Substitute.For<IRmqPublisher>();

            var configDic = new Dictionary<string, string>
                {
                    { "Config:Application:TargetUrls", "http://ya.ru http://google.com http://mail.ru" },
                    { "Config:Binding:SenderExchange", "TestExchange" },
                    { "Config:Binding:SenderRoutingKeys", "TestRk1 TestRk2 TestRk3" },
                };

            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(configDic);

            var fakeConfiguration = builder
                .Build()
                .GetSection("Config");

            var sut = new WebPageDownloaderEngine(
                dataRetrieverMock,
                fakeConfiguration,
                rmqPublisherMock);

            // Act
            sut.Start();

            // Assert
            dataRetrieverMock
                .Received(1)
                .RetrieveStringAsync("http://ya.ru");

            dataRetrieverMock
                 .Received(1)
                 .RetrieveStringAsync("http://google.com");

            dataRetrieverMock
                .Received(1)
                .RetrieveStringAsync("http://mail.ru");
        }

        [TestMethod]
        public void ShouldPublishOnlyMessage()
        {
            // Arange
            var configDic = new Dictionary<string, string>
            {
                { "Config:Application:TargetUrls", "http://ya.ru http://google.com http://mail.ru" },
                { "Config:Binding:SenderExchange", "TestExchange" },
                { "Config:Binding:SenderRoutingKeys", "TestRk1,TestRk2,TestRk3" },
            };

            var routingKeysStringArray = new string[3] { "TestRk1", "TestRk2", "TestRk3" };

            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(configDic);

            var fakeConfigurationSection = builder
                .Build()
                .GetSection("Config");

            var dataRetrieverMock = Substitute.For<IDataRetriever>();
            var rmqPublisherMock = Substitute.For<IRmqPublisher>();

            byte[] expectedData1 = Encoding.ASCII.GetBytes("TestPageContent1");
            byte[] expectedData2 = Encoding.ASCII.GetBytes("TestPageContent2");
            byte[] expectedData3 = Encoding.ASCII.GetBytes("TestPageContent3");

            dataRetrieverMock
                .RetrieveStringAsync("http://ya.ru")
                .Returns<string>("TestPageContent1");

            dataRetrieverMock
                .RetrieveStringAsync("http://google.com")
                .Returns<string>("TestPageContent2");

            dataRetrieverMock
                .RetrieveStringAsync("http://mail.ru")
                .Returns<string>("TestPageContent3");

            // Act
            var sut = new WebPageDownloaderEngine(
                dataRetrieverMock,
                fakeConfigurationSection,
                rmqPublisherMock);
            sut.Start();

            // Assert
            rmqPublisherMock
                .Received(1)
                .Publish(
                    Arg.Is<byte[]>(e => expectedData1.SequenceEqual(e)),
                    Arg.Is<string>("TestExchange"),
                    Arg.Is<string[]>(rk => routingKeysStringArray.SequenceEqual(rk)));

            rmqPublisherMock
                 .Received(1)
                 .Publish(Arg.Is<byte[]>(e => expectedData2.SequenceEqual(e)),
                    Arg.Is<string>("TestExchange"),
                    Arg.Is<string[]>(rk => routingKeysStringArray.SequenceEqual(rk))); ;

            rmqPublisherMock
                .Received(1)
                .Publish(Arg.Is<byte[]>(e => expectedData3.SequenceEqual(e)),
                    Arg.Is<string>("TestExchange"),
                    Arg.Is<string[]>(rk => routingKeysStringArray.SequenceEqual(rk)));
        }
    }
}