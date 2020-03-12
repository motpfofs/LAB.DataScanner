namespace LAB.DataScanner.Components.Tests.Unit.Generators.UrlsGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class UrlsGeneratorEngineTests
    {
        [TestMethod]
        public void ShouldGenerateUrlsBasedOnProperties()
        {
            // Arrange
            var rmqPublisherServiceMock = Substitute.For<IRmqPublisher>();

            var configDic = new Dictionary<string, string>
                {
                    { "Config:Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                    { "Config:Application:Sequences", "['0..2', '3..6', '4..5']" },

                    { "Config:Binding:SenderExchange", "TargetExchange" },
                    { "Config:Binding:SenderRoutingKeys", "['A', 'B']" }
                };

            var builder = new ConfigurationBuilder()
              .AddInMemoryCollection(configDic);

            var fakeConfiguration= builder
                .Build();

            var fakeConfigurationSection = fakeConfiguration.GetSection("Config");

            var sut = new UrlsGeneratorEngine(rmqPublisherServiceMock, fakeConfigurationSection);

            // Act
            sut.Start();

            // Assert 
            rmqPublisherServiceMock
               .Received()
               .Publish(Arg.Is<byte[]>(e => Encoding.UTF8.GetString(e) == "http://testSite/0/3/4"),
               Arg.Is("TargetExchange"),
               Arg.Is("A"));

            rmqPublisherServiceMock
               .Received()
               .Publish(Arg.Is<byte[]>(e => Encoding.UTF8.GetString(e) == "http://testSite/1/4/5"),
               Arg.Is("TargetExchange"),
               Arg.Is("B"));

            rmqPublisherServiceMock
              .Received(24)
              .Publish(Arg.Any<byte[]>(),
              Arg.Any<string>(),
              Arg.Is("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldSkipPublishingIfNoAnyBindingsInfo()
        {
            // Arrange
            var rmqPublisherServiceMock = Substitute.For<IRmqPublisher>();

            var configDic = new Dictionary<string, string>
                {
                    { "Config:Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                    { "Config:Application:Sequences", "['0..2', '3..6', '4..5']" },

                    { "Config:Binding:SenderExchange", "" },
                    { "Config:Binding:SenderRoutingKeys", "" }
                };

            var builder = new ConfigurationBuilder()
              .AddInMemoryCollection(configDic);

            var fakeConfiguration = builder
                .Build();

            var fakeConfigurationSection = fakeConfiguration.GetSection("Config");

            var sut = new UrlsGeneratorEngine(rmqPublisherServiceMock, fakeConfigurationSection);

            // Act
            sut.Start();
        }
    }
}
