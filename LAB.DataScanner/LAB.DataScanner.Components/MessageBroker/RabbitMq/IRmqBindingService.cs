namespace LAB.DataScanner.Components.MessageBroker.RabbitMq
{
    using LAB.DataScanner.Components.Models;

    public interface IRmqBindingService
    {
        string Build(BindingConfig data);
    }
}