namespace LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder
{
    using System;
    using RabbitMQ.Client.Events;

    public interface IRmqConsumer : IDisposable
    {
        void Ack(BasicDeliverEventArgs args);
        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler);
        public void StopListening();
        public void SetQueue(string queueName);
    }
}