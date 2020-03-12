namespace LAB.DataScanner.Components.Engines
{
    using System.Text;
    using LAB.DataScanner.Components.Extractors.TextExtractor;
    using LAB.DataScanner.Components.Loggers;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;

    public class TextExtractorEngine : ITextExtractorEngine
    {
        private const string BetweenCriteriaName = "between";
        private const string ContainsCriteriaName = "contains";

        private const string CriteriaConfigurationName = "Criteria";
        private const string SourceQueueConfigurationName = "ReceiverQueue";
        private const string SenderExchangeConfigurationName = "SenderExchange";
        private const string SenderRoutingKeysConfigurationName = "SenderRoutingKeys";
        private const string BindingConfigurationSectionName = "Binding";
        private const string ApplicationConfigurationSectionName = "Application";

        private const string StartWordConfigurationName = "StartWord";
        private const string StopWordConfigurationName = "StopWord";

        private const string TargetWordConfigurationName = "TargetWord";

        private const char routingKeySplitChar = ',';

        private readonly IConfigurationSection configurationSection;
        private readonly IRmqConsumer rmqConsumer;
        private readonly IRmqPublisher rmqPublisher;
        private readonly IExtractionMethods extractionMethods;
        private readonly ILoggerServiceSerilog loggerService;


        public TextExtractorEngine(
            IConfigurationSection configurationSection, 
            IRmqConsumer rmqConsumer,
            IRmqPublisher rmqPublisher,
            IExtractionMethods extractionMethods,
            ILoggerServiceSerilog loggerService)
        {
            this.loggerService = loggerService;
            this.configurationSection = configurationSection;
            this.rmqConsumer = rmqConsumer;
            this.rmqPublisher = rmqPublisher;
            this.extractionMethods = extractionMethods;
        }

        public void Start()
        {
            // Extract settings to separated private method?
            var bindingConfiguration = configurationSection.GetSection(BindingConfigurationSectionName);

            var sourceQueue = bindingConfiguration.GetValue<string>(SourceQueueConfigurationName);
            var senderExchange = bindingConfiguration.GetValue<string>(SenderExchangeConfigurationName);
            var senderRoutingKeysAsString = bindingConfiguration.GetValue<string>(SenderRoutingKeysConfigurationName);

            var senderRoutingKeysAsArray = senderRoutingKeysAsString.Split(routingKeySplitChar);

            var applicationConfiguration = configurationSection.GetSection(ApplicationConfigurationSectionName);

            var targetWord = applicationConfiguration.GetValue<string>(TargetWordConfigurationName);

            var startWord = applicationConfiguration.GetValue<string>(StartWordConfigurationName);
            var stopWord = applicationConfiguration.GetValue<string>(StopWordConfigurationName);

            var criteriaName = applicationConfiguration.GetValue<string>(CriteriaConfigurationName);

            rmqConsumer.SetQueue(sourceQueue);

            rmqConsumer.StartListening((model, ea) =>
            {
                var stringToParse = Encoding.UTF8.GetString(ea.Body);

                this.loggerService.Information("Get string: {StringForParsing}", stringToParse);

                string parsedString = null;
                if(criteriaName.ToLower() == ContainsCriteriaName)
                {
                    this.loggerService.Information("Extraction started with Target Word: {TargetWord}", targetWord);
                    parsedString = extractionMethods.Contains(targetWord, stringToParse);
                }
                if (criteriaName.ToLower() == BetweenCriteriaName)
                {
                    this.loggerService.Information("Extraction started with Start Word: {StartWord}, and End Word: {EndWord}", startWord, stopWord);
                    parsedString = extractionMethods.Between(startWord, stopWord, stringToParse);
                }
                var jsonString = JsonConvert.SerializeObject(parsedString);
                var outputData = Encoding.UTF8.GetBytes(jsonString);
                rmqPublisher.Publish(outputData, senderExchange, senderRoutingKeysAsArray); 
                this.loggerService.Information("EPublish to RmqBroker data: {OutputData}.", outputData);

                rmqConsumer.Ack(new BasicDeliverEventArgs() { DeliveryTag = ea.DeliveryTag });
                this.loggerService.Information("Sended Acknowledgment.");
            });
        }
    }
}