namespace LAB.DataScanner.Components.Engines
{
    using System;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Parsers.CsvConverter;
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client.Events;

    public class CsvToJsonConverterEngine : ICsvToJsonConverterEngine
    {
        private const string SourceQueueConfigurationName = "ReceiverQueue";
        private const string SenderExchangeConfigurationName = "SenderExchange";
        private const string SenderRoutingKeysConfigurationName = "SenderRoutingKeys";
        private const string BindingConfigurationSectionName = "Binding";
        private const string ApplicationConfigurationSectionName = "Application";

        private const string TargetColumnsConfigurationName = "Columns";
        private const string TargetRowsConfigurationName = "Rows";

        private const char routingKeySplitChar = ',';


        private const char ColumnsSplitChar = ' ';

        private readonly IConfigurationSection configurationSection;
        private readonly IRmqConsumer rmqConsumer;
        private readonly IRmqPublisher rmqPublisher;
        private readonly ICsvConverter csvToJsonParser;

        public CsvToJsonConverterEngine(
            IConfigurationSection configurationSection,
            IRmqConsumer rmqConsumer,
            IRmqPublisher rmqPublisher,
            ICsvConverter csvToJsonParser)
        {
            this.configurationSection = configurationSection;
            this.rmqConsumer = rmqConsumer;
            this.rmqPublisher = rmqPublisher;
            this.csvToJsonParser = csvToJsonParser;
        }

        public void Start()
        {
            var bindingConfiguration = configurationSection.GetSection(BindingConfigurationSectionName);

            var sourceQueue = bindingConfiguration.GetValue<string>(SourceQueueConfigurationName);
            var senderExchange = bindingConfiguration.GetValue<string>(SenderExchangeConfigurationName);
            var senderRoutingKeysAsString = bindingConfiguration.GetValue<string>(SenderRoutingKeysConfigurationName);

            var senderRoutingKeysAsArray = senderRoutingKeysAsString.Split(routingKeySplitChar);

            var applicationConfiguration = configurationSection.GetSection(ApplicationConfigurationSectionName);

            var targetColumnsAsString = applicationConfiguration.GetValue<string>(TargetColumnsConfigurationName);
            var targetRowsAsString = applicationConfiguration.GetValue<string>(TargetRowsConfigurationName);


            int[] targetColumns = targetColumnsAsString
                .Split(ColumnsSplitChar)
                .Select(h => int.Parse(h)).ToArray();
            int targetRows = int.Parse(targetRowsAsString);

            rmqConsumer.SetQueue(sourceQueue);

            rmqConsumer.StartListening((model, ea) =>
            {
                var csvToParse = Encoding.UTF8.GetString(ea.Body);
                var jsonFromCsv = csvToJsonParser.ConvertToJson(DateTime.Now.ToString(), csvToParse, targetColumns, targetRows);
                var outputData = Encoding.UTF8.GetBytes(jsonFromCsv);
                rmqPublisher.Publish(outputData, senderExchange, senderRoutingKeysAsArray);
                rmqConsumer.Ack(new BasicDeliverEventArgs() { DeliveryTag = ea.DeliveryTag });
            });
        }
    }
}
