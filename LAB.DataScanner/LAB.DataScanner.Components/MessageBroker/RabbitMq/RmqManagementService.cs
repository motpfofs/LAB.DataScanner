namespace LAB.DataScanner.Components.MessageBroker.RabbitMq
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Models;
    using Microsoft.Extensions.Configuration;

    public class RmqManagementService : IRmqManagementService, IDisposable
    {
        public RmqManagementService(HttpClient httpClient, IConfiguration configuration)
        {
            this.HttpClient = httpClient;
            this.Configuration = configuration;
        }

        public HttpClient HttpClient { get; }
        public IConfiguration Configuration { get; }

        public async Task CreateDirectExchange(string exchangeName)
        {
            string url = HttpClient.BaseAddress + "api/exchanges/%2f/" + exchangeName;
            Console.WriteLine(url);
            string content = "{\"type\":\"direct\",\"durable\":true}";
            StringContent stringContent = new StringContent(content);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await HttpClient.PutAsync(url, stringContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateQueue(string queueName)
        {
            string url = HttpClient.BaseAddress + "api/queues/%2f/" + queueName;
            string content = "{\"durable\":true}";
            StringContent stringContent = new StringContent(content);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await HttpClient.PutAsync(url, stringContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task BindQueueToExchange(string exchangeName, BindingConfigEntry queue)
        {
            string url = HttpClient.BaseAddress + "api/bindings/%2f/e/" + exchangeName + "/q/" + queue.ComponentName;
            string content = $"{{\"routing_key\":\"{queue.RoutingKey}\"}}";
            StringContent stringContent = new StringContent(content);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await HttpClient.PostAsync(url, stringContent);
            response.EnsureSuccessStatusCode();
        }

        public void Auth()
        {
            var credentials = Encoding.ASCII.GetBytes($"{this.Configuration["Credentials:Login"]}:{this.Configuration["Credentials:Password"]}");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            this.HttpClient.DefaultRequestHeaders.Authorization = header;
            this.HttpClient.BaseAddress = new Uri(this.Configuration["RabbitMqAddress"]);
        }

        public void Dispose()
        {
            this.HttpClient?.Dispose();
        }
    }
}