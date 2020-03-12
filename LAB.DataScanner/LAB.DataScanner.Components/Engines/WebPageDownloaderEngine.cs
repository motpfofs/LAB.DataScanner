namespace LAB.DataScanner.Components.Engines
{
    using System.Text;
    using LAB.DataScanner.Components.DataRetriever;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;

    public class WebPageDownloaderEngine : IWebPageDownloaderEngine
    {
        private const string BindingConfigurationSectionName = "Binding";
        private const string ApplicationConfigurationSectionName = "Application";
        private const string SenderExchangeConfigurationName = "SenderExchange";
        private const string SenderRoutingKeysConfigurationName = "SenderRoutingKeys";
        private const string TargetUrlsConfigurationSection = "TargetUrls";

        private const char routingKeySplitChar = ',';

        private const char TargetUrlsSplitChar = ' ';

        public readonly IRmqPublisher rmqPublisher;
        private readonly IConfigurationSection configurationSection;
        private readonly IDataRetriever dataRetriever;

        public WebPageDownloaderEngine(
            IDataRetriever dataRetriever, 
            IConfigurationSection configurationSection, 
            IRmqPublisher rmqPublisher)
        {
            this.dataRetriever = dataRetriever;
            this.configurationSection = configurationSection;
            this.rmqPublisher = rmqPublisher;
        }

        public async void Start()
        {
            var applicationConfiguration = configurationSection.GetSection(ApplicationConfigurationSectionName);
            var targetUrlsEntriesAsString = applicationConfiguration.GetValue<string>(TargetUrlsConfigurationSection);
            var targetUrlsEntries = targetUrlsEntriesAsString.Split(TargetUrlsSplitChar);

            var bindingConfiguration = configurationSection.GetSection(BindingConfigurationSectionName);

            var senderExchange = bindingConfiguration.GetValue<string>(SenderExchangeConfigurationName);
            var senderRoutingKeysAsString = bindingConfiguration.GetValue<string>(SenderRoutingKeysConfigurationName);

            var senderRoutingKeysAsArray = senderRoutingKeysAsString.Split(routingKeySplitChar);

            string pageDataAsString;
            foreach (var link in targetUrlsEntries)
            {
                pageDataAsString = await dataRetriever.RetrieveStringAsync(link);
                rmqPublisher.Publish(
                    Encoding.ASCII.GetBytes(pageDataAsString),
                    senderExchange,
                    senderRoutingKeysAsArray);
            }
        }
    }
}
