namespace LAB.DataScanner.Components.Tests.Unit.MessageBroker.RabbitMq
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string response;
        private readonly HttpStatusCode statusCode;

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            this.response = response;
            this.statusCode = statusCode;
        }

        public List<HttpRequestMessage> InputRequests { get; private set; } = new List<HttpRequestMessage>();
        public int NumberOfCalls { get; private set; } = 0;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            InputRequests.Add(request);
            NumberOfCalls++;
            return new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(response)
            };
        }
    }
}
