namespace LAB.DataScanner.Components.MessageBroker.RabbitMq
{
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Models;

    public interface IRmqManagementService
    {
        void Auth();
        Task CreateDirectExchange(string exchangeName);
        Task CreateQueue(string queueName);
        Task BindQueueToExchange(string exchangeName, BindingConfigEntry queue);
    }
}