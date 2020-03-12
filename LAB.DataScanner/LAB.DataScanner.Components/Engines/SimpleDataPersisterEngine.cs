namespace LAB.DataScanner.Components.Engines
{
    using System.Text;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Persisters.SimpleDataPersister;
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client.Events;

    public class SimpleDataPersisterEngine : ISimpleDataPersisterEngine
    {
        private const string BindingConfigurationSectionName = "Binding";
        private const string SourceQueueConfigurationName = "ReceiverQueue";

        private readonly IConfigurationSection configurationSection;
        private readonly IRmqConsumer rmqConsumer;
        private readonly IDbDataWriter dbDataWriter;

        public SimpleDataPersisterEngine(
           IConfigurationSection configurationSection,
           IRmqConsumer rmqConsumer,
           IDbDataWriter dbDataWriter)
        {
            this.configurationSection = configurationSection;
            this.rmqConsumer = rmqConsumer;
            this.dbDataWriter = dbDataWriter;
        }

        public void Start()
        {
            var bindingConfiguration = configurationSection.GetSection(BindingConfigurationSectionName);
            var sourceQueue = bindingConfiguration.GetValue<string>(SourceQueueConfigurationName);

            rmqConsumer.SetQueue(sourceQueue);

            rmqConsumer.StartListening((model, ea) =>
            {
                var jsonToSave = Encoding.UTF8.GetString(ea.Body);
                dbDataWriter.SaveMessagesToDb(new string[] { jsonToSave });
                rmqConsumer.Ack(new BasicDeliverEventArgs() { DeliveryTag = ea.DeliveryTag });
            });
        }
    }
}
