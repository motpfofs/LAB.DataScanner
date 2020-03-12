namespace LAB.DataScanner.Components.Tests.Unit.Extractors.ImageExtractor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.DataRetriever;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.Extractors.ImageExtractor;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using NSubstitute;

    [TestClass]
    public class ImageExtractorEngineTests
    {
        [TestMethod]
        public void ShouldCallImagesLinksExtractorWithAllLinks()
        {
            // Arrange
            var imagesLinksExtractorMock = Substitute.For<IImageLinksExtractor>();
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

            var fakeConfigurationSection = builder
                .Build()
                .GetSection("Config");

            var sut = new ImageExtractorEngine(
                imagesLinksExtractorMock,
                dataRetrieverMock,
                fakeConfigurationSection,
                rmqPublisherMock);

            // Act
            sut.Start();

            // Assert 
            imagesLinksExtractorMock
                .Received(1)
                .Extract(Arg.Is("http://ya.ru"));

            imagesLinksExtractorMock
                 .Received(1)
                 .Extract(Arg.Is("http://google.com"));

            imagesLinksExtractorMock
                .Received(1)
                .Extract(Arg.Is("http://mail.ru"));
        }

        [TestMethod]
        public void ShouldCallDownloadForEveryImageLink()
        {
            // Arrange
            var imagesLinksExtractorMock = Substitute.For<IImageLinksExtractor>();
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

            var fakeConfigurationSection = builder
                .Build()
                .GetSection("Config");

            var sut = new ImageExtractorEngine(
                imagesLinksExtractorMock,
                dataRetrieverMock,
                fakeConfigurationSection,
                rmqPublisherMock);

            List<string> imageLinks = new List<string>();
            List<string> nullImageLinks = null;

            imageLinks.Add("http://ya.ru/1.png");
            imageLinks.Add("http://ya.ru/2.png");
            imageLinks.Add("http://ya.ru/3.png");
            imageLinks.Add("http://ya.ru/4.png");

            imagesLinksExtractorMock
                .Extract(Arg.Is("http://ya.ru"))
                .Returns<List<string>>(imageLinks);

            imagesLinksExtractorMock
                 .Extract(Arg.Is("http://google.com"))
                 .Returns<List<string>>(nullImageLinks);

            imagesLinksExtractorMock
                .Extract(Arg.Is("http://mail.ru"))
                 .Returns<List<string>>(nullImageLinks);

            // Act
            sut.Start();

            // Assert 
            dataRetrieverMock
                .Received(1)
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/1.png"));

            dataRetrieverMock
                .Received(1)
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/2.png"));

            dataRetrieverMock
                .Received(1)
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/3.png"));

            dataRetrieverMock
                .Received(1)
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/4.png"));
        }


        [TestMethod]
        public void ShouldCallPublishForEveryImageMessage()
        {
            // Arrange
            var imagesLinksExtractorMock = Substitute.For<IImageLinksExtractor>();
            var dataRetrieverMock = Substitute.For<IDataRetriever>();
            var rmqPublisherMock = Substitute.For<IRmqPublisher>();

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

            var sut = new ImageExtractorEngine(
                imagesLinksExtractorMock,
                dataRetrieverMock,
                fakeConfigurationSection,
                rmqPublisherMock);

            List<string> imageLinks = new List<string>();
            List<string> nullImageLinks = null;

            imageLinks.Add("http://ya.ru/1.png");
            imageLinks.Add("http://ya.ru/2.png");
            imageLinks.Add("http://ya.ru/3.png");
            imageLinks.Add("http://ya.ru/4.png");

            byte[] firstImage = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            byte[] secondImage = { 0x6, 0x7, 0x8, 0x9, 0xA };
            byte[] thirdImage = { 0xB, 0xC, 0xD, 0xE, 0xF };
            byte[] fourthImage = { 0x0, 0x0, 0x0, 0x0, 0x0 };

            var firstImageMessage = new ImageMessage();
            firstImageMessage.ImageAsBase64 = Convert.ToBase64String(firstImage);
            firstImageMessage.ImageNameWithExtension = "1.png";

            var secondImageMessage = new ImageMessage();
            firstImageMessage.ImageAsBase64 = Convert.ToBase64String(secondImage);
            firstImageMessage.ImageNameWithExtension = "2.png";

            var thirdImageMessage = new ImageMessage();
            firstImageMessage.ImageAsBase64 = Convert.ToBase64String(thirdImage);
            firstImageMessage.ImageNameWithExtension = "3.png";

            var fourthImageMessage = new ImageMessage();
            firstImageMessage.ImageAsBase64 = Convert.ToBase64String(fourthImage);
            firstImageMessage.ImageNameWithExtension = "4.png";

            byte[] firstExpectedImagePush = 
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(firstImageMessage));

            byte[] secondExpectedImagePush = 
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(secondImageMessage));

            byte[] thirdExpectedImagePush = 
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(thirdImageMessage));

            byte[] fourthExpectedImagePush = 
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(fourthImageMessage));

            imagesLinksExtractorMock
                .Extract(Arg.Is("http://ya.ru"))
                .Returns<List<string>>(imageLinks);

            imagesLinksExtractorMock
                 .Extract(Arg.Is("http://google.com"))
                 .Returns<List<string>>(nullImageLinks);

            imagesLinksExtractorMock
                .Extract(Arg.Is("http://mail.ru"))
                 .Returns<List<string>>(nullImageLinks);

            dataRetrieverMock
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/1.png"))
                .Returns(Task.FromResult(firstImage));

            dataRetrieverMock
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/2.png"))
                .Returns(Task.FromResult(secondImage));

            dataRetrieverMock
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/3.png"))
                .Returns(Task.FromResult(thirdImage));

            dataRetrieverMock
                .RetrieveBytesAsync(Arg.Is("http://ya.ru/4.png"))
                .Returns(Task.FromResult(fourthImage));

            // Act
            sut.Start();

            // Assert 
            rmqPublisherMock
                .Received()
                .Publish(
                    Arg.Is<byte[]>(e => firstExpectedImagePush.SequenceEqual(e)),
                    Arg.Is<string>("TestExchange"),
                    Arg.Is<string[]>(rk => routingKeysStringArray.SequenceEqual(rk)));
        }
    }
}
