namespace LAB.DataScanner.Components.Tests.Unit.MessageBroker.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using LAB.DataScanner.Components.MessageBroker.RabbitMq;
    using LAB.DataScanner.Components.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class RmqManagementServiceTests
    {
        [TestMethod]
        public void ShouldMakeRequestsWithGivenData()
        {
            // Arrange
            string testAddress = "http://testurl.com/";
            string testLogin = "Login";
            string testPassword = "Password";
            string base64TestCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{testLogin}:{testPassword}"));
            BindingConfig testData = new BindingConfig()
            {
                ComponentName = "exchange_1",
                Bindings = new List<BindingConfigEntry>
                {
                    new BindingConfigEntry { ComponentName = "queue_1", RoutingKey = "#1" },
                    new BindingConfigEntry { ComponentName = "queue_2", RoutingKey = "#2" }
                }
            };
            int expectedNumberOfCalls = 5;
            MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.Created);
            IConfiguration configuration = Substitute.For<IConfiguration>();
            configuration["Credentials:Login"].Returns(testLogin);
            configuration["Credentials:Password"].Returns(testPassword);
            configuration["RabbitMqAddress"].Returns(testAddress);
            HttpClient httpClient = new HttpClient(httpMessageHandler);
            var managementProvider = new RmqManagementService(httpClient, configuration);
            var configurationService = new RmqBindingService(managementProvider);

            // Act
            configurationService.Build(testData);

            // Assert
            Assert.IsTrue(httpMessageHandler.NumberOfCalls == expectedNumberOfCalls);

            // Asserting that all requests have auth data in headers
            foreach (var sendedRequest in httpMessageHandler.InputRequests)
            {
                Assert.AreEqual(base64TestCredentials, sendedRequest.Headers.Authorization.Parameter);
            }

            // Checking request for adding exchange
            Assert.AreEqual(
                $"{testAddress}api/exchanges/%2f/{testData.ComponentName}",
                httpMessageHandler.InputRequests[0].RequestUri.AbsoluteUri);
            Assert.AreEqual(
                "application/json",
                httpMessageHandler.InputRequests[0].Content.Headers.ContentType.ToString());
            Assert.AreEqual(
                "{\"type\":\"direct\",\"durable\":true}",
                httpMessageHandler.InputRequests[0].Content.ReadAsStringAsync().Result);

            // Checking request for adding first queue
            Assert.AreEqual(
                $"{testAddress}api/queues/%2f/{testData.Bindings[0].ComponentName}",
                httpMessageHandler.InputRequests[1].RequestUri.AbsoluteUri);
            Assert.AreEqual(
                "application/json",
                httpMessageHandler.InputRequests[1].Content.Headers.ContentType.ToString());
            Assert.AreEqual(
                "{\"durable\":true}",
                httpMessageHandler.InputRequests[1].Content.ReadAsStringAsync().Result);

            // Checking request for binding first queue to created exchange
            Assert.AreEqual(
                $"{testAddress}api/bindings/%2f/e/{testData.ComponentName}" +
                $"/q/{testData.Bindings[0].ComponentName}",
                httpMessageHandler.InputRequests[2].RequestUri.AbsoluteUri);
            Assert.AreEqual(
                "application/json",
                httpMessageHandler.InputRequests[2].Content.Headers.ContentType.ToString());
            Assert.AreEqual(
                $"{{\"routing_key\":\"{testData.Bindings[0].RoutingKey}\"}}",
                httpMessageHandler.InputRequests[2].Content.ReadAsStringAsync().Result);

            // Checking request for adding second queue
            Assert.AreEqual(
                $"{testAddress}api/queues/%2f/{testData.Bindings[1].ComponentName}",
                httpMessageHandler.InputRequests[3].RequestUri.AbsoluteUri);
            Assert.AreEqual(
                "application/json",
                httpMessageHandler.InputRequests[3].Content.Headers.ContentType.ToString());
            Assert.AreEqual(
                "{\"durable\":true}",
                httpMessageHandler.InputRequests[3].Content.ReadAsStringAsync().Result);

            // Checking request for binding second queue to created exchange
            Assert.AreEqual(
                $"{testAddress}api/bindings/%2f/e/{testData.ComponentName}" +
                $"/q/{testData.Bindings[1].ComponentName}",
                httpMessageHandler.InputRequests[4].RequestUri.AbsoluteUri);
            Assert.AreEqual(
                "application/json",
                httpMessageHandler.InputRequests[4].Content.Headers.ContentType.ToString());
            Assert.AreEqual(
                $"{{\"routing_key\":\"{testData.Bindings[1].RoutingKey}\"}}",
                httpMessageHandler.InputRequests[4].Content.ReadAsStringAsync().Result);
        }
    }
}